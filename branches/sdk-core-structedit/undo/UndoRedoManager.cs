using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using AudioLib;
using urakawa.command;
using urakawa.progress;
using urakawa.media.data;
using urakawa.events.undo;
using urakawa.xuk;

namespace urakawa.undo
{
    [XukNameUglyPrettyAttribute("udoRdoMan", "UndoRedoManager")]
    public sealed class UndoRedoManager : XukAble, IUsingMediaData //IChangeNotifier
    {
        public sealed class Hooker
        {
            public interface Host
            {
                void OnUndoRedoManagerChanged(UndoRedoManagerEventArgs eventt, bool isTransactionActive, bool done, Command command);
            }

            private UndoRedoManager m_UndoRedoManager = null;
            private Host m_Host = null;
            private bool m_notifyLiveTransaction = false;

            public Hooker(UndoRedoManager undoRedoManager, Host host, bool notifyLiveTransaction)
            {
                m_UndoRedoManager = undoRedoManager;
                m_Host = host;
                m_notifyLiveTransaction = notifyLiveTransaction;

                ReHook();
            }

            public void ReHook()
            {
                if (Hooked)
                {
#if DEBUG
                    Debugger.Break();
#endif
                    return;
                }

                m_UndoRedoManager.CommandDone += OnUndoRedoManagerChanged;
                m_UndoRedoManager.CommandReDone += OnUndoRedoManagerChanged;
                m_UndoRedoManager.CommandUnDone += OnUndoRedoManagerChanged;
                m_UndoRedoManager.TransactionEnded += OnUndoRedoManagerChanged;
                m_UndoRedoManager.TransactionCancelled += OnUndoRedoManagerChanged;

                Hooked = true;
            }

            public void UnHook()
            {
                if (!Hooked)
                {
#if DEBUG
                    Debugger.Break();
#endif
                    return;
                }

                m_UndoRedoManager.CommandDone -= OnUndoRedoManagerChanged;
                m_UndoRedoManager.CommandReDone -= OnUndoRedoManagerChanged;
                m_UndoRedoManager.CommandUnDone -= OnUndoRedoManagerChanged;
                m_UndoRedoManager.TransactionEnded -= OnUndoRedoManagerChanged;
                m_UndoRedoManager.TransactionCancelled -= OnUndoRedoManagerChanged;

                Hooked = false;
            }

            private bool m_hooked = false;
            public bool Hooked
            {
                get
                {
                    return m_hooked;
                }
                set
                {
                    m_hooked = value;
                }
            }

            private void OnUndoRedoManagerChanged_CompositeCommandDispatch(UndoRedoManagerEventArgs eventt, bool isTransactionActive, bool done, Command command)
            {
                if (command is CompositeCommand)
                {
                    CompositeCommand compo = (CompositeCommand)command;
                    IEnumerable<Command> enumerable = done
                        ? compo.ChildCommands.ContentsAs_Enumerable
                        : compo.ChildCommands.ContentsAs_YieldEnumerableReversed;
                    foreach (Command childCommand in enumerable)
                    {
                        m_Host.OnUndoRedoManagerChanged(eventt, isTransactionActive, done, childCommand);
                    }
                }
                else
                {
                    m_Host.OnUndoRedoManagerChanged(eventt, isTransactionActive, done, command);
                }
            }

            public void OnUndoRedoManagerChanged(object sender, UndoRedoManagerEventArgs eventt)
            {
                if (!(eventt is DoneEventArgs
                    || eventt is UnDoneEventArgs
                    || eventt is ReDoneEventArgs
                    || eventt is TransactionEndedEventArgs
                    || eventt is TransactionCancelledEventArgs
                    ))
                {
#if DEBUG
                    Debugger.Break();
#endif
                    return;
                }

                bool isTransactionActive = m_UndoRedoManager.IsTransactionActive;
                if (isTransactionActive)
                {
                    DebugFix.Assert(eventt is DoneEventArgs || eventt is TransactionEndedEventArgs);

                    if (eventt is DoneEventArgs && !m_notifyLiveTransaction)
                    {
                        // we do not process each DoneEventArgs, instead we wait for the final resulting CompositeCommand
                        return;
                    }
                    //if (eventt is TransactionEndedEventArgs && m_notifyLiveTransaction)
                    //{
                    //    // we do not process the final resulting CompositeCommand, as we have already notified each DoneEventArgs
                    //    return;
                    //}
                }

                bool done = eventt is DoneEventArgs || eventt is ReDoneEventArgs || eventt is TransactionEndedEventArgs;
                DebugFix.Assert(done == !(eventt is UnDoneEventArgs || eventt is TransactionCancelledEventArgs));

                OnUndoRedoManagerChanged_CompositeCommandDispatch(eventt, isTransactionActive, done, eventt.Command);
            }
        }

