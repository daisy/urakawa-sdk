package org.daisy.urakawa.media;

import org.daisy.urakawa.exceptions.MethodParameterIsNull;
import org.daisy.urakawa.exceptions.TimeOffsetIsNegative;

/**
 * Media asset that is clipped at the begining and at the end.
 * {@link Media#isContinuous()} should return true.
 */
public interface ClippedMedia extends ExternalMedia {
    /**
     * @return the duration of the ClippedMedia (time between {@link #getClipEnd()} and {@link #getClipBegin()} markers). value in ms is > 0
     */
    public TimeDelta getDuration();

    /**
     * @return the clip-begin time >= 0 (relative to the begin of the media asset)
     */
    public Time getClipBegin();

    /**
     * @return the clip-end time >= 0 (relative to the end of the media asset)
     */
    public Time getClipEnd();

    /**
     * Sets the clip-begin time
     *
     * @param newClipBegin cannot be null, cannot correspond to a negative offset value
     * @see #getClipBegin()
     * @tagvalue Exceptions MethodParameterIsNull,TimeOffsetIsNegative
     */
    public void setClipBegin(Time newClipBegin) throws MethodParameterIsNull, TimeOffsetIsNegative;

    /**
     * Sets the clip-end time
     *
     * @param newClipEnd cannot be null, cannot correspond to a negative offset value
     * @see #getClipEnd()
     * @tagvalue Exceptions MethodParameterIsNull,TimeOffsetIsNegative
     */
    public void setClipEnd(Time newClipEnd) throws MethodParameterIsNull, TimeOffsetIsNegative;

    /**
     * Splits the ClippedMedia at the given split point relative to the clip-begin.
     * After execution the instance represents the ClippedMedia before the split timepoint.
     *
     * @param splitPoint cannot be null, cannot correspond to a negative offset value, Must correspond to a value < {@link #getDuration()} otherwise no split happens 
     * @return the ClippedMedia after the split timepoint, or null if sliptPoint is an illegal value (> {@link #getDuration()}).
     * @tagvalue Exceptions MethodParameterIsNull,TimeOffsetIsNegative
     */
    public ClippedMedia split(Time splitPoint) throws MethodParameterIsNull, TimeOffsetIsNegative;
}
