package org.daisy.urakawa.undo;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

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
public interface UndoRedoManager extends XukAble {
	/**
	 * Starts a transaction: marks the current level in the history as the point
	 * where the transaction begins Any following call to execute(Command) will
	 * push the Command into the history and execute it normally.
	 * isTransactionActive() must return true. Transactions can be nested, so
	 * programmers must make sure to start/end transactions in pairs (e.g. a
	 * call to endTransaction() for each startTransaction()). A transaction can
	 * be canceled (rollback), and all Commands un-executed using
	 * cancelTransaction().
	 */
	public void startTransaction();

	/**
	 * Ends the active transaction: retrieves the point in the history where
	 * startTransaction() was called, and extracts all following Commands from
	 * the history (that were not executed), wraps them in a new
	 * CompositeCommand, pushes this new command in the history and executes it.
	 * Transactions can be nested, so programmers must make sure to start/end
	 * transactions in pairs (e.g. a call to endTransaction() for each
	 * startTransaction()). isTransactionActive() then returns false, unless
	 * this was a nested transaction.
	 * 
	 * @throws UndoRedoTransactionIsNotStartedException
	 *             if there is currently no active transaction
	 */
	public void endTransaction()
			throws UndoRedoTransactionIsNotStartedException;

	/**
	 * This cancels the active transaction: rollbacks the Commands down to the
	 * point in the history where startTransaction() was called, and ends the
	 * transaction. isTransactionActive() then returns false, unless this was a
	 * nested transaction.
	 * 
	 * @throws UndoRedoTransactionIsNotStartedException
	 *             if there is currently no active transaction
	 */
	public void cancelTransaction()
			throws UndoRedoTransactionIsNotStartedException;

	/**
	 * @return true if there is currently an active transaction.
	 */
	public boolean isTransactionActive();

	/**
	 * Empties the undo-redo Commands.
	 * 
	 * @throws UndoRedoTransactionIsNotFinishedException
	 *             if an undo-redo transaction is currently active.
	 */
	public void flushCommands()
			throws UndoRedoTransactionIsNotFinishedException;

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
	 * @throws UndoRedoTransactionIsNotFinishedException
	 *             if an undo-redo transaction is currently active.
	 */
	public String getUndoShortDescription() throws CannotUndoException,
			UndoRedoTransactionIsNotFinishedException;

	/**
	 * <p>
	 * undoes the last executed Command
	 * </p>
	 * 
	 * @tagvalue Exceptions "CannotUndo"
	 * @throws CannotUndoException
	 * @see #canUndo()
	 * @see org.daisy.urakawa.undo.Command#unExecute()
	 * @throws UndoRedoTransactionIsNotFinishedException
	 *             if an undo-redo transaction is currently active.
	 */
	public void undo() throws CannotUndoException,
			UndoRedoTransactionIsNotFinishedException;

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
	 * @throws UndoRedoTransactionIsNotFinishedException
	 *             if an undo-redo transaction is currently active.
	 */
	public String getRedoShortDescription() throws CannotRedoException,
			UndoRedoTransactionIsNotFinishedException;

	/**
	 * <p>
	 * redoes the last undone Command
	 * </p>
	 * 
	 * @tagvalue Exceptions "CannotRedo"
	 * @throws CannotRedoException
	 * @see #canRedo()
	 * @see org.daisy.urakawa.undo.Command#execute()
	 * @throws UndoRedoTransactionIsNotFinishedException
	 *             if an undo-redo transaction is currently active.
	 */
	public void redo() throws CannotRedoException,
			UndoRedoTransactionIsNotFinishedException;

	/**
	 * <p>
	 * Executes and registers the given Command in the history, deleting all
	 * following undone Commands in the history (on the "right hand side"), if
	 * any.
	 * </p>
	 * 
	 * @param command
	 *            the Command to register in the history and to execute.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws UndoRedoTransactionIsNotFinishedException
	 *             if an undo-redo transaction is currently active.
	 */
	public void execute(Command command) throws MethodParameterIsNullException,
			UndoRedoTransactionIsNotFinishedException;

	/**
	 * <p>
	 * Tests whether it's possible to undo. Must return false if there is
	 * currently an active transaction.
	 * </p>
	 * 
	 * @return false if the history is empty, otherwise true if the last
	 *         executed Command (via "execute()" or "redo()")) is undoable.
	 * @see org.daisy.urakawa.undo.Command#canUnExecute()
	 */
	public boolean canUndo();

	/**
	 * <p>
	 * Tests whether it's possible to redo. Must return false if there is
	 * currently an active transaction.
	 * </p>
	 * 
	 * @return false if the history is empty, otherwise true if the last undone
	 *         Command (via "undo()")) is redoable.
	 */
	public boolean canRedo();
}
