package org.daisy.urakawa.media.data;

/**
 * @todo verify / add comments and exceptions
 */
public interface ManagedMedia extends WithMediaDataFactory {
	public MediaData getMediaData();

	public void setMediaData(MediaData data);
}
