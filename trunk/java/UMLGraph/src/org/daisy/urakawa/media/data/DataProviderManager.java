package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @depend - Composition 0..n DataProvider
 * @todo verify / add comments and exceptions
 */
public interface DataProviderManager extends WithPresentation,
		WithDataProviderFactory, XukAble, ValueEquatable<DataProviderManager> {
	public String getUidOfDataProvider(DataProvider provider)throws MethodParameterIsNullException;

	public DataProvider getDataProvider(String uid)throws MethodParameterIsNullException;

	public void detachDataProvider(DataProvider provider)throws MethodParameterIsNullException;

	public void detachDataProvider(String uid)throws MethodParameterIsNullException;

	public void addDataProvider(DataProvider provider)throws MethodParameterIsNullException;

	public List<DataProvider> getListOfManagedDataProviders();

	public void deleteUnusedDataProviders();
}
