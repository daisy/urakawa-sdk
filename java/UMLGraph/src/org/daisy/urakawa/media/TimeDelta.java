package org.daisy.urakawa.media;

/**
 * Abstract Time duration (could be in milliseconds, SMPTE, etc.).
 * This really is an interface "lollypop" that should be extended.
 * Typically, methods like getTimeMilliseconds(), getTimeSMPTE(), etc. should be available to the end-user of the API.
 * Can be a 0/positive value in the current local timebase. (cannot be negative)
 */
public interface TimeDelta {
}