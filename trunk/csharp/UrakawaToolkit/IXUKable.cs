using System;

namespace urakawa.xuk
{
	/// <summary>
	/// The interface to implement for serializing the core model in a roundtrip safe format.
	/// </summary>
	public interface IXUKable
	{
		/// <summary>
		/// The implementation of XUKin is expected to read and remove all tags up to and including the closing tag matching the element the reader was at when passed to it.
		/// The call is expected to be forwarded to any owned element, in effect making it a recursive read of the XUK file
		/// </summary>
		/// <param name="source">The XmlReader to read from</param>
		/// <returns>true is all things were deserialized as expected, false if anything unexpected was encountered</returns>
		bool XUKin(System.Xml.XmlReader source);

		/// <summary>
		/// The implementation of XUKout is expected to write a tag for the object it is called on.
		/// The call should be forwarded to any owned object, making it in effect be a recursive write of the CoreTree
		/// </summary>
		/// <param name="destination"></param>
		/// <returns></returns>
		bool XUKout(System.Xml.XmlWriter destination);
	}
}
