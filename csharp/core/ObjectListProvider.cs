using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace urakawa
{
    public class ObjectListProvider<T> where T : WithPresentation
    {
        //"lock() {}" is syntactic sugar for Monitor.Enter/Exit() with a try/finally wrapper => sync multiple threads owned by the same process.
        // Mutex is a heavyweight multi-process lock, implemented using Win32 interop wrapper => sync threads across several processes.

        private readonly Object LOCK = new object();

        private List<T> m_objects;

        public ObjectListProvider()
        {
            m_objects = new List<T>();
        }

        public int Count
        {
            get
            {
                lock (LOCK)
                {
                    return m_objects.Count;
                }
            }
        }



        public ReadOnlyCollection<T> ContentsAs_ReadOnlyCollectionWrapper
        {
            get
            {
                lock (LOCK)
                {
                    return new ReadOnlyCollection<T>(m_objects);
                }
            }
        }

        public ReadOnlyCollection<T> ContentsAs_ListAsReadOnly
        {
            get
            {
                lock (LOCK)
                {
                    return m_objects.AsReadOnly();
                }
            }
        }

        public List<T> ContentsAs_ListCopy
        {
            get
            {
                lock (LOCK)
                {
                    return new List<T>(m_objects);
                }
            }
        }

        public IEnumerable<T> ContentsAs_YieldEnumerable
        {
            get
            {
                lock (LOCK)
                {
                    foreach (T obj in m_objects)
                    {
                        yield return obj;
                    }

                    yield break;
                }
            }
        }

        public IEnumerable ContentsAs_ArrayListReadOnlyWrapper
        {
            get
            {
                lock (LOCK)
                {
                    return ArrayList.ReadOnly(m_objects);
                }
            }
        }

        public List<T> Contents
        {
            get
            {
                lock (LOCK)
                {
                    return m_objects;
                }
            }
        }

        public void Add(T obj)
        {
            lock (LOCK)
            {
                m_objects.Add(obj);
            }
        }

        public void Remove(T obj)
        {
            lock (LOCK)
            {
                m_objects.Remove(obj);
            }
        }

        public void Clear()
        {
            lock (LOCK)
            {
                m_objects.Clear();
            }
        }

        public bool Contains(T obj)
        {
            lock (LOCK)
            {
                return m_objects.Contains(obj);
            }
        }

        public T Get(int index)
        {
            lock (LOCK)
            {
                return m_objects[index];
            }
        }
    }
}
