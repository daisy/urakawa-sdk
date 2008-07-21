package org.daisy.urakawa.navigator;

import org.daisy.urakawa.exception.CheckedException;

/**
 * <p>
 * This exception is raised when trying to use a
 * {@link org.daisy.urakawa.core.ITreeNode} in the context of a
 * {@link org.daisy.urakawa.navigator.INavigator} that does not contain the
 * given node, when it should do.
 * </p>
 */
public class TreeNodeNotIncludedByNavigatorException extends CheckedException
{
    /**
	 * 
	 */
    private static final long serialVersionUID = 1917051686107595248L;
}
