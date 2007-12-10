package org.daisy.urakawa.xuk;

/**
 * <p>
 * This can be implemented using language-specific type, such as
 * "System.Xml.XmlWriter" in C#, or "org.xml.sax.XMLWriter" in Java.
 * </p>
 * <p>
 * Method examples are given in this interface to match the C#
 * "System.Xml.XmlWriter" implementation, for illustration purposes only.
 * </p>
 * 
 * @stereotype Language-Dependent
 */
public interface XmlDataWriter {
	/**
	 * @param localName
	 * @param namespace
	 */
	public void writeStartElement(String localName, String namespace);

	/**
	 * 
	 */
	public void close();

	/**
	 * 
	 */
	public void writeStartDocument();

	/**
	 * 
	 */
	public void writeEndDocument();

	/**
	 * 
	 */
	public void writeEndElement();
}
