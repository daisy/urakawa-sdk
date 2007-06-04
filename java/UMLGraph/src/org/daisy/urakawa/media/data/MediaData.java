package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @todo verify / add comments and exceptions
 * @depend - Aggregation 1..n DataProvider
 * @depend - Clone - MediaData
 * @depend - Aggregation 1 MediaDataManager
 */
public interface MediaData extends WithMediaDataManager, XukAble,
		ValueEquatable<MediaData> {
	public String getUid();

	public String getName();

	/**
	 * @param newName
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 */
	public void setName(String newName) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	public void delete();

	public MediaData copy();

	public List<DataProvider> getListOfUsedDataProviders();
}
