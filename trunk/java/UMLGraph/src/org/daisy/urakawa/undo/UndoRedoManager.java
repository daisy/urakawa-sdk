package org.daisy.urakawa.undo;

import java.util.List;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.MediaData;
import org.daisy.urakawa.xuk.XukAble;

/**
 * <p>
 * The Class realizing this interface must store the history of executed
 * Commands inside an undo stack, and conversely store re-done commands in a
 * redo stack.
 * </p>
 * <p>
 * A transaction is a mechanism by which a CompositeCommand is created
 * automatically for all commands executed between the calls to
 * startTransaction() and endTransaction(). However, the current transaction can
 * be canceled by calling cancelTransaction(), which rollbacks all the commands
 * executed since the call to startTransaction(). Transactions can be nested, as
 * long as start/endTransaction calls are in matching pairs (exceptions are
 * raised otherwise).
 * </p>
 * 
 * @depend - "Composition\n(history)" 0..n org.daisy.urakawa.undo.Command
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface UndoRedoManager extends WithPresentation, XukAble {
	/**
	 * Starts a transaction, with the given description for the resulting
	 * CompositeCommand. Any executed commands from then on will be part of this
	 * transaction, until the next call to endTransaction().
	 * isTransactionActive() then returns true.
	 * 
	 * @param shortDescription
	 *            cannot be null, cannot be empty string.
	 * @param longDescription
	 *            cannot be null, but can be empty string.
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>shortDescription</b>
	 */
	public void startTransaction(String shortDescription, String longDescription);

	/**
	 * Ends the active transaction: all the executed commands since the last
	 * call to startTransaction() are now wrapped in a CompositeCommand which
	 * gets pushed into the undo stack (or in the stack of the parent active
	 * transaction if this is a nested one). Transactions can be nested, so
	 * programmers must make sure to start/end transactions in pairs (e.g. a
	 * call to endTransaction() for each startTransaction()).
	 * isTransactionActive() then returns false, unless this was a nested
	 * transaction (in which case the parent transaction becomes active).
	 * 
	 * @throws UndoRedoTransactionIsNotStartedException
	 *             if there is currently no active transaction
	 */
	public void endTransaction()
			throws UndoRedoTransactionIsNotStartedException;

	/**
	 * This cancels the active transaction: rollbacks (effectively undoes) the
	 * executed Commands down to the last call to startTransaction(), and ends
	 * the transaction. isTransactionActive() then returns false, unless this
	 * was a nested transaction (in which case the parent transaction becomes
	 * active).
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
	 * Empties the undo-redo stack of Commands.
	 * 
	 * @throws UndoRedoTransactionIsNotEndedException
	 *             if an undo-redo transaction is currently active.
	 */
	public void flushCommands() throws UndoRedoTransactionIsNotEndedException;

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
	 * @throws UndoRedoTransactionIsNotEndedException
	 *             if an undo-redo transaction is currently active.
	 */
	public String getUndoShortDescription() throws CannotUndoException,
			UndoRedoTransactionIsNotEndedException;

	/**
	 * <p>
	 * undoes the last executed Command
	 * </p>
	 * 
	 * @tagvalue Exceptions "CannotUndo"
	 * @throws CannotUndoException
	 * @see #canUndo()
	 * @throws UndoRedoTransactionIsNotEndedException
	 *             if an undo-redo transaction is currently active.
	 */
	public void undo() throws CannotUndoException,
			UndoRedoTransactionIsNotEndedException;

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
	 * @throws UndoRedoTransactionIsNotEndedException
	 *             if an undo-redo transaction is currently active.
	 */
	public String getRedoShortDescription() throws CannotRedoException,
			UndoRedoTransactionIsNotEndedException;

	/**
	 * <p>
	 * redoes the last undone Command
	 * </p>
	 * 
	 * @tagvalue Exceptions "CannotRedo"
	 * @throws CannotRedoException
	 * @see #canRedo()
	 * @throws UndoRedoTransactionIsNotEndedException
	 *             if an undo-redo transaction is currently active.
	 */
	public void redo() throws CannotRedoException,
			UndoRedoTransactionIsNotEndedException;

	/**
	 * <p>
	 * Executes and registers the given Command in the undo stack. Clears the
	 * redo stack.
	 * </p>
	 * 
	 * @param command
	 *            the Command to execute.
	 * @tagvalue Exceptions
	 *           "MethodParameterIsNull,CannotExecuteIrreversibleCommand"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws CannotExecuteIrreversibleCommandException
	 *             if an undo-redo transaction is currently active, and the
	 *             given command is irreversible.
	 */
	public void execute(Command command) throws MethodParameterIsNullException,
			CannotExecuteIrreversibleCommandException;

	/**
	 * <p>
	 * Tests whether it's possible to undo. Must return false if there is
	 * currently an active transaction.
	 * </p>
	 * 
	 * @return false if the undo stack is empty.
	 */
	public boolean canUndo();

	/**
	 * <p>
	 * Tests whether it's possible to redo. Must return false if there is
	 * currently an active transaction.
	 * </p>
	 * 
	 * @return false if the redo stack is empty.
	 */
	public boolean canRedo();

	/**
	 * @return a non-null (but potentially empty) list of commands in the
	 *         current (active) transaction
	 */
	public List<Command> getListOfCommandsInCurrentTransactions();

	/**
	 * @return a non-null (but potentially empty) list of commands in the
	 *         current redo stack.
	 */
	public List<Command> getListOfRedoStackCommands();

	/**
	 * @return a non-null (but potentially empty) list of commands in the
	 *         current undo stack.
	 */
	public List<Command> getListOfUndoStackCommands();

	/**
	 * @return a non-null (but potentially empty) list of MediaData that are
	 *         used in the undo/redo stacks, and in the current active
	 *         transactions.
	 */
	public List<MediaData> getListOfUsedMediaData();
}
