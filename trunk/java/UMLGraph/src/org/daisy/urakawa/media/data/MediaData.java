package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @todo verify / add comments and exceptions
 * @depend - Aggregation 1..n DataProvider
 */
public interface MediaData extends WithMediaDataManager, XukAble,
		ValueEquatable<MediaData> {
	public String getUid();

	public String getName();

	/**
	 * 
	 * @param newName
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 */
	public void setName(String newName) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

	public void delete();

	public MediaData copy();

	public List<DataProvider> getListOfUsedDataProviders();
}
