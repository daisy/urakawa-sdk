package org.daisy.urakawa.command;

import org.daisy.urakawa.exception.CheckedException;

/**
 * <p>
 * This exception is raised when trying to execute a command and it fails.
 * </p>
 */
public class CommandCannotExecuteException extends CheckedException {

	/**
	 * 
	 */
	public CommandCannotExecuteException() {
		super();
	}

	/**
	 * @param e
	 */
	public CommandCannotExecuteException(Exception e) {
		super(e);
	}

	/**
	 * 
	 */
	private static final long serialVersionUID = 9217444179570204136L;
}
