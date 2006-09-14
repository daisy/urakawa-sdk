package org.daisy.urakawa.properties;

import org.daisy.urakawa.coreDataModel.CoreNode;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * The generic parent for all properties.
 */
public interface Property extends XUKAble {
    /**
     * @return a clone (or "copy") of this property.
     *         The actual Property object implementations must define the semantics of such copy,
     *         as it has critical implications in terms of memory management, shared object pools, etc.
     *         e.g.: a channel property that has a media object pointing to the actual file (like MP3 audio file)
     *         => Media should be sufficiently abstract and well-managed via the some sort of MediaAssetManager
     *         to guarantee that sharing conflicts are resolved transparently.
     */
    public Property copy();

    /**
     * Whether this property can be added to the given core node.
     * This method enables extending properties to define their own policy regarding whether a singl node
     * can be annotated with a unique or several of this type of property.
     *
     * @param node a node of the core tree.
     * @return true if and only if this property can be added to the given core node.
     * @throws MethodParameterIsNullException if the given node is null.
     */
    public boolean canBeAddedTo(CoreNode node) throws MethodParameterIsNullException;
}
