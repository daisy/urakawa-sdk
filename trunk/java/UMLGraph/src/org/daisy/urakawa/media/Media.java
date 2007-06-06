package org.daisy.urakawa.media;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.xuk.XukAble;

/**
 * This is the top-most generic interface for a media object. For example, an
 * {@link VideoMedia} type derives this interface by composition of other
 * interfaces of the data model, like {@link Continuous} and {@link Sized}. The
 * actual type (as per multimedia semantics) of the media object is given by the
 * {@link Media#getMediaType()} method, in order to separate the notion of media
 * type from the object-oriented concepts of interface and class.
 * 
 * @depend - Clone - org.daisy.urakawa.media.Media
 * @depend - Aggregation - org.daisy.urakawa.media.MediaType
 * @depend - Aggregation - org.daisy.urakawa.media.MediaFactory
 * @todo verify / add comments and exceptions
 */
public interface Media extends WithMediaFactory, XukAble, ValueEquatable<Media> {
	/**
	 * Gets the type of the media object. No matching setter method, because
	 * this getter should be a "hard-coded" built-in value for a Media subclass.
	 * 
	 * @return the type of the Media. Cannot be null.
	 */
	MediaType getMediaType();

	/**
	 * The "continuous" vs "discrete" media type. The
	 * {@link Media#isContinuous()} method always returns the boolean opposite
	 * of {@link Media#isDiscrete()}
	 * 
	 * @return true if this Media is continuous, false if it is discrete.
	 * @see Media#isDiscrete()
	 * @see <a
	 *      href="http://www.w3.org/TR/SMIL/extended-media-object.html#media-Definitions">SMIL
	 *      Definitions</a>
	 * @see <a
	 *      href="http://www.w3.org/TR/SMIL/smil-timing.html#Timing-DiscreteContinuousMedia">SMIL
	 *      Definitions</a>
	 */
	boolean isContinuous();

	/**
	 * The "continuous" vs "discrete" media type. The
	 * {@link Media#isContinuous()} method always returns the boolean opposite
	 * of {@link Media#isDiscrete()}
	 * 
	 * @return true if this Media is discrete, false if continuous.
	 * @see Media#isContinuous()
	 * @see <a
	 *      href="http://www.w3.org/TR/SMIL/extended-media-object.html#media-Definitions">SMIL
	 *      Definitions</a>
	 * @see <a
	 *      href="http://www.w3.org/TR/SMIL/smil-timing.html#Timing-DiscreteContinuousMedia">SMIL
	 *      Definitions</a>
	 */
	boolean isDiscrete();

	/**
	 * Tests whether this media is a sequence of other medias.
	 * 
	 * @return true if this media object is actually a sequence of other medias.
	 * @see SequenceMedia
	 */
	boolean isSequence();

	/**
	 * Creates a copy of this media object, using the same factory.
	 * 
	 * @return a copy of this Media object. if the factory was not set for this
	 *         media obejct (missing initialization ?)
	 * @tagvalue Exceptions "FactoryIsMissing"
	 */
	Media copy();
}