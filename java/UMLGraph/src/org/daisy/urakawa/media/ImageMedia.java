package org.daisy.urakawa.media;

/**
 * A class for images.
 * {@link Media#isContinuous()} should return false for static images like that.
 * {@link Media#getMediaType()} should return MediaType.IMAGE
 */
public interface ImageMedia extends Media, Located, Sized {
}