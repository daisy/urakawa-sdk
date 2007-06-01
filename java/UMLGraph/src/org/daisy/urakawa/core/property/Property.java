package org.daisy.urakawa.core.property;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @depend - Aggregation 1 TreeNode
 * @depend - - - PropertyType
 */
public interface Property extends XukAble, ValueEquatable<Property>  {
    /**
     * @return a clone (or "copy") of this property.
     */
    public Property copy();

    /**
     * @return the current "owner" of the Property.
     */
    public TreeNode getOwner();

    /**
     * @param newOwner cannot be NULL;
     * @stereotype Initialize
     */
    public void setOwner(TreeNode newOwner);
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