        public Hooker Hook(Hooker.Host host, bool notifyLiveTransaction)
        {
            return new Hooker(this, host, notifyLiveTransaction);
        }

        public override bool PrettyFormat
        {
            set { throw new NotImplementedException("PrettyFormat"); }
            get
            {
                return XukAble.m_PrettyFormat_STATIC;
            }
        }

        class DummyCommand : Command
        {
            public override bool CanExecute
            {
                get { throw new NotImplementedException(); }
            }

            public override void Execute()
            {
                throw new NotImplementedException();
            }

            public override void UnExecute()
            {
                throw new NotImplementedException();
            }

            public override bool CanUnExecute
            {
                get { throw new NotImplementedException(); }
            }
        }
        

        private Presentation mPresentation;

        /// <summary>
        /// Gets the <see cref="Presentation"/> associated with <c>this</c>
        /// </summary>
        /// <returns>The owning <see cref="Presentation"/></returns>
        public Presentation Presentation
        {
            get
            {
                return mPresentation;
            }
        }

        public UndoRedoManager(Presentation pres)
        {
            mPresentation = pres;

            mUndoStack = new Stack<Command>();
            mRedoStack = new Stack<Command>();
            mActiveTransactions = new Stack<CompositeCommand>();

            m_DirtyMarkerDummyCommand = new DummyCommand();
            m_DirtyMarkerCommand = m_DirtyMarkerDummyCommand;

            //TransactionStarted += this_transactionStarted;
            //TransactionEnded += this_transactionEnded;
            //TransactionCancelled += this_transactionCancelled;

            //CommandDone += this_commandDone;
            //CommandUnDone += this_commandUnDone;
            //CommandReDone += this_commandReDone;
        }
        #region Event related members

        /// <summary>
        /// Event fired after the <see cref="UndoRedoManager"/> has changed. 
        /// The event fire before any change specific event 
        /// </summary>
        //public event EventHandler<DataModelChangedEventArgs> Changed;

        /// <summary>
        /// Fires the <see cref="Changed"/> event 
        /// </summary>
        /// <param name="args">The arguments of the event</param>
        //private void NotifyChanged(DataModelChangedEventArgs args)
        //{
        //    EventHandler<DataModelChangedEventArgs> d = Changed;
        //    if (d != null) d(this, args);
        //}

        /// <summary>
        /// Event fired after a transaction has started
        /// </summary>
        public event EventHandler<TransactionStartedEventArgs> TransactionStarted;

        /// <summary>
        /// Fires the <see cref="TransactionStarted"/> event
        /// </summary>
        private void NotifyTransactionStarted(CompositeCommand command)
        {
            EventHandler<TransactionStartedEventArgs> d = TransactionStarted;
            if (d != null) d(this, new TransactionStartedEventArgs(this, command));
        }

        //private void this_transactionStarted(object sender, TransactionStartedEventArgs e)
        //{
        //    NotifyChanged(e);
        //}

        /// <summary>
        /// Event fired after a transaction has ended
        /// </summary>
        public event EventHandler<TransactionEndedEventArgs> TransactionEnded;

        /// <summary>
        /// Fires the <see cref="TransactionEnded"/> event
        /// </summary>
        private void NotifyTransactionEnded(CompositeCommand command)
        {
            EventHandler<TransactionEndedEventArgs> d = TransactionEnded;
            if (d != null) d(this, new TransactionEndedEventArgs(this, command));
        }

