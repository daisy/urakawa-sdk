package org.daisy.urakawa.media.data.audio;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.timing.TimeDelta;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * An extension of PCMFormatInfo with support for RIFF WAV header, and actual
 * data length.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface PCMFormatInfo extends ValueEquatable<PCMFormatInfo>, XukAble {
	/**
	 * @return the number of channels
	 */
	public short getNumberOfChannels();

	/**
	 * sets the number of channels
	 * 
	 * @param newValue
	 * @throws MethodParameterIsOutOfBoundsException
	 */
	public void setNumberOfChannels(short newValue)
			throws MethodParameterIsOutOfBoundsException;

	/**
	 * @return the sample rate
	 */
	public int getSampleRate();

	/**
	 * sets the sample rate
	 * 
	 * @param newValue
	 * @throws MethodParameterIsOutOfBoundsException
	 */
	public void setSampleRate(int newValue)
			throws MethodParameterIsOutOfBoundsException;

	/**
	 * @return the bit depth
	 */
	public short getBitDepth();

	/**
	 * sets the bit depth
	 * 
	 * @param newValue
	 * @throws MethodParameterIsOutOfBoundsException
	 */
	public void setBitDepth(short newValue)
			throws MethodParameterIsOutOfBoundsException;

	/**
	 * @return the byte rate
	 */
	public int getByteRate();

	/**
	 * @return the block align
	 */
	public short getBlockAlign();

	/**
	 * tests whether this format is compatible with the given one
	 * 
	 * @param pcmInfo
	 * @return true or false
	 * @throws MethodParameterIsNullException
	 */
	public boolean isCompatibleWith(PCMFormatInfo pcmInfo)
			throws MethodParameterIsNullException;

	/**
	 * @param dataLen
	 * @return the duration
	 */
	public TimeDelta getDuration(int dataLen);

	/**
	 * @param duration
	 * @return the length of the data in bytes for a given duration
	 * @throws TimeOffsetIsOutOfBoundsException
	 * @throws MethodParameterIsNullException
	 */
	public int getDataLength(TimeDelta duration)
			throws TimeOffsetIsOutOfBoundsException,
			MethodParameterIsNullException;

	/**
	 * @return a copy of this
	 */
	public PCMFormatInfo copy();
}
