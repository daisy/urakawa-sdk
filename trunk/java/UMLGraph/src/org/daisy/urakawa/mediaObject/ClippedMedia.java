package org.daisy.urakawa.mediaObject;
import org.daisy.urakawa.exceptions.*;


/**
 * 
 */
interface ClippedMedia extends MediaObject {
	
/**
 * @return the duration of the TimedMedia
 */
public TimeDelta getDuration() {};

/**
 * @return the begin time of the ClippedMedia within the external ressource
 */
public Time getBegin() {};

/**
 * @return the end time of the ClippedMedia within the external ressource
 */
public Time getEnd() {};

/**
 * Sets the begin time of the ClippedMedia within the external ressource.
 * 
 * @param newBegin cannot be null
 */
public void setBegin(Time newBegin) throws MethodParameterIsNull {};

/**
 * Sets the end time of the ClippedMedia within the external ressource.
 * 
 * @param newEnd cannot be null
 */
public void setEnd(Time newEnd) throws MethodParameterIsNull {};
}
