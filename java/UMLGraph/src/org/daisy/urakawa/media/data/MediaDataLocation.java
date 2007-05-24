package org.daisy.urakawa.media.data;

import org.daisy.urakawa.media.MediaLocation;

/**
 * @depend - Aggregation 1 MediaData
 */
public interface MediaDataLocation extends MediaLocation {
	MediaData getMediaData();
	void setMediaData(MediaData newData);
	MediaDataFactory getMediaDataFactory();
	MediaDataFactory setMediaDataFactory(MediaDataFactory fact);
}