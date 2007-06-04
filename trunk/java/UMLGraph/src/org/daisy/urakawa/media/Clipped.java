package org.daisy.urakawa.media;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;

/**
 * Media that is clipped at the beginning and at the end. It is only a virtual
 * clipping, similar to what W3C's SMIL clipBegin / clipEnd do.
 * 
 * @depend - "Composition\n(clipBegin/clipEnd)" 2 org.daisy.urakawa.media.timing.Time
 * @depend - - - org.daisy.urakawa.media.timing.TimeDelta
 * @todo verify / add comments and exceptions
 */
public interface Clipped extends Continuous {
	/**
	 * Sets the clipBegin, a time offset from the beginning of the media stream.
	 * 
	 * @param newClipBegin
	 *            cannot be null, must be within bounds [0..{@link Clipped#getClipEnd()}]
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws TimeOffsetIsOutOfBoundsException
	 *             if newClipBegin is not an authorized value
	 * @tagvalue Exceptions "MethodParameterIsNull-TimeOffsetIsOutOfBounds"
	 * @see Clipped#getClipEnd()
	 */
	public void setClipBegin(Time newClipBegin)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException;

	/**
	 * Sets the clipEnd, a time offset from the beginning of the media stream.
	 * 
	 * @param newClipEnd
	 *            cannot be null, must be within bounds [{@link Clipped#getClipBegin()}..{@link Continuous#getDuration()}]
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws TimeOffsetIsOutOfBoundsException
	 *             if newClipEnd is not an authorized value
	 * @tagvalue Exceptions "MethodParameterIsNull-TimeOffsetIsOutOfBounds"
	 * @see Clipped#getClipBegin()
	 */
	public void setClipEnd(Time newClipEnd)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException;

	/**
	 * Gets the clipBegin. The default value is no clipping: 0.
	 * 
	 * @return a time value in [0..{@link Clipped#getClipEnd()}]
	 * @see Clipped#setClipBegin(Time)
	 */
	public Time getClipBegin();

	/**
	 * Gets the clipEnd. The default value is no clipping: Time.MAX (or
	 * infinite).
	 * 
	 * @return a time value in [{@link Clipped#getClipBegin()}..{@link Continuous#getDuration()}]
	 * @see Clipped#setClipEnd(Time)
	 */
	public Time getClipEnd();
}
