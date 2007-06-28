package org.daisy.urakawa.undo;

import java.util.List;

import org.daisy.urakawa.media.data.MediaData;

/**
 * <p>
 * Classes realizing this interface must store the state of the object(s)
 * affected by the command execution (including undo/redo).
 * </p>
 */
public interface Command {
	/**
	 * Returns a list of MediaData objects that are in use by this command.
	 * 
	 * @return a non-null, possibly empty, list of Media objects
	 */
	public List<MediaData> getListOfUsedMediaData();

	/**
	 * <p>
	 * executes the reverse Command
	 * </p>
	 * 
	 * @tagvalue Exceptions "CannotUndo"
	 * @throws CannotUndoException
	 */
	public void unExecute() throws CannotUndoException;

	/**
	 * <p>
	 * Return a human-readable name for the reverse Command
	 * </p>
	 * 
	 * @return cannot be null, or empty string.
	 * @tagvalue Exceptions "CannotUndo"
	 * @throws CannotUndoException
	 */
	public String getUnExecuteShortDescription() throws CannotUndoException;

	/**
	 * <p>
	 * executes the Command
	 * </p>
	 */
	public void execute();

	/**
	 * <p>
	 * Return a human-readable name for the Command
	 * </p>
	 * 
	 * @return cannot be null, or empty string.
	 */
	public String getExecuteShortDescription();

	/**
	 * <p>
	 * Tests whether this Command is undoable (reversible).
	 * </p>
	 * 
	 * @return true if this Command is undoable.
	 */
	public boolean canUnExecute();
}
