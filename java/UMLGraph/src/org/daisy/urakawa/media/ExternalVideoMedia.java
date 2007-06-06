package org.daisy.urakawa.media;

/**
 * A concrete video media type, with an external "Located" resource which is
 * un-managed.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface ExternalVideoMedia extends Clipped, Located, VideoMedia {
}
