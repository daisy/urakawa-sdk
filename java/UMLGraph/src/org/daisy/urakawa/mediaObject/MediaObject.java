package org.daisy.urakawa.mediaObject;

/**
 * The root of the type hierarchy for Media Objects.
 */
public interface MediaObject {
    /**
     * {@link #isContinuous()} = !{@link #isDiscrete()}
     *
     * @return true if this MediaObject is continuous, false if discrete.
     * @see <a href="http://www.w3.org/TR/SMIL/extended-media-object.html#media-Definitions">SMIL Definitions</a>
     * @see <a href="http://www.w3.org/TR/SMIL/smil-timing.html#Timing-DiscreteContinuousMedia">SMIL Definitions</a>
     */
    boolean isContinuous();

    /**
     * Convenience method inverse of {@link #isContinuous()}
     *
     * @return true if this MediaObject is discrete, false if continuous.
     * @see #isContinuous()
     */
    boolean isDiscrete();

    /**
     * @return the type of the MediaObject. Correspond to the AudioObject, TextObject, etc. classes.
     */
    MediaObjectType getType();
}