package org.daisy.urakawa.media.data.audio;

import org.daisy.urakawa.media.AudioMedia;

/**
 * An audio media for which the data source is a managed asset
 * {@link AudioMediaData}.
 * 
 * @todo verify / add comments and exceptions
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Composition 1 org.daisy.urakawa.media.data.audio.AudioMediaData
 */
public interface ManagedAudioMedia extends WithAudioMediaData, AudioMedia {
}
