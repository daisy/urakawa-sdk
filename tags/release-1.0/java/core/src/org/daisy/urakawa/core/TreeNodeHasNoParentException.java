package org.daisy.urakawa.core;

import org.daisy.urakawa.exception.CheckedException;

/**
 * <p>
 * This exception is raised when a {@link org.daisy.urakawa.core.TreeNode} is
 * required to have a parent (aka "attached", or not tree root), but hasn't got
 * one.
 * </p>
 */
public class TreeNodeHasNoParentException extends CheckedException {
	/**
	 * 
	 */
	private static final long serialVersionUID = 2872260906355857543L;
}
