package org.daisy.urakawa.event.media.data.audio;

import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.media.data.audio.AudioMediaData;

/**
 * 
 *
 */
public class AudioMediaDataEvent extends DataModelChangedEvent {
	/**
	 * @param source
	 */
	public AudioMediaDataEvent(AudioMediaData source) {
		super(source);
		mSourceAudioMediaData = source;
	}

	private AudioMediaData mSourceAudioMediaData;

	/**
	 * @return media data
	 */
	public AudioMediaData getSourceAudioMediaData() {
		return mSourceAudioMediaData;
	}
}
