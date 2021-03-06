package org.daisy.urakawa.media;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.IValueEquatable;
import org.daisy.urakawa.IWithLanguage;
import org.daisy.urakawa.IWithPresentation;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.events.DataModelChangedEvent;
import org.daisy.urakawa.events.IEventHandler;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * This is the top-most generic interface for a media object.
 * 
 * @depend - Clone - org.daisy.urakawa.media.IMedia
 */
public interface IMedia extends IWithPresentation, IWithLanguage, IXukAble,
        IValueEquatable<IMedia>, IEventHandler<DataModelChangedEvent>
{
    /**
     * The "continuous" vs "discrete" media type. The
     * {@link IMedia#isContinuous()} method always returns the boolean opposite
     * of {@link IMedia#isDiscrete()}
     * 
     * @return true if this IMedia is continuous, false if it is discrete.
     * @see IMedia#isDiscrete()
     * @see "http://www.w3.org/TR/SMIL/extended-media-object.html#media-Definitions"
     * @see "http://www.w3.org/TR/SMIL/smil-timing.html#Timing-DiscreteContinuousMedia"
     */
    public boolean isContinuous();

    /**
     * The "continuous" vs "discrete" media type. The
     * {@link IMedia#isContinuous()} method always returns the boolean opposite
     * of {@link IMedia#isDiscrete()}
     * 
     * @return true if this IMedia is discrete, false if continuous.
     * @see IMedia#isContinuous()
     * @see "http://www.w3.org/TR/SMIL/extended-media-object.html#media-Definitions"
     * @see "http://www.w3.org/TR/SMIL/smil-timing.html#Timing-DiscreteContinuousMedia"
     */
    public boolean isDiscrete();

    /**
     * Tests whether this media is a sequence of other medias.
     * 
     * @return true if this media object is actually a sequence of other medias.
     * @see ISequenceMedia
     */
    public boolean isSequence();

    /**
     * <p>
     * Cloning method
     * </p>
     * 
     * @return a copy.
     */
    public IMedia copy();

    /**
     * @param destPres
     * @return can return null in case of failure.
     * @throws FactoryCannotCreateTypeException
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     */
    public IMedia export(Presentation destPres)
            throws FactoryCannotCreateTypeException,
            MethodParameterIsNullException;
}