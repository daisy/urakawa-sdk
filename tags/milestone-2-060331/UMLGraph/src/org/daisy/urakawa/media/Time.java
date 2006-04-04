package org.daisy.urakawa.media;

/**
 * Abstract Time offset (could be in milliseconds, SMPTE, etc.).
 * This really is an interface "lollypop" that should be extended.
 * Typically, methods like getTimeMilliseconds(), getTimeSMPTE(), etc. should be available to the end-user of the API.
 * Can be a negative/0/positive offset relative to the local timebase in the current context.
 */
public interface Time {
    /**
     * a helper method to help determine {@link org.daisy.urakawa.exceptions.TimeOffsetIsNegativeException}
     *
     * @return true if the associated time value is a negative offset (<0 "less than zero")
     */
    public boolean isNegativeTimeOffset();
}