        //private void this_transactionEnded(object sender, TransactionEndedEventArgs e)
        //{
        //    NotifyChanged(e);
        //}

        /// <summary>
        /// Event fired after a transaction has been cancelled
        /// </summary>
        public event EventHandler<TransactionCancelledEventArgs> TransactionCancelled;

        /// <summary>
        /// Fires the <see cref="TransactionCancelled"/> event
        /// </summary>
        private void NotifyTransactionCancelled(CompositeCommand command)
        {
            EventHandler<TransactionCancelledEventArgs> d = TransactionCancelled;
            if (d != null) d(this, new TransactionCancelledEventArgs(this, command));
        }

        //private void this_transactionCancelled(object sender, TransactionCancelledEventArgs e)
        //{
        //    NotifyChanged(e);
        //}

        /// <summary>
        /// Event fired after a command has been done/executed via the <see cref="UndoRedoManager"/>
        /// </summary>
        public event EventHandler<DoneEventArgs> CommandDone;

        /// <summary>
        /// Fires the <see cref="CommandDone"/> event
        /// </summary>
        /// <param name="doneCmd">The <see cref="Command"/> that was done</param>
        private void NotifyCommandDone(Command doneCmd)
        {
            EventHandler<DoneEventArgs> d = CommandDone;
            if (d != null) d(this, new DoneEventArgs(this, doneCmd));
        }

        //private void this_commandDone(object sender, DoneEventArgs e)
        //{
        //    NotifyChanged(e);
        //}

        /// <summary>
        /// Event fired after a command has been undone <see cref="UndoRedoManager"/>
        /// </summary>
        public event EventHandler<UnDoneEventArgs> CommandUnDone;

        /// <summary>
        /// Fires the <see cref="CommandUnDone"/> event
        /// </summary>
        /// <param name="unDoneCmd">The <see cref="Command"/> that was un-done</param>
        private void NotifyCommandUnDone(Command unDoneCmd)
        {
            EventHandler<UnDoneEventArgs> d = CommandUnDone;
            if (d != null) d(this, new UnDoneEventArgs(this, unDoneCmd));
        }

        //private void this_commandUnDone(object sender, UnDoneEventArgs e)
        //{
        //    NotifyChanged(e);
        //}

        /// <summary>
        /// Event fired after a command has been done/executed via the <see cref="UndoRedoManager"/>
        /// </summary>
        public event EventHandler<ReDoneEventArgs> CommandReDone;

        /// <summary>
        /// Fires the <see cref="CommandReDone"/> event
        /// </summary>
        /// <param name="reDoneCmd">The <see cref="Command"/> that was re-done</param>
        private void NotifyCommandReDone(Command reDoneCmd)
        {
            EventHandler<ReDoneEventArgs> d = CommandReDone;
            if (d != null) d(this, new ReDoneEventArgs(this, reDoneCmd));
        }

        //private void this_commandReDone(object sender, ReDoneEventArgs e)
        //{
        //    NotifyChanged(e);
        //}

        #endregion

        private Stack<Command> mUndoStack; // stack of commands to exception
        private Stack<Command> mRedoStack; // stack of commands to redo
        private Stack<CompositeCommand> mActiveTransactions;

        private readonly Command m_DirtyMarkerDummyCommand;

        private Command m_DirtyMarkerCommand;

        public void SetDirtyMarker()
        {
            if (mUndoStack.Count == 0) m_DirtyMarkerCommand = m_DirtyMarkerDummyCommand;
            else m_DirtyMarkerCommand = mUndoStack.Peek();
        }

        public bool IsOnDirtyMarker()
        {
            if (mUndoStack.Count == 0)
            {
                return m_DirtyMarkerCommand == m_DirtyMarkerDummyCommand;
            }
            return mUndoStack.Peek() == m_DirtyMarkerCommand;
        }

