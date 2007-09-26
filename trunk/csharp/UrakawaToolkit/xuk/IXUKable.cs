using System;

namespace urakawa.xuk
{
	/// <summary>
	/// The interface to implement for serializing the core model in a roundtrip safe format.
	/// </summary>
	public interface IXukAble
	{
		/// <summary>
		/// The implementation of XUKIn is expected to read and remove all tags
		/// up to and including the closing tag matching the element the reader was at when passed to it.
		/// The call is expected to be forwarded to any owned element, in effect making it a recursive read of the XUK file
		/// </summary>
		/// <param name="source">The XmlReader to read from</param>
		void XukIn(System.Xml.XmlReader source);

		/// <summary>
		/// The implementation of XukOut is expected to write a tag for the object it is called on.
		/// The call should be forwarded to any owned object, making it in effect be a recursive write
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		void XukOut(System.Xml.XmlWriter destination, Uri baseUri);

		/// <summary>
		/// Gets the local localName part of the QName identifying the type of the instance
		/// </summary>
		/// <returns>The local localName</returns>
		string getXukLocalName();

		/// <summary>
		/// Gets the namespace uri part of the QName identifying the type of the instance
		/// </summary>
		/// <returns>The namespace uri</returns>
		string getXukNamespaceUri();
	}
}
