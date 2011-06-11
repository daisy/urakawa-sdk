using System;
using System.Collections.Generic;
using System.Diagnostics;
using urakawa.exception;
using urakawa.xuk;

namespace urakawa
{
    public abstract class XukAbleManager<T> : WithPresentation where T : WithPresentation
    {
        private readonly Object LOCK = new object();
        private ObjectListProvider<T> m_managedObjects;

        // Any UID on and after this index is free to use.
        private ulong m_UidIndex = 0;

        public XukAbleManager(Presentation pres, string uidPrefix)
        {
            Presentation = pres;
            m_managedObjects = new ObjectListProvider<T>(this, false);
            m_UidPrefix = uidPrefix;
            m_UidFormat = m_UidPrefix + "{0:00000}";
        }

        private readonly string m_UidPrefix;
        public string UidPrefix
        {
            get { return m_UidPrefix; }
        }

        private readonly string m_UidFormat;
        public string UidFormat
        {
            get
            {
                return m_UidFormat;
            }
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
            m_UidIndex = 0;
            m_UidHashCollisions.Clear();

            foreach (T obj in m_managedObjects.ContentsAs_Enumerable)
            {
                obj.Uid = String.Format(UidFormat, m_UidIndex++);
            }

            if (!XukAble.UsePrefixedIntUniqueHashCodes)
            {
#if DEBUG
                Debugger.Break();
#endif //DEBUG

                foreach (T obj in m_managedObjects.ContentsAs_Enumerable)
                {
                    CheckUidHashCollision(obj);
                }
            }

            //{
            //    ulong index = 0;

            //    List<T> localList = m_managedObjects.ContentsAs_ListCopy;

            //    foreach (T obj in localList)
            //    {
            //        m_managedObjects.Remove(obj);
            //    }

            //    foreach (T obj in localList)
            //    {
            //        string newUid = Presentation.GetNewUid(m_UidPrefix, ref index);
            //        obj.Uid = newUid;
            //        m_managedObjects.Insert(m_managedObjects.Count, obj);
            //    }
            //}
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

        private List<int> m_UidHashCollisions = new List<int>(1);
        private void CheckUidHashCollision(int uidHash)
        {
            foreach (T obj in m_managedObjects.ContentsAs_Enumerable)
            {
                if (uidHash == obj.UidHash)
                {
                    if (!m_UidHashCollisions.Contains(uidHash))
                    {
                        m_UidHashCollisions.Add(uidHash);
                    }
                    return;
                }
            }
        }
        private void CheckUidHashCollision(XukAble xukAble)
        {
            foreach (T obj in m_managedObjects.ContentsAs_Enumerable)
            {
                if (obj == xukAble) continue;

                if (xukAble.UidHash == obj.UidHash)
                {
                    if (!m_UidHashCollisions.Contains(xukAble.UidHash))
                    {
                        m_UidHashCollisions.Add(xukAble.UidHash);
                    }
                    return;
                }
            }
        }

        private void CheckUidHashCollision_(int uidHash)
        {
            if (!m_UidHashCollisions.Contains(uidHash)) return;

            uint nMatches = 0;
            foreach (T objz in m_managedObjects.ContentsAs_Enumerable)
            {
                if (objz.UidHash == uidHash)
                {
                    nMatches++;
                }
            }
            if (nMatches <= 1)
            {
                m_UidHashCollisions.Remove(uidHash);
            }
        }

        public bool UidEquals(XukAble xukAble, string uid, int uidHash)
        {
            if (!XukAble.UsePrefixedIntUniqueHashCodes && m_UidHashCollisions.Contains(xukAble.UidHash))
            {
#if DEBUG
                Debugger.Break();
#endif //DEBUG

                return xukAble.Uid == uid;
            }

            return xukAble.UidHash == uidHash;

            //return (object.ReferenceEquals(xukAble.Uid, string.Intern(uid)));
        }

        private T GetManagedObject_NO_LOCK(string uid, int uidHash)
        {
            foreach (T obj in m_managedObjects.ContentsAs_Enumerable)
            {
                if (UidEquals(obj, uid, uidHash)) return obj;
            }

            throw new exception.IsNotManagerOfException(String.Format(
                                                                     "The manager does not manage an object with uid {0}",
                                                                     uid));
        }
        public T GetManagedObject(string uid)
        {
            int uidHash = XukAble.GetHashCode(uid);
            return GetManagedObject(uid, uidHash);
        }

        public T GetManagedObject(string uid, int uidHash)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new exception.MethodParameterIsNullException("uid cannot be null or empty");
            }
            if (m_managedObjects.UseLock)
            {
                lock (LOCK)
                {
                    return GetManagedObject_NO_LOCK(uid, uidHash);
                }
            }
            else
            {
                return GetManagedObject_NO_LOCK(uid, uidHash);
            }
        }

