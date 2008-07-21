package org.daisy.urakawa.core;

import org.daisy.urakawa.exception.CheckedException;

/**
 * <p>
 * This exception is raised when a {@link org.daisy.urakawa.core.ITreeNode}
 * reference points to itself via another reference.
 * </p>
 */
public class TreeNodeIsSelfException extends CheckedException
{
    /**
	 * 
	 */
    private static final long serialVersionUID = 3121985079751574564L;
}
