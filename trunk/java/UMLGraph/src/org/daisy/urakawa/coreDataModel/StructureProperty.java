package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.exceptions.MethodParameterIsEmptyString;
import org.daisy.urakawa.exceptions.MethodParameterIsNull;

import java.util.Enumeration;

/**
 * 
 */
public class StructureProperty implements Property {
    public PropertyType getType() {
        return null;
    }

    //enum StructureType {ELEMENT, TEXT;}
    public class StructureType implements Enumeration {
        public boolean hasMoreElements() {
            return false;  //To change body of implemented methods use File | Settings | File Templates.
        }

        public Object nextElement() {
            return null;  //To change body of implemented methods use File | Settings | File Templates.
        }
    }

    /**
     * The name of the structure element described by the StructureProperty
     * with DAISY this is used for the name of the XML element in the textual content document.
     * Cannot be NULL and cannot be an empty string.
     */
    private String mName;
    /**
     * The namespace of the structure element described by the StructureProperty
     * in DAISY this is the namespace of the xml element in the textual content document.
     * Cannot be NULL and can be an empty string.
     */
    private String mNamespace;
    /**
     * The type of the structure element described by the StructureProperty, one of element and text
     * in DAISY this is the type of xml node in the textual content document.
     * Remark that for a StructureProperty with mType text, mName and mNamespace and mAttributes
     * has no meaning if the StructureProperty describes xml.
     */
    private StructureType mType;

    /**
     * @return the mName. Cannot return NULL or an empty string, by contract.
     */
    public String getName() {
        return mName;
    }

    /**
     * @return mNamespace. Cannot return NULL but can be an empty string, by contract.
     */
    public String getNamespace() {
        return mNamespace;
    }

    /**
     * Sets mName.
     *
     * @param newName cannot be null, cannot be empty String
     */
    public void setName(String newName) throws MethodParameterIsNull, MethodParameterIsEmptyString {
    }

    /**
     * Sets mNamespace.
     *
     * @param newNS cannot be null,
     */
    public void setNamespace(String newNS) throws MethodParameterIsNull {
    }

    /**
     * @param attrName cannot be null, cannot be empty String
     * @return the value of the attribute with a given name.Cannot return NULL and cannot return an empty string.
     */
    public String getAttributeValue(String attrName) throws MethodParameterIsNull, MethodParameterIsEmptyString {
        return null;
    }

    /**
     * @param attrName cannot be null, cannot be empty String
     * @param attrNS   cannot be null, cannot be empty String
     * @return the value of the attribute with a given name and namespace. Cannot return NULL and cannot return an empty string.
     */
    public String getAttributeValue(String attrName, String attrNS) throws MethodParameterIsNull, MethodParameterIsEmptyString {
        return null;
    }

    /**
     * Sets the value of the attribute with a given name.
     *
     * @param attrName cannot be null, cannot be empty
     * @param value    cannot be null, cannot be empty
     */
    public void setAttributeValue(String attrName, String value) throws MethodParameterIsNull, MethodParameterIsEmptyString {
    }

    /**
     * Sets the value of the attribute with a given name and namespace.
     *
     * @param attrName cannot be null
     * @param attrNS   cannot be null
     * @param value    cannot be null
     */
    public void setAttributeValue(String attrName, String attrNS, String value) throws MethodParameterIsNull, MethodParameterIsEmptyString {
    }

    /**
     * @return mType
     */
    public StructureType getStructureType() {
        return null;
    }

    /**
     * Sets mType
     *
     * @param newType
     */
    public void setStructureType(StructureType newType) {
    }
}