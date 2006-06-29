package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.exceptions.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

import java.util.List;

/**
 * @depend 1 Composition 0..n XMLAttribute
 * @depend - Aggregation 1 XMLType
 */
public interface XMLProperty extends Property {
    /**
     * The type of the structure element described by the XMLProperty, one of element and text
     * in DAISY this is the type of xml node in the textual content document.
     * Remark that for a XMLProperty with mType text, mName and mNamespace and mAttributes
     * has no meaning if the XMLProperty describes xml.
     */
    public XMLType getXMLType();


    /**
     * The name of the structure element described by the XMLProperty
     * with DAISY this is used for the name of the XML element in the textual content document (DTBook).
     *
     * @return Cannot return NULL or an empty string, by contract.
     */
    public String getName();

    /**
     * The namespace of the structure element described by the XMLProperty
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
     * Should *only* be used at construction/initialization time (using the Factory).
     * (visibility is "public" because it's mandatory in Interfaces, but it would rather be "package"
     * so that only the Factory can call this method, not the end-user).
     *
     * @param newType
     * @stereotype Initialize
     */
    public void setXMLType(XMLType newType);

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