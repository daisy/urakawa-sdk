using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using AudioLib;
using urakawa.exception;
using urakawa.xuk;

namespace urakawa
{
    public class TypeAndQNames
    {
        private QualifiedName m_QName;
        public QualifiedName QName
        {
            get { return m_QName; }
            set { m_QName = value; }
        }

        private QualifiedName m_BaseQName;
        public QualifiedName BaseQName
        {
            get { return m_BaseQName; }
            set { m_BaseQName = value; }
        }

        public AssemblyName AssemblyName;
        public string ClassName;
        public Type Type;
    }


    public abstract class GenericXukAbleFactory<T> : XukAble where T : XukAble
    {
        private static readonly UglyPrettyName XukLocalName_NAME = new UglyPrettyName("name", "XukLocalName");
        private static readonly UglyPrettyName XukNamespaceUri_NAME = new UglyPrettyName("ns", "XukNamespaceUri");
        private static readonly UglyPrettyName BaseXukLocalName_NAME = new UglyPrettyName("baseName", "BaseXukLocalName");
        private static readonly UglyPrettyName BaseXukNamespaceUri_NAME = new UglyPrettyName("baseNs", "BaseXukNamespaceUri");

        private static readonly UglyPrettyName AssemblyName_NAME = new UglyPrettyName("assbly", "AssemblyName");
        private static readonly UglyPrettyName AssemblyVersion_NAME = new UglyPrettyName("assblyVer", "AssemblyVersion");
        private static readonly UglyPrettyName FullName_NAME = new UglyPrettyName("fName", "FullName");

        private static readonly UglyPrettyName Type_NAME = new UglyPrettyName("type", "Type");
        private static readonly UglyPrettyName RegisteredTypes_NAME = new UglyPrettyName("types", "RegisteredTypes");

        /// <summary>
        /// Clears the actory of any registered <see cref="Type"/>s
        /// </summary>
        protected override void Clear()
        {
            mRegisteredTypeAndQNames.Clear();
            base.Clear();
        }

        private List<TypeAndQNames> mRegisteredTypeAndQNames = new List<TypeAndQNames>();

        public IEnumerable<TypeAndQNames> EnumerateRegisteredTypeAndQNames()
        {
            foreach (TypeAndQNames tq in mRegisteredTypeAndQNames)
            {
                yield return tq;
            }

            yield break;
        }

        private TypeAndQNames typeAlreadyRegistered(Type t)
        {
            foreach (TypeAndQNames typeAndQNames in mRegisteredTypeAndQNames)
            {
                if (t == typeAndQNames.Type)
                {
                    return typeAndQNames;
                }
            }

            return null;
        }

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
            DebugFix.Assert(tq.Type != null);

            bool isTypeAlreadyRegistered = false;
            if (tq.Type != null)
            {
                isTypeAlreadyRegistered = typeAlreadyRegistered(tq.Type) != null;
            }

            foreach (TypeAndQNames typeAndQNames in mRegisteredTypeAndQNames)
            {
                // Does this QName resolve an existing registered BaseQName?
                if (typeAndQNames.BaseQName != null

                    && (string.IsNullOrEmpty(typeAndQNames.BaseQName.LocalName.Ugly)
                    || string.IsNullOrEmpty(typeAndQNames.BaseQName.LocalName.Pretty))

                    && typeAndQNames.BaseQName.NamespaceUri == tq.QName.NamespaceUri

                    && (tq.QName.LocalName.Ugly != null && typeAndQNames.BaseQName.LocalName.Ugly == tq.QName.LocalName.Ugly
                    || tq.QName.LocalName.Pretty != null && typeAndQNames.BaseQName.LocalName.Pretty == tq.QName.LocalName.Pretty)
                    )
                {
#if DEBUG
                    Debugger.Break();
#endif
                    string ugly = typeAndQNames.BaseQName.LocalName.Ugly ?? tq.QName.LocalName.Ugly;
                    string pretty = typeAndQNames.BaseQName.LocalName.Pretty ?? tq.QName.LocalName.Pretty;

                    DebugFix.Assert(!string.IsNullOrEmpty(ugly));
                    DebugFix.Assert(!string.IsNullOrEmpty(pretty));

                    typeAndQNames.BaseQName = new QualifiedName(new UglyPrettyName(ugly, pretty), typeAndQNames.BaseQName.NamespaceUri);
                }

                // Can this BaseQName be resolved by an existing registered QName?
                if (tq.BaseQName != null

                    && (string.IsNullOrEmpty(tq.BaseQName.LocalName.Ugly)
                    || string.IsNullOrEmpty(tq.BaseQName.LocalName.Pretty))

                    && tq.BaseQName.NamespaceUri == typeAndQNames.QName.NamespaceUri

                    && (typeAndQNames.QName.LocalName.Ugly != null && tq.BaseQName.LocalName.Ugly == typeAndQNames.QName.LocalName.Ugly
                    || typeAndQNames.QName.LocalName.Pretty != null && tq.BaseQName.LocalName.Pretty == typeAndQNames.QName.LocalName.Pretty)
                    )
                {
#if DEBUG
                    Debugger.Break();
#endif
                    string ugly = tq.BaseQName.LocalName.Ugly ?? typeAndQNames.QName.LocalName.Ugly;
                    string pretty = tq.BaseQName.LocalName.Pretty ?? typeAndQNames.QName.LocalName.Pretty;

                    DebugFix.Assert(!string.IsNullOrEmpty(ugly));
                    DebugFix.Assert(!string.IsNullOrEmpty(pretty));

                    tq.BaseQName = new QualifiedName(new UglyPrettyName(ugly, pretty), tq.BaseQName.NamespaceUri);
                }
            }

