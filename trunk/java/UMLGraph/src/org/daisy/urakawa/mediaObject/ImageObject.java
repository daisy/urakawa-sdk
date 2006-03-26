package org.daisy.urakawa.mediaObject;

/**
 * A class for images.
 * {@link MediaObject#isContinuous()} should return false for static images like that.
 * {@link MediaObject#getType()} should return MediaObjectType.IMAGE
 */
public interface ImageObject extends ExternalMediaAsset, Surface2D {
}