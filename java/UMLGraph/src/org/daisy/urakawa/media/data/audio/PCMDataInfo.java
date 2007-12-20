package org.daisy.urakawa.media.data.audio;

import org.daisy.urakawa.media.data.utilities.Stream;
import org.daisy.urakawa.media.timing.TimeDelta;

/**
 * An extension of PCMFormatInfo with support for RIFF WAV header, and actual
 * data length.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface PCMDataInfo extends PCMFormatInfo {
	public int getDataLength();

	public void setDataLength(int newValue);

	public TimeDelta getDuration();

	public PCMDataInfo parseRiffWaveHeader(Stream riffWaveStream);

	public boolean compareStreamData(Stream thisData, Stream otherdata,
			int length);
}
