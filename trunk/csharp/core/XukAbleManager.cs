using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using urakawa.exception;

namespace urakawa
{

    public class ObjectListProvider<T> where T : WithPresentation
    {
        private readonly Mutex m_Mutex;
        private List<T> m_objects;

        public ObjectListProvider()
        {
            m_Mutex = new Mutex();
            m_objects = new List<T>();
        }

        public int Count
        {
            get
            {
                m_Mutex.WaitOne();
                try
                {
                    return m_objects.Count;
                }
                finally
                {
                    m_Mutex.ReleaseMutex();
                }
            }
        }



        public ReadOnlyCollection<T> ContentsAs_ReadOnlyCollectionWrapper
        {
            get
            {
                m_Mutex.WaitOne();
                try
                {
                    return new ReadOnlyCollection<T>(m_objects);
                }
                finally
                {
                    m_Mutex.ReleaseMutex();
                }
            }
        }

        public ReadOnlyCollection<T> ContentsAs_ListAsReadOnly
        {
            get
            {
                m_Mutex.WaitOne();
                try
                {
                    return m_objects.AsReadOnly();
                }
                finally
                {
                    m_Mutex.ReleaseMutex();
                }
            }
        }

        public List<T> ContentsAs_ListCopy
        {
            get
            {
                m_Mutex.WaitOne();
                try
                {
                    return new List<T>(m_objects);
                }
                finally
                {
                    m_Mutex.ReleaseMutex();
                }
            }
        }

        public IEnumerable<T> ContentsAs_YieldEnumerable
        {
            get
            {
                m_Mutex.WaitOne();
                try
                {
                    foreach (T obj in m_objects)
                    {
                        yield return obj;
                    }
                }
                finally
                {
                    m_Mutex.ReleaseMutex();
                }
            }
        }

        public IEnumerable ContentsAs_ArrayListReadOnlyWrapper
        {
            get
            {
                m_Mutex.WaitOne();
                try
                {
                    return ArrayList.ReadOnly(m_objects);
                }
                finally
                {
                    m_Mutex.ReleaseMutex();
                }
            }
        }

        public List<T> Contents
        {
            get
            {
                m_Mutex.WaitOne();
                try
                {
                    return m_objects;
                }
                finally
                {
                    m_Mutex.ReleaseMutex();
                }
            }
        }

        public void Add(T obj)
        {
            m_Mutex.WaitOne();
            try
            {
                m_objects.Add(obj);
            }
            finally
            {
                m_Mutex.ReleaseMutex();
            }
        }

        public void Remove(T obj)
        {
            m_Mutex.WaitOne();
            try
            {
                m_objects.Remove(obj);
            }
            finally
            {
                m_Mutex.ReleaseMutex();
            }
        }

        public void Clear()
        {
            m_Mutex.WaitOne();
            try
            {
                m_objects.Clear();
            }
            finally
            {
                m_Mutex.ReleaseMutex();
            }
        }

        public bool Contains(T obj)
        {
            m_Mutex.WaitOne();
            try
            {
                return m_objects.Contains(obj);
            }
            finally
            {
                m_Mutex.ReleaseMutex();
            }
        }

        public T Get(int index)
        {
            m_Mutex.WaitOne();
            try
            {
                return m_objects[index];
            }
            finally
            {
                m_Mutex.ReleaseMutex();
            }
        }
    }

    public abstract class XukAbleManager<T> : WithPresentation where T : WithPresentation
    {
        private readonly Mutex m_Mutex;
        private ObjectListProvider<T> m_managedObjects;

        private readonly string m_UidPrefix;
        private ulong m_UidIndex = 0;

        public XukAbleManager(Presentation pres, string uidPrefix)
        {
            m_Mutex = new Mutex();
            Presentation = pres;
            m_managedObjects = new ObjectListProvider<T>();
            m_UidPrefix = uidPrefix;
        }

        public ObjectListProvider<T> ListProvider
        {
            get
            {
                return m_managedObjects;
            }
        }


        public void RegenerateUids()
        {
            m_Mutex.WaitOne();
            try
            {
                ulong index = 0;

                List<T> localList = m_managedObjects.ContentsAs_ListCopy;

                m_managedObjects.Clear();

                foreach (T obj in localList)
                {
                    string newUid = Presentation.GetNewUid(m_UidPrefix, ref index);
                    obj.Uid = newUid;
                    m_managedObjects.Add(obj);
                }
            }
            finally
            {
                m_Mutex.ReleaseMutex();
            }
        }



