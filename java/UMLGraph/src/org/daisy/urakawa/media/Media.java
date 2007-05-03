package org.daisy.urakawa.media;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.xuk.XukAble;
import org.daisy.urakawa.exceptions.FactoryIsMissingException;

/**
 * The root of the type hierarchy for {@link Media} Objects.
 * 
 * @depend - - - MediaType
 * @depend - Creator 1 MediaFactory
 */
public interface Media extends XukAble, ValueEquatable<Media>  {
    public MediaFactory getMediaFactory();

    /**
     * @stereotype initialize
     * @param fact
     */
    public void setMediaFactory(MediaFactory fact);

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
     * @return the type of the Media. Should be a type that can be casted to this Media interface. e.g. Correspond to the AudioMedia, TextMedia, etc. classes.
     */
    MediaType getMediaType();

    /**
     * @return a distinct copy of the Media object.
     */
    Media copy() throws FactoryIsMissingException;
}