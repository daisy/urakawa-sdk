package org.daisy.urakawa.media;

/**
 * 2D surface
 */
public interface ImageSize {
    /**
     * @return the width of the 2d surface in pixels (cannot be negative, the result type should be "unsigned int")
     */
    public int getWidth();

    /**
     * @return the height of the 2d surface in pixels (cannot be negative, the result type should be "unsigned int")
     */
    public int getHeight();
}
