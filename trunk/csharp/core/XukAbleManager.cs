using System;
using System.Collections.Generic;
using System.Threading;
using urakawa.exception;
using urakawa.media.data;
using urakawa.xuk;

namespace urakawa
{
    public abstract class XukAbleManager<T> : XukAble, IValueEquatable<XukAbleManager<T>> where T : WithPresentation
    {
        private Mutex m_Mutex = new Mutex();

        private List<T> m_managedObjects;
        private readonly string m_UidPrefix;
        private ulong m_UidIndex = 0;

        private readonly Presentation m_Presentation;
        public Presentation Presentation
        {
            get
            {
                return m_Presentation;
            }
        }

        public XukAbleManager(Presentation pres, string uidPrefix)
        {
            m_managedObjects = new List<T>();
            m_Presentation = pres;
            m_UidPrefix = uidPrefix;
        }

        public int NumberOfManagedObjects
        {
            get
            {
                m_Mutex.WaitOne();
                try
                {
                    return m_managedObjects.Count;
                }
                finally
                {
                    m_Mutex.ReleaseMutex();
                }
            }
        }

        public List<T> ListOfManagedObjects
        {
            get
            {
                m_Mutex.WaitOne();
                try
                {
                    return new List<T>(m_managedObjects);
                }
                finally
                {
                    m_Mutex.ReleaseMutex();
                }
            }
        }


        public void RegenerateUids()
        {
            m_Mutex.WaitOne();
            try
            {
                ulong index = 0;

                List<T> managedObjects = new List<T>(m_managedObjects);
                m_managedObjects.Clear();

                foreach (T obj in managedObjects)
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


        public void ClearManagedObjects()
        {
            m_Mutex.WaitOne();
            try
            {
                m_managedObjects.Clear();
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
                foreach (T obj in m_managedObjects)
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
                foreach (T objz in m_managedObjects)
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
                foreach (T objz in m_managedObjects)
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
                foreach (T objz in m_managedObjects)
                {
                    if (objz == obj)
                    {
                        m_managedObjects.Remove(obj);
                        return;
                    }
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
                foreach (T objz in m_managedObjects)
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
                foreach (T obj in m_managedObjects)
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

        public virtual bool ValueEquals(XukAbleManager<T> other)
        {
            if (other == null)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            if (other.NumberOfManagedObjects != NumberOfManagedObjects)
            {
                return false;
            }

            foreach (T obj in m_managedObjects)
            {
                if (!other.IsManagerOf(obj.Uid)) return false;
                if (!other.GetManagedObject(obj.Uid).ValueEquals(obj)) return false;
            }

            return true;
        }

        #endregion
    }
}
