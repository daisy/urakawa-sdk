package org.daisy.urakawa.media;

import org.daisy.urakawa.IdentifiableInterface;
import org.daisy.urakawa.xuk.XukAble;

/**
 * The root of the type hierarchy for Media Objects.
 *
 * @depend - - - MediaType
 */
public interface Media extends XukAble, IdentifiableInterface {
    /**
     * {@link #isContinuous()} = !{@link #isDiscrete()}
     *
     * @return true if this Media is continuous, false if discrete.
     * @see <a href="http://www.w3.org/TR/SMIL/extended-media-object.html#media-Definitions">SMIL Definitions</a>
     * @see <a href="http://www.w3.org/TR/SMIL/smil-timing.html#Timing-DiscreteContinuousMedia">SMIL Definitions</a>
     */
    boolean isContinuous();

    /**
     * Convenience method inverse of {@link #isContinuous()}
     *
     * @return true if this Media is discrete, false if continuous.
     * @see #isContinuous()
     */
    boolean isDiscrete();

    /**
     * @return the type of the Media. Correspond to the AudioMedia, TextMedia, etc. classes.
     */
    MediaType getType();

    /**
     * @return a distinct copy of the Media object.
     */
    Media copy();
}