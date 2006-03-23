package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.exceptions.MethodParameterIsEmptyString;
import org.daisy.urakawa.exceptions.MethodParameterIsNull;

/**
 * 
 */
public class Attribute {
    /**
     * The name of the Attribute. Cannot be NULL and cannot be an empty string.
     */
    private String mName;
    /**
     * The value of the Attribute, Cannot be NULL and cannot be an empty string.
     */
    private String mValue;
    /**
     * The namespace of the Attribute. Cannot be NULL but can be an empty string.
     */
    private String mNamespace;

    /**
     * @return mNAme. Cannot return NULL and cannot return an empty string.
     */
    public String getName() {
        return null;
    }

    /**
     * Sets mName.
     *
     * @param newName cannot be null, cannot be empty String
     */
    public void setName(String newName) throws MethodParameterIsNull, MethodParameterIsEmptyString {
    }

    /**
     * @return mValue. Cannot return NULL and cannot return an empty string.
     */
    public String getValue() {
        return null;
    }

    /**
     * Sets mValue.
     *
     * @param newValue cannot be null, cannot be empty String
     */
    public void setValue(String newValue) throws MethodParameterIsNull, MethodParameterIsEmptyString {
    }

    /**
     * @return mnamespace. Cannot return NULL but can return an empty string.
     */
    public String getNamespace() {
        return null;
    }

    /**
     * Sets mNamespace.
     * 
     * @param newNS cannot be null, cannot be empty String
     */
    public void setNamespace(String newNS) throws MethodParameterIsNull, MethodParameterIsEmptyString {} 
}
