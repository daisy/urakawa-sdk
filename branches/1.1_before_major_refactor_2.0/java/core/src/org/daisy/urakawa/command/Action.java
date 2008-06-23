package org.daisy.urakawa.command;

/**
 *
 */
public interface Action extends WithShortLongDescription {
	/**
	 * <p>
	 * executes the Command
	 * </p>
	 * 
	 * @tagvalue Events "CommandExecuted"
	 * @tagvalue Exceptions "CommandCannotExecute"
	 * @throws CommandCannotExecuteException
	 *             when the Command cannot be executed
	 */
	public void execute() throws CommandCannotExecuteException;

	/**
	 * <p>
	 * Tests whether this command can be executed.
	 * </p>
	 * 
	 * @return true if this command can be executed.
	 */
	public boolean canExecute();
}
