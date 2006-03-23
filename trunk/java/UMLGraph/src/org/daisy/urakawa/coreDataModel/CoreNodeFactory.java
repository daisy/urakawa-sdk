package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.mediaObject.*;
import org.daisy.urakawa.exceptions.*;


/**
 * 
 */
interface CoreNodeFactory {
	
/**
 * Creates a new node, which is not linked to the core data tree yet.
 * @return cannot return null.
 */
public CoreNode createNode() {};
}