package org.daisy.urakawa.undo;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * The Class realizing this interface must store the history of Commands and
 * maintain the current Undo/Redo state (pointer in the dynamic stack of
 * Commands). An application should execute all the operations that affect the
 * state of the data model via this manager (or "controler"), so that write
 * access is consistent in respect to the Undo/Redo requirements (a
 * non-registered modification of the data model will potentially corrupt
 * further undo operation, particularly with object composition/recursion, such
 * as with trees).
 * 
 * @depend - "Composition\n(history)" 0..n Command
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface CommandManager {
	/**
	 * @return a human-readable name for the next Undoable Command
	 * @tagvalue Exceptions "CannotUndo"
	 * @see #canUndo()
	 * @see org.daisy.urakawa.undo.Command#getUnExecuteShortDescription()
	 */
	public String getUndoShortDescription() throws CannotUndoException;

	/**
	 * undoes the last executed Command
	 * 
	 * @tagvalue Exceptions "CannotUndo"
	 * @see #canUndo()
	 * @see org.daisy.urakawa.undo.Command#unExecute() ()
	 */
	public void undo() throws CannotUndoException;

	/**
	 * @return a human-readable name for the next Redoable Command
	 * @tagvalue Exceptions "CannotRedo"
	 * @see #canRedo()
	 * @see org.daisy.urakawa.undo.Command#getExecuteShortDescription()
	 */
	public String getRedoShortDescription() throws CannotRedoException;

	/**
	 * redoes the last undone Command
	 * 
	 * @tagvalue Exceptions "CannotRedo"
	 * @see #canRedo()
	 * @see org.daisy.urakawa.undo.Command#execute()
	 */
	public void redo() throws CannotRedoException;

	/**
	 * Executes and registers the given Command in the history, deleting all
	 * following undone Commands in the history (on the "right hand side"), if
	 * any. In some special cases (e.g. user typing text letter by letter, but
	 * undo/redo applies to full word or sentence), this method my take the
	 * responsibility to automatically merge a series of Commands into a
	 * CompositeCommand.
	 * 
	 * @param command
	 *            the Command to register in the history and to execute.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public void execute(Command command) throws MethodParameterIsNullException;

	/**
	 * @return false if the history is empty, otherwise true if the last
	 *         executed Command (via "execute()" or "redo()")) is undoable.
	 * @see org.daisy.urakawa.undo.Command#canUnExecute()
	 */
	public boolean canUndo();

	/**
	 * @return false if the history is empty, otherwise true if the last undone
	 *         Command (via "undo()")) is redoable.
	 */
	public boolean canRedo();
}