            //DebugFix.Assert(!isTypeAlreadyRegistered);

            if (!isTypeAlreadyRegistered)
            {
                mRegisteredTypeAndQNames.Add(tq);
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

        tryAgain:
            if (t.BaseType != null && typeof(T).IsAssignableFrom(t.BaseType)) // && t.BaseType != typeof(T))
            {
                if (t.BaseType.IsAbstract)
                {
                    t = t.BaseType;
                    goto tryAgain;
                }

                TypeAndQNames existing = typeAlreadyRegistered(t.BaseType);
                if (existing == null)
                {
                    QualifiedName qCheck = XukAble.GetXukQualifiedName(t.BaseType);
                    if (string.IsNullOrEmpty(qCheck.NamespaceUri)
                        || qCheck.LocalName == null
                        || string.IsNullOrEmpty(qCheck.LocalName.Pretty)
                        || string.IsNullOrEmpty(qCheck.LocalName.Ugly))
                    {
#if DEBUG
                        Debugger.Break();
#endif
                        t = t.BaseType;
                        goto tryAgain;
                    }

                    existing = RegisterType(t.BaseType);
                }

                if (existing != null)
                {
                    tq.BaseQName = new QualifiedName(
                        new UglyPrettyName(existing.QName.LocalName.Ugly, existing.QName.LocalName.Pretty),
                        existing.QName.NamespaceUri);
                }
            }

            RegisterType(tq);
            return tq;
        }

        private TypeAndQNames LookupTypeAndQNames(QualifiedName qname)
        {
            foreach (TypeAndQNames typeAndQNames in mRegisteredTypeAndQNames)
            {
                if (typeAndQNames.QName.NamespaceUri == qname.NamespaceUri
                    && (typeAndQNames.QName.LocalName.Ugly == qname.LocalName.Ugly
                    || typeAndQNames.QName.LocalName.Pretty == qname.LocalName.Pretty)
                    )
                {
                    return typeAndQNames;
                }
            }
#if DEBUG
            Debugger.Break();
#endif
            return null;
        }

