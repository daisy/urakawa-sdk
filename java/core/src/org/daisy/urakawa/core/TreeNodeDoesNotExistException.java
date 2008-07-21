package org.daisy.urakawa.core;

import org.daisy.urakawa.exception.CheckedException;

/**
 * <p>
 * This exception is raised when a {@link org.daisy.urakawa.core.ITreeNode} is
 * required to exist in the tree, but doesn't.
 * </p>
 */
public class TreeNodeDoesNotExistException extends CheckedException
{
    /**
	 * 
	 */
    private static final long serialVersionUID = 5738860246353449181L;
}
