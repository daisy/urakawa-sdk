package org.daisy.urakawa.media.data;

import org.daisy.urakawa.media.MediaPresentation;

/**
 * @depend - Composition 1 MediaDataFactory
 * @depend - Composition 1 MediaDataManager
 * @depend - Composition 1 DataProviderManager
 * @todo verify / add comments and exceptions
 */
public interface MediaDataPresentation extends MediaPresentation,
		WithMediaDataFactory, WithMediaDataManager, WithDataProviderManager {
}
