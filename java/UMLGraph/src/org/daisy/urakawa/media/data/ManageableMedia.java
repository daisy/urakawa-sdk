package org.daisy.urakawa.media.data;

import java.util.List;

public interface ManageableMedia {
	public MediaData getMediaData();
	public void setMediaData(MediaData data);
	List<DataProvider> getListOfUsedDataProviders();
}
