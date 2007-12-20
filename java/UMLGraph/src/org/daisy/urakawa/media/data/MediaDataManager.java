package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.IsNotManagerOfException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.data.audio.PCMFormatInfo;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @depend - Composition 0..n org.daisy.urakawa.media.data.MediaData
 * @depend - Clone - org.daisy.urakawa.media.data.MediaData
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @stereotype XukAble
 */
public interface MediaDataManager extends WithPresentation, XukAble,
		ValueEquatable<MediaDataManager> {
	/**
	 * @param uid
	 * @return true or false
	 * @throws MethodParameterIsNullException
	 * @throws MethodParameterIsEmptyStringException
	 */
	public boolean isManagerOf(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * @param uid
	 * @return data
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 */
	public MediaData getMediaData(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * @param data
	 * @return uid
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws IsNotManagerOfException
	 */
	public String getUidOfMediaData(MediaData data)
			throws MethodParameterIsNullException, IsNotManagerOfException;

	/**
	 * @param data
	 * @throws MethodParameterIsNullException
	 */
	public void addMediaData(MediaData data)
			throws MethodParameterIsNullException;

	/**
	 * @return list
	 */
	public List<String> getListOfUids();

	/**
	 * @param data
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public void removeMediaData(MediaData data)
			throws MethodParameterIsNullException;

	/**
	 * @param uid
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 */
	public void removeMediaData(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * @param data
	 * @return data
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws IsNotManagerOfException
	 */
	public MediaData copyMediaData(MediaData data)
			throws MethodParameterIsNullException, IsNotManagerOfException;

	/**
	 * @param uid
	 * @return data
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 * @throws IsNotManagerOfException
	 */
	public MediaData copyMediaData(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException, IsNotManagerOfException;

	/**
	 * @return list
	 */
	public List<MediaData> getListOfMediaData();

	/**
	 * @param newValue
	 * @throws InvalidDataFormatException
	 */
	public void setEnforceSinglePCMFormat(boolean newValue)
			throws InvalidDataFormatException;

	/**
	 * @return true or false
	 */
	public boolean getEnforceSinglePCMFormat();

	/**
	 * @param numberOfChannels
	 * @param sampleRate
	 * @param bitDepth
	 * @throws MethodParameterIsOutOfBoundsException 
	 */
	public void setDefaultPCMFormat(short numberOfChannels, int sampleRate,
			short bitDepth) throws MethodParameterIsOutOfBoundsException;

	/**
	 * @param newDefault
	 * @throws MethodParameterIsNullException
	 * @throws InvalidDataFormatException
	 */
	public void setDefaultPCMFormat(PCMFormatInfo newDefault)
			throws MethodParameterIsNullException, InvalidDataFormatException;

	/**
	 * @param numberOfChannels
	 * @throws MethodParameterIsOutOfBoundsException 
	 */
	public void setDefaultNumberOfChannels(short numberOfChannels) throws MethodParameterIsOutOfBoundsException;

	/**
	 * @param sampleRate
	 * @throws MethodParameterIsOutOfBoundsException 
	 */
	public void setDefaultSampleRate(int sampleRate) throws MethodParameterIsOutOfBoundsException;

	/**
	 * @param bitDepth
	 * @throws MethodParameterIsOutOfBoundsException 
	 */
	public void setDefaultBitDepth(short bitDepth) throws MethodParameterIsOutOfBoundsException;

	/**
	 * @return factory
	 */
	public MediaDataFactory getMediaDataFactory();

	/**
	 * @return factory
	 */
	public DataProviderFactory getDataProviderFactory();

	/**
	 * @return format info
	 */
	public PCMFormatInfo getDefaultPCMFormat();

	/**
	 * @param data
	 * @param uid
	 * @throws MethodParameterIsEmptyStringException
	 * @throws MethodParameterIsNullException
	 */
	public void setDataMediaDataUid(MediaData data, String uid)
			throws MethodParameterIsEmptyStringException,
			MethodParameterIsNullException;

	/**
	 * 
	 */
	public void deleteUnusedMediaData();
}
