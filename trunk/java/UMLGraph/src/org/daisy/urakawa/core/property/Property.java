package org.daisy.urakawa.core.property;

import org.daisy.urakawa.core.CoreNode;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @depend - Aggregation 1 CoreNode
 * @depend - - - PropertyType
 */
public interface Property extends XukAble {
    /**
     * @return a clone (or "copy") of this property.
     *         The actual Property object implementations must define the semantics of such copy,
     *         as it has critical implications in terms of memory management, shared object pools, etc.
     *         e.g.: a ChannelsProperty that has Media objects pointing to actual files (like MP3 audio files)
     *         => Media should be sufficiently abstract and well-managed via the some sort of MediaAssetManager
     *         to guarantee that sharing conflicts are resolved transparently.
     */
    public Property copy();

    /**
     * @return the current "owner" of the Property.
     */
    public CoreNode getOwner();

    /**
     * @param newOwner cannot be NULL;
     * @stereotype Initialize
     */
    public void setOwner(CoreNode newOwner);
}

/**
 * @return the PropertyType of the Property.
 */
//public PropertyType getType();

/**
 * Should *only* be used at construction/initialization time (using the Factory).
 * (visibility is "public" because it's mandatory in Interfaces, but it would rather be "package"
 * so that only the Factory can call this method, not the end-user).
 *
 * @param type
 * @stereotype Initialize
 * @tagvalue Exceptions "PropertyTypeIsIllegal"
 */
//public void setType(PropertyType type) throws PropertyTypeIsIllegalException;

