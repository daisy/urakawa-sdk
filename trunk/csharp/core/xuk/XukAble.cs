using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using urakawa.exception;
using urakawa.progress;

namespace urakawa.xuk
{
    /// <summary>
    /// Common base class for classes that implement <see cref="IXukAble"/>
    /// </summary>
    public abstract class XukAble : IXukAble
    {
        private struct XukNameAndPrettyFlag
        {
            public string name;
            public bool isPretty;
        }

        private readonly Dictionary<Type, XukNameAndPrettyFlag> m_TypeNameMap = new Dictionary<Type, XukNameAndPrettyFlag>();

        private string GetTypeNameFormatted(Type t)
        {
            XukNameAndPrettyFlag xuk;
            m_TypeNameMap.TryGetValue(t, out xuk);
            if (!string.IsNullOrEmpty(xuk.name))
            {
                if (IsPrettyFormat() == xuk.isPretty)
                {
                    return xuk.name;
                }
            }


            //if (m_TypeNameMap.ContainsKey(t)) return m_TypeNameMap[t];

            string name = t.Name;

            PropertyInfo info = typeof(XukStrings).GetProperty(name, BindingFlags.Static | BindingFlags.Public);
            if (info != null)
            {
                if (info.PropertyType == typeof(string))
                {
                    string n = (info.GetValue(null, null) as string) ?? name;
                    xuk.name = n;
                    xuk.isPretty = IsPrettyFormat();
                    m_TypeNameMap.Add(t, xuk);
                    return n;
                }
            }
            else
            {
                FieldInfo info2 = t.GetField("XukString", BindingFlags.Static | BindingFlags.Public);
                if (info2 != null)
                {
                    if (info2.FieldType == typeof(string))
                    {
                        string n = (info2.GetValue(null) as string) ?? name;
                        xuk.name = n;
                        xuk.isPretty = IsPrettyFormat();
                        m_TypeNameMap.Add(t, xuk);
                        return n;
                    }
                }
            }

            System.Diagnostics.Debug.Fail("Type name not found ??");
            return name;
        }

        public virtual bool IsPrettyFormat()
        {
            return XukStrings.IsPrettyFormat;
        }

        public virtual void SetPrettyFormat(bool pretty)
        {
            throw new NotImplementedException();
        }

        public static readonly string XUK_NS = "http://www.daisy.org/urakawa/xuk/2.0";


        /// <summary>
        /// The path of the W3C XmlSchema defining the XUK namespace
        /// </summary>
        public static readonly string XUK_XSD_PATH = "xuk.xsd";

        #region IXUKAble members

        private QualifiedName m_MissingTypeOriginalXukedName = null;
        public QualifiedName MissingTypeOriginalXukedName
        {
            set { m_MissingTypeOriginalXukedName = value; }
            get { return m_MissingTypeOriginalXukedName; }
        }



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
                    string msg = String.Format("XukIn cancelled at element {0}:{1}", XukLocalName,
                                               XukNamespaceUri);
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
                throw;
            }
            catch (Exception e)
            {
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
            string uid = source.GetAttribute(XukStrings.Uid);
            if (!string.IsNullOrEmpty(uid)) Uid = uid;
        }

        /// <summary>
        /// Reads a child of a XukAble xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler of progress</param>
        protected virtual void XukInChild(XmlReader source, IProgressHandler handler)
        {
            if (source.NodeType == XmlNodeType.Element && !source.IsEmptyElement) source.ReadSubtree().Close(); //Read past unknown child 
        }

