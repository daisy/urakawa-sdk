package org.daisy.urakawa.media;

import org.daisy.urakawa.media.timing.TimeDelta;

/**
 * A media implementing this interface as a length in the time space, and its
 * {@link Media#isContinuous()} should return true (its inverse method
 * {@link Media#isDiscrete()} should return false). Typically, images are a
 * discrete media, whereas audio clips are continuous.
 * 
 * @see <a
 *      href="http://www.w3.org/TR/SMIL/extended-media-object.html#media-Definitions">SMIL
 *      Definitions</a>
 * @see <a
 *      href="http://www.w3.org/TR/SMIL/smil-timing.html#Timing-DiscreteContinuousMedia">SMIL
 *      Definitions</a>
 * @see Media#isContinuous()
 * @see Media#isDiscrete()
 * @depend - Composition - TimeDelta
 * @todo verify / add comments and exceptions
 */
public interface Continuous {
	/**
	 * The duration of the media object.
	 * 
	 * @return the duration.
	 */
	TimeDelta getDuration();
}
