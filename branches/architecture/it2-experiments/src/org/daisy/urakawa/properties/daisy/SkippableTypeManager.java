package org.daisy.urakawa.properties.daisy;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

import java.util.List;

/**
 * Manages the list of available skippable types in the presentation.
 *
 * @depend - Composition 0..n SkippableType
 */
public interface SkippableTypeManager {
    /**
     * Adds an existing skippable type to the list.
     *
     * @param skippableType cannot be null, skippableType must not already exist in the list.
     * @tagvalue Exceptions "MethodParameterIsNull, SkippableTypeAlreadyExists"
     */
    public void addSkippableType(SkippableType skippableType) throws MethodParameterIsNullException;

    /**
     * Removes a given skippable type from the Presentation instance.
     *
     * @param skippableType cannot be null, the skippable type must exist in the list of current skippable types
     * @tagvalue Exceptions "MethodParameterIsNull, SkippableTypeDoesNotExist"
     */
    public void removeSkippableType(SkippableType skippableType) throws MethodParameterIsNullException;

    /**
     * @return the list of skippable types that are used in the presentation. Cannot return null (no skippable types = returns an empty list).
     */
    public List<SkippableType> getSkippableTypes();
}
