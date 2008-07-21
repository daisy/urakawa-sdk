package org.daisy.urakawa.metadata;

import java.util.List;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * Adding and Removing metadata for the IPresentation. This corresponds to a UML
 * composition relationship, so this IMetadata is owned by the IPresentation.
 * </p>
 * 
 */
public interface IWithMetadata
{
    /**
     * Adds the given IMetadata to the IProject
     * 
     * @param iMetadata cannot be null
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * 
     * @tagvalue Events "MetadataAdded"
     */
    public void addMetadata(IMetadata iMetadata)
            throws MethodParameterIsNullException;

    /**
     * Gets a list of all the IMetadata in the IProject.
     * 
     * @return cannot be null (but can return empty list)
     */
    public List<IMetadata> getListOfMetadata();

    /**
     * Gets a list of all the IMetadata in the IProject with the given name.
     * 
     * @return cannot be null (but can return empty list)
     * @param name cannot be null or empty string.
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * @throws MethodParameterIsEmptyStringException Empty string '' method
     *         parameters are forbidden
     * 
     */
    public List<IMetadata> getListOfMetadata(String name)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException;

    /**
     * Deletes all the IMetadata with the given name.
     * 
     * @param name cannot be null or empty string.
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * @throws MethodParameterIsEmptyStringException Empty string '' method
     *         parameters are forbidden
     * 
     */
    public void deleteMetadata(String name)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException;

    /**
     * Deletes the given IMetadata
     * 
     * @param iMetadata cannot be null
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * 
     * @tagvalue Events "MetadataRemoved"
     */
    public void deleteMetadata(IMetadata iMetadata)
            throws MethodParameterIsNullException;
}
