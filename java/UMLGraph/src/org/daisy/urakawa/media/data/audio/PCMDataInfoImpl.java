package org.daisy.urakawa.media.data.audio;

import java.io.InputStream;

import org.daisy.urakawa.media.timing.TimeDelta;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class PCMDataInfoImpl extends PCMFormatInfoImpl implements PCMDataInfo {
	public int getDataLength() {
		return 0;
	}

	public TimeDelta getDuration() {
		return null;
	}

	public void parseRiffWaveHeader(InputStream input) {
	}

	public void setDataLength(int newValue) {
	}

	public void writeRiffWaveHeader(InputStream output) {
	}
}
