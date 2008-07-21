package org.daisy.urakawa.undo;

import org.daisy.urakawa.exception.CheckedException;

/**
 * <p>
 * This exception is raised when trying to run an operation that is not
 * permitted while an undo-redo transition is currently active (started and not
 * finished).
 * </p>
 */
public class UndoRedoTransactionIsNotEndedException extends CheckedException
{
    /**
	 * 
	 */
    private static final long serialVersionUID = 2238643020587506157L;
}
