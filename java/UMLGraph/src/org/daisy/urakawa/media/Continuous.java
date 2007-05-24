package org.daisy.urakawa.media;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.TimeOffsetIsOutOfBoundsException;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;

public interface Continuous {
	TimeDelta getDuration();

	/**
	 * Splits the Clipped at the given split point. After execution this
	 * instance represents the Clipped before ("on the left") the split
	 * timepoint.
	 * 
	 * @param splitPoint
	 *            cannot be null, must be within bounds
	 *            [getClipBegin()..getClipEnd()]
	 * @return the Clipped after ("on the right") the split timepoint.
	 * @tagvalue Exceptions "MethodParameterIsNull, TimeOffsetIsOutOfBounds"
	 */
	public Continuous split(Time splitPoint)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException;

	// public Continuous merge(Continuous media) throws
	// MethodParameterIsNullException;

}