        /// <summary>
        /// Gets a list of all <see cref="MediaData"/> used by the undo/redo manager associated <see cref="Command"/>s,
        /// here a <see cref="Command"/> is considered associated with the manager if it is in the undo or redo stacks 
        /// or if it is part of the currently active transaction
        /// </summary>
        /// <returns>The list</returns>
        public IEnumerable<MediaData> UsedMediaData
        {
            get
            {
                //List<MediaData> res = new List<MediaData>();
                List<Command> commands = new List<Command>();
                commands.AddRange(mUndoStack);
                commands.AddRange(mRedoStack);

                foreach (CompositeCommand trans in mActiveTransactions)
                {
                    commands.AddRange(trans.ChildCommands.ContentsAs_Enumerable);
                }

                foreach (Command cmd in commands)
                {
                    foreach (MediaData md in cmd.UsedMediaData)
                    {
                        yield return md; // the duplicates don't matter.
                        //if (!res.Contains(md)) res.Add(md);
                    }
                }
                yield break;
                //return res;
            }
        }

        /// <summary>
        /// Get the name of the next exception command.
        /// </summary>
        /// <exception cref="urakawa.exception.CannotUndoException">Thrown when there is no command to exception.</exception>
        public string UndoShortDescription
        {
            get
            {
                if (mUndoStack.Count == 0) throw new exception.CannotUndoException("There is no command to exception.");
                return mUndoStack.Peek().ShortDescription;
            }
        }

        /// <summary>
        /// Undo the last executed command.
        /// </summary>
        /// <exception cref="urakawa.exception.CannotUndoException">Thrown when there is no command to undo.</exception>
        public void Undo()
        {
            if (IsTransactionActive)
            {
                throw new exception.UndoRedoTransactionHasNotEndedException(
                    "Can not undo while an transaction is active");
            }
            if (mUndoStack.Count == 0) throw new exception.CannotUndoException("There is no command to undo.");
            mUndoStack.Peek().UnExecute();
            Command cmd = mUndoStack.Pop();
            mRedoStack.Push(cmd);
            NotifyCommandUnDone(cmd);
        }

        /// <summary>
        /// Get the name of the next redo command.
        /// </summary>
        /// <exception cref="urakawa.exception.CannotRedoException">Thrown when there is no command to redo.</exception>
        public string RedoShortDescription
        {
            get
            {
                if (mRedoStack.Count == 0) throw new exception.CannotRedoException("There is no command to redo.");
                return mRedoStack.Peek().ShortDescription;
            }
        }

        /// <summary>
        /// Redo the last unexecuted command.
        /// </summary>
        /// <exception cref="urakawa.exception.CannotRedoException">Thrown when there is no command to redo.</exception>
        public void Redo()
        {
            if (mRedoStack.Count == 0) throw new exception.CannotRedoException("There is no command to redo.");
            mRedoStack.Peek().Execute();
            Command cmd = mRedoStack.Pop();
            mUndoStack.Push(cmd);
            NotifyCommandReDone(cmd);
        }

        /// <summary>
        /// Execute and register the given command in the exception history and clear the redo history.
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <exception cref="exception.MethodParameterIsNullException">Thrown when a null command is given.</exception>
        /// <exception cref="exception.IrreversibleCommandDuringActiveUndoRedoTransactionException"></exception>
        public void Execute(Command command)
        {
            if (command == null) throw new exception.MethodParameterIsNullException("Command cannot be null.");
            pushCommand(command);
            command.Execute();
            NotifyCommandDone(command);
        }

        /// <summary>
        /// Pushes a <see cref="Command"/> into the undo stack or appends it to the currently active transaction, 
        /// if one such exists
        /// </summary>
        /// <param name="command">The command</param>
        /// <exception cref="exception.IrreversibleCommandDuringActiveUndoRedoTransactionException">
        /// When trying to push a irreversible <see cref="Command"/> during an active transaction
        /// </exception>
        private void pushCommand(Command command)
        {
            if (IsTransactionActive)
            {
                if (!command.CanUnExecute)
                {
                    throw new exception.IrreversibleCommandDuringActiveUndoRedoTransactionException(
                        "Can not execute an irreversible command when a transaction is active");
                }
                ObjectListProvider<Command> list = mActiveTransactions.Peek().ChildCommands;
                list.Insert(list.Count, command);
            }
            else
            {
                if (command.CanUnExecute)
                {
                    mUndoStack.Push(command);

                    if (mRedoStack.Contains(m_DirtyMarkerCommand))
                    {
                        m_DirtyMarkerCommand = null;
                    }
                    mRedoStack.Clear();
                }
                else
                {
                    FlushCommands();
                }
            }
        }

