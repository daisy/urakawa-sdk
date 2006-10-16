package org.daisy.urakawa.properties;

import org.daisy.urakawa.exceptions.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * All the operations (aka "class methods") exposed here
 * have the same "return" value specification:
 * "return true if the operation is allowed in the current context, otherwise false."
 * When a user-agent of this API/Toolkit attempts to call a method "doXXX()" when
 * a corresponding "canDoXXX()" method returns false, then a "OperationNotValidException" error should be raised.
 *
 * @see org.daisy.urakawa.exceptions.OperationNotValidException
 * @see XMLProperty
 */
public interface XMLPropertyValidator {
    /**
     * @param attr cannot be null
     * @return true if the attribute was already existing, which means after method is executed the attribute has been overriden by the new value.
     * @tagvalue Exceptions "MethodParameterIsNull"
     * @see XMLProperty#setAttribute(XMLAttributeattr)
     */
    public boolean canSetAttribute(XMLAttribute attr) throws MethodParameterIsNullException;

    /**
     * @param newNS cannot be null,
     * @stereotype Initialize
     * @tagvalue Exceptions "MethodParameterIsNull"
     * @see XMLProperty#setNamespace(StringnewNS)
     */
    public boolean canSetNamespace(String newNS) throws MethodParameterIsNullException;

    /**
     * @param newName cannot be null, cannot be empty String
     * @stereotype Initialize
     * @tagvalue Exceptions "MethodParameterIsNull, MethodParameterIsEmptyString"
     * @see XMLProperty#setName(String newname)
     */
    public boolean canSetName(String newName) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;
}

