using System;

namespace urakawa.core
{
	/// <summary>
	/// The interface to implement for serializing the core model in a roundtrip safe format.
	/// </summary>
	public interface IXUKable
	{
		/// <summary>
		/// The implementation of XUKin is expected to read and remove all tags up to and including the closing tag matching the element the reader was at when passed to it.
		/// </summary>
		/// <param name="source">The XmlReader to read from</param>
		/// <returns>true is all things were deserialized as expected, false if anything unexpected was encountered</returns>
		bool XUKin(System.Xml.XmlReader source);
		bool XUKout(System.Xml.XmlWriter destination);
	}
}
