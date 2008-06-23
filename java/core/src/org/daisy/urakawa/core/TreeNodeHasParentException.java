package org.daisy.urakawa.core;

import org.daisy.urakawa.exception.CheckedException;

/**
 * <p>
 * This exception is raised when a {@link org.daisy.urakawa.core.TreeNode} is
 * required to _not_ have a parent (aka "detached", or tree root), but has one.
 * </p>
 */
public class TreeNodeHasParentException extends CheckedException {
	/**
	 * 
	 */
	private static final long serialVersionUID = -2186227700168305653L;
}
