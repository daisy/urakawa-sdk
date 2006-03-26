package org.daisy.urakawa.mediaObject;

/**
 * Time length expressed in milliseconds.
 */
public interface TimeDelta {
    /**
     * @return the time duration in milliseconds, cannot be negative (return type should be "unsigned long").
     */
    public long getTimeDelta();
}