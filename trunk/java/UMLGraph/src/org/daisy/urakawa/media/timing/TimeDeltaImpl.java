package org.daisy.urakawa.media.timing;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class TimeDeltaImpl implements TimeDelta {
	public TimeDelta addTimeDelta(TimeDelta other)
			throws MethodParameterIsNullException {
		return null;
	}

	public double getTimeDeltaAsMillisecondFloat() {
		return 0;
	}

	public long getTimeDeltaAsMilliseconds() {
		return 0;
	}

	public void setTimeDelta(long timeDeltaAsMS) {
	}

	public void setTimeDelta(double timeDeltaAsMSF) {
	}
}
