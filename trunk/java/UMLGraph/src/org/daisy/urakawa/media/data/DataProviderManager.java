package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @depend - Composition 0..n DataProvider
 * @todo verify / add comments and exceptions
 */
public interface DataProviderManager extends WithPresentation,
		WithDataProviderFactory, XukAble, ValueEquatable<DataProviderManager> {
	public String getUidOfDataProvider(DataProvider provider);

	public DataProvider getDataProvider(String uid);

	public void detachDataProvider(DataProvider provider);

	public void detachDataProvider(String uid);

	public void addDataProvider(DataProvider provider);

	public List<DataProvider> getListOfManagedDataProviders();

	public void deleteUnusedDataProviders();
}
