package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.xuk.XukAble;

/**
 *
 */
public interface MediaData extends XukAble, ValueEquatable<MediaData> {
	List<DataProvider> getListOfUsedDataProviders();
	
	MediaDataManager getMediaDataManager();

	void setMediaDataManager(MediaDataManager mngr);

	String getUid();
	
	String getName();
	
	void setName(String newName);

	void delete();

	MediaData copy();
}
