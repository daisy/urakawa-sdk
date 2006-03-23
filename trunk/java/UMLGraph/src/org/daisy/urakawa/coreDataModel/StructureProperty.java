package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.mediaObject.*;
import org.daisy.urakawa.exceptions.*;


/**
 * 
 */
public class StructureProperty implements Property {

	enum StructureType {
		ELEMENT, TEXT;
	}
	
/**
 * The name of the structure element described by the StructureProperty
 * with DAISY this is used for the name of the XML element in the textual content document.
 * Cannot be NULL and cannot be an empty string.
 */
private string mName;

/**
 * The namespace of the structure element described by the StructureProperty
 * in DAISY this is the namespace of the xml element in the textual content document.
 * Cannot be NULL and can be an empty string.
 */
private string mNamespace;

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
public string getName() {return mName;} 

/**
 * @return mNamespace. Cannot return NULL but can be an empty string, by contract.
 */
public string getNamespace() {return mNamespace;} 

/**
 * Sets mName.
 *
 * @param newName cannot be null, cannot be empty String
 */
public void setName(string newName) throws MethodParameterIsNull, MethodParameterIsEmptyString {} 

/**
 * Sets mNamespace.
 * 
 * @param newNS cannot be null,
 */
public void setNamespace(string newNS) throws MethodParameterIsNull {} 

/**
 * @param attrName cannot be null, cannot be empty String
 * @return the value of the attribute with a given name.Cannot return NULL and cannot return an empty string.
 */
public string getAttributeValue(string attrName) throws MethodParameterIsNull, MethodParameterIsEmptyString {} 

/**
 * @param attrName cannot be null, cannot be empty String
 * @param attrNS cannot be null, cannot be empty String
 * @return the value of the attribute with a given name and namespace. Cannot return NULL and cannot return an empty string.
 */
public string getAttributeValue(string attrName, string attrNS) throws MethodParameterIsNull, MethodParameterIsEmptyString {} 

/**
 * Sets the value of the attribute with a given name.
 * 
 * @param attrName cannot be null, cannot be empty
 * @param value cannot be null, cannot be empty
 */
public void setAttributeValue(string attrName, string value) throws MethodParameterIsNull, MethodParameterIsEmptyString {} 

/**
 * Sets the value of the attribute with a given name and namespace.
 * 
 * @param attrName cannot be null
 * @param attrNS cannot be null
 * @param value cannot be null
 */
public void setAttributeValue(string attrName, string attrNS, string value) throws MethodParameterIsNull, MethodParameterIsEmptyString {} 

/**
 * @return mType
 */
public StructureType getStructureType() {} 

/**
 * Sets mType
 *
 * @param newType 
 */
public void setStructureType(StructureType newType) {} 
}