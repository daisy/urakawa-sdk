using System;
using System.Xml;
using urakawa.progress;

namespace urakawa.xuk
{
    /// <summary>
    /// The interface to implement for serializing the core model in a roundtrip safe format.
    /// </summary>
    public interface IXukAble
    {
        string Uid { get; set; }

        string GetXukNamespace();
        string GetXukName();
        UglyPrettyName GetXukName_();
        QualifiedName GetXukQualifiedName();

        /// <summary>
        /// The implementation of XUKIn is expected to read and remove all tags
        /// up to and including the closing tag matching the element the reader was at when passed to it.
        /// The call is expected to be forwarded to any owned element, in effect making it a recursive read of the XUK file
        /// </summary>
        /// <param name="source">The XmlReader to read from</param>
        /// <param name="handler">The handler for progress</param>
        void XukIn(XmlReader source, IProgressHandler handler);

        /// <summary>
        /// The implementation of XukOut is expected to write a tag for the object it is called on.
        /// The call should be forwarded to any owned object, making it in effect be a recursive write
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        /// <param name="handler">The handler for progress</param>
        void XukOut(XmlWriter destination, Uri baseUri, IProgressHandler handler);

        ///<summary>
        /// Determines whether the XUK output is ugly/compressed or pretty/expanded/indented/formatted.
        ///</summary>
        bool PrettyFormat { get; set; }
    }
}