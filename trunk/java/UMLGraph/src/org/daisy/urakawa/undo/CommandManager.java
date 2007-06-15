package org.daisy.urakawa.undo;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * The Class realizing this interface must store the history of Commands and
 * maintain the current Undo/Redo state (e.g. pointer in the dynamic stack of
 * Commands).
 * </p>
 * 
 * @depend - "Composition\n(history)" 0..n org.daisy.urakawa.undo.Command
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface CommandManager {
	/**
	 * <p>
	 * Returns a human-readable name for the next Undoable Command.
	 * </p>
	 * 
	 * @return cannot be null or empty string.
	 * @tagvalue Exceptions "CannotUndo"
	 * @throws CannotUndoException
	 * @see #canUndo()
	 * @see org.daisy.urakawa.undo.Command#getUnExecuteShortDescription()
	 */
	public String getUndoShortDescription() throws CannotUndoException;

	/**
	 * <p>
	 * undoes the last executed Command
	 * </p>
	 * 
	 * @tagvalue Exceptions "CannotUndo"
	 * @throws CannotUndoException
	 * @see #canUndo()
	 * @see org.daisy.urakawa.undo.Command#unExecute() ()
	 */
	public void undo() throws CannotUndoException;

	/**
	 * <p>
	 * Returns a human-readable name for the next Redoable Command
	 * </p>
	 * 
	 * @return cannot be null or empty string.
	 * @tagvalue Exceptions "CannotRedo"
	 * @throws CannotRedoException
	 * @see #canRedo()
	 * @see org.daisy.urakawa.undo.Command#getExecuteShortDescription()
	 */
	public String getRedoShortDescription() throws CannotRedoException;

	/**
	 * <p>
	 * redoes the last undone Command
	 * </p>
	 * 
	 * @tagvalue Exceptions "CannotRedo"
	 * @throws CannotRedoException
	 * @see #canRedo()
	 * @see org.daisy.urakawa.undo.Command#execute()
	 */
	public void redo() throws CannotRedoException;

	/**
	 * <p>
	 * Executes and registers the given Command in the history, deleting all
	 * following undone Commands in the history (on the "right hand side"), if
	 * any.
	 * </p>
	 * <p>
	 * In some special cases (e.g. user typing text letter by letter, but
	 * undo/redo applies to full word or sentence), this method my take the
	 * responsibility to automatically merge a series of Commands into a
	 * CompositeCommand.
	 * </p>
	 * 
	 * @param command
	 *            the Command to register in the history and to execute.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public void execute(Command command) throws MethodParameterIsNullException;

	/**
	 * <p>
	 * Tests whether it's possible to undo.
	 * </p>
	 * 
	 * @return false if the history is empty, otherwise true if the last
	 *         executed Command (via "execute()" or "redo()")) is undoable.
	 * @see org.daisy.urakawa.undo.Command#canUnExecute()
	 */
	public boolean canUndo();

	/**
	 * <p>
	 * Tests whether it's possible to redo.
	 * </p>
	 * 
	 * @return false if the history is empty, otherwise true if the last undone
	 *         Command (via "undo()")) is redoable.
	 */
	public boolean canRedo();
}
