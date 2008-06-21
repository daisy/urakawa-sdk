package org.daisy.urakawa.event.media.data.audio;

import org.daisy.urakawa.media.data.audio.IAudioMediaData;
import org.daisy.urakawa.media.data.audio.IPCMFormatInfo;

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
	public PCMFormatChangedEvent(IAudioMediaData source,
			IPCMFormatInfo newFormat, IPCMFormatInfo prevFormat) {
		super(source);
		mNewPCMFormat = newFormat;
		mPreviousPCMFormat = prevFormat;
	}

	private IPCMFormatInfo mNewPCMFormat;
	private IPCMFormatInfo mPreviousPCMFormat;

	/**
	 * @return format
	 */
	public IPCMFormatInfo getNewPCMFormat() {
		return mNewPCMFormat;
	}

	/**
	 * @return format
	 */
	public IPCMFormatInfo getPreviousPCMFormat() {
		return mPreviousPCMFormat;
	}
}
