package org.daisy.urakawa.undo;

import org.daisy.urakawa.exception.CheckedException;

/**
 * <p>
 * This exception is raised when trying to redo an action, but the operation
 * fails.
 * </p>
 */
public class CannotRedoException extends CheckedException {
	/**
	 * 
	 */
	private static final long serialVersionUID = 3431387950719820251L;
}
