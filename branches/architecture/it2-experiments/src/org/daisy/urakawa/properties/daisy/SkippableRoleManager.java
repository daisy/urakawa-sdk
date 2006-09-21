package org.daisy.urakawa.properties.daisy;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

import java.util.List;

/**
 * Manages the list of available skippable types in the presentation.
 *
 * @depend - Composition 0..n SkippableRole
 */
public interface SkippableRoleManager {
    /**
     * Adds an existing skippable type to the list.
     *
     * @param skippableRole cannot be null, skippableRole must not already exist in the list.
     * @tagvalue Exceptions "MethodParameterIsNull, SkippableRoleAlreadyExists"
     */
    public void addSkippableRole(SkippableRole skippableRole) throws MethodParameterIsNullException;

    /**
     * Removes a given skippable type from the Presentation instance.
     *
     * @param skippableRole cannot be null, the skippable type must exist in the list of current skippable types
     * @tagvalue Exceptions "MethodParameterIsNull, SkippableRoleDoesNotExist"
     */
    public void removeSkippableRole(SkippableRole skippableRole) throws MethodParameterIsNullException;

    /**
     * @return the list of skippable types that are used in the presentation. Cannot return null (no skippable types = returns an empty list).
     */
    public List<SkippableRole> getSkippableRoles();
}
