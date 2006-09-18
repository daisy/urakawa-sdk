package org.daisy.urakawa.properties.daisy;

import org.daisy.urakawa.properties.core.LabelledNavDestinationProperty;

/**
 * A destination of a NavMap.
 * @depend - - 1 NavMap
 */
public interface NavPointProperty extends LabelledNavDestinationProperty {

    /**
     * Restricts to NavMap
     */
    NavMap getNavStruct();
}
