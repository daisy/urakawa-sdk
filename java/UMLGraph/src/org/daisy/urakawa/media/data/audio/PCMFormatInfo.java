package org.daisy.urakawa.media.data.audio;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.media.timing.TimeDelta;
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
	public short getNumberOfChannels();

	public void setNumberOfChannels(short newValue);

	public int getSampleRate();

	public void setSampleRate(int newValue);

	public short getBitDepth();

	public void setBitDepth(short newValue);

	public int getByteRate();

	public short getBlockAlign();

	public boolean isCompatibleWith(PCMFormatInfo pcmInfo);

	public TimeDelta getDuration(int dataLen);

	public int getDataLength(TimeDelta duration);

	public PCMFormatInfo copy();
}
