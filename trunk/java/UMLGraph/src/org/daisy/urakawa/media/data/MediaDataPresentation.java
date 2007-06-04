package org.daisy.urakawa.media.data;

import java.util.List;

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
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface MediaDataPresentation extends WithMediaDataFactory,
		WithMediaDataManager, WithDataProviderManager {
	/**
	 * Convenience method to get the full list of MediaData objects used in the
	 * presentation
	 * 
	 * @return a non-null list, which can be empty.
	 */
	public List<MediaData> getListOfUsedMediaData();
}
