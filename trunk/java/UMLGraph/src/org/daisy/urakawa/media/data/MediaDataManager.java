package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @depend - Composition 0..n MediaData
 */
public interface MediaDataManager extends XukAble, ValueEquatable<MediaDataManager> {
	MediaDataPresentation getPresentation();

	void setPresentation(MediaDataPresentation pres);

	MediaDataFactory getMediaDataFactory();
	void setMediaDataFactory(MediaDataFactory fact);

	DataProviderFactory getDataProviderFactory();
	void getDataProviderFactory(DataProviderFactory fact);

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
