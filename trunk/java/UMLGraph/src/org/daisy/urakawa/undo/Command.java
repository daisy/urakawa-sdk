package org.daisy.urakawa.undo;

import java.util.List;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.event.ChangeNotifier;
import org.daisy.urakawa.event.undo.CommandEvent;
import org.daisy.urakawa.media.data.MediaData;
import org.daisy.urakawa.xuk.XukAble;

/**
 * <p>
 * Classes realizing this interface must store the state of the object(s)
 * affected by the command execution.
 * </p>
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 * @stereotype XukAble
 * @depend - Event - org.daisy.urakawa.event.undo.CommandExecutedEvent
 * @depend - Event - org.daisy.urakawa.event.undo.CommandUnExecutedEvent
 */
public interface Command extends XukAble, WithPresentation,
		WithShortLongDescription, ChangeNotifier<CommandEvent> {
	/**
	 * <p>
	 * Returns a list of MediaData objects that are in use by this command.
	 * </p>
	 * 
	 * @return a non-null, possibly empty, list of Media objects
	 */
	public List<MediaData> getListOfUsedMediaData();

	/**
	 * <p>
	 * executes the Command
	 * </p>
	 * @tagvalue Events "CommandExecuted"
	 * @tagvalue Exceptions "CommandCannotExecute"
	 * @throws CommandCannotExecuteException
	 *             when the Command cannot be executed
	 */
	public void execute() throws CommandCannotExecuteException;

	/**
	 * <p>
	 * executes the reverse Command
	 * </p>
	 * @tagvalue Events "CommandUnExecuted"
	 * @tagvalue Exceptions "CommandCannotUnExecute"
	 * @throws CommandCannotUnExecuteException
	 *             when the Command cannot be un-executed
	 */
	public void unExecute() throws CommandCannotUnExecuteException;

	/**
	 * <p>
	 * Tests whether this command can be un-executed.
	 * </p>
	 * 
	 * @return true if this command can be un-executed.
	 */
	public boolean canUnExecute();

	/**
	 * <p>
	 * Tests whether this command can be executed.
	 * </p>
	 * 
	 * @return true if this command can be executed.
	 */
	public boolean canExecute();
}
