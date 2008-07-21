package org.daisy.urakawa.undo;

import java.net.URI;
import java.util.LinkedList;
import java.util.List;
import java.util.Stack;

import org.daisy.urakawa.AbstractXukAbleWithPresentation;
import org.daisy.urakawa.command.ICommand;
import org.daisy.urakawa.command.CommandCannotExecuteException;
import org.daisy.urakawa.command.CommandCannotUnExecuteException;
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
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.IMediaData;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.xuk.IXukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public final class UndoRedoManager extends AbstractXukAbleWithPresentation implements
        IUndoRedoManager
{
    private Stack<ICommand> mUndoStack;
    private Stack<ICommand> mRedoStack;
    private Stack<ICompositeCommand> mActiveTransactions;

    /**
	 * 
	 */
    public UndoRedoManager()
    {
        mUndoStack = new Stack<ICommand>();
        mRedoStack = new Stack<ICommand>();
        mActiveTransactions = new Stack<ICompositeCommand>();
    }

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

    public List<ICommand> getListOfUndoStackCommands()
    {
        return new LinkedList<ICommand>(mUndoStack);
    }

    public List<ICommand> getListOfRedoStackCommands()
    {
        return new LinkedList<ICommand>(mRedoStack);
    }

    public List<ICommand> getListOfCommandsInCurrentTransactions()
    {
        List<ICommand> res = new LinkedList<ICommand>();
        for (ICompositeCommand trans : mActiveTransactions)
        {
            res.addAll(trans.getListOfCommands());
        }
        return res;
    }

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

    public boolean canUndo()
    {
        if (isTransactionActive())
            return false;
        if (mUndoStack.size() == 0)
            return false;
        return true;
    }

    public boolean canRedo()
    {
        if (isTransactionActive())
            return false;
        if (mRedoStack.size() == 0)
            return false;
        return true;
    }

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
        try
        {
            newTrans = getPresentation().getCommandFactory()
                    .createCompositeCommand();
        }
        catch (IsNotInitializedException e1)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e1);
        }
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

    public boolean isTransactionActive()
    {
        return (mActiveTransactions.size() > 0);
    }

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
                    catch (IsNotInitializedException e1)
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

    @SuppressWarnings("unused")
    @Override
    protected void xukInAttributes(IXmlDataReader source, IProgressHandler ph)
            throws MethodParameterIsNullException,
            XukDeserializationFailedException, ProgressCancelledException
    {
        /**
         * Does nothing.
         */
    }

    @SuppressWarnings("unused")
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
