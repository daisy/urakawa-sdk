package org.daisy.urakawa.media;

import java.net.URI;
import java.util.List;

/**
 * This interface represents a basic "media presentation" with:
 * <ul>
 * <li> a factory for creating media objects. </li>
 * <li> a URI pointing at the directory containing the media objects </li>
 * </ul>
 * This is convenience interface for the design only, in order to organize the
 * data model in smaller modules.
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 * @depend - Composition 1 MediaFactory
 * @todo verify / add comments and exceptions
 */
public interface MediaPresentation extends WithMediaFactory {
	URI getBaseUri();

	void setBaseUri(URI newBase);

	List<Media> getListOfUsedMedia();
}
