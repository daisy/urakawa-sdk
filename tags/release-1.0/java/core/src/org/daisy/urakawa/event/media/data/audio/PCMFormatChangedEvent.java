package org.daisy.urakawa.event.media.data.audio;

import org.daisy.urakawa.media.data.audio.AudioMediaData;
import org.daisy.urakawa.media.data.audio.PCMFormatInfo;

/**
 * 
 *
 */
public class PCMFormatChangedEvent extends AudioMediaDataEvent {
	/**
	 * @param source
	 * @param newFormat
	 * @param prevFormat
	 */
	public PCMFormatChangedEvent(AudioMediaData source,
			PCMFormatInfo newFormat, PCMFormatInfo prevFormat) {
		super(source);
		mNewPCMFormat = newFormat;
		mPreviousPCMFormat = prevFormat;
	}

	private PCMFormatInfo mNewPCMFormat;
	private PCMFormatInfo mPreviousPCMFormat;

	/**
	 * @return format
	 */
	public PCMFormatInfo getNewPCMFormat() {
		return mNewPCMFormat;
	}

	/**
	 * @return format
	 */
	public PCMFormatInfo getPreviousPCMFormat() {
		return mPreviousPCMFormat;
	}
}
