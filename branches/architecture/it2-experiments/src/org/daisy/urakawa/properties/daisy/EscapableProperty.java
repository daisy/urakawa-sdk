package org.daisy.urakawa.properties.daisy;

import org.daisy.urakawa.properties.core.FlagProperty;
import org.daisy.urakawa.coreDataModel.CoreNode;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * A node with this property is escapable.
 */
public interface EscapableProperty extends FlagProperty{

    /**
     * Only one EscapableProperty can be set to a CoreNode.
     * @hidden
     */
    boolean canBeAddedTo(CoreNode node) throws MethodParameterIsNullException;
}
