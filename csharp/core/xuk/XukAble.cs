using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using AudioLib;
using urakawa.progress;

namespace urakawa.xuk
{
    [XukNamespaceAttribute("http://www.daisy.org/urakawa/xuk/2.0")]
    public abstract class XukAble : IXukAble
    {
        protected static readonly UglyPrettyName Language_NAME = new UglyPrettyName("lang", "Language");
        protected static readonly UglyPrettyName Name_NAME = new UglyPrettyName("n", "Name");
        protected static readonly UglyPrettyName Value_NAME = new UglyPrettyName("v", "Value");
        protected static readonly UglyPrettyName LocalName_NAME = new UglyPrettyName("n", "LocalName");
        protected static readonly UglyPrettyName NamespaceUri_NAME = new UglyPrettyName("ns", "NamespaceUri");

        public static string ReadXukAttribute(XmlReader xmlReader, UglyPrettyName name)
        {
            string attrValue = xmlReader.GetAttribute(name.Ugly);
            if (attrValue == null) //string.IsNullOrEmpty(attrValue))
            {
                attrValue = xmlReader.GetAttribute(name.Pretty);
            }
            return attrValue;
        }

        [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
        protected sealed class XukNameUglyPrettyAttribute : Attribute
        {
            public readonly UglyPrettyName Name;
            public XukNameUglyPrettyAttribute(string ugly, string pretty)
            {
                Name = new UglyPrettyName(ugly, pretty);
            }
        }

        [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
        protected sealed class XukNamespaceAttribute : Attribute
        {
            public readonly string Namespace;
            public XukNamespaceAttribute(string str)
            {
                Namespace = str;
            }
        }

#if false //DEBUG
        static XukAble()
        {
            Debugger.Break();

            try
            {
                PropertyInfo[] properties = typeof(XukStrings).GetProperties(BindingFlags.Public | BindingFlags.Static);


                DebugFix.Assert(!string.IsNullOrEmpty(XUK_NS));

                List<string> list = new List<string>();

                foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Console.WriteLine(ass.FullName);

                    foreach (Type type in ass.GetTypes())
                    {
                        if (type == typeof(XukAble) || typeof(XukAble).IsAssignableFrom(type))
                        {
                            Console.WriteLine("-----------");
                            Console.WriteLine(type.FullName);


                            Type typeConcrete = type;
                            if (type.ContainsGenericParameters)
                            {
                                DebugFix.Assert(type.IsAbstract);

                                Type[] types = type.GetGenericArguments();
                                DebugFix.Assert(types.Length == 1);

                                typeConcrete = type.MakeGenericType(types[0].BaseType);
                            }

                            FieldInfo[] fields = typeConcrete.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                            foreach (FieldInfo field in fields)
                            {
                                if (field.FieldType != typeof(UglyPrettyName))
                                {
                                    continue;
                                }

                                const string NAME = "_NAME";
                                bool okay = field.Name.EndsWith(NAME);
                                DebugFix.Assert(okay);
                                if (okay)
                                {
                                    Console.WriteLine(".....");
                                    Console.WriteLine(field.Name);

                                    UglyPrettyName name = (UglyPrettyName)field.GetValue(typeConcrete);
                                    string fieldName = field.Name.Substring(0, field.Name.Length - NAME.Length);

                                    Console.WriteLine(fieldName);

                                    PropertyInfo found_ = null;
                                    foreach (PropertyInfo property in properties)
                                    {
                                        if (property.PropertyType == typeof(string)
                                            && property.Name == fieldName)
                                        {
                                            found_ = property;
                                            break;
                                        }
                                    }

                                    DebugFix.Assert(found_ != null);
                                    if (found_ != null)
                                    {
                                        XukStrings.IsPrettyFormat = false;
                                        string uglyCheck = (string)found_.GetValue(typeof(XukStrings), null);
                                        DebugFix.Assert(name.Ugly == uglyCheck);

                                        Console.WriteLine(uglyCheck);


                                        XukStrings.IsPrettyFormat = true;
                                        string prettyCheck = (string)found_.GetValue(typeof(XukStrings), null);
                                        DebugFix.Assert(name.Pretty == prettyCheck);

                                        Console.WriteLine(prettyCheck);
                                    }
                                    Console.WriteLine(".....");
                                }
                            }

                            string ns = GetXukNamespace(type);
                            DebugFix.Assert(!string.IsNullOrEmpty(ns));

                            if (type == typeof(XukAble))
                            {
                                Console.WriteLine(ns);
                            }

                            if (type.FullName.StartsWith("Obi."))
                            {
                                DebugFix.Assert(ns == "http://www.daisy.org/urakawa/obi");
                            }
                            else
                            {
                                DebugFix.Assert(ns == "http://www.daisy.org/urakawa/xuk/2.0");
                            }

                            if (type.IsAbstract)
                            {
                                Console.WriteLine("abstract");
                                continue;
                            }

                            if (type.FullName.StartsWith("Obi."))
                            {
                                if (!type.FullName.StartsWith("Obi.Commands"))
                                {
                                    string pretty_ = GetXukName(type, true);
                                    string ugly_ = GetXukName(type, false);

                                    DebugFix.Assert(!string.IsNullOrEmpty(pretty_));
                                    DebugFix.Assert(!string.IsNullOrEmpty(ugly_));

                                    DebugFix.Assert(pretty_ == ugly_);

                                    Console.WriteLine(pretty_);

                                    if (type.Name == "ObiPresentation")
                                    {
                                        DebugFix.Assert(pretty_ == type.Name);
                                    }
                                    else if (type.Name == "ObiNode")
                                    {
                                        DebugFix.Assert(pretty_ == type.Name);
                                    }
                                    else if (type.Name == "PhraseNode")
                                    {
                                        DebugFix.Assert(pretty_ == "phrase");
                                    }
                                    else if (type.Name == "SectionNode")
                                    {
                                        DebugFix.Assert(pretty_ == "section");
                                    }
                                    else if (type.Name == "ObiRootNode")
                                    {
                                        DebugFix.Assert(pretty_ == "root");
                                    }
                                    else if (type.Name == "EmptyNode")
                                    {
                                        DebugFix.Assert(pretty_ == "empty");
                                    }
                                    else
                                    {
                                        Debugger.Break();
                                    }
                                }

                                continue;
                            }

                            if (type.Name == "DummyCommand")
                            {
                                continue;
                            }

                            string pretty = GetXukName(type, true);
                            if (!string.IsNullOrEmpty(pretty))
                            {
                                Console.WriteLine(pretty);

                                if (list.Contains(pretty))
                                {
                                    Debugger.Break();
                                }
                                else
                                {
                                    list.Add(pretty);
                                }

                                if (type.Name == "CSSExternalFileData")
                                {
                                    DebugFix.Assert(pretty == "CssExternalFileData");
                                }
                                else if (type.Name == "XSLTExternalFileData")
                                {
                                    DebugFix.Assert(pretty == "XsltExternalFileData");
                                }
                                else if (type.Name == "ExternalFilesDataManager")
                                {
                                    DebugFix.Assert(pretty == "ExternalFileDataManager");
                                }
                                else
                                {
                                    DebugFix.Assert(type.Name == pretty);
                                }
                            }
                            else
                            {
                                Debugger.Break();
                            }


                            string ugly = GetXukName(type, false);
                            if (!string.IsNullOrEmpty(ugly))
                            {
                                Console.WriteLine(ugly);

                                if (list.Contains(ugly))
                                {
                                    Debugger.Break();
                                }
                                else
                                {
                                    list.Add(ugly);
                                }
                            }
                            else
                            {
                                Debugger.Break();
                            }


                            PropertyInfo found = null;
                            foreach (PropertyInfo property in properties)
                            {
                                if (property.PropertyType == typeof(string)
                                    && (property.Name == type.Name
                                        ||
                                        (type.Name == "ExternalFilesDataManager"
                                         && property.Name == "ExternalFileDataManager")
                                       ))
                                {
                                    found = property;
                                    break;
                                }
                            }

                            DebugFix.Assert(found != null);
                            if (found != null)
                            {
                                XukStrings.IsPrettyFormat = false;
                                string uglyCheck = (string)found.GetValue(typeof(XukStrings), null);
                                DebugFix.Assert(ugly == uglyCheck);

                                XukStrings.IsPrettyFormat = true;
                                string prettyCheck = (string)found.GetValue(typeof(XukStrings), null);
                                DebugFix.Assert(pretty == prettyCheck);
                            }
                        }
                    }
                }

                // Make sure default is false, to at least open exising projects whilst testing.
                // (for as long as the refactoring goes on to remove dependency on static XukStrings)
                XukStrings.IsPrettyFormat = false;
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }

            Debugger.Break();
        }
#endif

        public static readonly string XUK_NS = GetXukNamespace(typeof(XukAble));

        public static string GetXukNamespace(Type type)
        {
            Attribute attribute = Attribute.GetCustomAttribute(
                type,
                typeof(XukNamespaceAttribute));

            DebugFix.Assert(attribute != null);

            return attribute == null ? string.Empty
                : ((XukNamespaceAttribute)attribute).Namespace;
        }

        private string m_XukNamespace;
        public string GetXukNamespace()
        {
            if (m_XukNamespace == null)
            {
                m_XukNamespace = GetXukNamespace(GetType());
            }
            return m_XukNamespace;
        }

        public string GetXukName()
        {
            UglyPrettyName xukName = GetXukName_();
            if (xukName == null)
            {
                return null;
            }
            return PrettyFormat ? xukName.Pretty : xukName.Ugly;
        }

        private UglyPrettyName m_XukName;
        public UglyPrettyName GetXukName_()
        {
            if (m_XukName == null)
            {
                m_XukName = GetXukName(GetType());
            }
            return m_XukName;
        }

        public static UglyPrettyName GetXukName(Type type)
        {
            Attribute attribute = Attribute.GetCustomAttribute(
                type,
                typeof(XukNameUglyPrettyAttribute));

            DebugFix.Assert(attribute != null);

            if (attribute == null)
            {
                return null;
            }

            return ((XukNameUglyPrettyAttribute)attribute).Name;
        }

        public static string GetXukName(Type type, bool pretty)
        {
            UglyPrettyName xukName = GetXukName(type);
            if (xukName == null)
            {
                return null;
            }
            return pretty ? xukName.Pretty : xukName.Ugly;
        }

        public QualifiedName GetXukQualifiedName()
        {
            return new QualifiedName(GetXukName_(), GetXukNamespace());
        }

        public static QualifiedName GetXukQualifiedName(Type type)
        {
            return new QualifiedName(GetXukName(type), GetXukNamespace(type));
        }

        //TODO: terrible HACK!! :(
        public static bool m_PrettyFormat_STATIC;

        public abstract bool PrettyFormat { get; set; }


        private QualifiedName m_MissingTypeOriginalXukedName = null;
        public QualifiedName MissingTypeOriginalXukedName
        {
            set { m_MissingTypeOriginalXukedName = value; }
            get { return m_MissingTypeOriginalXukedName; }
        }



        private static int m_NamespacePrefixIndex = 0;
        private static readonly Dictionary<string, string> m_NamespacePrefixMap = new Dictionary<string, string>();
        private static string getXukNamespacePrefix(string namespaceUri)
        {
            string str;
            if (m_NamespacePrefixMap.TryGetValue(namespaceUri, out str))
            {
                if (string.IsNullOrEmpty(str))
                {
#if DEBUG
                    Debugger.Break();
#endif
                    str = "nsfix";
                }
                return str;
            }

            //if (m_NamespacePrefixMap.ContainsKey(namespaceUri))
            //{
            //    return m_NamespacePrefixMap[namespaceUri];
            //}

            m_NamespacePrefixIndex++;
            string prefix = "ns" + m_NamespacePrefixIndex;
            m_NamespacePrefixMap.Add(namespaceUri, prefix);
            return prefix;
        }





        public static readonly string XUK_XSD_PATH = "xuk.xsd";

        //public static ulong ComputeUidHash(string uid)
        //{
        //    int index = uid.IndexOfAny(m_digits);
        //    if (index != -1)
        //    {
        //        ulong uidHash;
        //        bool success = ulong.TryParse(uid.Substring(index), out uidHash);
        //        if (success) return uidHash;
        //    }

        //    return ulong.MaxValue;
        //}


#if !UidStringComparisonNoHashCodeOptimization

        // getter costs a lot of CPU time when called millions of time...so we use an unsafe public member :(
        public int UidHash = int.MaxValue;

        private const uint zeroChar = (uint)'0';
        private static char[] m_digitsNoZero = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        public static bool UsePrefixedIntUniqueHashCodes = true;
        public static int GetHashCode(string uid)
        {
            if (UsePrefixedIntUniqueHashCodes)
            {
                uint uidHash = 0;
                int index = uid.IndexOfAny(m_digitsNoZero);
                if (index != -1)
                {
                    bool success = true;
                    uint factorTen = 1;
                    for (int i = uid.Length - 1; i >= index; i--)
                    {
                        uint c = uid[i] - zeroChar;
                        if (c < 0 || c > 9)
                        {
#if DEBUG
                            Debugger.Break();
#endif //DEBUG
                            success = false;
                            break;
                        }

                        uidHash += factorTen * c;
                        factorTen *= 10;
                    }

                    if (success)
                    {
                        return (int)uidHash;
                    }

                    //uint uidHash;
                    //bool success = uint.TryParse(uid.Substring(index), out uidHash);
                    //if (success)
                    //{
                    //    return (int)uidHash;
                    //}
                }
                else
                {
                    index = uid.LastIndexOf('0');
                    if (index == uid.Length - 1)
                    {
                        return 0;
                    }

#if DEBUG
                    Debugger.Break();
#endif //DEBUG
                }

#if DEBUG
                Debugger.Break();
#endif //DEBUG

                UsePrefixedIntUniqueHashCodes = false;
                return GetHashCode(uid);


                //unsafe
                //{
                //    fixed (char* str = uid)
                //    {
                //        char* chPtr = str;
                //        int num = 352654597;
                //        int num2 = num;
                //        int* numPtr = (int*)chPtr;
                //        for (int i = uid.Length; i > 0; i -= 4)
                //        {
                //            num = (((num << 5) + num) + (num >> 27)) ^ numPtr[0];
                //            if (i <= 2)
                //            {
                //                break;
                //            }
                //            num2 = (((num2 << 5) + num2) + (num2 >> 27)) ^ numPtr[1];
                //            numPtr += 2;
                //        }
                //        return (num + (num2 * 1566083941));
                //    }
                //    fixed (char* str = ((char*)uid))
                //    {
                //        char* chPtr = str;
                //        int num = 0x15051505;
                //        int num2 = num;
                //        int* numPtr = (int*)chPtr;
                //        for (int i = uid.Length; i > 0; i -= 4)
                //        {
                //            num = (((num << 5) + num) + (num >> 0x1b)) ^ numPtr[0];
                //            if (i <= 2)
                //            {
                //                break;
                //            }
                //            num2 = (((num2 << 5) + num2) + (num2 >> 0x1b)) ^ numPtr[1];
                //            numPtr += 2;
                //        }
                //        return (num + (num2 * 0x5d588b65));
                //    }
                //}
            }
            else
            {
                return uid.GetHashCode();
            }
        }

#endif //UidStringComparisonNoHashCodeOptimization

        protected static readonly UglyPrettyName Uid_NAME = new UglyPrettyName("uid", "Uid");

        private string m_Uid = null;
        public string Uid
        {
            set
            {
                if (value != null)
                {
                    //m_Uid = string.Intern(value);
                    m_Uid = value;

#if !UidStringComparisonNoHashCodeOptimization
                    UidHash = GetHashCode(m_Uid);
#endif //UidStringComparisonNoHashCodeOptimization

                }
                else
                {
                    m_Uid = null;
#if !UidStringComparisonNoHashCodeOptimization
                    UidHash = int.MaxValue;
#endif //UidStringComparisonNoHashCodeOptimization
                }
            }
            get { return m_Uid; }
        }

        /// <summary>
        /// Clears the <see cref="XukAble"/> of any data - called at the beginning of <see cref="XukIn"/>
        /// </summary>
        protected virtual void Clear()
        {
        }

        /// <summary>
        /// The implementation of XUKIn is expected to read and remove all tags
        /// up to and including the closing tag matching the element the reader was at when passed to it.
        /// The call is expected to be forwarded to any owned element, in effect making it a recursive read of the XUK file
        /// </summary>
        /// <param name="source">The XmlReader to read from</param>
        /// <param name="handler">The handler for progress</param>
        public virtual void XukIn(XmlReader source, IProgressHandler handler)
        {
            if (source == null)
            {
                throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
            }
            if (source.NodeType != XmlNodeType.Element)
            {
                throw new exception.XukException("Can not read XukAble from a non-element node");
            }
            if (handler != null)
            {
                if (handler.NotifyProgress())
                {
                    string msg = String.Format("XukIn cancelled at element {0}:{1}",
                        GetXukName(),
                        GetXukNamespace());
                    throw new exception.ProgressCancelledException(msg);
                }
            }
            try
            {
                Clear();
                XukInAttributes(source);
                if (!source.IsEmptyElement)
                {
                    while (source.Read())
                    {
                        if (source.NodeType == XmlNodeType.Element)
                        {
                            XukInChild(source, handler);
                        }
                        else if (source.NodeType == XmlNodeType.Text)
                        {
                            XukInChild(source, handler);
                            break;
                        }
                        else if (source.NodeType == XmlNodeType.EndElement)
                        {
                            break;
                        }
                        if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
                    }
                }
            }
            catch (exception.ProgressCancelledException)
            {
                throw;
            }
            catch (exception.XukException)
            {
#if DEBUG
                Debugger.Break();
#endif
                throw;
            }
            catch (Exception e)
            {
#if DEBUG
                Debugger.Break();
#endif
                throw new exception.XukException(
                    String.Format("An exception occured during XukIn of XukAble: {0}", e.Message),
                    e);
            }
        }

        /// <summary>
        /// Reads the attributes of a XukAble xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected virtual void XukInAttributes(XmlReader source)
        {
            string uid = ReadXukAttribute(source, Uid_NAME);
            if (!string.IsNullOrEmpty(uid))
            {
                Uid = uid;
            }
        }

        /// <summary>
        /// Reads a child of a XukAble xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler of progress</param>
        protected virtual void XukInChild(XmlReader source, IProgressHandler handler)
        {
            if (source.NodeType == XmlNodeType.Element && !source.IsEmptyElement)
            {
                source.ReadSubtree().Close(); //Read past unknown child 
            }
        }

        /// <summary>
        /// Write a XukAble element to a XUK file representing the <see cref="XukAble"/> instance
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        /// <param name="handler">The handler for progress</param>
        public virtual void XukOut(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            if (destination == null)
            {
                throw new exception.MethodParameterIsNullException(
                    "Can not XukOut to a null XmlWriter");
            }
            if (handler != null)
            {
                if (handler.NotifyProgress())
                {
                    string msg = String.Format("XukOut cancelled at {0}", ToString());
                    throw new exception.ProgressCancelledException(msg);
                }
            }
            try
            {
                if (MissingTypeOriginalXukedName != null)
                {
                    if (MissingTypeOriginalXukedName.NamespaceUri == XukAble.XUK_NS)
                    {
                        destination.WriteStartElement(MissingTypeOriginalXukedName.LocalName.z(PrettyFormat), MissingTypeOriginalXukedName.NamespaceUri);
                    }
                    else
                    {
                        string prefix = getXukNamespacePrefix(MissingTypeOriginalXukedName.NamespaceUri);
                        destination.WriteStartElement(prefix, MissingTypeOriginalXukedName.LocalName.z(PrettyFormat), MissingTypeOriginalXukedName.NamespaceUri);
                    }
                }
                else
                {
                    if (GetXukNamespace() == XukAble.XUK_NS)
                    {
                        destination.WriteStartElement(GetXukName(), XukAble.XUK_NS);
                    }
                    else
                    {
                        string prefix = getXukNamespacePrefix(GetXukNamespace());
                        destination.WriteStartElement(prefix, GetXukName(), GetXukNamespace());
                    }
                }
                XukOutAttributes(destination, baseUri);
                XukOutChildren(destination, baseUri, handler);
                destination.WriteEndElement();
            }
            catch (exception.ProgressCancelledException)
            {
                throw;
            }
            catch (exception.XukException)
            {
#if DEBUG
                Debugger.Break();
#endif
                throw;
            }
            catch (Exception e)
            {
#if DEBUG
                Debugger.Break();
#endif
                throw new exception.XukException(
                    String.Format("An exception occured during XukOut of XukAble: {0}", e.Message),
                    e);
            }
        }

        /// <summary>
        /// Writes the attributes of a XukAble element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected virtual void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            if (!string.IsNullOrEmpty(Uid))
            {
                destination.WriteAttributeString(Uid_NAME.z(PrettyFormat), Uid);
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
        protected virtual void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
        }

        //public string XukLocalName
        //{
        //    get
        //    {
        //        return GetTypeNameFormatted();
        //        //return GetTypeNameFormatted(GetType());
        //        //return GetType().Name;
        //    }
        //}

        //public abstract string GetTypeNameFormatted();

        //public string XukNamespaceUri
        //{
        //    get
        //    {
        //        //return XUK_NS;
        //        return GetXukNamespaceUri(GetType());
        //    }
        //}

        //public virtual string GetXukNamespaceUri()
        //{
        //    return XUK_NS;
        //}

        //        private static readonly Dictionary<Type, string> m_TypeNamespaceUriMap = new Dictionary<Type, string>();

        //        private static string GetXukNamespaceUri(Type t)
        //        {
        //            if (!typeof(XukAble).IsAssignableFrom(t))
        //            {
        //                throw new exception.MethodParameterIsWrongTypeException(
        //                    "Cannot get the XukNamespaceUri of a type that does not inherit XukAble");
        //            }

        //            string str;
        //            if (m_TypeNamespaceUriMap.TryGetValue(t, out str))
        //            {
        //                if (string.IsNullOrEmpty(str))
        //                {
        //#if DEBUG
        //                    Debugger.Break();
        //#endif
        //                    str = XukAble.XUK_NS;
        //                }

        //                return str;
        //            }

        //            //if (m_TypeNamespaceUriMap.ContainsKey(t)) return m_TypeNamespaceUriMap[t];

        //            FieldInfo fi = t.GetField("XUK_NS", BindingFlags.Static | BindingFlags.Public);
        //            if (fi != null)
        //            {
        //                if (fi.FieldType == typeof(string))
        //                {
        //                    string uri = (fi.GetValue(null) as string) ?? "";
        //                    m_TypeNamespaceUriMap.Add(t, uri);
        //                    return uri;
        //                }
        //            }

        //            return GetXukNamespaceUri(t.BaseType);
        //        }

        //public QualifiedName XukQualifiedName
        //{
        //    get
        //    {
        //        return new QualifiedName(GetTypeNameFormatted(), XukNamespaceUri);
        //        //GetXukNamespaceUri(GetType())
        //        //return GetXukQualifiedName(GetType());
        //    }
        //}

        //public QualifiedName GetXukQualifiedName(Type t)
        //{
        //    if (!typeof(XukAble).IsAssignableFrom(t))
        //    {
        //        string msg = String.Format(
        //            "Only Types deriving {0} can be given here !", typeof(XukAble).FullName);
        //        throw new MethodParameterIsWrongTypeException(msg);
        //    }
        //    if (t == null)
        //    {
        //        throw new MethodParameterIsNullException("Cannot get the Xuk QualifiedName of a null Type");
        //    }
        //    return new QualifiedName(GetTypeNameFormatted(t), GetXukNamespaceUri(t));
        //}


        //        private struct XukNameAndPrettyFlag
        //        {
        //            public string name;
        //            public bool isPretty;
        //        }

        //        private readonly Dictionary<Type, XukNameAndPrettyFlag> m_TypeNameMap = new Dictionary<Type, XukNameAndPrettyFlag>();

        //        private string GetTypeNameFormatted(Type t)
        //        {
        //            XukNameAndPrettyFlag xuk;
        //            bool update = false;

        //            if (m_TypeNameMap.TryGetValue(t, out xuk)
        //                //&& xuk != null
        //                && !string.IsNullOrEmpty(xuk.name))
        //            {
        //                if (PrettyFormat == xuk.isPretty)
        //                {
        //                    return xuk.name;
        //                }
        //                else
        //                {
        //                    update = true;
        //                }
        //            }


        //            //if (m_TypeNameMap.ContainsKey(t)) return m_TypeNameMap[t];

        //            string name = t.Name;

        //            PropertyInfo info = typeof(XukStrings).GetProperty(name, BindingFlags.Static | BindingFlags.Public);
        //            if (info != null)
        //            {
        //                if (info.PropertyType == typeof(string))
        //                {
        //                    string n = (info.GetValue(null, null) as string) ?? name;
        //                    xuk.name = n;
        //                    xuk.isPretty = PrettyFormat;

        //                    if (update)
        //                    {
        //                        m_TypeNameMap.Remove(t);
        //                    }
        //                    m_TypeNameMap.Add(t, xuk);

        //                    return n;
        //                }
        //            }
        //            else
        //            {
        //#if NET40 && DEBUG
        //                Debugger.Break();
        //#endif

        //                FieldInfo info2 = t.GetField("XukString", BindingFlags.Static | BindingFlags.Public);
        //                if (info2 != null)
        //                {
        //                    if (info2.FieldType == typeof(string))
        //                    {
        //                        string n = (info2.GetValue(null) as string) ?? name;
        //                        xuk.name = n;
        //                        xuk.isPretty = PrettyFormat;

        //                        if (update)
        //                        {
        //                            m_TypeNameMap.Remove(t);
        //                        }
        //                        m_TypeNameMap.Add(t, xuk);

        //                        return n;
        //                    }
        //                }
        //            }
        //#if DEBUG
        //            Debugger.Break();
        //#endif
        //            System.Diagnostics.Debug.Fail("Type name not found ??");
        //            return name;
        //        }


        //        public static readonly string XUK_NS = "http://www.daisy.org/urakawa/xuk/2.0";

        //        public static readonly string XUK_NAME_PRETTY = "XukAble";
        //        public static readonly string XUK_NAME_UGLY = "xukA";

        //        public string XUK_NAME
        //        {
        //            get
        //            {
        //                return PrettyFormat ? XUK_NAME_PRETTY : XUK_NAME_UGLY;
        //            }
        //        }

        //        public QualifiedName getXUK_QNAME()
        //        {
        //            return new QualifiedName(XUK_NAME, XUK_NS);
        //        }

        //        public static string getXUK_NAME<T>(bool pretty) where T : XukAble
        //        {
        //            Type t = typeof(T);
        //            return getXUK_NAME(t, pretty);
        //        }

        //        public static string getXUK_NAME(Type t, bool pretty)
        //        {
        //            if (t == null)
        //            {
        //                throw new MethodParameterIsNullException("XUK_NAME(Type t is null!)");
        //            }

        //            //if (t == typeof (XukAble))
        //            //{
        //            //    return pretty ? XUK_NAME_PRETTY : XUK_NAME_UGLY;
        //            //}

        //            if (!typeof(XukAble).IsAssignableFrom(t))
        //            {
        //                string msg = String.Format("XUK_NAME(Type t must derive {0})", typeof(XukAble).FullName);
        //                throw new MethodParameterIsWrongTypeException(msg);
        //            }

        //            FieldInfo fi = t.GetField(pretty ? "XUK_NAME_PRETTY" : "XUK_NAME_UGLY", BindingFlags.Static | BindingFlags.Public);

        //            if (fi == null || fi.FieldType != typeof(string))
        //            {
        //#if DEBUG
        //                Debugger.Break();
        //#endif
        //                //return getXUK_NAME(t.BaseType, pretty);
        //                return null;
        //            }

        //            string name = fi.GetValue(null) as string;
        //            if (string.IsNullOrEmpty(name))
        //            {
        //#if DEBUG
        //                Debugger.Break();
        //#endif
        //                return null;
        //            }

        //            return name;
        //        }

        //        public static QualifiedName getXUK_QNAME(Type t, bool pretty)
        //        {
        //            return new QualifiedName(getXUK_NAME(t, pretty), XUK_NS);
        //        }


    }
}