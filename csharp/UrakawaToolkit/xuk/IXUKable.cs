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
		/// <param localName="source">The XmlReader to read from</param>
		/// <returns><c>true</c> is all things were deserialized as expected, <c>false</c> if anything unexpected was encountered</returns>
		bool XukIn(System.Xml.XmlReader source);

		/// <summary>
		/// The implementation of XukOut is expected to write a tag for the object it is called on.
		/// The call should be forwarded to any owned object, making it in effect be a recursive write
		/// </summary>
		/// <param localName="destination"></param>
		/// <returns><c>true</c> is all things were serialized as expected, <c>false</c> if anything unexpected was encountered</returns>
		bool XukOut(System.Xml.XmlWriter destination);

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
