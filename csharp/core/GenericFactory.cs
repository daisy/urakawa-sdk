using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace urakawa
{
    internal class GenericFactory<T> : WithPresentation where T : WithPresentation
    {
        private struct TypeAndQNames
        {
            public string QName;
            public string BaseQName;
            public Type Type;
        }

        protected override void Clear()
        {
            mRegisteredTypeAndQNames.Clear();
            mRegisteredTypes.Clear();
            base.Clear();
        }

        private Dictionary<string, TypeAndQNames> mRegisteredTypeAndQNames = new Dictionary<string, TypeAndQNames>();
        private List<Type> mRegisteredTypes = new List<Type>();

        private string RegisterType(Type t)
        {
            
            if (!typeof(T).IsAssignableFrom(t))
            {
                string msg = String.Format(
                    "Cannot register the type {0} with the factory since it does not implement {1}",
                    t.FullName, typeof (T).FullName);
                throw new exception.OperationNotValidException(msg);
            }
            TypeAndQNames tq = new TypeAndQNames();
            tq.Type = t;
            tq.QName = String.Format(
                "{0}:{1}",
                xuk.XukAble.GetXukNamespaceUri(t),
                t.Name);
            if (mRegisteredTypeAndQNames.ContainsKey(tq.QName))
            {
                if (mRegisteredTypeAndQNames[tq.QName].Type==t) return tq.QName;
                throw new exception.XukQNameNotUniqueWithinFactoryException(
                    tq.QName, t, mRegisteredTypeAndQNames[tq.QName].Type);
            }
            if (typeof(T).IsAssignableFrom(t.BaseType))
            {
                tq.BaseQName = RegisterType(t.BaseType);
            }
            mRegisteredTypeAndQNames.Add(tq.QName, tq);
            mRegisteredTypes.Add(t);
            return tq.QName;
        }

        private bool IsRegistered(Type t)
        {
            return mRegisteredTypes.Contains(t);
        }


        private Type LookupType(string qname)
        {
            if (qname == null) return null;
            if (mRegisteredTypeAndQNames.ContainsKey(qname))
            {
                TypeAndQNames t = mRegisteredTypeAndQNames[qname];
                if (t.Type != null) return t.Type;
                return LookupType(t.BaseQName);
            }
            return null;
        }

        public U Create<U>() where U : T, new()
        {
            U res = new U();
            res.Presentation = Presentation;
            Type t = typeof (U);
            if (!IsRegistered(t)) RegisterType(t);
            return res;
        }

        public T Create(string xukLN, string xukNS)
        {
            string qname = String.Format("{0}:{1}", xukNS, xukLN);
            Type t = LookupType(qname);
            if (t == null) return null;
            ConstructorInfo ci = t.GetConstructor(new Type[] {});
            if (ci!=null)
            {
                T res = ci.Invoke(new object[] {}) as T;
                if (res!=null)
                {
                    res.Presentation = Presentation;
                    return res;
                }
            }
            return null;
        }

        
    }
}
