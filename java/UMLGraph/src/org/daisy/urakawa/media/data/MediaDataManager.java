package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @todo verify / add comments and exceptions
 * @depend - Composition 0..n org.daisy.urakawa.media.data.MediaData
 * @depend - Clone - org.daisy.urakawa.media.data.MediaData
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface MediaDataManager extends WithPresentation, XukAble,
		ValueEquatable<MediaDataManager> {
	/**
	 * 
	 * @param uid
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 */
	public MediaData getMediaData(String uid)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

	/**
	 * 
	 * @param data
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public String getUidOfMediaData(MediaData data)throws MethodParameterIsNullException;

	public void addMediaData(MediaData data);

	public List<String> getListOfUids();

	/**
	 * 
	 * @param data
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public void detachMediaData(MediaData data)throws MethodParameterIsNullException;

	/**
	 * 
	 * @param uid
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 */
	public void deleteMediaData(String uid)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

	/**
	 * 
	 * @param data
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public MediaData copyMediaData(MediaData data)throws MethodParameterIsNullException;

	/**
	 * 
	 * @param uid
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 */
	public MediaData copyMediaData(String uid)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

	public List<MediaData> getListOfMediaData();
}
