package org.daisy.urakawa.property.xml;

import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * Getting and Setting a QName (fully qualified name), made of a local name and
 * a namespace.
 * </p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface IWithQualifiedName
{
    /**
     * The local part of the fully qualified name ("QName")
     * 
     * @return Cannot return NULL or an empty string.
     * @throws IsNotInitializedException
     */
    public String getLocalName() throws IsNotInitializedException;

    /**
     * The namespace part of the fully qualified name ("QName")
     * 
     * @return Cannot return NULL but can be an empty string.
     * @throws IsNotInitializedException
     */
    public String getNamespace() throws IsNotInitializedException;

    /**
     * The namespace part of the fully qualified name ("QName")
     * 
     * @param newNS
     *        cannot be null, but can be empty.
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @stereotype Initialize
     */
    public void setNamespace(String newNS)
            throws MethodParameterIsNullException;

    /**
     * The local part of the fully qualified name ("QName")
     * 
     * @param newName
     *        cannot be null, cannot be empty String
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws MethodParameterIsEmptyStringException
     *         Empty string '' method parameters are forbidden
     * @stereotype Initialize
     */
    public void setLocalName(String newName)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException;

    /**
     * @see #setLocalName(String)
     * @see #setNamespace(String)
     * @param localname
     *        cannot be null, cannot be empty String
     * @param namespace
     *        cannot be null, but can be empty String
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws MethodParameterIsEmptyStringException
     *         Empty string '' method parameter is forbidden for the local name
     * @stereotype Initialize
     * @tagvalue Events "QNameChanged"
     */
    public void setQName(String localname, String namespace)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException;
}
