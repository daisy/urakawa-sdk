package org.daisy.urakawa.media;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.WithLanguage;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * This is the top-most generic interface for a media object. For example, an
 * {@link VideoMedia} type derives this interface by composition of other
 * interfaces of the data model, like {@link Continuous} and {@link Sized}.
 * 
 * @depend - Clone - org.daisy.urakawa.media.Media
 * @stereotype XukAble
 */
public interface Media extends WithPresentation, WithLanguage, XukAble,
		ValueEquatable<Media>, EventHandler<DataModelChangedEvent> {
	/**
	 * Convenience method to get the Media factory via the Presentation.
	 * 
	 * @return factory
	 */
	public MediaFactory getMediaFactory();

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
	public boolean isContinuous();

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
	public boolean isDiscrete();

	/**
	 * Tests whether this media is a sequence of other medias.
	 * 
	 * @return true if this media object is actually a sequence of other medias.
	 * @see SequenceMedia
	 */
	public boolean isSequence();

	/**
	 * <p>
	 * Cloning method
	 * </p>
	 * 
	 * @return a copy.
	 */
	public Media copy();

	/**
	 * @param destPres
	 * @return can return null in case of failure.
	 * @throws FactoryCannotCreateTypeException
	 * @tagvalue Exceptions "FactoryCannotCreateType-MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public Media export(Presentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException;
}