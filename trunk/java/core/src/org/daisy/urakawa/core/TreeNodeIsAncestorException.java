package org.daisy.urakawa.core;

import org.daisy.urakawa.exception.CheckedException;

/**
 * <p>
 * This exception is raised when a {@link org.daisy.urakawa.core.ITreeNode} is
 * required to _not_ be an ancestor of another node, but is.
 * </p>
 */
public class TreeNodeIsAncestorException extends CheckedException
{
    /**
	 * 
	 */
    private static final long serialVersionUID = 6051246993466625479L;
}
