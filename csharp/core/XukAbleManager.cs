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
            lock (LOCK)
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
        }



        public T GetManagedObject(string uid)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new exception.MethodParameterIsNullException("uid cannot be null or empty");
            }
            lock (LOCK)
            {
                foreach (T obj in m_managedObjects.ContentsAs_YieldEnumerable)
                {
                    if (obj.Uid == uid) return obj;
                }
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
            lock (LOCK)
            {
                foreach (T objz in m_managedObjects.ContentsAs_YieldEnumerable)
                {
                    if (objz == obj) return objz.Uid;
                }
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

            lock (LOCK)
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
        }

        public virtual void RemoveManagedObject(T obj)
        {
            lock (LOCK)
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
            lock (LOCK)
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
        }


        public bool IsManagerOf(string uid)
        {
            lock (LOCK)
            {
                foreach (T obj in m_managedObjects.ContentsAs_YieldEnumerable)
                {
                    if (obj.Uid == uid) return true;
                }
                return false;
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
