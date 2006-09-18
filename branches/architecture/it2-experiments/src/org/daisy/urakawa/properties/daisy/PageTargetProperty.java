package org.daisy.urakawa.properties.daisy;

import org.daisy.urakawa.properties.core.LabelledNavDestinationProperty;

/**
 * A destination of a PageList.
 * @depend - - 1 PageList
 */
public interface PageTargetProperty extends LabelledNavDestinationProperty {
    /**
     * @return The number of the page defined by this property.
     */
    int getPageNumber();

    /**
     * Restricts to PageList.
     */
    PageList getNavStruct();

}
