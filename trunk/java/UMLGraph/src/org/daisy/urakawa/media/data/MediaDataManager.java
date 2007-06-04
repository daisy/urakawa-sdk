package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @todo verify / add comments and exceptions
 * @depend - Composition 0..n MediaData
 */
public interface MediaDataManager extends WithDataProviderFactory,
		WithMediaDataFactory, WithPresentation, XukAble,
		ValueEquatable<MediaDataManager> {
	/**
	 * 
	 * @param uid
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 */
	MediaData getMediaData(String uid)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

	/**
	 * 
	 * @param data
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	String getUidOfMediaData(MediaData data)throws MethodParameterIsNullException;

	void addMediaData(MediaData data);

	List<String> getListOfUids();

	/**
	 * 
	 * @param data
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	void detachMediaData(MediaData data)throws MethodParameterIsNullException;

	/**
	 * 
	 * @param uid
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 */
	void deleteMediaData(String uid)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

	/**
	 * 
	 * @param data
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	MediaData copyMediaData(MediaData data)throws MethodParameterIsNullException;

	/**
	 * 
	 * @param uid
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 */
	MediaData copyMediaData(String uid)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

	List<MediaData> getListOfManagedMediaData();
}
