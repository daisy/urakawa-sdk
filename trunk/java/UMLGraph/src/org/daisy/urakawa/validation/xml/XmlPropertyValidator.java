package org.daisy.urakawa.validation.xml;

import org.daisy.urakawa.exceptions.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.properties.xml.XmlProperty;
import org.daisy.urakawa.properties.xml.XmlAttribute;

/**
 * All the operations (aka "class methods") exposed here
 * have the same "return" value specification:
 * "return true if the operation is allowed in the current context, otherwise false."
 * When a user-agent of this API/Toolkit attempts to call a method "doXXX()" when
 * a corresponding "canDoXXX()" method returns false, then a "OperationNotValidException" error should be raised.
 *
 * @see org.daisy.urakawa.exceptions.OperationNotValidException
 * @see XmlProperty
 */
public interface XmlPropertyValidator {
    /**
     * @param attr cannot be null
     * @tagvalue Exceptions "MethodParameterIsNull"
     * @see XmlProperty#setAttribute(XmlAttribute)
     */
    public boolean canSetAttribute(XmlAttribute attr) throws MethodParameterIsNullException;

    /**
     * @param localName cannot be null, cannot be empty.
     * @param namespace cannot be null, but can be empty.
     * @param value     cannot be null, but can be empty.
     * @tagvalue Exceptions "MethodParameterIsNull"
     * @see XmlProperty#setAttribute(String,String,String)
     */
    public boolean canSetAttribute(String localName, String namespace, String value) throws MethodParameterIsNullException;

    /**
     * @param attr cannot be null
     * @tagvalue Exceptions "MethodParameterIsNull"
     * @see XmlProperty#removeAttribute(XmlAttribute)
     */
    public boolean canRemoveAttribute(XmlAttribute attr) throws MethodParameterIsNullException;

    /**
     * @param localName cannot be null, cannot be empty.
     * @param namespace cannot be null, but can be empty.
     * @tagvalue Exceptions "MethodParameterIsNull"
     * @see XmlProperty#removeAttribute(String,String)
     */
    public boolean canRemoveAttribute(String localName, String namespace) throws MethodParameterIsNullException;

    /**
     * @param newNS cannot be null,
     * @stereotype Initialize
     * @tagvalue Exceptions "MethodParameterIsNull"
     * @see XmlProperty#setNamespace(String)
     */
    public boolean canSetNamespace(String newNS) throws MethodParameterIsNullException;

    /**
     * @param newName cannot be null, cannot be empty String
     * @stereotype Initialize
     * @tagvalue Exceptions "MethodParameterIsNull, MethodParameterIsEmptyString"
     * @see XmlProperty#setName(String)
     */
    public boolean canSetName(String newName) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;
}

