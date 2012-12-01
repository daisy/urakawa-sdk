using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using AudioLib;
using urakawa.exception;
using urakawa.xuk;

namespace urakawa
{
    ///<summary>
    ///</summary>
    ///<typeparam name="T"></typeparam>
    public abstract class GenericXukAbleFactory<T> : XukAble where T : XukAble
    {
        private static readonly UglyPrettyName XukLocalName_NAME = new UglyPrettyName("name", "XukLocalName");
        private static readonly UglyPrettyName XukNamespaceUri_NAME = new UglyPrettyName("ns", "XukNamespaceUri");
        private static readonly UglyPrettyName BaseXukLocalName_NAME = new UglyPrettyName("baseName", "BaseXukLocalName");
        private static readonly UglyPrettyName BaseXukNamespaceUri_NAME = new UglyPrettyName("baseNs", "BaseXukNamespaceUri");

        private class TypeAndQNames
        {
            public QualifiedName QName;
            public QualifiedName BaseQName;
            public AssemblyName AssemblyName;
            public string ClassName;
            public Type Type;

            public void ReadFromXmlReader(XmlReader rd, bool pretty)
            {
                AssemblyName = new AssemblyName(rd.GetAttribute(XukStrings.AssemblyName));

                if (rd.GetAttribute(XukStrings.AssemblyVersion) != null)
                {
                    AssemblyName.Version = new Version(rd.GetAttribute(XukStrings.AssemblyVersion));
                }

                ClassName = rd.GetAttribute(XukStrings.FullName);

                if (AssemblyName != null && ClassName != null)
                {
                    try
                    {
                        Assembly a = Assembly.Load(AssemblyName);
                        try
                        {
                            Type = a.GetType(ClassName);
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            Debugger.Break();
#endif
                            Console.WriteLine("ClassName: " + ClassName);
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);

                            Type = null;
                        }
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Debugger.Break();
#endif
                        Console.WriteLine("AssemblyName: " + AssemblyName);
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                        Type = null;
                    }
                }
                else
                {
#if DEBUG
                    Debugger.Break();
#endif
                    Console.WriteLine("Type XukIn error?!");
                    Console.WriteLine("AssemblyName: " + AssemblyName);
                    Console.WriteLine("ClassName: " + ClassName);

                    Type = null;
                }

                string xukLocalName = readXmlAttribute(rd, XukLocalName_NAME);

                UglyPrettyName name = null;

                if (Type != null)
                {
                    UglyPrettyName nameCheck = GetXukName(Type);
                    DebugFix.Assert(nameCheck != null);

                    if (nameCheck != null)
                    {
                        if (pretty)
                        {
                            DebugFix.Assert(xukLocalName == nameCheck.Pretty);
                        }
                        else
                        {
                            DebugFix.Assert(xukLocalName == nameCheck.Ugly);
                        }

                        name = nameCheck;
                    }
                }

                DebugFix.Assert(name != null);
                if (name == null)
                {
                    name = new UglyPrettyName(
                     !pretty ? xukLocalName : null,
                     pretty ? xukLocalName : null);
                }

                QName = new QualifiedName(
                    name,
                    readXmlAttribute(rd, XukNamespaceUri_NAME) ?? "");

