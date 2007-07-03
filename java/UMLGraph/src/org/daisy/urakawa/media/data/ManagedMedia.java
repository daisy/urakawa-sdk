package org.daisy.urakawa.media.data;


/**
 * An media for which the data source is managed data
 * {@link MediaData}.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 1 org.daisy.urakawa.media.data.audio.MediaData
 * @depend - Clone - org.daisy.urakawa.media.data.audio.ManagedMedia
 */
public interface ManagedMedia extends WithMediaData {
}
