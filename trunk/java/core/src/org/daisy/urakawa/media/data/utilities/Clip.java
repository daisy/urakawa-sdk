package org.daisy.urakawa.media.data.utilities;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.timing.ITime;
import org.daisy.urakawa.media.timing.ITimeDelta;
import org.daisy.urakawa.media.timing.TimeImpl;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;

/**
 * Generic media clip
 */
public abstract class Clip {
	private ITime mClipBegin = new TimeImpl();

	/**
	 * @return time
	 */
	public ITime getClipBegin() {
		return mClipBegin.copy();
	}

	/**
	 * @param newClipBegin
	 * @throws MethodParameterIsNullException
	 * @throws TimeOffsetIsOutOfBoundsException 
	 */
	public void setClipBegin(ITime newClipBegin)
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

	private ITime mClipEnd = null;

	/**
	 * @return time
	 */
	public ITime getClipEnd() {
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
	public void setClipEnd(ITime newClipEnd)
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
	public ITimeDelta getDuration() {
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
	public abstract ITimeDelta getMediaDuration();
}