        /// <summary>
        /// Return true if the exception is history is non-empty.
        /// </summary>
        public bool CanUndo
        {
            get
            {
                if (IsTransactionActive) return false;
                if (mUndoStack.Count == 0) return false;
                return true;
            }
        }

        /// <summary>
        /// Return true if the redo is history is non-empty.
        /// </summary>
        public bool CanRedo
        {
            get
            {
                if (IsTransactionActive) return false;
                if (mRedoStack.Count == 0) return false;
                return true;
            }
        }

        /// <summary>
        /// Starts a transaction: marks the current level in the history as the point where the transaction begins.
        /// Any following call to <see cref="Execute"/> will push the a <see cref="Command"/> into the history and execute it normally.
        /// After the call, <see cref="IsTransactionActive"/> must return true. 
        /// Transactions can be nested, so programmers must make sure to start and end/cancel transactions in pairs 
        /// (e.g. a call to <see cref="EndTransaction"/> for each <see cref="StartTransaction"/>).
        /// A transaction can be canceled (rollback), and all <see cref="Command"/>s un-executed 
        /// by calling <see cref="CancelTransaction"/>.
        /// </summary>
        /// <param name="shortDesc">
        /// A short human-readable decription of the transaction, 
        /// if <c>null</c> a default short description will be generated based on the short descriptions of the <see cref="Command"/>s in the transaction
        /// </param>
        /// <param name="longDesc">
        /// A long human-readable decription of the transaction, 
        /// if <c>null</c> a default long description will be generated based on the long descriptions of the <see cref="Command"/>s in the transaction
        /// </param>
        public void StartTransaction(string shortDesc, string longDesc)
        {
#if DEBUG
            if (IsTransactionActive)
            {
                Debugger.Break();
            }
#endif
            CompositeCommand newTrans = Presentation.CommandFactory.CreateCompositeCommand();
            newTrans.ShortDescription = shortDesc;
            newTrans.LongDescription = longDesc;
            mActiveTransactions.Push(newTrans);
            NotifyTransactionStarted(newTrans);
        }

        public bool IsTransactionEmpty
        {
            get
            {
                if (!IsTransactionActive)
                {
                    throw new exception.UndoRedoTransactionIsNotStartedException(
                        "Can not probe transaction while no is active");
                }
                CompositeCommand command = mActiveTransactions.Peek();
                return command.ChildCommands.Count <= 0;
            }
        }

        /// <summary>
        /// Ends the active transaction: 
        /// Wraps any <see cref="Command"/>s executed since the latest <see cref="StartTransaction"/> call
        /// in a <see cref="CompositeCommand"/> and pushes this to the undo stack.
        /// </summary>
        public void EndTransaction()
        {
            if (!IsTransactionActive)
            {
                throw new exception.UndoRedoTransactionIsNotStartedException(
                    "Can not end transaction while no is active");
            }
            CompositeCommand command = mActiveTransactions.Pop();
            if (command.ChildCommands.Count > 0)
            {
                if (command.ChildCommands.Count == 1)
                {
                    pushCommand(command.ChildCommands.Get(0));
                }
                else
                {
                    for (int i = 0; i < command.ChildCommands.Count; i++)
                    {
                        Command childCommand = command.ChildCommands.Get(i);
                        childCommand.TransactionIndex = i;
                        childCommand.TransactionTotalCount = command.ChildCommands.Count;
                    }
                    pushCommand(command);
                }
            }
            NotifyTransactionEnded(command);

            //if (command.ChildCommands.Count == 0)
            //{
            //    NotifyTransactionCancelled(command);
            //}
            //else
            //{
            //    NotifyTransactionEnded(command);
            //}
        }

