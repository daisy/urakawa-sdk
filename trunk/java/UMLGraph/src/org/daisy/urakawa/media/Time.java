package org.daisy.urakawa.media;

/**
 * Abstract Time offset (could be in milliseconds, SMPTE, etc.).
 * Can be a negative/0/positive offset relative to the local timebase in the current context.
 */
public interface Time {
    /**
     * a helper method to help determine {@link org.daisy.urakawa.exceptions.TimeOffsetIsNegativeException} 
     * @return true if the associated time value is a negative offset (<0 "less than zero")
     */
    public boolean isNegativeTimeOffset();
}