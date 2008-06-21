package org.daisy.urakawa.media;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.timing.ITime;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;

/**
 * IMedia that is clipped at the beginning and at the end. It is only a virtual
 * clipping, similar to what W3C's SMIL clipBegin / clipEnd do.
 * 
 * @depend - "Composition\n(clipBegin,clipEnd)" 2 org.daisy.urakawa.media.timing.ITime
 * 
 */
public interface IClipped extends IContinuous {
	/**
	 * Sets the clipBegin, a time offset from the beginning of the media stream.
	 * 
	 * @param newClipBegin
	 *            cannot be null, must be within bounds [0..{@link IClipped#getClipEnd()}]
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws TimeOffsetIsOutOfBoundsException
	 *             if newClipBegin is not an authorized value
	 * @tagvalue Exceptions "MethodParameterIsNull-TimeOffsetIsOutOfBounds"
	 * @tagvalue Events "ClipChanged"
	 * @see IClipped#getClipEnd()
	 */
	public void setClipBegin(ITime newClipBegin)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException;

	/**
	 * Sets the clipEnd, a time offset from the beginning of the media stream.
	 * 
	 * @param newClipEnd
	 *            cannot be null, must be within bounds [{@link IClipped#getClipBegin()}..{@link IContinuous#getDuration()}]
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws TimeOffsetIsOutOfBoundsException
	 *             if newClipEnd is not an authorized value
	 * @tagvalue Exceptions "MethodParameterIsNull-TimeOffsetIsOutOfBounds"
	 * @tagvalue Events "ClipChanged"
	 * @see IClipped#getClipBegin()
	 */
	public void setClipEnd(ITime newClipEnd)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException;

	/**
	 * Gets the clipBegin. The default value is no clipping: 0.
	 * 
	 * @return a time value in [0..{@link IClipped#getClipEnd()}]
	 * @see IClipped#setClipBegin(ITime)
	 */
	public ITime getClipBegin();

	/**
	 * Gets the clipEnd. The default value is no clipping: ITime.MAX (or
	 * infinite).
	 * 
	 * @return a time value in [{@link IClipped#getClipBegin()}..{@link IContinuous#getDuration()}]
	 * @see IClipped#setClipEnd(ITime)
	 */
	public ITime getClipEnd();
}