        private static int m_NamespacePrefixIndex = 0;
        private static readonly Dictionary<string, string> m_NamespacePrefixMap = new Dictionary<string, string>();
        private static string GetNamespacePrefix(string namespaceUri)
        {
            string str;
            m_NamespacePrefixMap.TryGetValue(namespaceUri, out str);
            if (!string.IsNullOrEmpty(str)) return str;

            //if (m_NamespacePrefixMap.ContainsKey(namespaceUri))
            //{
            //    return m_NamespacePrefixMap[namespaceUri];
            //}

            m_NamespacePrefixIndex++;
            string prefix = "ns" + m_NamespacePrefixIndex;
            m_NamespacePrefixMap.Add(namespaceUri, prefix);
            return prefix;
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
                        destination.WriteStartElement(MissingTypeOriginalXukedName.LocalName, MissingTypeOriginalXukedName.NamespaceUri);
                    }
                    else
                    {
                        string prefix = GetNamespacePrefix(MissingTypeOriginalXukedName.NamespaceUri);
                        destination.WriteStartElement(prefix, MissingTypeOriginalXukedName.LocalName, MissingTypeOriginalXukedName.NamespaceUri);
                    }
                }
                else
                {
                    if (XukNamespaceUri == XukAble.XUK_NS)
                    {
                        destination.WriteStartElement(XukLocalName, XukNamespaceUri);
                    }
                    else
                    {
                        string prefix = GetNamespacePrefix(XukNamespaceUri);
                        destination.WriteStartElement(prefix, XukLocalName, XukNamespaceUri);
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
                throw;
            }
            catch (Exception e)
            {
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
                destination.WriteAttributeString(XukStrings.Uid, Uid);
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

        /// <summary>
        /// Gets the local name part of the QName representing a <see cref="XukAble"/> in Xuk. 
        /// This will always be the name of the <see cref="Type"/> of <c>this</c>
        /// </summary>
        /// <returns>The local name part</returns>
        public string XukLocalName
        {
            get
            {
                return GetTypeNameFormatted();
                //return GetTypeNameFormatted(GetType());
                //return GetType().Name;
            }
        }

        public abstract string GetTypeNameFormatted();

        /// <summary>
        /// Gets the namespace uri part of the QName representing a <see cref="XukAble"/> in Xuk
        /// </summary>
        /// <returns>The namespace uri part</returns>
        public string XukNamespaceUri
        {
            get
            {
                //return XUK_NS;
                return GetXukNamespaceUri(GetType());
            }
        }

        /// <summary>
        /// Gets the Xuk <see cref="QualifiedName"/> of the instance (conveneince for <c>GetXukQualifiedName(GetType());</c>)
        /// </summary>
        public QualifiedName XukQualifiedName
        {
            get
            {
                return new QualifiedName(GetTypeNameFormatted(), XukNamespaceUri);
                //GetXukNamespaceUri(GetType())
                //return GetXukQualifiedName(GetType());
            }
        }

        /// <summary>
        /// Gets the Xuk QName of a <see cref="XukAble"/> <see cref="Type"/> in the form <c>[NS_URI:]LOCALNAME</c>,
        /// calls method <see cref="GetXukNamespaceUri"/> to get the xuk namespace uri
        /// </summary>
        /// <param name="t">The <see cref="Type"/>, must inherit <see cref="XukAble"/></param>
        /// <returns>The qname</returns>
        /// <exception cref="MethodParameterIsNullException">Thrown when <paramref name="t"/> is <c>null</c></exception>
        public QualifiedName GetXukQualifiedName(Type t)
        {
            if (!typeof(XukAble).IsAssignableFrom(t))
            {
                string msg = String.Format(
                    "Only Types deriving {0} can be given here !", typeof(XukAble).FullName);
                throw new MethodParameterIsWrongTypeException(msg);
            }
            if (t == null)
            {
                throw new MethodParameterIsNullException("Cannot get the Xuk QualifiedName of a null Type");
            }
            return new QualifiedName(GetTypeNameFormatted(t), GetXukNamespaceUri(t));
        }


        private static readonly Dictionary<Type, string> m_TypeNamespaceUriMap = new Dictionary<Type, string>();

        /// <summary>
        /// Gets the Xuk namespace uri of a <see cref="XukAble"/> <see cref="Type"/>,
        /// by searching up the class heirarchy for a <see cref="Type"/> 
        /// with a <c>public static</c> field names <c>XUK_NS</c>
        /// </summary>
        /// <param name="t">The <see cref="Type"/>, must inherit <see cref="XukAble"/></param>
        /// <returns>The xuk namespace uri</returns>
        private static string GetXukNamespaceUri(Type t)
        {
            if (!typeof(XukAble).IsAssignableFrom(t))
            {
                throw new exception.MethodParameterIsWrongTypeException(
                    "Cannot get the XukNamespaceUri of a type that does not inherit XukAble");
            }

            string str;
            m_TypeNamespaceUriMap.TryGetValue(t, out str);
            if (!string.IsNullOrEmpty(str)) return str;

            //if (m_TypeNamespaceUriMap.ContainsKey(t)) return m_TypeNamespaceUriMap[t];

            FieldInfo fi = t.GetField("XUK_NS", BindingFlags.Static | BindingFlags.Public);
            if (fi != null)
            {
                if (fi.FieldType == typeof(string))
                {
                    string uri = (fi.GetValue(null) as string) ?? "";
                    m_TypeNamespaceUriMap.Add(t, uri);
                    return uri;
                }
            }
            return GetXukNamespaceUri(t.BaseType);
        }


        #endregion
    }
}