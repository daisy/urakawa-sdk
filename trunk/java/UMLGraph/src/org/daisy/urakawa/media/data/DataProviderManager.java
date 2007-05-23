package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.xuk.XukAble;

public interface DataProviderManager extends XukAble, ValueEquatable<DataProviderManager> {
	MediaDataPresentation getMediaDataPresentation();

	void setPresentation(MediaDataPresentation ownerPres);

	DataProviderFactory getDataProviderFactory();

	String getUidOfDataProvider(DataProvider provider);

	DataProvider getDataProvider(String uid);

	void detachDataProvider(DataProvider provider);

	void detachDataProvider(String uid);

	void addDataProvider(DataProvider provider);

	List<DataProvider> getListOfManagedDataProviders();
}
