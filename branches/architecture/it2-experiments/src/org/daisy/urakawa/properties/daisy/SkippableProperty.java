package org.daisy.urakawa.properties.daisy;

import org.daisy.urakawa.properties.core.FlagProperty;
import org.daisy.urakawa.coreDataModel.CoreNode;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * A node with this property is skippable.
 */
public interface SkippableProperty extends FlagProperty {
    /**
     * @return The skippable type of the skippable node annotated with this property.
     */
    SkippableType getSkippableType();

    @Override
    /**
     * Only one SkippableProperty can be set to a CoreNode for a given SkippableType.
     */
    boolean canBeAddedTo(CoreNode node) throws MethodParameterIsNullException;
}
