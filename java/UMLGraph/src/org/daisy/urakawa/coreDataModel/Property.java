package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.mediaObject.*;
import org.daisy.urakawa.exceptions.*;


enum PropertyType {
	STRUCTURE, SMIL, CHANNEL, XML;
}

/**
 * 
 */
public interface Property {
	
/**
 * Each class that implements the Property interface must have getType() return a different PropertyType.
 * This in Java would have been implemented with explicit class cast and instanceof operator,
 * but for C++ we have a safer type enumeration.
 * 
 * @return the PropertyType of the Property.
 */
public PropertyType getType() {};
}
