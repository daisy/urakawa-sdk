package org.daisy.urakawa.media;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.TimeOffsetIsOutOfBoundsException;

/**
 * Media asset that is clipped at the begining and at the end.
 * {@link Media#isContinuous()} should return true.
 *
 * @depend - "Composition\n(clipBegin/clipEnd)" 2 Time
 * @depend - - - TimeDelta
 */
public interface ClippedMedia extends ExternalMedia {
    /**
     * Sets the clip-begin time (from the begin of the underlying media asset (0))
     *
     * @param newClipBegin cannot be null, must be within bounds [0..getIntrinsicDuration()-getClipEnd()]
     * @tagvalue Exceptions "MethodParameterIsNull, TimeOffsetIsOutOfBounds"
     * @see #getClipBegin()
     */
    public void setClipBegin(Time newClipBegin) throws MethodParameterIsNullException, TimeOffsetIsOutOfBoundsException;

    /**
     * Sets the clip-end time (from the end of the underlying media asset (getIntrinsicDuration()))
     *
     * @param newClipEnd cannot be null, must be within bounds [0..getIntrinsicDuration()-getClipBegin()]
     * @tagvalue Exceptions "MethodParameterIsNull, TimeOffsetIsOutOfBounds"
     * @see #getClipEnd()
     */
    public void setClipEnd(Time newClipEnd) throws MethodParameterIsNullException, TimeOffsetIsOutOfBoundsException;

    /**
     * Splits the ClippedMedia at the given split point.
     * After execution this instance represents the ClippedMedia before ("on the left") the split timepoint.
     *
     * @param splitPoint cannot be null, must be within bounds [getClipBegin()..getIntrinsicDuration()-getClipEnd()]
     * @return the ClippedMedia after ("on the right") the split timepoint.
     * @tagvalue Exceptions "MethodParameterIsNull, TimeOffsetIsOutOfBounds"
     */
    public ClippedMedia split(Time splitPoint) throws MethodParameterIsNullException, TimeOffsetIsOutOfBoundsException;

    /**
     * @return the duration of the ClippedMedia (time between {@link #getClipBegin()} and {@link #getClipEnd()} markers).
     */
    public TimeDelta getClippedDuration();

    /**
     * @return the duration of the underlying media (intrisic duration).
     */
    public TimeDelta getIntrinsicDuration();

    /**
     * @return the clip-begin time. Default is a zero value.
     * @see #setClipBegin(Time)
     */
    public Time getClipBegin();

    /**
     * @return the clip-end time. Default is a zero value.
     * @see #setClipEnd(Time)
     */
    public Time getClipEnd();
}
