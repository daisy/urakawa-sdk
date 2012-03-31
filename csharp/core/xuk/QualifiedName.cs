using System;
using System.Collections.Generic;
using System.Text;
using urakawa.exception;

namespace urakawa.xuk
{
    /// <summary>
    /// Represents a QName
    /// </summary>
    public class QualifiedName
    {
        /// <summary>
        /// The local name part of the qualified name
        /// </summary>
        public readonly string LocalName;

        /// <summary>
        /// The namespace uri part of the qualified name
        /// </summary>
        public readonly string NamespaceUri;

        /// <summary>
        /// The qualified name in the form <c>[<see cref="NamespaceUri"/>:]<see cref="LocalName"/></c>
        /// </summary>
        public string FullyQualifiedName
        {
            get
            {
                if (NamespaceUri == "") return LocalName;
                return String.Format("{0}:{1}", NamespaceUri, LocalName);
            }
        }

        ///// <summary>
        ///// Parses a QName in form <c>[NamespaceUri:]LocalName</c>
        ///// </summary>
        ///// <param name="qn">The QName to parse</param>
        ///// <returns>The parsed <see cref="QualifiedName"/></returns>
        ///// <exception cref="MethodParameterIsNullException">
        ///// Thrown when <paramref name="qn"/> is <c>null</c>
        ///// </exception>
        ///// <exception cref="MethodParameterIsOutOfBoundsException">
        ///// Thrown when <paramref name="qn"/> cannot be parsed
        ///// </exception>
        ///// <exception cref="MethodParameterIsEmptyStringException">
        ///// Thrwon when the LocalName part of <paramref name="qn"/> is an empty string
        ///// </exception>
        //public static QualifiedName ParseQName(string qn)
        //{
        //    if (qn==null)
        //    {
        //        throw new MethodParameterIsNullException("The LocalName or the NamespaceUri of the QualifiedName cannot be parsed from a null QName");
        //    }
        //    string[] parts = qn.Split(':');
        //    string ln;
        //    string ns = "";
        //    if (parts.Length>1)
        //    {
        //        ln = parts[parts.Length-1];
        //        ns = parts[0];
        //        for (int i=1; i<parts.Length-1; i++)
        //        {
        //            ns += ":" + parts[i];
        //        }
        //    }
        //    else
        //    {
        //        ln = qn;
        //    }
        //    return new QualifiedName(ln, ns);
        //}

        /// <summary>
        /// Constructor initializing the <see cref="LocalName"/> and <see cref="NamespaceUri"/> fields
        /// </summary>
        /// <param name="ln">The value for field <see cref="LocalName"/></param>
        /// <param name="nsuri">The value for field <see cref="NamespaceUri"/></param>
        public QualifiedName(string ln, string nsuri)
        {
            if (nsuri == null)
            {
                throw new MethodParameterIsNullException("The NamespaceUri of the qualified name cannot be null");
            }
            if (string.IsNullOrEmpty(ln))
            {
                throw new MethodParameterIsEmptyStringException("The LocalName of the QualifiedName cannot be null or an empty string");
            }
            LocalName = ln;
            NamespaceUri = nsuri;
        }

        /// <summary>
        /// Gets the textual representation of the <see cref="QualifiedName"/>
        /// </summary>
        /// <returns>The value of property <see cref="FullyQualifiedName"/></returns>
        public override string ToString()
        {
            return FullyQualifiedName;
        }
    }
}
