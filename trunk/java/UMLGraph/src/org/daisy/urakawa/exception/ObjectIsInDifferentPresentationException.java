package org.daisy.urakawa.exception;

import org.daisy.urakawa.core.TreeNode;

/**
 * <p>
 * This exception is raised when a {@link org.daisy.urakawa.core.TreeNode} is
 * required to be in the same {@link org.daisy.urakawa.Presentation} as another
 * node, but isn't.
 * </p>
 */
public class ObjectIsInDifferentPresentationException extends
		CheckedException {
	/**
	 * 
	 */
	private static final long serialVersionUID = 7503001642396792101L;
}
