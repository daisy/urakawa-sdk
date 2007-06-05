package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @todo verify / add comments and exceptions
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 * @depend - Composition 0..n org.daisy.urakawa.media.data.DataProvider
 */
public interface DataProviderManager extends WithPresentation, XukAble,
		ValueEquatable<DataProviderManager> {
	/**
	 * @param provider
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public String getUidOfDataProvider(DataProvider provider)
			throws MethodParameterIsNullException;

	/**
	 * @param uid
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 */
	public DataProvider getDataProvider(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * @param provider
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public void detachDataProvider(DataProvider provider)
			throws MethodParameterIsNullException;

	/**
	 * @param uid
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 */
	public void detachDataProvider(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * @param provider
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public void addDataProvider(DataProvider provider)
			throws MethodParameterIsNullException;

	public List<DataProvider> getListOfDataProviders();

	public void deleteUnusedDataProviders();
}
