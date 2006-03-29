package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.exceptions.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * 
 */
public interface Attribute {

    /**
     * @return mNAme. Cannot return NULL and cannot return an empty string.
     */
    public String getName();

    /**
     * Sets mName.
     *
     * @param newName cannot be null, cannot be empty String
     */
    public void setName(String newName) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

    /**
     * @return mValue. Cannot return NULL and cannot return an empty string.
     */
    public String getValue();

    /**
     * Sets mValue.
     *
     * @param newValue cannot be null, cannot be empty String
     */
    public void setValue(String newValue) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;
    /**
     * @return mnamespace. Cannot return NULL but can return an empty string.
     */
    public String getNamespace();

    /**
     * Sets mNamespace.
     * 
     * @param newNS cannot be null, cannot be empty String
     */
    public void setNamespace(String newNS) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException; 
}
