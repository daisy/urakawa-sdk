package org.daisy.urakawa.media;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.TimeOffsetIsOutOfBoundsException;
import org.daisy.urakawa.media.timing.Time;

/**
 * Media that is clipped at the begining and at the end.
 * {@link Media#isContinuous()} should return true.
 *
 * @depend - "Composition\n(clipBegin/clipEnd)" 2 Time
 * @depend - - - TimeDelta
 */
public interface Clipped extends Continuous {
    /**
     * Sets the clip-begin time (from the begin of the underlying media clip/file (0ms)).
     * Implementations should ensure that default values for clipBegin is 0 for newly created instances of Clipped medias.
     *
     * @param newClipBegin cannot be null, must be within bounds [0..getClipEnd()]
     * @tagvalue Exceptions "MethodParameterIsNull, TimeOffsetIsOutOfBounds"
     * @see #getClipBegin()
     */
    public void setClipBegin(Time newClipBegin) throws MethodParameterIsNullException, TimeOffsetIsOutOfBoundsException;

    /**
     * Sets the clip-end time (from the begin of the underlying media clip/file (0ms))
     * Implementations should ensure that default values for clipEnd is infinite (or more likely Time.MAX) for newly created instances of Clipped medias.
     *
     * @param newClipEnd cannot be null, must be within bounds [getClipBegin()..n]
     * @tagvalue Exceptions "MethodParameterIsNull, TimeOffsetIsOutOfBounds"
     * @see #getClipEnd()
     */
    public void setClipEnd(Time newClipEnd) throws MethodParameterIsNullException, TimeOffsetIsOutOfBoundsException;

    /**
     * @return the clip-begin time in [0..getClipEnd()]
     * @see #setClipBegin(Time)
     */
    public Time getClipBegin();

    /**
     * @return the clip-end time in [getClipBegin()..n]
     * @see #setClipEnd(Time)
     */
    public Time getClipEnd();
}
