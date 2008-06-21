package org.daisy.urakawa.media.timing;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class TimeImpl implements ITime {
	private long mTime;

	/**
	 * 
	 */
	public TimeImpl() {
		mTime = 0;
	}

	/**
	 * @param value
	 */
	public TimeImpl(long value) {
		mTime = value;
	}

	/**
	 * @param val
	 * @throws MethodParameterIsNullException
	 * @throws MethodParameterIsEmptyStringException
	 * @throws TimeStringRepresentationIsInvalidException
	 */
	public TimeImpl(String val) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException,
			TimeStringRepresentationIsInvalidException {
		if (val == null) {
			throw new MethodParameterIsNullException();
		}
		if (val == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		setTime(new TimeImpl().parse(val).getTimeAsMilliseconds());
	}

	public ITime getZero() {
		return new TimeImpl();
	}

	public ITime getMaxValue() {
		return new TimeImpl(Long.MAX_VALUE);
	}

	public ITime getMinValue() {
		return new TimeImpl(Long.MIN_VALUE);
	}

	public boolean isLessThanOrEqualTo(ITime otherTime)
			throws MethodParameterIsNullException {
		if (otherTime == null) {
			throw new MethodParameterIsNullException();
		}
		return otherTime.isGreaterThanOrEqualTo(this);
	}

	public boolean isGreaterThanOrEqualTo(ITime otherTime)
			throws MethodParameterIsNullException {
		if (otherTime == null) {
			throw new MethodParameterIsNullException();
		}
		return !isLessThan(otherTime);
	}

	public boolean isLessThan(ITime otherTime)
			throws MethodParameterIsNullException {
		if (otherTime == null) {
			throw new MethodParameterIsNullException();
		}
		return otherTime.isGreaterThan(this);
	}

	public boolean isEqualTo(ITime otherTime)
			throws MethodParameterIsNullException {
		if (otherTime == null) {
			throw new MethodParameterIsNullException();
		}
		if (isGreaterThan(otherTime))
			return false;
		if (otherTime.isGreaterThan(this))
			return false;
		return true;
	}

	public boolean isGreaterThan(ITime otherTime)
			throws MethodParameterIsNullException {
		if (otherTime == null) {
			throw new MethodParameterIsNullException();
		}
		return (getTimeAsMilliseconds() > ((ITime) otherTime)
				.getTimeAsMilliseconds());
	}

	public ITime addTimeDelta(ITimeDelta other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		return new TimeImpl(getTimeAsMilliseconds()
				+ other.getTimeDeltaAsMilliseconds());
	}

	public ITime subtractTime(ITime other) throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		return new TimeImpl(getTimeAsMilliseconds()
				- other.getTimeAsMilliseconds());
	}

	public ITime subtractTimeDelta(ITimeDelta other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		return new TimeImpl(mTime - other.getTimeDeltaAsMilliseconds());
	}

	public void setTime(long newTime) {
		mTime = newTime;
	}

	public ITime addTime(ITime other) {
		return new TimeImpl(getTimeAsMilliseconds()
				+ other.getTimeAsMilliseconds());
	}

	public long getTimeAsMilliseconds() {
		return mTime;
	}

	public ITimeDelta getTimeDelta(ITime t) throws MethodParameterIsNullException {
		if (t == null) {
			throw new MethodParameterIsNullException();
		}
		long value = getTimeAsMilliseconds() - t.getTimeAsMilliseconds();
		if (value < 0) {
			value = -value;
		}
		return new TimeDeltaImpl(value);
	}

	public boolean isNegativeTimeOffset() {
		return (mTime < 0);
	}

	public ITime copy() {
		return new TimeImpl(mTime);
	}

	public ITime parse(String stringRepresentation)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException,
			TimeStringRepresentationIsInvalidException {
		if (stringRepresentation == null) {
			throw new MethodParameterIsNullException();
		}
		if (stringRepresentation == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		try {
			return new TimeImpl(Long.parseLong(stringRepresentation));
		} catch (Exception e) {
			throw new TimeStringRepresentationIsInvalidException();
		}
	}

	@Override
	public String toString() {
		return Long.toString(mTime);
	}
}
