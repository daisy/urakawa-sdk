package org.daisy.urakawa.properties.daisy;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.NavListAlreadyExistsException;
import org.daisy.urakawa.exceptions.NavListDoesNotExistException;

import java.util.List;

/**
 * Manages the list of available nav lists in the presentation.
 *
 * @depend - Composition 1..n NavList
 */
public interface NavListsManager {
    /**
     * Adds an existing NavList to the list.
     *
     * @param navList cannot be null, navList must not already exist in the list.
     * @tagvalue Exceptions "MethodParameterIsNull, NavListAlreadyExists"
     */
    public void addNavList(NavList navList) throws MethodParameterIsNullException, NavListAlreadyExistsException;

    /**
     * Removes a given navList from the Presentation instance.
     *
     * @param navList cannot be null, the navList must exist in the list of current navLists
     * @tagvalue Exceptions "MethodParameterIsNull, NavListDoesNotExist"
     */
    public void removeNavList(NavList navList) throws MethodParameterIsNullException, NavListDoesNotExistException;

    /**
     * @return the list of nav lists that are used in the presentation. Cannot return null (no navLists = returns an empty list).
     */
    public List<NavList> getNavLists();
}
