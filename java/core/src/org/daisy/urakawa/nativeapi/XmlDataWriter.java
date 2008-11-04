package org.daisy.urakawa.nativeapi;

/**
 * Place-holder for a real implementation in Java. It should be replaced with
 * StAX, really.
 */
public class XmlDataWriter implements IXmlDataWriter
{
    /**
     * @param fs
     */
    public XmlDataWriter(IStream fs)
    {
        // Needs implementing !
    }

    public void close()
    {
        /**
         * To implement.
         */
    }

    public void writeEndDocument()
    {
        /**
         * To implement.
         */
    }

    public void writeEndElement()
    {
        /**
         * To implement.
         */
    }

    public void writeStartDocument()
    {
        /**
         * To implement.
         */
    }

    public void writeStartElement(String localName, String namespace)
    {
        /**
         * To implement.
         */
    }

    public void writeAttributeString(String str1, String str2)
    {
        /**
         * To implement.
         */
    }

    public void writeString(String str)
    {
        /**
         * To implement.
         */
    }

    public void writeAttributeString(String str1, String str2, String str3,
            String str4)
    {
        /**
         * To implement.
         */
    }

    public IStream getBaseStream()
    {
        return null;
    }
}
