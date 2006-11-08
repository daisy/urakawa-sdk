package org.daisy.urakawa.xuk;

/**
 *
 */
public interface XukAble {
    /**
     * The implementation of XukIn is expected to read and remove all tags up to and including the closing tag matching the element the reader was at when passed to it.
     * The call is expected to be forwarded to any owned element, in effect making it a recursive read of the XUK file
     *
     * @param source specified as URI but could be any other input type (e.g. XML stream)
     * @return true if de-serialization went well.
     */
    public boolean XukIn(XmlDataReader source);

    /**
     * The implementation of XukOut is expected to write a tag for the object it is called on.
     * The call should be forwarded to any owned object, making it in effect be a recursive write of the CoreTree.
     *
     * @param destination specified as URI but could be any other input type (e.g. XML stream)
     * @return true if serialization went well.
     */
    public boolean XukOut(XmlDataWriter destination);

    /**
     * @return cannot be NULL or empty.
     */
    public String getXukLocalName();

    /**
     * @return cannot be NULL, but may be empty.
     */
    public String getXukNamespaceURI();
}