        /// <summary>
        /// Cancels the active transaction:
        /// Any <see cref="Command"/>s executed since the latest <see cref="StartTransaction"/> call
        /// are un-executed
        /// </summary>
        public void CancelTransaction()
        {
            if (!IsTransactionActive)
            {
                throw new exception.UndoRedoTransactionIsNotStartedException(
                    "Can not end transaction while no is active");
            }

            CompositeCommand command = mActiveTransactions.Pop();
            command.UnExecute();

            //CompositeCommand command = mActiveTransactions.Pop();
            //if (command.ChildCommands.Count > 0)
            //{
            //    command.UnExecute();
            //}
            NotifyTransactionCancelled(command);
        }

        /// <summary>
        /// Gets a <see cref="bool"/> indicating is there is a currently active undo/redo transaction
        /// </summary>
        /// <returns></returns>
        public bool IsTransactionActive
        {
            get { return (mActiveTransactions.Count > 0); }
        }

        /// <summary>
        /// Clears the manager of commands, clearing the undo and redo stacks
        /// </summary>
        public void FlushCommands()
        {
            if (IsTransactionActive)
            {
                throw new exception.UndoRedoTransactionHasNotEndedException(
                    "Can not flush command while a transaction is active");
            }
            if (mUndoStack.Contains(m_DirtyMarkerCommand))
            {
                m_DirtyMarkerCommand = null;
            }
            mUndoStack.Clear();

            if (mRedoStack.Contains(m_DirtyMarkerCommand))
            {
                m_DirtyMarkerCommand = null;
            }
            mRedoStack.Clear();
        }

        #region IXUKAble members

        /// <summary>
        /// Clearing the <see cref="UndoRedoManager"/>, killing all active transactions
        /// and flushing <see cref="Command"/>s from the undo and redo stacks
        /// </summary>
        protected override void Clear()
        {
            while (IsTransactionActive)
            {
                EndTransaction();
            }
            FlushCommands();
            base.Clear();
        }

        /// <summary>
        /// Reads a child of a UndoRedoManager xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            bool readItem = false;

            if (source.NamespaceURI == XukAble.XUK_NS)
            {
                readItem = true;
                if (source.LocalName == XukStrings.UndoStack)
                {
                    XukInCommandStack(source, mUndoStack, handler);
                }
                else if (source.LocalName == XukStrings.RedoStack)
                {
                    XukInCommandStack(source, mRedoStack, handler);
                }
                else if (source.LocalName == XukStrings.ActiveTransactions)
                {
                    XukInCommandStack(source, mActiveTransactions, handler);
                }
                else
                {
                    readItem = false;
                }
            }


            if (!(readItem || source.IsEmptyElement))
            {
                source.ReadSubtree().Close(); //Read past unknown child 
            }
        }

        private void XukInCommandStack<T>(XmlReader source, Stack<T> stack, IProgressHandler handler) where T : Command
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        Command cmd = Presentation.CommandFactory.Create(source.LocalName, source.NamespaceURI);
                        if (!(cmd is T))
                        {
                            throw new exception.XukException(
                                String.Format("Could not create a {2} matching XUK QName {1}:{0}", source.LocalName,
                                              source.NamespaceURI, typeof(T).Name));
                        }
                        cmd.XukIn(source, handler);
                        stack.Push((T)cmd);
                    }
                    else if (source.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }
                    if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
                }
            }
        }

        /// <summary>
        /// Write the child elements of a UndoRedoManager element.
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            destination.WriteStartElement(XukStrings.UndoStack, XukAble.XUK_NS);
            foreach (Command cmd in mUndoStack)
            {
                cmd.XukOut(destination, baseUri, handler);
            }
            destination.WriteEndElement();
            destination.WriteStartElement(XukStrings.RedoStack, XukAble.XUK_NS);
            foreach (Command cmd in mRedoStack)
            {
                cmd.XukOut(destination, baseUri, handler);
            }
            destination.WriteEndElement();
            destination.WriteStartElement(XukStrings.ActiveTransactions);
            foreach (CompositeCommand cmd in mActiveTransactions)
            {
                cmd.XukOut(destination, baseUri, handler);
            }
            destination.WriteEndElement();
            base.XukOutChildren(destination, baseUri, handler);
        }

        #endregion

    }
}