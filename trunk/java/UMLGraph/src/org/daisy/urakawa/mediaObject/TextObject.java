package org.daisy.urakawa.mediaObject;
import org.daisy.urakawa.exceptions.*;


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
