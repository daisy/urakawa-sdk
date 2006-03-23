package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.mediaObject.*;
import org.daisy.urakawa.exceptions.*;



/**
 * 
 */
public class Attribute {

/**
 * The name of the Attribute. Cannot be NULL and cannot be an empty string.
 */
private string mName;

/**
 * The value of the Attribute, Cannot be NULL and cannot be an empty string.
 */
private string mValue;

/**
 * The namespace of the Attribute. Cannot be NULL but can be an empty string.
 * 
 */
private string mNamespace;

/**
 * @return mNAme. Cannot return NULL and cannot return an empty string.
 */
public string getName() {} 

/**
 * Sets mName.
 * 
 * @param newName cannot be null, cannot be empty String
 */
public void setName(string newName) throws MethodParameterIsNull, MethodParameterIsEmptyString {} 

/**
 * @return mValue. Cannot return NULL and cannot return an empty string.
 */
public string getValue() {} 

/**
 * Sets mValue.
 * 
 * @param newValue cannot be null, cannot be empty String
 */
public void setValue(string newValue) throws MethodParameterIsNull, MethodParameterIsEmptyString {} 

/**
 * @return mnamespace. Cannot return NULL but can return an empty string.
 */
public string getNamespace() {} 

/**
 * Sets mNamespace.
 * 
 * @param newNS cannot be null, cannot be empty String
 */
public void setNamespace(string newNS) throws MethodParameterIsNull, MethodParameterIsEmptyString {} 
}
