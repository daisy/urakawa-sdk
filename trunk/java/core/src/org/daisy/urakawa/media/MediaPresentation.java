package org.daisy.urakawa.media;

import java.util.List;

import org.daisy.urakawa.core.TreeNode;
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
 * @depend - Aggregation 0..n Media 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface MediaPresentation {
	/**
	 * Convenience method to get the list of Media used by the given TreeNode,
	 * that is to say the Media objects that are associated to a TreeNode via a
	 * ChannelsProperty.
	 * 
	 * @param node
	 *            cannot be null
	 * @return a non-null list, which can be empty.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public List<Media> getListOfMediaUsedByTreeNode(TreeNode node)
			throws MethodParameterIsNullException;

	/**
	 * Gets the list of all used Media objects in the Presentation.
	 * 
	 * @see #getListOfMediaUsedByTreeNode(TreeNode)
	 * @return a non-null list, which can be empty.
	 */
	public List<Media> getListOfUsedMedia();
}
