package org.daisy.urakawa.command;

/**
 * an execute-only Action (a Command adds unexecute facility)
 */
public interface IAction extends IWithShortLongDescription {
	/**
	 * <p>
	 * executes the ICommand
	 * </p>
	 * 
	 * @tagvalue Events "CommandExecuted"
	 * @tagvalue Exceptions "CommandCannotExecute"
	 * @throws CommandCannotExecuteException
	 *             when the ICommand cannot be executed
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
