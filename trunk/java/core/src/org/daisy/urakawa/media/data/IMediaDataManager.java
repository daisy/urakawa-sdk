package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.IValueEquatable;
import org.daisy.urakawa.IWithPresentation;
import org.daisy.urakawa.exception.IsNotManagerOfException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.data.audio.IPCMFormatInfo;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * @depend - Composition 0..n org.daisy.urakawa.media.data.IMediaData
 * @depend - Clone - org.daisy.urakawa.media.data.IMediaData
 * @depend - Aggregation 1 org.daisy.urakawa.IPresentation
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface IMediaDataManager extends IWithPresentation, IXukAble,
        IValueEquatable<IMediaDataManager>
{
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
     * 
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * @throws MethodParameterIsEmptyStringException Empty string '' method
     *         parameters are forbidden
     */
    public IMediaData getMediaData(String uid)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException;

    /**
     * @param data
     * @return uid
     * 
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * @throws IsNotManagerOfException
     */
    public String getUidOfMediaData(IMediaData data)
            throws MethodParameterIsNullException, IsNotManagerOfException;

    /**
     * @param data
     * @throws MethodParameterIsNullException
     */
    public void addMediaData(IMediaData data)
            throws MethodParameterIsNullException;

    /**
     * @return list
     */
    public List<String> getListOfUids();

    /**
     * @param data
     * 
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     */
    public void removeMediaData(IMediaData data)
            throws MethodParameterIsNullException;

    /**
     * @param uid
     * 
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * @throws MethodParameterIsEmptyStringException Empty string '' method
     *         parameters are forbidden
     */
    public void removeMediaData(String uid)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException;

    /**
     * @param data
     * @return data
     * 
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * @throws IsNotManagerOfException
     */
    public IMediaData copyMediaData(IMediaData data)
            throws MethodParameterIsNullException, IsNotManagerOfException;

    /**
     * @param uid
     * @return data
     * 
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * @throws MethodParameterIsEmptyStringException Empty string '' method
     *         parameters are forbidden
     * @throws IsNotManagerOfException
     */
    public IMediaData copyMediaData(String uid)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException, IsNotManagerOfException;

    /**
     * @return list
     */
    public List<IMediaData> getListOfMediaData();

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
    public void setDefaultPCMFormat(IPCMFormatInfo newDefault)
            throws MethodParameterIsNullException, InvalidDataFormatException;

    /**
     * @param numberOfChannels
     * @throws MethodParameterIsOutOfBoundsException
     */
    public void setDefaultNumberOfChannels(short numberOfChannels)
            throws MethodParameterIsOutOfBoundsException;

    /**
     * @param sampleRate
     * @throws MethodParameterIsOutOfBoundsException
     */
    public void setDefaultSampleRate(int sampleRate)
            throws MethodParameterIsOutOfBoundsException;

    /**
     * @param bitDepth
     * @throws MethodParameterIsOutOfBoundsException
     */
    public void setDefaultBitDepth(short bitDepth)
            throws MethodParameterIsOutOfBoundsException;

    /**
     * @return format info
     */
    public IPCMFormatInfo getDefaultPCMFormat();

    /**
     * @param data
     * @param uid
     * @throws MethodParameterIsEmptyStringException
     * @throws MethodParameterIsNullException
     */
    public void setDataMediaDataUid(IMediaData data, String uid)
            throws MethodParameterIsEmptyStringException,
            MethodParameterIsNullException;
}
