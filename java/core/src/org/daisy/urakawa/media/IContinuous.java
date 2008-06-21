package org.daisy.urakawa.media;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.timing.ITime;
import org.daisy.urakawa.media.timing.ITimeDelta;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;

/**
 * A media implementing this interface as a length in the time space, and its
 * {@link IMedia#isContinuous()} should return true (its inverse method
 * {@link IMedia#isDiscrete()} should return false). Typically, images are a
 * discrete media, whereas audio clips are continuous.
 * 
 * @see <a
 *      href="http://www.w3.org/TR/SMIL/extended-media-object.html#media-Definitions">SMIL
 *      Definitions</a>
 * @see <a
 *      href="http://www.w3.org/TR/SMIL/smil-timing.html#Timing-DiscreteContinuousMedia">SMIL
 *      Definitions</a>
 * @see IMedia#isContinuous()
 * @see IMedia#isDiscrete()
 * @depend - Composition 1 org.daisy.urakawa.media.timing.ITimeDelta
 */
public interface IContinuous {
	/**
	 * Splits the continuous media at a given time point, leaving the instance
	 * with the part before the split point and creating a new media with the
	 * part after.
	 * 
	 * @param splitPoint
	 *            must be within ]0, getDuration()[
	 * @return right hand side of the split continuous media (the part after the
	 *         split point)
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws TimeOffsetIsOutOfBoundsException
	 *             if the given time point is not within ]0, getDuration()[
	 */
	IContinuous split(ITime splitPoint) throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException;

	/**
	 * The duration of the media object.
	 * 
	 * @return the duration.
	 */
	ITimeDelta getDuration();
}