        public T GetManagedObject(string uid)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new exception.MethodParameterIsNullException("uid cannot be null or empty");
            }
            m_Mutex.WaitOne();
            try
            {
                foreach (T obj in m_managedObjects.ContentsAs_YieldEnumerable)
                {
                    if (obj.Uid == uid) return obj;
                }
            }
            finally
            {
                m_Mutex.ReleaseMutex();
            }
            throw new exception.IsNotManagerOfException(String.Format(
                                                                     "The manager does not manage an object with uid {0}",
                                                                     uid));
        }

        public string GetUidOfManagedObject(T obj)
        {
            if (obj == null)
            {
                throw new exception.MethodParameterIsNullException("channel parameter is null");
            }
            m_Mutex.WaitOne();
            try
            {
                foreach (T objz in m_managedObjects.ContentsAs_YieldEnumerable)
                {
                    if (objz == obj) return objz.Uid;
                }
            }
            finally
            {
                m_Mutex.ReleaseMutex();
            }
            throw new exception.IsNotManagerOfException("The given object is not managed by this Manager");
        }

        public void SetUidOfManagedObject(T obj, string uid)
        {
            if (obj == null)
            {
                throw new exception.MethodParameterIsNullException("channel parameter is null");
            }

            if (string.IsNullOrEmpty(uid))
            {
                throw new exception.MethodParameterIsEmptyStringException("uid parameter cannot be null or empty string");
            }

            m_Mutex.WaitOne();
            try
            {
                string oldUid = null;
                foreach (T objz in m_managedObjects.ContentsAs_YieldEnumerable)
                {
                    if (objz.Uid == uid && objz != obj)
                    {
                        throw new exception.ObjectIsAlreadyManagedException(
                            String.Format("Another managed object exists with uid {0}", uid));
                    }
                    if (objz == obj)
                    {
                        if (objz.Uid == obj.Uid)
                        {
                            return;
                        }
                        oldUid = objz.Uid;
                    }
                }
                if (string.IsNullOrEmpty(oldUid))
                {
                    throw new exception.IsNotManagerOfException("The given object is not managed by this Manager");
                }
                obj.Uid = uid;
            }
            finally
            {
                m_Mutex.ReleaseMutex();
            }
        }

        public virtual void RemoveManagedObject(T obj)
        {
            m_Mutex.WaitOne();
            try
            {
                T objectToRemove = null;
                foreach (T objz in m_managedObjects.ContentsAs_YieldEnumerable)
                {
                    if (objz == obj)
                    {
                        objectToRemove = obj;
                        break;
                    }
                }
                if (objectToRemove != null)
                {
                    m_managedObjects.Remove(obj);
                    return;
                }
            }
            finally
            {
                m_Mutex.ReleaseMutex();
            }
            throw new exception.IsNotManagerOfException("The given object is not managed by this Manager");
        }

        public void AddManagedObject(T obj)
        {
            AddManagedObject(obj, Presentation.GetNewUid(m_UidPrefix, ref m_UidIndex));
        }

        public abstract bool CanAddManagedObject(T managedObject);
        private void AddManagedObject(T obj, string uid)
        {
            if (obj == null)
            {
                throw new exception.MethodParameterIsNullException("parameter is null");
            }
            if (string.IsNullOrEmpty(uid))
            {
                throw new exception.MethodParameterIsNullException("uid parameter cannot be null or empty");
            }
            m_Mutex.WaitOne();
            try
            {
                foreach (T objz in m_managedObjects.ContentsAs_YieldEnumerable)
                {
                    if (obj == objz)
                    {
                        throw new exception.ObjectIsAlreadyManagedException(
                            "The given object is already managed by the Manager");
                    }
                    if (objz.Uid == uid)
                    {
                        throw new exception.ObjectIsAlreadyManagedException(
                           String.Format("Another managed object exists with uid {0}", uid));
                    }
                }

                if (!CanAddManagedObject(obj))
                {
                    throw new CannotManageObjectException("the given object cannot be added to the Manager.");
                }

                obj.Uid = uid;
                m_managedObjects.Add(obj);
            }
            finally
            {
                m_Mutex.ReleaseMutex();
            }

        }


        public bool IsManagerOf(string uid)
        {
            m_Mutex.WaitOne();
            try
            {
                foreach (T obj in m_managedObjects.ContentsAs_YieldEnumerable)
                {
                    if (obj.Uid == uid) return true;
                }
                return false;
            }
            finally
            {
                m_Mutex.ReleaseMutex();
            }
        }


        #region IValueEquatable<ChannelsManager> Members

        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            XukAbleManager<T> otherz = other as XukAbleManager<T>;
            if (otherz == null)
            {
                return false;
            }

            if (otherz.ListProvider.Count != m_managedObjects.Count)
            {
                return false;
            }

            foreach (T obj in m_managedObjects.ContentsAs_YieldEnumerable)
            {
                if (!otherz.IsManagerOf(obj.Uid)) return false;
                if (!otherz.GetManagedObject(obj.Uid).ValueEquals(obj)) return false;
            }

            return true;
        }

        #endregion
    }
}
