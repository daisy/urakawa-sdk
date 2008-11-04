package org.daisy.urakawa.undo;

import java.net.URI;
import java.util.LinkedList;
import java.util.List;
import java.util.Stack;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.command.CommandCannotExecuteException;
import org.daisy.urakawa.command.CommandCannotUnExecuteException;
import org.daisy.urakawa.command.ICommand;
import org.daisy.urakawa.command.ICompositeCommand;
import org.daisy.urakawa.events.DataModelChangedEvent;
import org.daisy.urakawa.events.Event;
import org.daisy.urakawa.events.EventHandler;
import org.daisy.urakawa.events.IEventHandler;
import org.daisy.urakawa.events.IEventListener;
import org.daisy.urakawa.events.undo.CommandDoneEvent;
import org.daisy.urakawa.events.undo.CommandReDoneEvent;
import org.daisy.urakawa.events.undo.CommandUnDoneEvent;
import org.daisy.urakawa.events.undo.TransactionCancelledEvent;
import org.daisy.urakawa.events.undo.TransactionEndedEvent;
import org.daisy.urakawa.events.undo.TransactionStartedEvent;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.IMediaData;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.xuk.AbstractXukAble;
import org.daisy.urakawa.xuk.IXukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * <p>
 * This class stores the history of executed Commands inside an undo stack, and
 * conversely store re-done commands in a redo stack.
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
 * @depend - Composition 0..n org.daisy.urakawa.undo.Command
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 */
public final class UndoRedoManager extends AbstractXukAble implements
        IEventHandler<DataModelChangedEvent>
{
    private Presentation mPresentation;

    /**
     * @return the Presentation owner
     */
    public Presentation getPresentation()
    {
        return mPresentation;
    }

    private Stack<ICommand> mUndoStack;
    private Stack<ICommand> mRedoStack;
    private Stack<ICompositeCommand> mActiveTransactions;

    /**
     * @param pres
     * @throws MethodParameterIsNullException
     */
    public UndoRedoManager(Presentation pres)
            throws MethodParameterIsNullException
    {
        if (pres == null)
        {
            throw new MethodParameterIsNullException();
        }
        mUndoStack = new Stack<ICommand>();
        mRedoStack = new Stack<ICommand>();
        mActiveTransactions = new Stack<ICompositeCommand>();
    }

    /**
     * Empties the undo-redo stack of Commands.
     * 
     * @throws UndoRedoTransactionIsNotEndedException
     *         if an undo-redo transaction is currently active.
     */
    public void flushCommands() throws UndoRedoTransactionIsNotEndedException
    {
        if (isTransactionActive())
        {
            throw new UndoRedoTransactionIsNotEndedException();
        }
        mUndoStack.clear();
        mRedoStack.clear();
    }

    protected void pushCommand(ICommand iCommand)
            throws CannotExecuteIrreversibleCommandException
    {
        if (isTransactionActive())
        {
            if (!iCommand.canUnExecute())
            {
                throw new CannotExecuteIrreversibleCommandException();
            }
            try
            {
                mActiveTransactions.peek().append(iCommand);
            }
            catch (MethodParameterIsNullException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
        else
        {
            if (iCommand.canUnExecute())
            {
                mUndoStack.push(iCommand);
                mRedoStack.clear();
            }
            else
            {
                try
                {
                    flushCommands();
                }
                catch (UndoRedoTransactionIsNotEndedException e)
                {
                    // Should never happen
                    throw new RuntimeException("WTF ??!", e);
                }
            }
        }
    }

    /**
     * <p>
     * Executes and registers the given ICommand in the undo stack. Clears the
     * redo stack.
     * </p>
     * 
     * @tagvalue Events "CommandDone"
     * @param iCommand
     *        the ICommand to execute.
     *        "MethodParameterIsNull-CommandCannotExecute-CannotExecuteIrreversibleCommand"
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws CommandCannotExecuteException
     *         when the given ICommand cannot be executed.
     * @throws CannotExecuteIrreversibleCommandException
     *         if an undo-redo transaction is currently active, and the given
     *         command is irreversible.
     */
    public void execute(ICommand iCommand)
            throws MethodParameterIsNullException,
            CannotExecuteIrreversibleCommandException,
            CommandCannotExecuteException
    {
        if (iCommand == null)
            throw new MethodParameterIsNullException();
        pushCommand(iCommand);
        iCommand.execute();
        notifyListeners(new CommandDoneEvent(this, iCommand));
    }

    /**
     * <p>
     * undoes the last executed ICommand
     * </p>
     * 
     * @tagvalue Events "CommandUnDone"
     * @throws CannotUndoException
     * @see #canUndo()
     * @throws UndoRedoTransactionIsNotEndedException
     *         if an undo-redo transaction is currently active.
     */
    public void undo() throws CannotUndoException,
            UndoRedoTransactionIsNotEndedException
    {
        if (isTransactionActive())
        {
            throw new UndoRedoTransactionIsNotEndedException();
        }
        if (mUndoStack.size() == 0)
            throw new CannotUndoException();
        try
        {
            mUndoStack.peek().unExecute();
        }
        catch (CommandCannotUnExecuteException e)
        {
            throw new CannotUndoException();
        }
        ICommand cmd = mUndoStack.pop();
        mRedoStack.push(cmd);
        try
        {
            notifyListeners(new CommandUnDoneEvent(this, cmd));
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    /**
     * <p>
     * redoes the last undone ICommand
     * </p>
     * 
     * @tagvalue Events "CommandReDone"
     * @throws CannotRedoException
     * @see #canRedo()
     * @throws UndoRedoTransactionIsNotEndedException
     *         if an undo-redo transaction is currently active.
     */
    public void redo() throws CannotRedoException,
            UndoRedoTransactionIsNotEndedException
    {
        if (isTransactionActive())
        {
            throw new UndoRedoTransactionIsNotEndedException();
        }
        if (mRedoStack.size() == 0)
            throw new CannotRedoException();
        try
        {
            mRedoStack.peek().execute();
        }
        catch (CommandCannotExecuteException e)
        {
            throw new CannotRedoException();
        }
        ICommand cmd = mRedoStack.pop();
        mUndoStack.push(cmd);
        try
        {
            notifyListeners(new CommandReDoneEvent(this, cmd));
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    /**
     * <p>
     * Returns a human-readable name for the next Redoable ICommand
     * </p>
     * 
     * @return cannot be null or empty string.
     * @throws CannotRedoException
     * @see #canRedo()
     * @see org.daisy.urakawa.command.ICommand#getShortDescription()
     * @throws UndoRedoTransactionIsNotEndedException
     *         if an undo-redo transaction is currently active.
     */
    public String getRedoShortDescription() throws CannotRedoException,
            UndoRedoTransactionIsNotEndedException
    {
        if (mRedoStack.size() == 0)
        {
            throw new CannotRedoException();
        }
        if (isTransactionActive())
        {
            throw new UndoRedoTransactionIsNotEndedException();
        }
        return mRedoStack.peek().getShortDescription();
    }

    /**
     * <p>
     * Returns a human-readable name for the next Undoable ICommand.
     * </p>
     * 
     * @return cannot be null or empty string.
     * @throws CannotUndoException
     * @see #canUndo()
     * @see org.daisy.urakawa.command.ICommand#getShortDescription()
     * @throws UndoRedoTransactionIsNotEndedException
     *         if an undo-redo transaction is currently active.
     */
    public String getUndoShortDescription() throws CannotUndoException,
            UndoRedoTransactionIsNotEndedException
    {
        if (mUndoStack.size() == 0)
        {
            throw new CannotUndoException();
        }
        if (isTransactionActive())
        {
            throw new UndoRedoTransactionIsNotEndedException();
        }
        return mUndoStack.peek().getShortDescription();
    }

    /**
     * @return a non-null (but potentially empty) list of commands in the
     *         current undo stack.
     */
    public List<ICommand> getListOfUndoStackCommands()
    {
        return new LinkedList<ICommand>(mUndoStack);
    }

    /**
     * @return a non-null (but potentially empty) list of commands in the
     *         current redo stack.
     */
    public List<ICommand> getListOfRedoStackCommands()
    {
        return new LinkedList<ICommand>(mRedoStack);
    }

    /**
     * @return a non-null (but potentially empty) list of commands in the
     *         current (active) transaction
     */
    public List<ICommand> getListOfCommandsInCurrentTransactions()
    {
        List<ICommand> res = new LinkedList<ICommand>();
        for (ICompositeCommand trans : mActiveTransactions)
        {
            res.addAll(trans.getListOfCommands());
        }
        return res;
    }

    /**
     * @return a non-null (but potentially empty) list of IMediaData that are
     *         used in the undo/redo stacks, and in the current active
     *         transactions.
     */
    public List<IMediaData> getListOfUsedMediaData()
    {
        List<IMediaData> res = new LinkedList<IMediaData>();
        List<ICommand> iCommands = new LinkedList<ICommand>();
        iCommands.addAll(getListOfUndoStackCommands());
        iCommands.addAll(getListOfRedoStackCommands());
        iCommands.addAll(getListOfCommandsInCurrentTransactions());
        for (ICommand cmd : iCommands)
        {
            for (IMediaData md : cmd.getListOfUsedMediaData())
            {
                if (!res.contains(md))
                    res.add(md);
            }
        }
        return res;
    }

    /**
     * <p>
     * Tests whether it's possible to undo. Must return false if there is
     * currently an active transaction.
     * </p>
     * 
     * @return false if the undo stack is empty.
     */
    public boolean canUndo()
    {
        if (isTransactionActive())
            return false;
        if (mUndoStack.size() == 0)
            return false;
        return true;
    }

    /**
     * <p>
     * Tests whether it's possible to redo. Must return false if there is
     * currently an active transaction.
     * </p>
     * 
     * @return false if the redo stack is empty.
     */
    public boolean canRedo()
    {
        if (isTransactionActive())
            return false;
        if (mRedoStack.size() == 0)
            return false;
        return true;
    }

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
     * @param shortDesc
     *        cannot be null, cannot be empty string.
     * @param longDesc
     *        cannot be null, but can be empty string.
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws MethodParameterIsEmptyStringException
     *         Empty string '' method parameter is forbidden:
     *         <b>shortDescription</b>
     */
    public void startTransaction(String shortDesc, String longDesc)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException
    {
        if (shortDesc == null || longDesc == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (shortDesc.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        ICompositeCommand newTrans;
        newTrans = getPresentation().getCommandFactory()
                .createCompositeCommand();
        try
        {
            newTrans.setShortDescription(shortDesc);
            newTrans.setLongDescription(longDesc);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (MethodParameterIsEmptyStringException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        mActiveTransactions.push(newTrans);
        try
        {
            notifyListeners(new TransactionStartedEvent(this));
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

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
     * @throws UndoRedoTransactionIsNotStartedException
     *         if there is currently no active transaction
     */
    public void endTransaction()
            throws UndoRedoTransactionIsNotStartedException
    {
        if (!isTransactionActive())
        {
            throw new UndoRedoTransactionIsNotStartedException();
        }
        try
        {
            pushCommand(mActiveTransactions.pop());
        }
        catch (CannotExecuteIrreversibleCommandException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        try
        {
            notifyListeners(new TransactionEndedEvent(this));
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    /**
     * This cancels the active transaction: rollbacks (effectively undoes) the
     * executed Commands down to the last call to startTransaction(), and ends
     * the transaction. isTransactionActive() then returns false, unless this
     * was a nested transaction (in which case the parent transaction becomes
     * active).
     * 
     * @tagvalue Events "TransactionCancelled"
     * @throws UndoRedoTransactionIsNotStartedException
     *         if there is currently no active transaction
     */
    public void cancelTransaction()
            throws UndoRedoTransactionIsNotStartedException
    {
        if (!isTransactionActive())
        {
            throw new UndoRedoTransactionIsNotStartedException();
        }
        try
        {
            mActiveTransactions.pop().unExecute();
        }
        catch (CommandCannotUnExecuteException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        try
        {
            notifyListeners(new TransactionCancelledEvent(this));
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    /**
     * @return true if there is currently an active transaction.
     */
    public boolean isTransactionActive()
    {
        return (mActiveTransactions.size() > 0);
    }

    /**
     * @hidden
     */
    @Override
    protected void clear()
    {
        while (isTransactionActive())
        {
            try
            {
                endTransaction();
            }
            catch (UndoRedoTransactionIsNotStartedException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
        try
        {
            flushCommands();
        }
        catch (UndoRedoTransactionIsNotEndedException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        // super.clear();
    }

    /**
     * @hidden
     */
    @Override
    protected void xukInChild(IXmlDataReader source, IProgressHandler ph)
            throws XukDeserializationFailedException,
            ProgressCancelledException
    {
        // To avoid event notification overhead, we bypass this:
        if (false && ph != null && ph.notifyProgress())
        {
            throw new ProgressCancelledException();
        }
        boolean readItem = false;
        if (source.getNamespaceURI() == IXukAble.XUK_NS)
        {
            readItem = true;
            String str = source.getLocalName();
            if (str == "mUndoStack")
            {
                xukInCommandStack(source, mUndoStack, ph);
            }
            else
                if (str == "mRedoStack")
                {
                    xukInCommandStack(source, mRedoStack, ph);
                }
                else
                    if (str == "mActiveTransactions")
                    {
                        xukInCommandStack(source, mActiveTransactions, ph);
                    }
                    else
                    {
                        readItem = false;
                    }
        }
        if (!readItem)
        {
            try
            {
                super.xukInChild(source, ph);
            }
            catch (MethodParameterIsNullException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ?!", e);
            }
        }
    }

    /**
     * @hidden
     */
    @SuppressWarnings("unchecked")
    private <T extends ICommand> void xukInCommandStack(IXmlDataReader source,
            Stack<T> stack, IProgressHandler ph)
            throws XukDeserializationFailedException,
            ProgressCancelledException
    {
        if (ph != null && ph.notifyProgress())
        {
            throw new ProgressCancelledException();
        }
        if (!source.isEmptyElement())
        {
            while (source.read())
            {
                if (source.getNodeType() == IXmlDataReader.ELEMENT)
                {
                    ICommand cmd = null;
                    try
                    {
                        cmd = getPresentation().getCommandFactory()
                                .create(source.getLocalName(),
                                        source.getNamespaceURI());
                    }
                    catch (MethodParameterIsNullException e1)
                    {
                        // Should never happen
                        throw new RuntimeException("WTF ??!", e1);
                    }
                    catch (MethodParameterIsEmptyStringException e1)
                    {
                        // Should never happen
                        throw new RuntimeException("WTF ??!", e1);
                    }
                    stack.push((T) cmd);
                    try
                    {
                        cmd.xukIn(source, ph);
                    }
                    catch (MethodParameterIsNullException e)
                    {
                        // Should never happen
                        throw new RuntimeException("WTF ??!", e);
                    }
                }
                else
                    if (source.getNodeType() == IXmlDataReader.END_ELEMENT)
                    {
                        break;
                    }
                if (source.isEOF())
                    throw new XukDeserializationFailedException();
            }
        }
    }

    /**
     * @hidden
     */
    @Override
    protected void xukOutChildren(IXmlDataWriter destination, URI baseUri,
            IProgressHandler ph) throws XukSerializationFailedException,
            ProgressCancelledException
    {
        // To avoid event notification overhead, we bypass this:
        if (false && ph != null && ph.notifyProgress())
        {
            throw new ProgressCancelledException();
        }
        destination.writeStartElement("mUndoStack", IXukAble.XUK_NS);
        for (ICommand cmd : mUndoStack)
        {
            try
            {
                cmd.xukOut(destination, baseUri, ph);
            }
            catch (MethodParameterIsNullException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
        destination.writeEndElement();
        destination.writeStartElement("mRedoStack", IXukAble.XUK_NS);
        for (ICommand cmd : mRedoStack)
        {
            try
            {
                cmd.xukOut(destination, baseUri, ph);
            }
            catch (MethodParameterIsNullException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
        destination.writeEndElement();
        destination.writeStartElement("mActiveTransactions", IXukAble.XUK_NS);
        for (ICompositeCommand cmd : mActiveTransactions)
        {
            try
            {
                cmd.xukOut(destination, baseUri, ph);
            }
            catch (MethodParameterIsNullException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
        destination.writeEndElement();
        // super.xukOutChildren(destination, baseUri);
    }

    protected IEventHandler<Event> mTransactionStartedEventNotifier = new EventHandler();
    protected IEventHandler<Event> mTransactionEndedEventNotifier = new EventHandler();
    protected IEventHandler<Event> mTransactionCancelledEventNotifier = new EventHandler();
    protected IEventHandler<Event> mCommandDoneEventNotifier = new EventHandler();
    protected IEventHandler<Event> mCommandUnDoneEventNotifier = new EventHandler();
    protected IEventHandler<Event> mCommandReDoneEventNotifier = new EventHandler();
    protected IEventHandler<Event> mDataModelEventNotifier = new EventHandler();
    protected IEventListener<DataModelChangedEvent> mBubbleEventListener = new IEventListener<DataModelChangedEvent>()
    {
        public <K extends DataModelChangedEvent> void eventCallback(K event)
                throws MethodParameterIsNullException
        {
            if (event == null)
            {
                throw new MethodParameterIsNullException();
            }
            notifyListeners(event);
        }
    };

    /**
     * @hidden
     */
    public <K extends DataModelChangedEvent> void notifyListeners(K event)
            throws MethodParameterIsNullException
    {
        if (event == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (TransactionStartedEvent.class.isAssignableFrom(event.getClass()))
        {
            mTransactionStartedEventNotifier.notifyListeners(event);
        }
        else
            if (TransactionCancelledEvent.class.isAssignableFrom(event
                    .getClass()))
            {
                mTransactionCancelledEventNotifier.notifyListeners(event);
            }
            else
                if (TransactionEndedEvent.class.isAssignableFrom(event
                        .getClass()))
                {
                    mTransactionEndedEventNotifier.notifyListeners(event);
                }
                else
                    if (CommandDoneEvent.class.isAssignableFrom(event
                            .getClass()))
                    {
                        mCommandDoneEventNotifier.notifyListeners(event);
                    }
                    else
                        if (CommandUnDoneEvent.class.isAssignableFrom(event
                                .getClass()))
                        {
                            mCommandUnDoneEventNotifier.notifyListeners(event);
                        }
                        else
                            if (CommandReDoneEvent.class.isAssignableFrom(event
                                    .getClass()))
                            {
                                mCommandReDoneEventNotifier
                                        .notifyListeners(event);
                            }
        mDataModelEventNotifier.notifyListeners(event);
    }

    /**
     * @hidden
     */
    public <K extends DataModelChangedEvent> void registerListener(
            IEventListener<K> listener, Class<K> klass)
            throws MethodParameterIsNullException
    {
        if (listener == null || klass == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (TransactionStartedEvent.class.isAssignableFrom(klass))
        {
            mTransactionStartedEventNotifier.registerListener(listener, klass);
        }
        else
            if (TransactionCancelledEvent.class.isAssignableFrom(klass))
            {
                mTransactionCancelledEventNotifier.registerListener(listener,
                        klass);
            }
            else
                if (TransactionEndedEvent.class.isAssignableFrom(klass))
                {
                    mTransactionEndedEventNotifier.registerListener(listener,
                            klass);
                }
                else
                    if (CommandDoneEvent.class.isAssignableFrom(klass))
                    {
                        mCommandDoneEventNotifier.registerListener(listener,
                                klass);
                    }
                    else
                        if (CommandUnDoneEvent.class.isAssignableFrom(klass))
                        {
                            mCommandUnDoneEventNotifier.registerListener(
                                    listener, klass);
                        }
                        else
                            if (CommandReDoneEvent.class
                                    .isAssignableFrom(klass))
                            {
                                mCommandReDoneEventNotifier.registerListener(
                                        listener, klass);
                            }
                            else
                            {
                                mDataModelEventNotifier.registerListener(
                                        listener, klass);
                            }
    }

    /**
     * @hidden
     */
    public <K extends DataModelChangedEvent> void unregisterListener(
            IEventListener<K> listener, Class<K> klass)
            throws MethodParameterIsNullException
    {
        if (listener == null || klass == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (TransactionStartedEvent.class.isAssignableFrom(klass))
        {
            mTransactionStartedEventNotifier
                    .unregisterListener(listener, klass);
        }
        else
            if (TransactionCancelledEvent.class.isAssignableFrom(klass))
            {
                mTransactionCancelledEventNotifier.unregisterListener(listener,
                        klass);
            }
            else
                if (TransactionEndedEvent.class.isAssignableFrom(klass))
                {
                    mTransactionEndedEventNotifier.unregisterListener(listener,
                            klass);
                }
                else
                    if (CommandDoneEvent.class.isAssignableFrom(klass))
                    {
                        mCommandDoneEventNotifier.unregisterListener(listener,
                                klass);
                    }
                    else
                        if (CommandUnDoneEvent.class.isAssignableFrom(klass))
                        {
                            mCommandUnDoneEventNotifier.unregisterListener(
                                    listener, klass);
                        }
                        else
                            if (CommandReDoneEvent.class
                                    .isAssignableFrom(klass))
                            {
                                mCommandReDoneEventNotifier.unregisterListener(
                                        listener, klass);
                            }
                            else
                            {
                                mDataModelEventNotifier.unregisterListener(
                                        listener, klass);
                            }
    }

    /**
     * @hidden
     */
    @Override
    protected void xukInAttributes(IXmlDataReader source, IProgressHandler ph)
            throws MethodParameterIsNullException,
            XukDeserializationFailedException, ProgressCancelledException
    {
        /**
         * Does nothing.
         */
    }

    /**
     * @hidden
     */
    @Override
    protected void xukOutAttributes(IXmlDataWriter destination, URI baseUri,
            IProgressHandler ph) throws XukSerializationFailedException,
            MethodParameterIsNullException, ProgressCancelledException
    {
        /**
         * Does nothing.
         */
    }
}
