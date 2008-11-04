package org.daisy.urakawa.nativeapi;

/**
 * <p>
 * This interface does not have to be implemented "as-is". It is basically a
 * place-holder for a XML pull-parser, such as "System.Xml.XmlReader" in C#, or
 * StAX Java implementations. For more information, see
 * http://www.xmlpull.org/impls.shtml and http://stax.codehaus.org/
 * </p>
 * <p>
 * Note: the methods in this interface are directly inspired from the the C#
 * "System.Xml.XmlReader" implementation, but any XML pull-parser API should
 * provide a similar, if not an identical interface.
 * </p>
 * 
 * @stereotype Language-Dependent
 */
public interface IXmlDataReader
{
    /**
     * See {@link IXmlDataReader#getNodeType()}
     */
    public static int ELEMENT = 0;
    /**
     * See {@link IXmlDataReader#getNodeType()}
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
     * @return a new IXmlDataReader
     */
    public IXmlDataReader readSubtree();

    /**
	 * 
	 */
    public void close();

    /**
     * @return the node type
     */
    public int getNodeType();

    /**
     * @param name
     *        attribute name
     * @return value
     */
    public String getAttribute(String name);

    /**
     * @return true or false.
     */
    public boolean read();

    /**
     * @param localName
     * @param namespace
     * @return true or false.
     */
    public boolean readToFollowing(String localName, String namespace);

    /**
     * @return a string
     */
    public String getLocalName();

    /**
     * @return a string
     */
    public String getNamespaceURI();

    /**
     * @return a URI
     */
    public String getBaseURI();

    /**
     * @return true or false
     */
    public boolean moveToFirstAttribute();

    /**
     * @return true or false
     */
    public boolean moveToNextAttribute();

    /**
	 * 
	 */
    public void moveToElement();

    /**
     * @return attribute name
     */
    public String getName();

    /**
     * @return attribute value
     */
    public String getValue();

    /**
     * @return text
     */
    public String readElementContentAsString();

    /**
     * @return underlying stream
     */
    public IStream getBaseStream();
}