        //private string GetUidOfManagedObject_NO_LOCK(T obj)
        //{
        //    foreach (T objz in m_managedObjects.ContentsAs_Enumerable)
        //    {
        //        if (objz == obj) return objz.Uid;
        //    }

        //    throw new exception.IsNotManagerOfException("The given object is not managed by this Manager");
        //}

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


        //public void SetUidOfManagedObject_NO_LOCK(T obj, string uid)
        //{
        //    foreach (T objz in m_managedObjects.ContentsAs_Enumerable)
        //    {
        //        if (UidEquals(objz, uid) && objz != obj)
        //        {
        //            throw new exception.ObjectIsAlreadyManagedException(
        //                String.Format("Another managed object exists with uid {0}", uid));
        //        }
        //        if (objz == obj)
        //        {
        //            if (UidEquals(objz, obj.Uid))
        //            {
        //                return;
        //            }

        //            CheckUidHashCollision_(obj.UidHash);
        //            obj.Uid = uid;
        //            CheckUidHashCollision(obj.UidHash);
        //            return;
        //        }
        //    }

        //    throw new exception.IsNotManagerOfException("The given object is not managed by this Manager");
        //}


        //public void SetUidOfManagedObject(T obj, string uid)
        //{
        //    if (obj == null)
        //    {
        //        throw new exception.MethodParameterIsNullException("channel parameter is null");
        //    }

        //    if (string.IsNullOrEmpty(uid))
        //    {
        //        throw new exception.MethodParameterIsEmptyStringException("uid parameter cannot be null or empty string");
        //    }
        //    if (m_managedObjects.UseLock)
        //    {
        //        lock (LOCK)
        //        {
        //            SetUidOfManagedObject_NO_LOCK(obj, uid);
        //        }
        //    }
        //    else
        //    {
        //        SetUidOfManagedObject_NO_LOCK(obj, uid);
        //    }
        //}

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

                if (!XukAble.UsePrefixedIntUniqueHashCodes)
                {
#if DEBUG
                    Debugger.Break();
#endif //DEBUG

                    CheckUidHashCollision_(objectToRemove.UidHash);
                }
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
            //AddManagedObject(obj, Presentation.GetNewUid(m_UidPrefix, ref m_UidIndex));
            AddManagedObject(obj, String.Format(UidFormat, m_UidIndex++));
        }

        public abstract bool CanAddManagedObject(T managedObject);
        private void AddManagedObject_NO_LOCK(T obj, string uid, bool safetyChecks)
        {
            obj.Uid = uid;

            if (!XukAble.UsePrefixedIntUniqueHashCodes)
            {
#if DEBUG
                Debugger.Break();
#endif //DEBUG
                CheckUidHashCollision(obj.UidHash);
            }

            if (safetyChecks)
            {
                foreach (T objz in m_managedObjects.ContentsAs_Enumerable)
                {
                    if (obj == objz)
                    {
                        throw new exception.ObjectIsAlreadyManagedException(
                            "The given object is already managed by the Manager");
                    }
                    if (UidEquals(objz, obj.Uid, obj.UidHash))
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

            m_managedObjects.Insert(m_managedObjects.Count, obj);
        }

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

        public bool IsManagerOf_NO_LOCK(string uid, int uidHash)
        {
            //int uidHash = XukAble.GetHashCode(uid);

            foreach (T obj in m_managedObjects.ContentsAs_Enumerable)
            {
                if (UidEquals(obj, uid, uidHash)) return true;
            }
            return false;
        }

        public bool IsManagerOf(string uid, int uidHash)
        {
            if (m_managedObjects.UseLock)
            {
                lock (LOCK)
                {
                    return IsManagerOf_NO_LOCK(uid, uidHash);
                }
            }
            else
            {
                return IsManagerOf_NO_LOCK(uid, uidHash);
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
                if (!otherz.IsManagerOf(obj.Uid, obj.UidHash)) return false;
                if (!otherz.GetManagedObject(obj.Uid, obj.UidHash).ValueEquals(obj)) return false;
            }

            return true;
        }

        #endregion
    }
}
