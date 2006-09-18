package org.daisy.urakawa.properties.core;

import org.daisy.urakawa.properties.Property;
import org.daisy.urakawa.coreDataModel.CoreNode;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * Annotates a node as being one of the destinations listed in a navigable structure.
 * @depend - - 1 NavStruct
 * @depend - - 0..1 NavDestinationProperty
 * @depend - - 0..1 NavDestinationProperty
 */
public interface NavDestinationProperty extends Property {
    /**
     * Returns the navigation structure this destination belongs to. Must not be null.
     * @return The navigation structure this destination belongs to.
     */
    NavStruct getNavStruct();

    /**
     * Returns the previous destination in the navigable structure this destination belongs to,
     * or null if this is the first.
     * @return the previous destination.
     */
    NavDestinationProperty previous();

    /**
     * Returns the next destination in the navigable structure this destination belongs to,
     * or null if this is the last.
     * @return the next destination.
     */
    NavDestinationProperty next();

    /**
     * Only one NavDestinationProperty can be set to a CoreNode for a given NavStruct
     */
    boolean canBeAddedTo(CoreNode node) throws MethodParameterIsNullException;
}