                string baseXukLocalName = readXmlAttribute(rd, BaseXukLocalName_NAME);
                if (!string.IsNullOrEmpty(baseXukLocalName))
                {
                    UglyPrettyName nameBase = new UglyPrettyName(
                    !pretty ? baseXukLocalName : null,
                    pretty ? baseXukLocalName : null);

                    BaseQName = new QualifiedName(
                        nameBase,
                        readXmlAttribute(rd, BaseXukNamespaceUri_NAME) ?? "");
                }
                else
                {
                    BaseQName = null;
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

        //public void RefreshQNames()
        //{
        //    foreach (TypeAndQNames tq in mRegisteredTypeAndQNames)
        //    {
        //        if (tq.Type != null)
        //        {
        //            tq.QName = GetXukQualifiedName(tq.Type);

        //            if (tq.BaseQName != null
        //                && tq.Type.BaseType != null)
        //            {
        //                tq.BaseQName = GetXukQualifiedName(tq.Type.BaseType);
        //            }
        //        }
        //    }
        //    {
        //        Dictionary<string, TypeAndQNames> newDict = new Dictionary<string, TypeAndQNames>();
        //        Dictionary<string, TypeAndQNames>.Enumerator enu = mRegisteredTypeAndQNamesByQualifiedName.GetEnumerator();
        //        while (enu.MoveNext())
        //        {
        //            KeyValuePair<string, TypeAndQNames> pair = enu.Current;
        //            TypeAndQNames tq = new TypeAndQNames();
        //            if (pair.Value.Type != null)
        //            {
        //                tq.QName = GetXukQualifiedName(pair.Value.Type);
        //                tq.Type = pair.Value.Type;
        //                tq.ClassName = pair.Value.Type.FullName;
        //                tq.AssemblyName = pair.Value.Type.Assembly.GetName();
        //                if (pair.Value.BaseQName != null)
        //                {
        //                    tq.BaseQName = GetXukQualifiedName(pair.Value.Type.BaseType);
        //                }
        //            }
        //            else
        //            {
        //                tq.QName = new QualifiedName(pair.Value.QName.LocalName, pair.Value.QName.NamespaceUri);
        //                tq.Type = null;
        //                tq.ClassName = pair.Value.ClassName;
        //                tq.AssemblyName = pair.Value.AssemblyName;
        //                if (pair.Value.BaseQName != null)
        //                {
        //                    tq.BaseQName = new QualifiedName(pair.Value.BaseQName.LocalName, pair.Value.BaseQName.NamespaceUri);
        //                }
        //            }
        //            newDict.Add(tq.QName.FullyQualifiedName, tq);
        //        }
        //        mRegisteredTypeAndQNamesByQualifiedName.Clear();
        //        mRegisteredTypeAndQNamesByQualifiedName = newDict;
        //    }

        //    {
        //        Dictionary<Type, TypeAndQNames> newDict = new Dictionary<Type, TypeAndQNames>();
        //        Dictionary<Type, TypeAndQNames>.Enumerator enu = mRegisteredTypeAndQNamesByType.GetEnumerator();
        //        while (enu.MoveNext())
        //        {
        //            KeyValuePair<Type, TypeAndQNames> pair = enu.Current;
        //            TypeAndQNames tq = new TypeAndQNames();

        //            if (pair.Value.Type != null)
        //            {
        //                tq.QName = GetXukQualifiedName(pair.Value.Type);
        //                tq.Type = pair.Value.Type;
        //                tq.ClassName = pair.Value.Type.FullName;
        //                tq.AssemblyName = pair.Value.Type.Assembly.GetName();
        //                if (pair.Value.BaseQName != null)
        //                {
        //                    tq.BaseQName = GetXukQualifiedName(pair.Value.Type.BaseType);
        //                }
        //                newDict.Add(pair.Value.Type, tq);
        //            }
        //        }
        //        mRegisteredTypeAndQNamesByType.Clear();
        //        mRegisteredTypeAndQNamesByType = newDict;
        //    }
        //}

        private void RegisterType(TypeAndQNames tq)
        {
            mRegisteredTypeAndQNames.Add(tq);
            mRegisteredTypeAndQNamesByQualifiedName.Add(tq.QName.GetFlatString(PrettyFormat), tq);
            if (tq.Type != null)
            {
                mRegisteredTypeAndQNamesByType.Add(tq.Type, tq);
            }
        }

        private TypeAndQNames RegisterType(Type t)
        {
            if (!typeof(T).IsAssignableFrom(t))
            {
                string msg = String.Format(
                    "Only Types inheriting {0} can be registered with the factory", typeof(T).FullName);
                throw new MethodParameterIsWrongTypeException(msg);
            }
            if (t.IsAbstract)
            {
                string msg = String.Format(
                    "The abstract Type {0} cannot be registered with the factory", t.FullName);
                throw new MethodParameterIsWrongTypeException(msg);
            }
            TypeAndQNames tq = new TypeAndQNames();
            tq.QName = GetXukQualifiedName(t);
            tq.Type = t;
            tq.ClassName = t.FullName;
            tq.AssemblyName = t.Assembly.GetName();

            if (typeof(T).IsAssignableFrom(t.BaseType)
                && !mRegisteredTypeAndQNamesByType.ContainsKey(t.BaseType)
                && !t.BaseType.IsAbstract)
            {
                tq.BaseQName = RegisterType(t.BaseType).QName;
            }
            RegisterType(tq);
            return tq;
        }

        private Type LookupType(string qname)
        {
            if (string.IsNullOrEmpty(qname))
            {
                return null;
            }

            TypeAndQNames obj;

            if (mRegisteredTypeAndQNamesByQualifiedName.TryGetValue(qname, out obj)
                && obj != null) //mRegisteredTypeAndQNamesByQualifiedName.ContainsKey(qname))
            {
                // obj = mRegisteredTypeAndQNamesByQualifiedName[qname];
                if (obj.Type != null)
                {
                    return obj.Type;
                }

                return LookupType(obj.BaseQName);
            }

            return null;
        }

        private Type LookupType(QualifiedName qname)
        {
            if (qname == null)
            {
                return null;
            }
            return LookupType(qname.GetFlatString(PrettyFormat));
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
            // subclasses do extra work, if needed.
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
            if (!mRegisteredTypeAndQNamesByType.ContainsKey(t))
            {
                RegisterType(t);
            }
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
            if (t == null)
            {
                throw new MethodParameterIsNullException("Cannot create an instnce of a null Type");
            }
            ConstructorInfo ci = t.GetConstructor(new Type[] { });
            if (ci != null)
            {
                if (!ci.IsPublic)
                {
                    return null;
                }
                T res = ci.Invoke(new object[] { }) as T;
                if (res != null)
                {
                    InitializeInstance(res);
                    if (!mRegisteredTypeAndQNamesByType.ContainsKey(t))
                    {
                        RegisterType(t);
                    }
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
            if (t == null)
            {
                return null;
            }
            T obj = Create(t);
            TypeAndQNames tt = mRegisteredTypeAndQNamesByQualifiedName[qname];
            if (tt.Type == null)
            {
                // not real type of qname => type of first available ancestor in the type inheritance chain.
                obj.MissingTypeOriginalXukedName = tt.QName;
            }
            return obj;
        }

        protected void XukOutRegisteredTypes(XmlWriter destination, Uri baseUri, progress.IProgressHandler handler)
        {
            foreach (TypeAndQNames tp in mRegisteredTypeAndQNames)
            {
                destination.WriteStartElement(XukStrings.Type, XukAble.XUK_NS);
                destination.WriteAttributeString(XukLocalName_NAME.z(PrettyFormat), tp.QName.LocalName.z(PrettyFormat));
                destination.WriteAttributeString(XukNamespaceUri_NAME.z(PrettyFormat), tp.QName.NamespaceUri);
                if (tp.BaseQName != null)
                {
                    destination.WriteAttributeString(BaseXukLocalName_NAME.z(PrettyFormat), tp.BaseQName.LocalName.z(PrettyFormat));
                    destination.WriteAttributeString(BaseXukNamespaceUri_NAME.z(PrettyFormat), tp.BaseQName.NamespaceUri);
                }
                if (tp.Type != null)
                {
                    tp.AssemblyName = tp.Type.Assembly.GetName();
                    tp.ClassName = tp.Type.FullName;
                }
                if (tp.AssemblyName != null)
                {
                    destination.WriteAttributeString(XukStrings.AssemblyName, tp.AssemblyName.Name);
                    destination.WriteAttributeString(XukStrings.AssemblyVersion, tp.AssemblyName.Version.ToString());
                }
                if (tp.ClassName != null)
                {
                    destination.WriteAttributeString(XukStrings.FullName, tp.ClassName);
                }
                destination.WriteEndElement();
            }
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
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, progress.IProgressHandler handler)
        {
            destination.WriteStartElement(XukStrings.RegisteredTypes, XukAble.XUK_NS);

            XukOutRegisteredTypes(destination, baseUri, handler);

            destination.WriteEndElement();

            base.XukOutChildren(destination, baseUri, handler);
        }

        /// <summary>
        /// Reads a child of a XukAble xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler of progress</param>
        protected override void XukInChild(XmlReader source, progress.IProgressHandler handler)
        {
            if (source.LocalName == XukStrings.RegisteredTypes && source.NamespaceURI == XukAble.XUK_NS)
            {
                XukInRegisteredTypes(source, handler);
                return;
            }
            base.XukInChild(source, handler);
        }

        protected void XukInRegisteredType(XmlReader source)
        {
            if (source.LocalName == XukStrings.Type && source.NamespaceURI == XukAble.XUK_NS)
            {
                TypeAndQNames tq = new TypeAndQNames();
                tq.ReadFromXmlReader(source, PrettyFormat);
                RegisterType(tq);
            }
            if (!source.IsEmptyElement)
            {
                source.ReadSubtree().Close();
            }
        }

        protected void XukInRegisteredTypes(XmlReader source, progress.IProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        XukInRegisteredType(source);
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
