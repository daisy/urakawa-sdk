package org.daisy.urakawa.events.media.data.audio;

import org.daisy.urakawa.media.data.audio.IAudioMediaData;
import org.daisy.urakawa.media.timing.ITime;
import org.daisy.urakawa.media.timing.ITimeDelta;

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
	public AudioDataRemovedEvent(IAudioMediaData source, ITime fromPoint,
			ITimeDelta dur) {
		super(source);
		mRemovedFromPoint = fromPoint.copy();
		mDuration = dur.copy();
	}

	private ITime mRemovedFromPoint;
	private ITimeDelta mDuration;

	/**
	 * @return time
	 */
	public ITime getRemovedFromPoint() {
		return mRemovedFromPoint;
	}

	/**
	 * @return time
	 */
	public ITimeDelta getDuration() {
		return mDuration;
	}
}
