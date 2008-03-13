package org.daisy.urakawa.event.media.data.audio;

import org.daisy.urakawa.media.data.audio.AudioMediaData;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;

/**
 * 
 *
 */
public class AudioDataInsertedEvent extends AudioMediaDataEvent {
	/**
	 * @param source
	 * @param insPoint
	 * @param dur
	 */
	public AudioDataInsertedEvent(AudioMediaData source, Time insPoint,
			TimeDelta dur) {
		super(source);
		mInsertPoint = insPoint.copy();
		mDuration = dur.copy();
	}

	private Time mInsertPoint;
	private TimeDelta mDuration;

	/**
	 * @return time
	 */
	public Time getInsertPoint() {
		return mInsertPoint;
	}

	/**
	 * @return time
	 */
	public TimeDelta getDuration() {
		return mDuration;
	}
}
