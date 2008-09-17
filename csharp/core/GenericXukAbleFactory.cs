using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using urakawa.exception;
using urakawa.xuk;

namespace urakawa
{
    ///<summary>
    ///</summary>
    ///<typeparam name="T"></typeparam>
    public class GenericXukAbleFactory<T> : XukAble where T : XukAble
    {
        private class TypeAndQNames
        {
            public QualifiedName QName;
            public QualifiedName BaseQName;
            public AssemblyName AssemblyName;
            public string FullName;
            public Type Type;

            public void ReadFromXmlReader(XmlReader rd)
            {
                QName = new QualifiedName(rd.GetAttribute("XukLocalName"), rd.GetAttribute("XukNamespaceUri") ?? "");
                if (rd.GetAttribute("BaseXukLocalName") != null)
                {
                    BaseQName = new QualifiedName(rd.GetAttribute("BaseXukLocalName"), rd.GetAttribute("BaseXukNamespaceUri") ?? "");
                }
                else
                {
                    BaseQName = null;
                }
                AssemblyName = new AssemblyName(rd.GetAttribute("AssemblyName"));
                if (rd.GetAttribute("AssemblyVersion") != null)
                {
                    AssemblyName.Version = new Version(rd.GetAttribute("AssemblyVersion"));
                }
                FullName = rd.GetAttribute("FullName");
                if (AssemblyName != null && FullName != null)
                {
                    try
                    {
                        Assembly a = Assembly.Load(AssemblyName);
                        try
                        {
                            Type = a.GetType(FullName);
                        }
                        catch (ArgumentException)
                        {
                            Type = null;
                        }
                    }
                    catch (FileLoadException)
                    {
                        Type = null;
                    }
                    catch (FileNotFoundException)
                    {
                        Type = null;
                    }
                }
                else
                {
                    Type = null;
                }
            }
        }


        /// <summary>
        /// Clears the actory of any registered <see cref="Type"/>s
        /// </summary>
        protected override void Clear()
        {
            mRegisteredTypeAndQNames.Clear();
            mRegisteredTypeAndQNamesByQualifiedName.Clear();
            mRegisteredTypeAndQNamesByType.Clear();
            base.Clear();
        }

        private List<TypeAndQNames> mRegisteredTypeAndQNames = new List<TypeAndQNames>();
        private Dictionary<string, TypeAndQNames> mRegisteredTypeAndQNamesByQualifiedName = new Dictionary<string, TypeAndQNames>();
        private Dictionary<Type, TypeAndQNames> mRegisteredTypeAndQNamesByType = new Dictionary<Type, TypeAndQNames>();

        private void RegisterType(TypeAndQNames tq)
        {
            mRegisteredTypeAndQNames.Add(tq);
            mRegisteredTypeAndQNamesByQualifiedName.Add(tq.QName.QName, tq);
            if (tq.Type != null) mRegisteredTypeAndQNamesByType.Add(tq.Type, tq);
        }

        private TypeAndQNames RegisterType(Type t)
        {
            if (!typeof(T).IsAssignableFrom(t))
            {
                string msg = String.Format(
                    "Only Types inheriting {0} can be registered with the factory", typeof(T).FullName);
                throw new MethodParameterIsWrongTypeException(msg);
            }
            TypeAndQNames tq = new TypeAndQNames();
            tq.QName = GetXukQualifiedName(t);
            tq.Type = t;
            tq.FullName = t.FullName;
            tq.AssemblyName = t.Assembly.GetName();
            if (typeof(T).IsAssignableFrom(t.BaseType) && !mRegisteredTypeAndQNamesByType.ContainsKey(t.BaseType))
            {
                tq.BaseQName = RegisterType(t.BaseType).QName;
            }
            RegisterType(tq);
            return tq;
        }

        private bool IsRegistered(Type t)
        {
            return mRegisteredTypeAndQNamesByType.ContainsKey(t);
        }

        private Type LookupType(string qname)
        {
            if (mRegisteredTypeAndQNamesByQualifiedName.ContainsKey(qname))
            {
                TypeAndQNames t = mRegisteredTypeAndQNamesByQualifiedName[qname];
                if (t.Type != null) return t.Type;
                return LookupType(t.BaseQName);
            }
            return null;
        }

        private Type LookupType(QualifiedName qname)
        {
            if (qname == null) return null;
            return LookupType(qname.QName);
        }

        /// <summary>
        /// Inistalizes an created instance  
        /// </summary>
        /// <param name="instance">The instance to initialize</param>
        /// <remarks>
        /// In derived factories, this method can be overridden in order to do additional initialization.
        /// In this case the developer must remember to call <c>base.InitializeInstance(instance)</c>
        /// </remarks>
        protected virtual void InitializeInstance(T instance)
        {
            
        }

