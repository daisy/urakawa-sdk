package org.daisy.urakawa.media.timing;

/**
 * <p>
 * This exception is raised when trying to use a time offset that is negative
 * when it should not be.
 * </p>
 */
public class TimeOffsetIsNegativeException extends
		TimeOffsetIsOutOfBoundsException {
	/**
	 * 
	 */
	private static final long serialVersionUID = 1950794547262877014L;
}
