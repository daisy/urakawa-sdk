package org.daisy.urakawa.coreDataModel;

import java.util.Enumeration;

/**
 * 
 */
public interface Property {
    //enum PropertyType {STRUCTURE, SMIL, CHANNEL, XML;}
    public class PropertyType implements Enumeration {
        public boolean hasMoreElements() {
            return false;  //To change body of implemented methods use File | Settings | File Templates.
        }

        public Object nextElement() {
            return null;  //To change body of implemented methods use File | Settings | File Templates.
        }
    }

    /**
     * Each class that implements the Property interface must have getType() return a different PropertyType.
     * This in Java would have been implemented with explicit class cast and instanceof operator,
     * but for C++ we have a safer type enumeration.
     *
     * @return the PropertyType of the Property.
     */
    public PropertyType getType();
}