        /// <summary>
        /// Creates an instance of type <typeparamref name="U"/>
        /// </summary>
        /// <typeparam name="U">The type of create - must inherit <typeparamref name="T"/>
        /// and have a public constructor with no arguments</typeparam>
        /// <returns>The instance</returns>
        public U Create<U>() where U : T, new()
        {
            U res = new U();
            InitializeInstance(res);
            Type t = typeof(U);
            if (!IsRegistered(t)) RegisterType(t);
            return res;
        }

        /// <summary>
        /// Create an instance of a given <see cref="Type"/>
        /// </summary>
        /// <param name="t">
        /// The <see cref="Type"/> of the instance to create,
        /// cannot be null and must implement <typeparamref name="T"/> and
        /// and have a public constructor with no arguments
        /// </param>
        /// <returns>
        /// The created instance - <c>null</c> if <paramref name="t"/> is not a <typeparamref name="T"/>
        /// or if <paramref name="t"/> has no public constructor with no arguments 
        /// </returns>
        public T Create(Type t)
        {
            if (t == null) throw new MethodParameterIsNullException("Cannot create an instnce of a null Type");
            ConstructorInfo ci = t.GetConstructor(new Type[] { });
            if (ci != null)
            {
                if (!ci.IsPublic) return null;
                T res = ci.Invoke(new object[] { }) as T;
                if (res != null)
                {
                    InitializeInstance(res);
                    if (!IsRegistered(t)) RegisterType(t);
                    return res;
                }
            }
            return null;
        }

        /// <summary>
        /// Creates an instance of a <see cref="Type"/>
        /// registered with a given xuk qualified name
        /// </summary>
        /// <param name="xukLN">The local name part of the xuk qualified name</param>
        /// <param name="xukNS">The lnamespace uri part of the xuk qualified name</param>
        /// <returns>
        /// The instance or <c>null</c> if the given xuk qualified name
        /// is not recognized by the factory as matching a <see cref="Type"/> that can be instantiated
        /// </returns>
        public T Create(string xukLN, string xukNS)
        {
            string qname = String.Format("{0}:{1}", xukNS, xukLN);
            Type t = LookupType(qname);
            if (t == null) return null;
            return Create(t);
        }


        /// <summary>
        /// Write the child elements of a XukAble element.
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, progress.ProgressHandler handler)
        {
            destination.WriteStartElement("mRegisteredTypes", XUK_NS);
            foreach (TypeAndQNames tp in mRegisteredTypeAndQNames)
            {
                destination.WriteStartElement("Type", XUK_NS);
                destination.WriteAttributeString("XukLocalName", tp.QName.LocalName);
                destination.WriteAttributeString("XukNamespaceUri", tp.QName.NamespaceUri);
                if (tp.BaseQName != null)
                {
                    destination.WriteAttributeString("BaseXukLocalName", tp.BaseQName.LocalName);
                    destination.WriteAttributeString("BaseXukNamespaceUri", tp.BaseQName.NamespaceUri);
                }
                if (tp.Type != null)
                {
                    tp.AssemblyName = tp.Type.Assembly.GetName();
                    tp.FullName = tp.Type.FullName;
                }
                if (tp.AssemblyName != null)
                {
                    destination.WriteAttributeString("AssemblyName", tp.AssemblyName.Name);
                    destination.WriteAttributeString("AssemblyVersion", tp.AssemblyName.Version.ToString());
                }
                if (tp.FullName != null) destination.WriteAttributeString("FullName", tp.FullName);
                destination.WriteEndElement();
            }
            destination.WriteEndElement();
            base.XukOutChildren(destination, baseUri, handler);
        }

        /// <summary>
        /// Reads a child of a XukAble xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler of progress</param>
        protected override void XukInChild(XmlReader source, progress.ProgressHandler handler)
        {
            if (source.LocalName == "mRegisteredTypes" && source.NamespaceURI == XUK_NS)
            {
                XukInRegisteredTypes(source);
                return;
            }
            base.XukInChild(source, handler);
        }

        private void XukInRegisteredTypes(XmlReader source)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        if (source.LocalName == "Type" && source.NamespaceURI == XUK_NS)
                        {
                            TypeAndQNames tq = new TypeAndQNames();
                            tq.ReadFromXmlReader(source);
                            RegisterType(tq);
                        }
                        if (!source.IsEmptyElement)
                        {
                            source.ReadSubtree().Close();
                        }
                    }
                    else if (source.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }
                    if (source.EOF) throw new XukException("Unexpectedly reached EOF");
                }
            }
        }
    }
}
