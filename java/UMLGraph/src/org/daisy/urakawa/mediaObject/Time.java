package org.daisy.urakawa.mediaObject;

/**
 * Time offset expressed in milliseconds.
 */
public interface Time {
    /**
     * @return the time in milliseconds, can be a negative/0/positive offset relative to the local timebase.
     */
    public long getTimeMilliseconds();
}