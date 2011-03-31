using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using urakawa.events;
using urakawa.xuk;

namespace urakawa
{
    public class ObjectAddedEventArgs<T> : DataModelChangedEventArgs where T : XukAble
    {
        public readonly T m_AddedObject;

        public ObjectAddedEventArgs(XukAble source, T obj)
            : base(source)
        {
            m_AddedObject = obj;
        }
    }

    public class ObjectRemovedEventArgs<T> : DataModelChangedEventArgs where T : XukAble
    {
        public readonly T m_RemovedObject;
        public readonly int m_RemovedObjectPosition;

        public ObjectRemovedEventArgs(XukAble source, T obj, int oldPos)
            : base(source)
        {
            m_RemovedObject = obj;
            m_RemovedObjectPosition = oldPos;
        }
    }

    public class ObjectListProvider<T> where T : XukAble
    {

        #region backing fieds

        //"lock() {}" is syntactic sugar for Monitor.Enter/Exit() with a try/finally wrapper => sync multiple threads owned by the same process.
        // Mutex is a heavyweight multi-process lock, implemented using Win32 interop wrapper => sync threads across several processes.

        private readonly Object LOCK = new object();

        private bool m_useLock = false;
        public bool UseLock
        {
            get { return m_useLock; }
        }
        private List<T> m_objects;
        private XukAble m_Owner;

        #endregion backing fieds

        #region ctr

        public ObjectListProvider(XukAble owner, bool useLock)
        {
            m_Owner = owner;
            m_useLock = useLock;
            m_objects = new List<T>();
        }

        #endregion ctr

        #region list adaptation

        public ReadOnlyCollection<T> ContentsAs_ReadOnlyCollectionWrapper
        {
            get
            {
                if (m_useLock)
                {
                    lock (LOCK)
                    {
                        return new ReadOnlyCollection<T>(m_objects);
                    }
                }
                else
                {
                    return new ReadOnlyCollection<T>(m_objects);
                }
            }
        }

        public ReadOnlyCollection<T> ContentsAs_ListAsReadOnly
        {
            get
            {
                if (m_useLock)
                {
                    lock (LOCK)
                    {
                        return m_objects.AsReadOnly();
                    }
                }
                else
                {
                    return m_objects.AsReadOnly();
                }
            }
        }

        public List<T> ContentsAs_ListCopy
        {
            get
            {
                if (m_useLock)
                {
                    lock (LOCK)
                    {
                        return new List<T>(m_objects);
                    }
                }
                else
                {
                    return new List<T>(m_objects);
                }
            }
        }

        public IEnumerable<T> ContentsAs_Enumerable
        {
            get
            {
                if (m_useLock)
                {
                    lock (LOCK)
                    {
                        return m_objects;
                    }
                }
                else
                {
                    return m_objects;
                }
            }
        }

        //public IEnumerable<T> ContentsAs_YieldEnumerable
        //{
        //    get
        //    {
        //        if (m_useLock)
        //        {
        //            lock (LOCK)
        //            {
        //                foreach (T obj in m_objects)
        //                {
        //                    yield return obj;
        //                }

        //                yield break;
        //            }
        //        }
        //        else
        //        {
        //            foreach (T obj in m_objects)
        //            {
        //                yield return obj;
        //            }

        //            yield break;
        //        }
        //    }
        //}
        public IEnumerable<T> ContentsAs_YieldEnumerableReversed
        {
            get
            {
                if (m_useLock)
                {
                    lock (LOCK)
                    {
                        for (int i = m_objects.Count - 1; i >= 0; i--)
                        {
                            yield return m_objects[i];
                        }

                        yield break;
                    }
                }
                else
                {
                    for (int i = m_objects.Count - 1; i >= 0; i--)
                    {
                        yield return m_objects[i];
                    }

                    yield break;
                }
            }
        }

        public IEnumerable ContentsAs_ArrayListReadOnlyWrapper
        {
            get
            {
                if (m_useLock)
                {
                    lock (LOCK)
                    {
                        return ArrayList.ReadOnly(m_objects);
                    }
                }
                else
                {
                    return ArrayList.ReadOnly(m_objects);
                }
            }
        }

        #endregion list adaptation

        #region read-only access

        public int Count
        {
            get
            {
                if (m_useLock)
                {
                    lock (LOCK)
                    {
                        return m_objects.Count;
                    }
                }
                else
                {
                    return m_objects.Count;
                }
            }
        }

        public T Get(int index)
        {
            if (m_useLock)
            {
                lock (LOCK)
                {
                    return m_objects[index];
                }
            }
            else
            {
                return m_objects[index];
            }
        }

        public int IndexOf(T obj)
        {
            if (m_useLock)
            {
                lock (LOCK)
                {
                    return m_objects.IndexOf(obj);
                }
            }
            else
            {
                return m_objects.IndexOf(obj);
            }
        }

        #endregion read-only access

        #region event stuff

        public event EventHandler<ObjectAddedEventArgs<T>> ObjectAdded;
        protected void RaiseObjectAddedEvent(T obj)
        {
            EventHandler<ObjectAddedEventArgs<T>> d = ObjectAdded;
            if (d != null)
            {
                d(m_Owner, new ObjectAddedEventArgs<T>(m_Owner, obj));
            }
        }

        public event EventHandler<ObjectRemovedEventArgs<T>> ObjectRemoved;
        protected void RaiseObjectRemovedEvent(T obj, int oldPos)
        {
            EventHandler<ObjectRemovedEventArgs<T>> d = ObjectRemoved;
            if (d != null)
            {
                d(m_Owner, new ObjectRemovedEventArgs<T>(m_Owner, obj, oldPos));
            }
        }

        #endregion event stuff

        #region modifiers

        public void Insert(int index, T obj)
        {
            if (m_useLock)
            {
                lock (LOCK)
                {
                    m_objects.Insert(index, obj);
                }
            }
            else
            {
                m_objects.Insert(index, obj);
            }
            RaiseObjectAddedEvent(obj);
        }

        public void Remove(T obj)
        {
            int index = -1;

            if (m_useLock)
            {
                lock (LOCK)
                {
                    index = m_objects.IndexOf(obj);
                    m_objects.Remove(obj);
                }
            }
            else
            {
                index = m_objects.IndexOf(obj);
                m_objects.Remove(obj);
            }
            if (index != -1)
            {
                RaiseObjectRemovedEvent(obj, index);
            }
        }

        #endregion modifiers


    }
}
