package mediaObject;

import java.util.List;
import java.util.Map;
import java.util.ArrayList;
import java.util.Arrays;

import exceptions.*;

/**
 * @view
 * @opt nodefillcolor LemonChiffon
 *
 * @match class *
 * @opt hide
 *
 * @match class mediaObject.*
 * @opt !hide
 *
 */
class ViewMediaObject extends ViewBase {}



/**
 * Time duration expressed in milliseconds (cannot be negative).
 */
class TimeDelta {

/**
 * 
 */
private unsigned_long mTimeDelta;
}

/**
 * Time point expressed in milliseconds, relative to the local timebase, can be negative.
 * 
 */
class Time {

/**
 * 
 */
private long mTime;
}

/**
 * 
 */
interface MediaObject {}

/**
 * 
 */
class ImageObject implements ExtAssetMedia {}

/**
 * 
 */
class VideoObject implements ClippedMedia {

/**
 * Splits the VideoObject at the given split point.
 * After execution the instance represents the video before the split point.
 * 
 * @param splitPoint cannot be null
 * @return a VideoObject representing the video after the split point.
 */
public VideoObject splitVideo(Time splitPoint) throws MethodParameterIsNull {} 
}

/**
 * 
 */
class AudioObject {

/**
 * Splits the AudioObject at the given split point.
 * After execution the instance represents the audio before the split point.
 *
 * @param splitPoint cannot be null
 * @return a AudioObject representing the audio after the split point
 */
public AudioObject splitAudio(Time splitPoint) throws MethodParameterIsNull {} 
}

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

/**
 * 
 */
interface ExtAssetMedia extends MediaObject {
	
/**
 * 
 * @return the URI of the external asset. by contract, cannot return NULL or empty String.
 */
public string getURI() {};

/**
 * Sets the URI of the external asset.
 * 
 * @param newURI cannot be null
 */
public void setURI(string newURI) throws MethodParameterIsEmptyString, MethodParameterIsNull {};
}

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


/**
 * 
 */
class TextObject implements MediaObject {

/**
 * 
 */
private string mText;

/**
 * @return cannot return NULL or empty String
 */
public string getText() {return mText;} 

/**
 * @param text cannot be null, cannot be empty String
 */
public void setText(string text) throws MethodParameterIsNull, MethodParameterIsEmptyString {mText = text;} 
}
