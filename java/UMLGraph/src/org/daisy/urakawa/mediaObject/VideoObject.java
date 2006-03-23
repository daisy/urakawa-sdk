package org.daisy.urakawa.mediaObject;

import org.daisy.urakawa.exceptions.MethodParameterIsNull;

/**
 * 
 */
public class VideoObject implements ClippedMedia {
    /**
     * Splits the VideoObject at the given split point.
     * After execution the instance represents the video before the split point.
     *
     * @param splitPoint cannot be null
     * @return a VideoObject representing the video after the split point.
     */
    public VideoObject splitVideo(Time splitPoint) throws MethodParameterIsNull {
        return null;
    }

    public TimeDelta getDuration() {
        return null;
    }

    public Time getBegin() {
        return null;
    }

    public Time getEnd() {
        return null;
    }

    public void setBegin(Time newBegin) throws MethodParameterIsNull {
    }

    public void setEnd(Time newEnd) throws MethodParameterIsNull {
    }
}