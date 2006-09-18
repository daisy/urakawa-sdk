package org.daisy.urakawa.properties.daisy;

import org.daisy.urakawa.properties.core.LabelledNavDestinationProperty;

/**
 * A destination of a NavList.
 * @depend - - 1 NavList
 */
public interface NavTargetProperty extends LabelledNavDestinationProperty {

    /**
     * Restricts to NavList
     */
    NavList getNavStruct();
}
