package org.daisy.urakawa.properties.xml;

import org.daisy.urakawa.exceptions.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 *
 */
public interface XmlAttribute extends XukAble {
    /**
     * @return the container element for this attribute.
     */
    public XmlProperty getParent();

    /**
     * The name of the XML attribute
     *
     * @return Cannot return NULL or an empty string, by contract.
     */
    public String getName();

    /**
     * The namespace of the XML attribute
     *
     * @return Cannot return NULL but can be an empty string, by contract.
     */
    public String getNamespace();

    /**
     * @return The attribute value. Cannot return NULL and cannot return an empty string.
     */
    public String getValue();

    /**
     * @param newValue cannot be null, cannot be empty String
     * @tagvalue Exceptions "MethodParameterIsNull, MethodParameterIsEmptyString"
     */
    public void setValue(String newValue) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

    /**
     * Should *only* be used at construction/initialization time (using the Factory).
     * (visibility is "public" because it's mandatory in Interfaces, but it would rather be "package"
     * so that only the Factory can call this method, not the end-user).
     *
     * @param newName cannot be null, cannot be empty String
     * @stereotype Initialize
     * @tagvalue Exceptions "MethodParameterIsNull, MethodParameterIsEmptyString"
     */
    public void setName(String newName) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

    /**
     * Should *only* be used at construction/initialization time (using the Factory).
     * (visibility is "public" because it's mandatory in Interfaces, but it would rather be "package"
     * so that only the Factory can call this method, not the end-user).
     *
     * @param newNS cannot be null,
     * @stereotype Initialize
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public void setNamespace(String newNS) throws MethodParameterIsNullException;

    /**
     * @return a distinct copy of the XmlAttribute object.
     */
    XmlAttribute copy();
}
