package org.daisy.urakawa.coreDataModel;

/**
 * @depend - Aggregation 1 PropertyType
 */
public interface Property {
    /**
     * @return the PropertyType of the Property.
     */
    public PropertyType getType();

    /**
     * Should *only* be used at construction/initialization time (using the Factory).
     * (visibility is "public" because it's mandatory in Interfaces, but it would rather be "package"
     * so that only the Factory can call this method, not the end-user).
     *
     * @param type
     * @stereotype Initialize
     */
    public void setType(PropertyType type);
}
