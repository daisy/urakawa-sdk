package org.daisy.urakawa.undo;

import java.util.List;

import org.daisy.urakawa.IWithPresentation;
import org.daisy.urakawa.command.ICommand;
import org.daisy.urakawa.command.CommandCannotExecuteException;
import org.daisy.urakawa.events.DataModelChangedEvent;
import org.daisy.urakawa.events.IEventHandler;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.IMediaData;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * <p>
 * The Class realizing this interface must store the history of executed
 * Commands inside an undo stack, and conversely store re-done commands in a
 * redo stack.
 * </p>
 * <p>
 * A transaction is a mechanism by which a ICompositeCommand is created
 * automatically for all commands executed between the calls to
 * startTransaction() and endTransaction(). However, the current transaction can
 * be canceled by calling cancelTransaction(), which rollbacks all the commands
 * executed since the call to startTransaction(). Transactions can be nested, as
 * long as start/endTransaction calls are in matching pairs (exceptions are
 * raised otherwise).
 * </p>
 * 
 * @depend - Event - org.daisy.urakawa.event.undo.CommandDoneEvent
 * @depend - Event - org.daisy.urakawa.event.undo.CommandUnDoneEvent
 * @depend - Event - org.daisy.urakawa.event.undo.CommandReDoneEvent
 * @depend - Event - org.daisy.urakawa.event.undo.TransactionStartedEvent
 * @depend - Event - org.daisy.urakawa.event.undo.TransactionEndedEvent
 * @depend - Event - org.daisy.urakawa.event.undo.TransactionCancelledEvent
 * @depend - "Composition\n(undo/redo stacks)" 0..n org.daisy.urakawa.undo.Command
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface IUndoRedoManager extends IWithPresentation, IXukAble,
        IEventHandler<DataModelChangedEvent>
{
    /**
     * Starts a transaction, with the given description for the resulting
     * ICompositeCommand. Any executed commands from then on will be part of
     * this transaction, until the next call to endTransaction().
     * isTransactionActive() then returns true. Transactions can be nested, so
     * programmers must make sure to start and end/cancel transactions in pairs
     * (e.g. a call to endTransaction() for each startTransaction()).A
     * transaction can be canceled (rollback), and all Commands un-executed by
     * calling cancelTransaction().
     * 
     * @tagvalue Events "TransactionStarted"
     * @param shortDescription cannot be null, cannot be empty string.
     * @param longDescription cannot be null, but can be empty string.
     * 
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * @throws MethodParameterIsEmptyStringException Empty string '' method
     *         parameter is forbidden: <b>shortDescription</b>
     */
    public void startTransaction(String shortDescription, String longDescription)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException;

    /**
     * Ends the active transaction: all the executed commands since the last
     * call to startTransaction() are now wrapped in a ICompositeCommand which
     * gets pushed into the undo stack (or in the stack of the parent active
     * transaction if this is a nested one). Transactions can be nested, so
     * programmers must make sure to start/end transactions in pairs (e.g. a
     * call to endTransaction() for each startTransaction()).
     * isTransactionActive() then returns false, unless this was a nested
     * transaction (in which case the parent transaction becomes active).
     * 
     * @tagvalue Events "TransactionEnded"
     * 
     * @throws UndoRedoTransactionIsNotStartedException if there is currently no
     *         active transaction
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
     * @tagvalue Events "TransactionCancelled"
     * 
     * @throws UndoRedoTransactionIsNotStartedException if there is currently no
     *         active transaction
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
     * 
     * @throws UndoRedoTransactionIsNotEndedException if an undo-redo
     *         transaction is currently active.
     */
    public void flushCommands() throws UndoRedoTransactionIsNotEndedException;

    /**
     * <p>
     * Returns a human-readable name for the next Undoable ICommand.
     * </p>
     * 
     * @return cannot be null or empty string.
     * 
     * @throws CannotUndoException
     * @see #canUndo()
     * @see org.daisy.urakawa.command.ICommand#getShortDescription()
     * @throws UndoRedoTransactionIsNotEndedException if an undo-redo
     *         transaction is currently active.
     */
    public String getUndoShortDescription() throws CannotUndoException,
            UndoRedoTransactionIsNotEndedException;

    /**
     * <p>
     * undoes the last executed ICommand
     * </p>
     * 
     * @tagvalue Events "CommandUnDone"
     * 
     * @throws CannotUndoException
     * @see #canUndo()
     * @throws UndoRedoTransactionIsNotEndedException if an undo-redo
     *         transaction is currently active.
     */
    public void undo() throws CannotUndoException,
            UndoRedoTransactionIsNotEndedException;

    /**
     * <p>
     * Returns a human-readable name for the next Redoable ICommand
     * </p>
     * 
     * @return cannot be null or empty string.
     * 
     * @throws CannotRedoException
     * @see #canRedo()
     * @see org.daisy.urakawa.command.ICommand#getShortDescription()
     * @throws UndoRedoTransactionIsNotEndedException if an undo-redo
     *         transaction is currently active.
     */
    public String getRedoShortDescription() throws CannotRedoException,
            UndoRedoTransactionIsNotEndedException;

    /**
     * <p>
     * redoes the last undone ICommand
     * </p>
     * 
     * @tagvalue Events "CommandReDone"
     * 
     * @throws CannotRedoException
     * @see #canRedo()
     * @throws UndoRedoTransactionIsNotEndedException if an undo-redo
     *         transaction is currently active.
     */
    public void redo() throws CannotRedoException,
            UndoRedoTransactionIsNotEndedException;

    /**
     * <p>
     * Executes and registers the given ICommand in the undo stack. Clears the
     * redo stack.
     * </p>
     * 
     * @tagvalue Events "CommandDone"
     * @param iCommand the ICommand to execute.
     * 
     *           "MethodParameterIsNull-CommandCannotExecute-CannotExecuteIrreversibleCommand"
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * @throws CommandCannotExecuteException when the given ICommand cannot be
     *         executed.
     * @throws CannotExecuteIrreversibleCommandException if an undo-redo
     *         transaction is currently active, and the given command is
     *         irreversible.
     */
    public void execute(ICommand iCommand)
            throws MethodParameterIsNullException,
            CommandCannotExecuteException,
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
    public List<ICommand> getListOfCommandsInCurrentTransactions();

    /**
     * @return a non-null (but potentially empty) list of commands in the
     *         current redo stack.
     */
    public List<ICommand> getListOfRedoStackCommands();

    /**
     * @return a non-null (but potentially empty) list of commands in the
     *         current undo stack.
     */
    public List<ICommand> getListOfUndoStackCommands();

    /**
     * @return a non-null (but potentially empty) list of IMediaData that are
     *         used in the undo/redo stacks, and in the current active
     *         transactions.
     */
    public List<IMediaData> getListOfUsedMediaData();
}
