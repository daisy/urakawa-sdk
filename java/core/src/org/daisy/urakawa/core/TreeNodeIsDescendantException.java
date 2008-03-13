package org.daisy.urakawa.core;

import org.daisy.urakawa.exception.CheckedException;

/**
 * <p>
 * This exception is raised when a {@link org.daisy.urakawa.core.TreeNode} is
 * required to _not_ be a descendant of another node, but is.
 * </p>
 */
public class TreeNodeIsDescendantException extends CheckedException {
	/**
	 * 
	 */
	private static final long serialVersionUID = 4402176535570555702L;
}
