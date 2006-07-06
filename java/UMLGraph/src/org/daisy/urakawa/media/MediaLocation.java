package org.daisy.urakawa.media;

/**
 * Abstract location. e.g.: URI
 */
public interface MediaLocation {
    /**
     * @return a distinct copy of the MediaLocation object.
     */
    MediaLocation copy();
}
