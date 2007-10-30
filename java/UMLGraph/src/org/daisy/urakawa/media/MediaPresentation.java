package org.daisy.urakawa.media;

import java.net.URI;
import java.util.List;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

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
 */
public interface MediaPresentation extends WithMediaFactory {
	public URI getRootUri();

	/**
	 * @param newBase
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public void setRootUri(URI newBase) throws MethodParameterIsNullException;

	/**
	 * Convenience method to get the full list of Media objects used in the
	 * presentation
	 * 
	 * @return a non-null list, which can be empty.
	 * @stereotype Convenience
	 */
	public List<Media> getListOfUsedMedia();
}
