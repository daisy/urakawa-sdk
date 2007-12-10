package org.daisy.urakawa.xuk;

/**
 * <p>
 * This can be implemented using language-specific type, such as
 * "System.Xml.XmlReader" in C#, or "org.xml.sax.XMLReader" in Java.
 * </p>
 * <p>
 * Method examples are given in this interface to match the C#
 * "System.Xml.XmlReader" implementation, for illustration purposes only.
 * </p>
 * 
 * @stereotype Language-Dependent
 */
public interface XmlDataReader {
	/**
	 * See {@link XmlDataReader#getNodeType()}
	 */
	public static int ELEMENT = 0;
	/**
	 * See {@link XmlDataReader#getNodeType()}
	 */
	public static int END_ELEMENT = 1;

	/**
	 * @return true or false
	 */
	public boolean isEmptyElement();
	
	/**
	 * @return true or false.
	 */
	public boolean isEOF();

	/**
	 * @return a new XmlDataReader
	 */
	public XmlDataReader readSubtree();

	/**
	 * 
	 */
	public void close();

	/**
	 * @return the node type
	 */
	public int getNodeType();

	/**
	 * @return true or false.
	 */
	public boolean read();
}
