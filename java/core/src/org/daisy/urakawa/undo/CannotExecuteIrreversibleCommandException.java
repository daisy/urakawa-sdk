package org.daisy.urakawa.undo;

import org.daisy.urakawa.exception.CheckedException;

/**
 * <p>
 * This exception is raised when trying to execute an irreversible command while
 * a transaction is currently active. fails.
 * </p>
 */
public class CannotExecuteIrreversibleCommandException extends CheckedException
{
    /**
	 * 
	 */
    private static final long serialVersionUID = 5668628925103303238L;
}
