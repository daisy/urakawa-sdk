package org.daisy.urakawa.event.media.data.audio;

import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.media.data.audio.IAudioMediaData;

/**
 * 
 *
 */
public class AudioMediaDataEvent extends DataModelChangedEvent {
	/**
	 * @param source
	 */
	public AudioMediaDataEvent(IAudioMediaData source) {
		super(source);
		mSourceAudioMediaData = source;
	}

	private IAudioMediaData mSourceAudioMediaData;

	/**
	 * @return media data
	 */
	public IAudioMediaData getSourceAudioMediaData() {
		return mSourceAudioMediaData;
	}
}