        private Type LookupType(QualifiedName qname)
        {
            if (qname == null)
            {
#if DEBUG
                Debugger.Break();
#endif
                return null;
            }

            TypeAndQNames typeAndQNames = LookupTypeAndQNames(qname);
            if (typeAndQNames != null)
            {
                if (typeAndQNames.Type != null)
                {
                    return typeAndQNames.Type;
                }
#if DEBUG
                Debugger.Break();
#endif
                return LookupType(typeAndQNames.BaseQName);
            }
#if DEBUG
            Debugger.Break();
#endif
            return null;
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
            if (typeAlreadyRegistered(t) == null)
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
                    if (typeAlreadyRegistered(t) == null)
                    {
                        RegisterType(t);
                    }
                    return res;
                }
            }
            return null;
        }

        public T Create(string xukLN, string xukNS)
        {
            //string qname = String.Format("{0}:{1}", xukNS, xukLN);
            UglyPrettyName name = new UglyPrettyName(!PrettyFormat ? xukLN : null, PrettyFormat ? xukLN : null);
            QualifiedName qname = new QualifiedName(name, xukNS);

            Type t = LookupType(qname);
            if (t == null)
            {
#if DEBUG
                Debugger.Break();
#endif
                return null;
            }

            T obj = Create(t);

            TypeAndQNames tt = LookupTypeAndQNames(qname);
            DebugFix.Assert(tt != null);
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
                destination.WriteStartElement(Type_NAME.z(PrettyFormat), XukAble.XUK_NS);
                destination.WriteAttributeString(XukLocalName_NAME.z(PrettyFormat), tp.QName.LocalName.z(PrettyFormat));
                destination.WriteAttributeString(XukNamespaceUri_NAME.z(PrettyFormat), tp.QName.NamespaceUri);
                if (tp.BaseQName != null)
                {
                    DebugFix.Assert(!string.IsNullOrEmpty(tp.BaseQName.LocalName.Pretty)
                        || !string.IsNullOrEmpty(tp.BaseQName.LocalName.Ugly));

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
                    destination.WriteAttributeString(AssemblyName_NAME.z(PrettyFormat), tp.AssemblyName.Name);
                    destination.WriteAttributeString(AssemblyVersion_NAME.z(PrettyFormat), tp.AssemblyName.Version.ToString());
                }
                if (tp.ClassName != null)
                {
                    destination.WriteAttributeString(FullName_NAME.z(PrettyFormat), tp.ClassName);
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
            destination.WriteStartElement(RegisteredTypes_NAME.z(PrettyFormat), XukAble.XUK_NS);

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
            if (RegisteredTypes_NAME.Match(source.LocalName)
                && source.NamespaceURI == XukAble.XUK_NS)
            {
                XukInRegisteredTypes(source, handler);
                return;
            }
            base.XukInChild(source, handler);
        }

        private void readTypeAndQNamesFromXmlReader(TypeAndQNames tq, XmlReader rd, bool pretty)
        {
            tq.AssemblyName = new AssemblyName(ReadXukAttribute(rd, AssemblyName_NAME));

            if (ReadXukAttribute(rd, AssemblyVersion_NAME) != null)
            {
                tq.AssemblyName.Version = new Version(ReadXukAttribute(rd, AssemblyVersion_NAME));
            }

            tq.ClassName = ReadXukAttribute(rd, FullName_NAME);

            if (tq.AssemblyName != null && tq.ClassName != null)
            {
                try
                {
                    //Assembly a = Assembly.Load(tq.AssemblyName);
#if NET6_0_OR_GREATER
                    Assembly a = Assembly.Load(tq.AssemblyName.Name);
#else
                    Assembly a = Assembly.Load(tq.AssemblyName);
#endif
                    try
                    {
                        tq.Type = a.GetType(tq.ClassName);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                            Debugger.Break();
#endif
                        Console.WriteLine("ClassName: " + tq.ClassName);
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);

                        tq.Type = null;
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                        Debugger.Break();
#endif
                    Console.WriteLine("AssemblyName: " + tq.AssemblyName);
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    tq.Type = null;
                }
            }
            else
            {
#if DEBUG
                    Debugger.Break();
#endif
                Console.WriteLine("Type XukIn error?!");
                Console.WriteLine("AssemblyName: " + tq.AssemblyName);
                Console.WriteLine("ClassName: " + tq.ClassName);

                tq.Type = null;
            }

            string xukLocalName = ReadXukAttribute(rd, XukLocalName_NAME);

            UglyPrettyName name = null;

            if (tq.Type != null)
            {
                UglyPrettyName nameCheck = GetXukName(tq.Type);
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

            tq.QName = new QualifiedName(
                name,
                ReadXukAttribute(rd, XukNamespaceUri_NAME) ?? "");

            string baseXukLocalName = ReadXukAttribute(rd, BaseXukLocalName_NAME);
            if (!string.IsNullOrEmpty(baseXukLocalName))
            {
                UglyPrettyName nameBase = new UglyPrettyName(
                !pretty ? baseXukLocalName : null,
                pretty ? baseXukLocalName : null);

                tq.BaseQName = new QualifiedName(
                    nameBase,
                    ReadXukAttribute(rd, BaseXukNamespaceUri_NAME) ?? "");
            }
            else
            {
                tq.BaseQName = null;
            }
        }
    
        protected void XukInRegisteredType(XmlReader source)
        {
            if (Type_NAME.Match(source.LocalName)
                && source.NamespaceURI == XukAble.XUK_NS)
            {
                TypeAndQNames tq = new TypeAndQNames();

                readTypeAndQNamesFromXmlReader(tq, source, PrettyFormat);

                if (tq.Type == null)
                {
#if DEBUG
                    Debugger.Break();
#endif
                    RegisterType(tq);
                }
                else
                {
                    TypeAndQNames tq_ = RegisterType(tq.Type);

                    DebugFix.Assert(tq_.AssemblyName.Name == tq.AssemblyName.Name);
                    //DebugFix.Assert(tq_.AssemblyName.Version == tq.AssemblyName.Version);

                    DebugFix.Assert(tq_.ClassName == tq.ClassName);
                    DebugFix.Assert(tq_.Type == tq.Type);

                    DebugFix.Assert(tq_.QName.NamespaceUri == tq.QName.NamespaceUri);

                    DebugFix.Assert(tq_.QName.LocalName.Ugly == tq.QName.LocalName.Ugly);
                    DebugFix.Assert(tq_.QName.LocalName.Pretty == tq.QName.LocalName.Pretty);

                    if (tq_.BaseQName != null && tq.BaseQName != null)
                    {
                        DebugFix.Assert(tq_.BaseQName.NamespaceUri == tq.BaseQName.NamespaceUri);

                        if (!String.IsNullOrEmpty(tq.BaseQName.LocalName.Ugly))
                        {
                            DebugFix.Assert(tq_.BaseQName.LocalName.Ugly == tq.BaseQName.LocalName.Ugly);
                        }

                        if (!String.IsNullOrEmpty(tq.BaseQName.LocalName.Pretty))
                        {
                            DebugFix.Assert(tq_.BaseQName.LocalName.Pretty == tq.BaseQName.LocalName.Pretty);
                        }
                    }
                }
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
