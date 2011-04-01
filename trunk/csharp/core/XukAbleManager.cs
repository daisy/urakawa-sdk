using System;
using System.Collections.Generic;
using urakawa.exception;

namespace urakawa
{
    public abstract class XukAbleManager<T> : WithPresentation where T : WithPresentation
    {
        private readonly Object LOCK = new object();
        private ObjectListProvider<T> m_managedObjects;

        private readonly string m_UidPrefix;
        private ulong m_UidIndex = 0;

        public XukAbleManager(Presentation pres, string uidPrefix)
        {
            Presentation = pres;
            m_managedObjects = new ObjectListProvider<T>(this, false);
            m_UidPrefix = uidPrefix;
        }

        public ObjectListProvider<T> ManagedObjects
        {
            get
            {
                return m_managedObjects;
            }
        }


        private void RegenerateUids_NO_LOCK()
        {
            ulong index = 0;

            List<T> localList = m_managedObjects.ContentsAs_ListCopy;

            foreach (T obj in localList)
            {
                m_managedObjects.Remove(obj);
            }

            foreach (T obj in localList)
            {
                string newUid = Presentation.GetNewUid(m_UidPrefix, ref index);
                obj.Uid = newUid;
                m_managedObjects.Insert(m_managedObjects.Count, obj);
            }
        }

        public void RegenerateUids()
        {
            if (m_managedObjects.UseLock)
            {
                lock (LOCK)
                {
                    RegenerateUids_NO_LOCK();
                }
            }
            else
            {
                RegenerateUids_NO_LOCK();
            }
        }



        private T GetManagedObject_NO_LOCK(string uid)
        {
            foreach (T obj in m_managedObjects.ContentsAs_Enumerable)
            {
                if (obj.Uid == uid) return obj;
            }

            throw new exception.IsNotManagerOfException(String.Format(
                                                                     "The manager does not manage an object with uid {0}",
                                                                     uid));
        }

        public T GetManagedObject(string uid)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new exception.MethodParameterIsNullException("uid cannot be null or empty");
            }
            if (m_managedObjects.UseLock)
            {
                lock (LOCK)
                {
                    return GetManagedObject_NO_LOCK(uid);
                }
            }
            else
            {
                return GetManagedObject_NO_LOCK(uid);
            }
        }

        private string GetUidOfManagedObject_NO_LOCK(T obj)
        {
            foreach (T objz in m_managedObjects.ContentsAs_Enumerable)
            {
                if (objz == obj) return objz.Uid;
            }

            throw new exception.IsNotManagerOfException("The given object is not managed by this Manager");
        }

        //public string GetUidOfManagedObject(T obj)
        //{
        //    if (obj == null)
        //    {
        //        throw new exception.MethodParameterIsNullException("channel parameter is null");
        //    }
        //    if (m_managedObjects.UseLock)
        //    {
        //        lock (LOCK)
        //        {
        //            return GetUidOfManagedObject_NO_LOCK(obj);
        //        }
        //    }
        //    else
        //    {
        //        return GetUidOfManagedObject_NO_LOCK(obj);
        //    }
        //}


        public void SetUidOfManagedObject_NO_LOCK(T obj, string uid)
        {
            string oldUid = null;
            foreach (T objz in m_managedObjects.ContentsAs_Enumerable)
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
            if (m_managedObjects.UseLock)
            {
                lock (LOCK)
                {
                    SetUidOfManagedObject_NO_LOCK(obj, uid);
                }
            }
            else
            {
                SetUidOfManagedObject_NO_LOCK(obj, uid);
            }
        }

        private void RemoveManagedObject_NO_LOCK(T obj)
        {
            T objectToRemove = null;
            foreach (T objz in m_managedObjects.ContentsAs_Enumerable)
            {
                if (objz == obj)
                {
                    objectToRemove = obj;
                    break;
                }
            }
            if (objectToRemove != null)
            {
                m_managedObjects.Remove(objectToRemove);
                return;
            }

            throw new exception.IsNotManagerOfException("The given object is not managed by this Manager");
        }

        public virtual void RemoveManagedObject(T obj)
        {
            if (m_managedObjects.UseLock)
            {
                lock (LOCK)
                {
                    RemoveManagedObject_NO_LOCK(obj);
                }
            }
            else
            {
                RemoveManagedObject_NO_LOCK(obj);
            }
        }

        public void AddManagedObject(T obj)
        {
            AddManagedObject(obj, Presentation.GetNewUid(m_UidPrefix, ref m_UidIndex));
        }

        public abstract bool CanAddManagedObject(T managedObject);
        private void AddManagedObject_NO_LOCK(T obj, string uid, bool safetyChecks)
        {
            if (safetyChecks)
            {
                foreach (T objz in m_managedObjects.ContentsAs_Enumerable)
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
            }

            if (!CanAddManagedObject(obj))
            {
                throw new CannotManageObjectException("the given object cannot be added to the Manager.");
            }

            obj.Uid = uid;
            m_managedObjects.Insert(m_managedObjects.Count, obj);
        }

        public void AddManagedObject(T obj, string uid)
        {
            if (obj == null)
            {
                throw new exception.MethodParameterIsNullException("parameter is null");
            }
            if (string.IsNullOrEmpty(uid))
            {
                throw new exception.MethodParameterIsNullException("uid parameter cannot be null or empty");
            }

            if (m_managedObjects.UseLock)
            {
                lock (LOCK)
                {
                    AddManagedObject_NO_LOCK(obj, uid, true);
                }
            }
            else
            {
                AddManagedObject_NO_LOCK(obj, uid, true);
            }
        }
        public void AddManagedObject_NoSafetyChecks(T obj, string uid)
        {
            if (obj == null)
            {
                throw new exception.MethodParameterIsNullException("parameter is null");
            }
            if (string.IsNullOrEmpty(uid))
            {
                throw new exception.MethodParameterIsNullException("uid parameter cannot be null or empty");
            }

            if (m_managedObjects.UseLock)
            {
                lock (LOCK)
                {
                    AddManagedObject_NO_LOCK(obj, uid, false);
                }
            }
            else
            {
                AddManagedObject_NO_LOCK(obj, uid, false);
            }
        }

        public bool IsManagerOf_NO_LOCK(string uid)
        {
            foreach (T obj in m_managedObjects.ContentsAs_Enumerable)
            {
                if (obj.Uid == uid) return true;
            }
            return false;
        }

        public bool IsManagerOf(string uid)
        {
            if (m_managedObjects.UseLock)
            {
                lock (LOCK)
                {
                    return IsManagerOf_NO_LOCK(uid);
                }
            }
            else
            {
                return IsManagerOf_NO_LOCK(uid);
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

            if (otherz.ManagedObjects.Count != m_managedObjects.Count)
            {
                return false;
            }

            foreach (T obj in m_managedObjects.ContentsAs_Enumerable)
            {
                if (!otherz.IsManagerOf(obj.Uid)) return false;
                if (!otherz.GetManagedObject(obj.Uid).ValueEquals(obj)) return false;
            }

            return true;
        }

        #endregion
    }
}
