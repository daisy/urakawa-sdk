package org.daisy.urakawa.properties.core;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.NavStructAlreadyExistsException;
import org.daisy.urakawa.exceptions.NavStructDoesNotExistException;

import java.util.List;

/**
 * Manages the list of available navigable structures in the presentation.
 *
 * @depend - Composition 0..n NavStruct
 */
public interface NavStructManager {
    /**
     * Adds an existing NavStruct to the list.
     *
     * @param navStruct cannot be null, navStruct must not already exist in the list.
     * @tagvalue Exceptions "MethodParameterIsNull, NavStructAlreadyExists"
     */
    public void addNavStruct(NavStruct navStruct) throws MethodParameterIsNullException, NavStructAlreadyExistsException;

    /**
     * Removes a given navStruct from the Presentation instance.
     *
     * @param navStruct cannot be null, the navStruct must exist in the list of current navStructs
     * @tagvalue Exceptions "MethodParameterIsNull, NavStructDoesNotExist"
     */
    public void removeNavStruct(NavStruct navStruct) throws MethodParameterIsNullException, NavStructDoesNotExistException;

    /**
     * @return the list of nav stucts that are used in the presentation. Cannot return null (no navStructs = returns an empty list).
     */
    public List<NavStruct> getNavStructs();
}
