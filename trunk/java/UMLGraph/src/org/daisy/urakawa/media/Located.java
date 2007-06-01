package org.daisy.urakawa.media;

/**
 * Specifies the location of the data resource for a media object.
 * 
 * @todo verify / add comments and exceptions
 */
public interface Located {
	String getSrc();

	void setSrc(String newSrc);
}