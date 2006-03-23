package org.daisy.urakawa.mediaObject;
import org.daisy.urakawa.exceptions.*;

/**
 * 
 */
class SequenceMediaObject implements MediaObject {

/**
 * @param index must be in bounds: [0..sequence.size-1]
 * @return the MediaObject item at a given index.
 */
public MediaObject getItem(unsigned_int index) throws MethodParameterIsValueOutOfBounds {} 

/**
 * Sets the MediaObject at a given index. Replaces the existing MediaObject, and returns it.
 * 
 * @param index must be in bounds: [0..sequence.size-1]
 * @param newItem cannot be null
 * @return 
 */
public MediaObject setItem(unsigned_int index, MediaObject newItem) throws MethodParameterIsNull, MethodParameterIsValueOutOfBounds {} 

/**
 * Appends a new MediaObject to the end of the sequence.
 * 
 * @param newItem cannot be null
 */
public void appendItem(MediaObject newItem) throws MethodParameterIsNull {} 

/**
 * Removes the MediaObject at a given index, and returns it.
 * 
 * @param index must be in bounds: [0..sequence.size-1]
 * @return 
 */
public MediaObject removeItem(unsigned_int index) throws MethodParameterIsValueOutOfBounds {} 

/**
 * 
 * @return the number of MediaObject items in the sequence
 */
public unsigned_int getCount() {} 
}