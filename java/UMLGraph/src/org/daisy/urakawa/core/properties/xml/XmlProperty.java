package org.daisy.urakawa.core.properties.xml;

import org.daisy.urakawa.exceptions.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.core.properties.Property;

import java.util.List;

/**
 * @depend 1 Composition 0..n XmlAttribute
 * @depend - Aggregation 1 XmlType
 */
public interface XmlProperty extends Property {
    /**
     * The type of the structure element described by the XmlProperty, one of element and text
     * in DAISY this is the type of xml node in the textual content document.
     * Remark that for a XmlProperty with mType text, mName and mNamespace and mAttributes
     * has no meaning if the XmlProperty describes xml.
     */
    public XmlType getXMLType();

    /**
     * The name of the structure element described by the XmlProperty
     * with DAISY this is used for the name of the XML element in the textual content document (DTBook).
     *
     * @return Cannot return NULL or an empty string, by contract.
     */
    public String getName();

    /**
     * The namespace of the structure element described by the XmlProperty
     * in DAISY this is the namespace of the xml element in the textual content document (DTBook).
     *
     * @return Cannot return NULL but can be an empty string, by contract.
     */
    public String getNamespace();

    /**
     * @return The list of attributes the XML element has.
     */
    public List getListOfAttributes();

    /**
     * @param attr cannot be null
     * @return true if the attribute was already existing, which means after method is executed the attribute has been overriden by the new value.
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public boolean setAttribute(XmlAttribute attr) throws MethodParameterIsNullException;

    /**
     * @param namespace cannot be null, but can be empty.
     * @param name      cannot be null
     * @return returns the attribute for the given namespace and local name. can return NULL.
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public XmlAttribute getAttribute(String namespace, String name) throws MethodParameterIsNullException;

    /**
     * Should *only* be used at construction/initialization time (using the Factory).
     * (visibility is "public" because it's mandatory in Interfaces, but it would rather be "package"
     * so that only the Factory can call this method, not the end-user).
     *
     * @param newType
     * @stereotype Initialize
     */
    public void setXMLType(XmlType newType);

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
     * Should *only* be used at construction/initialization time (using the Factory).
     * (visibility is "public" because it's mandatory in Interfaces, but it would rather be "package"
     * so that only the Factory can call this method, not the end-user).
     *
     * @param newName cannot be null, cannot be empty String
     * @stereotype Initialize
     * @tagvalue Exceptions "MethodParameterIsNull, MethodParameterIsEmptyString"
     */
    public void setName(String newName) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;
}