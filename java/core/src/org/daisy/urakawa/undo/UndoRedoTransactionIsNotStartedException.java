package org.daisy.urakawa.undo;

import org.daisy.urakawa.exception.CheckedException;

/**
 * <p>
 * This exception is raised when trying to run an operation that is not
 * permitted while an undo-redo transition is not currently active (not
 * started).
 * </p>
 */
public class UndoRedoTransactionIsNotStartedException extends CheckedException {
	/**
	 * 
	 */
	private static final long serialVersionUID = -4845462825860674740L;
}
