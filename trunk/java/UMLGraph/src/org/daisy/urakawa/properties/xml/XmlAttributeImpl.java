package org.daisy.urakawa.properties.xml;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * The actual implementation to be implemented by the implementation team ;)
 * All method bodies must be completed for realizing the required business logic.
 * -
 * Generally speaking, an end-user would not need to use this class directly.
 * They would just manipulate the corresponding abstract interface and use the provided
 * default factory implementation to create this class instances transparently.
 * -
 * However, this is the DEFAULT implementation for the API/Toolkit:
 * end-users should feel free to use this class as such (it's public after all),
 * or they can sub-class it in order to specialize their application.
 */
public class XmlAttributeImpl implements XmlAttribute {
    /**
     * @hidden
     */
    public XmlProperty getParent() {
        return null;
    }

    /**
     * @hidden
     */
    public String getName() {
        return null;
    }

    /**
     * @hidden
     */
    public String getNamespace() {
        return null;
    }

    /**
     * @hidden
     */
    public void setName(String newName) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException {
    }

    /**
     * @hidden
     */
    public void setNamespace(String newNS) throws MethodParameterIsNullException {
    }

    /**
     * @hidden
     */
    public XmlAttribute copy() {
        return null;
    }

    /**
     * @hidden
     */
    public String getValue() {
        return null;
    }

    /**
     * @hidden
     */
    public void setValue(String newValue) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException {
    }

    /**
     * @hidden
     */
    public boolean XukIn(XmlDataReader source) {
        return false;
    }

    /**
     * @hidden
     */
    public boolean XukOut(XmlDataWriter destination) {
        return false;
    }

    /**
     * @hidden
     */
    public String getXukLocalName() {
        return null;
    }

    /**
     * @hidden
     */
    public String getXukNamespaceURI() {
        return null;
    }
}
