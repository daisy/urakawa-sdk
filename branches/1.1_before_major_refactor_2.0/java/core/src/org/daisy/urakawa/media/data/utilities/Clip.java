package org.daisy.urakawa.media.data.utilities;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;
import org.daisy.urakawa.media.timing.TimeImpl;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;

/**
 * Generic media clip
 */
public abstract class Clip {
	private Time mClipBegin = new TimeImpl();

	/**
	 * @return time
	 */
	public Time getClipBegin() {
		return mClipBegin.copy();
	}

	/**
	 * @param newClipBegin
	 * @throws MethodParameterIsNullException
	 * @throws TimeOffsetIsOutOfBoundsException 
	 */
	public void setClipBegin(Time newClipBegin)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		if (newClipBegin == null) {
			throw new MethodParameterIsNullException();
		}
		if (newClipBegin.isGreaterThan(getClipEnd())) {
			throw new TimeOffsetIsOutOfBoundsException();
		}
		mClipBegin = newClipBegin.copy();
	}

	private Time mClipEnd = null;

	/**
	 * @return time
	 */
	public Time getClipEnd() {
		if (mClipEnd == null) {
			try {
				return new TimeImpl().getZero()
						.addTimeDelta(getMediaDuration());
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		return mClipEnd.copy();
	}

	/**
	 * @param newClipEnd
	 * @throws TimeOffsetIsOutOfBoundsException
	 */
	public void setClipEnd(Time newClipEnd)
			throws TimeOffsetIsOutOfBoundsException {
		if (newClipEnd == null) {
			mClipEnd = null;
		} else {
			try {
				if (newClipEnd.isLessThan(getClipBegin())) {
					throw new TimeOffsetIsOutOfBoundsException();
				}
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
			mClipEnd = newClipEnd.copy();
		}
	}

	/**
	 * @return true or false
	 */
	public boolean isClipEndTiedToEOM() {
		return (mClipEnd == null);
	}

	/**
	 * @return time
	 */
	public TimeDelta getDuration() {
		try {
			return getClipEnd().getTimeDelta(getClipBegin());
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	/**
	 * @return time
	 */
	public abstract TimeDelta getMediaDuration();
}
