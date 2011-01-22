using System;
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
        /*
        public abstract bool IsPrettyFormat();
        public abstract void SetPrettyFormat(bool pretty);
        */

        public virtual bool IsPrettyFormat()
        {
            throw new NotImplementedException();
        }

        public virtual void SetPrettyFormat(bool pretty)
        {
            throw new NotImplementedException();
        }

        public const string XUK_NS = "http://www.daisy.org/urakawa/xuk/2.0";


        /// <summary>
        /// The path of the W3C XmlSchema defining the XUK namespace
        /// </summary>
        public static string XUK_XSD_PATH = "xuk.xsd";

        #region IXUKAble members

        private QualifiedName m_MissingTypeOriginalXukedName = null;
        public QualifiedName MissingTypeOriginalXukedName
        {
            set { m_MissingTypeOriginalXukedName = value; }
            get { return m_MissingTypeOriginalXukedName; }
        }

        private string m_Uid = null;
        public virtual string Uid
        {
            set { m_Uid = value; }
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
                    destination.WriteStartElement(MissingTypeOriginalXukedName.LocalName, MissingTypeOriginalXukedName.NamespaceUri);
                }
                else
                {
                    destination.WriteStartElement(XukLocalName, XukNamespaceUri);
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
                return XUK_NS;
                //return GetXukNamespaceUri(GetType());
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
        public static QualifiedName GetXukQualifiedName(Type t)
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

        private static string GetTypeNameFormatted(Type t)
        {
            string name = t.Name;

            PropertyInfo info = typeof(XukStrings).GetProperty(name, BindingFlags.Static | BindingFlags.Public);
            if (info == null)
            {
                info = t.GetProperty("XukString", BindingFlags.Static | BindingFlags.Public);
            }
            if (info != null)
            {
                if (info.PropertyType == typeof(string))
                {
                    return (info.GetValue(null, null) as string) ?? name;
                }
            }
            System.Diagnostics.Debug.Fail("Type name not found ??");
            return name;
        }

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
            FieldInfo fi = t.GetField("XUK_NS", BindingFlags.Static | BindingFlags.Public);
            if (fi != null)
            {
                if (fi.FieldType == typeof(string))
                {
                    return (fi.GetValue(null) as string) ?? "";
                }
            }
            return GetXukNamespaceUri(t.BaseType);
        }


        #endregion
    }
}