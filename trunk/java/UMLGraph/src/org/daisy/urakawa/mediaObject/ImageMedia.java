package org.daisy.urakawa.mediaObject;

/**
 * A class for images.
 * {@link Media#isContinuous()} should return false for static images like that.
 * {@link Media#getType()} should return MediaType.IMAGE
 */
public interface ImageMedia extends ExternalMedia, Surface2D {
}