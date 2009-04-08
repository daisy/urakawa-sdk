package org.daisy.urakawa.event.media.data.audio;

import org.daisy.urakawa.media.data.audio.AudioMediaData;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;

/**
 *
 *
 */
public class AudioDataRemovedEvent extends AudioMediaDataEvent {
	/**
	 * @param source
	 * @param fromPoint
	 * @param dur
	 */
	public AudioDataRemovedEvent(AudioMediaData source, Time fromPoint,
			TimeDelta dur) {
		super(source);
		mRemovedFromPoint = fromPoint.copy();
		mDuration = dur.copy();
	}

	private Time mRemovedFromPoint;
	private TimeDelta mDuration;

	/**
	 * @return time
	 */
	public Time getRemovedFromPoint() {
		return mRemovedFromPoint;
	}

	/**
	 * @return time
	 */
	public TimeDelta getDuration() {
		return mDuration;
	}
}
