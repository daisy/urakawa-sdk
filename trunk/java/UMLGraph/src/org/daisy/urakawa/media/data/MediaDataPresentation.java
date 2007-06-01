package org.daisy.urakawa.media.data;

import org.daisy.urakawa.media.MediaPresentation;

/**
 * This interface represents a basic "media presentation" with:
 * <ul>
 * <li> a factory for creating media data. </li>
 * <li> a manager for the created media data. </li>
 * <li> a manager for the data providers associated to the media data objects.
 * </li>
 * </ul>
 * This is convenience interface for the design only, in order to organize the
 * data model in smaller modules.
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @depend - Composition 1 MediaDataFactory
 * @depend - Composition 1 MediaDataManager
 * @depend - Composition 1 DataProviderManager
 */
public interface MediaDataPresentation extends MediaPresentation,
		WithMediaDataFactory, WithMediaDataManager, WithDataProviderManager {
}
