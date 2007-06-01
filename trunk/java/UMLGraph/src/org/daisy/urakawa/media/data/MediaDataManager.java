package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @todo verify / add comments and exceptions
 * @depend - Composition 0..n MediaData
 */
public interface MediaDataManager extends WithDataProviderFactory,
		WithMediaDataFactory, WithPresentation, XukAble,
		ValueEquatable<MediaDataManager> {
	MediaData getMediaData(String uid);

	String getUidOfMediaData(MediaData data);

	void addMediaData(MediaData data);

	List<String> getListOfUids();

	void detachMediaData(MediaData data);

	void deleteMediaData(String uid);

	MediaData copyMediaData(MediaData data);

	MediaData copyMediaData(String uid);

	List<MediaData> getListOfManagedMediaData();
}
