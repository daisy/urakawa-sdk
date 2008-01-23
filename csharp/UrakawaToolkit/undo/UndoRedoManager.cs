using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using urakawa.xuk;
using urakawa.media.data;
using urakawa.events;
using urakawa.events.undo;

namespace urakawa.undo
{
	/// <summary>
	/// The command manager.
	/// </summary>
	public class UndoRedoManager : WithPresentation, IXukAble, IChangeNotifier
	{
		
		#region Event related members

		/// <summary>
		/// Event fired after the <see cref="UndoRedoManager"/> has changed. 
		/// The event fire before any change specific event 
		/// </summary>
		public event EventHandler<urakawa.events.DataModelChangedEventArgs> changed;
		/// <summary>
		/// Fires the <see cref="changed"/> event 
		/// </summary>
		/// <param name="args">The arguments of the event</param>
		protected void notifyChanged(urakawa.events.DataModelChangedEventArgs args)
		{
			EventHandler<urakawa.events.DataModelChangedEventArgs> d = changed;
			if (d != null) d(this, args);
		}
		/// <summary>
		/// Event fired after a transaction has started
		/// </summary>
		public event EventHandler<urakawa.events.undo.TransactionStartedEventArgs> transactionStarted;
		/// <summary>
		/// Fires the <see cref="transactionStarted"/> event
		/// </summary>
		protected void notifyTransactionStarted()
		{
			EventHandler<urakawa.events.undo.TransactionStartedEventArgs> d = transactionStarted;
			if (d != null) d(this, new TransactionStartedEventArgs(this));
		}
		void this_transactionStarted(object sender, TransactionStartedEventArgs e)
		{
			notifyChanged(e);
		}
		/// <summary>
		/// Event fired after a transaction has ended
		/// </summary>
		public event EventHandler<urakawa.events.undo.TransactionEndedEventArgs> transactionEnded;
		/// <summary>
		/// Fires the <see cref="transactionEnded"/> event
		/// </summary>
		protected void notifyTransactionEnded()
		{
			EventHandler<urakawa.events.undo.TransactionEndedEventArgs> d = transactionEnded;
			if (d != null) d(this, new TransactionEndedEventArgs(this));
		}
		void this_transactionEnded(object sender, TransactionEndedEventArgs e)
		{
			notifyChanged(e);
		}
		/// <summary>
		/// Event fired after a transaction has been cancelled
		/// </summary>
		public event EventHandler<urakawa.events.undo.TransactionCancelledEventArgs> transactionCancelled;
		/// <summary>
		/// Fires the <see cref="transactionCancelled"/> event
		/// </summary>
		protected void notifyTransactionCancelled()
		{
			EventHandler<urakawa.events.undo.TransactionCancelledEventArgs> d = transactionCancelled;
			if (d != null) d(this, new TransactionCancelledEventArgs(this));
		}
		void this_transactionCancelled(object sender, TransactionCancelledEventArgs e)
		{
			notifyChanged(e);
		}
		/// <summary>
		/// Event fired after a command has been done/executed via the <see cref="UndoRedoManager"/>
		/// </summary>
		public event EventHandler<urakawa.events.undo.CommandDoneEventArgs> commandDone;
		/// <summary>
		/// Fires the <see cref="commandDone"/> event
		/// </summary>
		/// <param name="doneCmd">The <see cref="ICommand"/> that was done</param>
		protected void notifyCommandDone(ICommand doneCmd)
		{
			EventHandler<urakawa.events.undo.CommandDoneEventArgs> d = commandDone;
			if (d != null) d(this, new CommandDoneEventArgs(this, doneCmd));
		}
		void this_commandDone(object sender, CommandDoneEventArgs e)
		{
			notifyChanged(e);
		}
		/// <summary>
		/// Event fired after a command has been undone <see cref="UndoRedoManager"/>
		/// </summary>
		public event EventHandler<urakawa.events.undo.CommandUnDoneEventArgs> commandUnDone;
		/// <summary>
		/// Fires the <see cref="commandUnDone"/> event
		/// </summary>
		/// <param name="unDoneCmd">The <see cref="ICommand"/> that was un-done</param>
		protected void notifyCommandUnDone(ICommand unDoneCmd)
		{
			EventHandler<urakawa.events.undo.CommandUnDoneEventArgs> d = commandUnDone;
			if (d != null) d(this, new CommandUnDoneEventArgs(this, unDoneCmd));
		}
		void this_commandUnDone(object sender, CommandUnDoneEventArgs e)
		{
			notifyChanged(e);
		}
		/// <summary>
		/// Event fired after a command has been done/executed via the <see cref="UndoRedoManager"/>
		/// </summary>
		public event EventHandler<urakawa.events.undo.CommandReDoneEventArgs> commandReDone;
		/// <summary>
		/// Fires the <see cref="commandReDone"/> event
		/// </summary>
		/// <param name="reDoneCmd">The <see cref="ICommand"/> that was re-done</param>
		protected void notifyCommandReDone(ICommand reDoneCmd)
		{
			EventHandler<urakawa.events.undo.CommandReDoneEventArgs> d = commandReDone;
			if (d != null) d(this, new CommandReDoneEventArgs(this, reDoneCmd));
		}
		void this_commandReDone(object sender, CommandReDoneEventArgs e)
		{
			notifyChanged(e);
		}

		#endregion



		private Stack<ICommand> mUndoStack;  // stack of commands to exception
		private Stack<ICommand> mRedoStack;  // stack of commands to redo
		private Stack<CompositeCommand> mActiveTransactions;

		/// <summary>
		/// Gets a list of the <see cref="ICommand"/>s currently in the undo stack
		/// </summary>
		/// <returns>The list</returns>
		public List<ICommand> getListOfUndoStackCommands()
		{
			return new List<ICommand>(mUndoStack);
		}

		/// <summary>
		/// Gets a list of the <see cref="ICommand"/>s currently in the redo stack
		/// </summary>
		/// <returns>The list</returns>
		public List<ICommand> getListOfRedoStackCommands()
		{
			return new List<ICommand>(mRedoStack);
		}

		/// <summary>
		/// Gets a list of the <see cref="ICommand"/>s in the currently active transactions.
		/// </summary>
		/// <returns>The list - empty if no transactions are currently active</returns>
		public List<ICommand> getListOfCommandsInCurrentTransactions()
		{
			List<ICommand> res = new List<ICommand>();
			foreach (CompositeCommand trans in mActiveTransactions)
			{
				res.AddRange(trans.getListOfCommands());
			}
			return res;
		}

		/// <summary>
		/// Gets a list of all <see cref="MediaData"/> used by the undo/redo manager associated <see cref="ICommand"/>s,
		/// here a <see cref="ICommand"/> is considered associated with the manager if it is in the undo or redo stacks 
		/// or if it is part of the currently active transaction
		/// </summary>
		/// <returns>The list</returns>
		public List<MediaData> getListOfUsedMediaData()
		{
			List<MediaData> res = new List<MediaData>();
			List<ICommand> commands = new List<ICommand>();
			commands.AddRange(getListOfUndoStackCommands());
			commands.AddRange(getListOfRedoStackCommands());
			commands.AddRange(getListOfCommandsInCurrentTransactions());
			foreach (ICommand cmd in commands)
			{
				foreach (MediaData md in cmd.getListOfUsedMediaData())
				{
					if (!res.Contains(md)) res.Add(md);
				}
			}
			return res;
		}

		/// <summary>
		/// Create an empty command manager.
		/// </summary>
		internal protected UndoRedoManager()
		{
			mUndoStack = new Stack<ICommand>();
			mRedoStack = new Stack<ICommand>();
			mActiveTransactions = new Stack<CompositeCommand>();
			transactionStarted += new EventHandler<TransactionStartedEventArgs>(this_transactionStarted);
			transactionEnded += new EventHandler<TransactionEndedEventArgs>(this_transactionEnded);
			transactionCancelled += new EventHandler<TransactionCancelledEventArgs>(this_transactionCancelled);
			commandDone += new EventHandler<CommandDoneEventArgs>(this_commandDone);
			commandUnDone += new EventHandler<CommandUnDoneEventArgs>(this_commandUnDone);
			commandReDone += new EventHandler<CommandReDoneEventArgs>(this_commandReDone);
		}

		/// <summary>
		/// Get the name of the next exception command.
		/// </summary>
		/// <exception cref="urakawa.exception.CannotUndoException">Thrown when there is no command to exception.</exception>
		public string getUndoShortDescription()
		{
			if (mUndoStack.Count == 0) throw new exception.CannotUndoException("There is no command to exception.");
			return mUndoStack.Peek().getShortDescription();
		}

		/// <summary>
		/// Undo the last executed command.
		/// </summary>
		/// <exception cref="urakawa.exception.CannotUndoException">Thrown when there is no command to undo.</exception>
		public virtual void undo()
		{
			if (isTransactionActive())
			{
				throw new exception.UndoRedoTransactionHasNotEndedException(
					"Can not undo while an transaction is active");
			}
			if (mUndoStack.Count == 0) throw new exception.CannotUndoException("There is no command to undo.");
			mUndoStack.Peek().unExecute();
			ICommand cmd = mUndoStack.Pop();
			mRedoStack.Push(cmd);
			notifyCommandUnDone(cmd);
		}

		/// <summary>
		/// Get the name of the next redo command.
		/// </summary>
		/// <exception cref="urakawa.exception.CannotRedoException">Thrown when there is no command to redo.</exception>
		public string getRedoShortDescription()
		{
			if (mRedoStack.Count == 0) throw new exception.CannotRedoException("There is no command to redo.");
			return mRedoStack.Peek().getShortDescription();
		}

		/// <summary>
		/// Redo the last unexecuted command.
		/// </summary>
		/// <exception cref="urakawa.exception.CannotRedoException">Thrown when there is no command to redo.</exception>
		public virtual void redo()
		{
			if (mRedoStack.Count == 0) throw new exception.CannotRedoException("There is no command to redo.");
			mRedoStack.Peek().execute();
			ICommand cmd = mRedoStack.Pop();
			mUndoStack.Push(cmd);
			notifyCommandReDone(cmd);
		}

		/// <summary>
		/// Execute and register the given command in the exception history and clear the redo history.
		/// </summary>
		/// <param name="command">The command to execute</param>
		/// <exception cref="exception.MethodParameterIsNullException">Thrown when a null command is given.</exception>
		/// <exception cref="exception.IrreversibleCommandDuringActiveUndoRedoTransactionException"></exception>
		public virtual void execute(ICommand command)
		{
			if (command == null) throw new exception.MethodParameterIsNullException("Command cannot be null.");
			pushCommand(command);
			command.execute();
			notifyCommandDone(command);
		}

		/// <summary>
		/// Pushes a <see cref="ICommand"/> into the undo stack or appends it to the currently active transaction, 
		/// if one such exists
		/// </summary>
		/// <param name="command">The command</param>
		/// <exception cref="exception.IrreversibleCommandDuringActiveUndoRedoTransactionException">
		/// When trying to push a irreversible <see cref="ICommand"/> during an active transaction
		/// </exception>
		protected void pushCommand(ICommand command)
		{
			if (isTransactionActive())
			{
				if (!command.canUnExecute())
				{
					throw new exception.IrreversibleCommandDuringActiveUndoRedoTransactionException(
						"Can not execute an irreversible command when a transaction is active");
				}
				mActiveTransactions.Peek().append(command);
			}
			else
			{
				if (command.canUnExecute())
				{
					mUndoStack.Push(command);
					mRedoStack.Clear();
				}
				else
				{
					flushCommands();
				}
			}
		}

		/// <summary>
		/// Return true if the exception is history is non-empty.
		/// </summary>
		public bool canUndo()
		{
			if (isTransactionActive()) return false;
			if (mUndoStack.Count == 0) return false;
			return true;
		}

		/// <summary>
		/// Return true if the redo is history is non-empty.
		/// </summary>
		public bool canRedo()
		{
			if (isTransactionActive()) return false;
			if (mRedoStack.Count == 0) return false;
			return true;
		}

		/// <summary>
		/// Starts a transaction: marks the current level in the history as the point where the transaction begins.
		/// Any following call to <see cref="execute"/> will push the a <see cref="ICommand"/> into the history and execute it normally.
		/// After the call, <see cref="isTransactionActive"/> must return true. 
		/// Transactions can be nested, so programmers must make sure to start and end/cancel transactions in pairs 
		/// (e.g. a call to <see cref="endTransaction"/> for each <see cref="startTransaction"/>).
		/// A transaction can be canceled (rollback), and all <see cref="ICommand"/>s un-executed 
		/// by calling <see cref="cancelTransaction"/>.
		/// </summary>
		/// <param name="shortDesc">
		/// A short human-readable decription of the transaction, 
		/// if <c>null</c> a default short description will be generated based on the short descriptions of the <see cref="ICommand"/>s in the transaction
		/// </param>
		/// <param name="longDesc">
		/// A long human-readable decription of the transaction, 
		/// if <c>null</c> a default long description will be generated based on the long descriptions of the <see cref="ICommand"/>s in the transaction
		/// </param>
		public void startTransaction(string shortDesc, string longDesc)
		{
			CompositeCommand newTrans = getPresentation().getCommandFactory().createCompositeCommand();
			newTrans.setShortDescription(shortDesc);
			newTrans.setLongDescription(longDesc);
			mActiveTransactions.Push(newTrans);
			notifyTransactionStarted();
		}

		/// <summary>
		/// Ends the active transaction: 
		/// Wraps any <see cref="ICommand"/>s executed since the latest <see cref="startTransaction"/> call
		/// in a <see cref="CompositeCommand"/> and pushes this to the undo stack.
		/// </summary>
		public void endTransaction()
		{
			if (!isTransactionActive())
			{
				throw new exception.UndoRedoTransactionIsNotStartedException(
					"Can not end transaction while no is active");
			}
			pushCommand(mActiveTransactions.Pop());
			notifyTransactionEnded();
		}

		/// <summary>
		/// Cancels the active transaction:
		/// Any <see cref="ICommand"/>s executed since the latest <see cref="startTransaction"/> call
		/// are un-executed
		/// </summary>
		public void cancelTransaction()
		{
			if (!isTransactionActive())
			{
				throw new exception.UndoRedoTransactionIsNotStartedException(
					"Can not end transaction while no is active");
			}
			mActiveTransactions.Pop().unExecute();
			notifyTransactionCancelled();
		}

		/// <summary>
		/// Gets a <see cref="bool"/> indicating is there is a currently active undo/redo transaction
		/// </summary>
		/// <returns></returns>
		public bool isTransactionActive()
		{
			return (mActiveTransactions.Count>0);
		}

		/// <summary>
		/// Clears the manager of commands, clearing the undo and redo stacks
		/// </summary>
		public void flushCommands()
		{
			if (isTransactionActive())
			{
				throw new exception.UndoRedoTransactionHasNotEndedException("Can not flush command while a transaction is active");
			}
			mUndoStack.Clear();
			mRedoStack.Clear();
		}


		#region IXUKAble members

		protected override void clear()
		{
			while (isTransactionActive())
			{
				endTransaction();
			}
			flushCommands();
			base.clear();
		}

		/// <summary>
		/// Reads a child of a UndoRedoManager xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected override void xukInChild(XmlReader source)
		{
			bool readItem = false;

			if (source.NamespaceURI == ToolkitSettings.XUK_NS)
			{
				switch (source.LocalName)
				{
					case "mUndoStack":
						xukInCommandStack<ICommand>(source, mUndoStack);
						break;
					case "mRedoStack":
						xukInCommandStack<ICommand>(source, mRedoStack);
						break;
					case "mActiveTransactions":
						xukInCommandStack<CompositeCommand>(source, mActiveTransactions);
						break;
				}
			}


			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past unknown child 
			}
		}

		private void xukInCommandStack<T>(XmlReader source, Stack<T> stack) where T : ICommand
		{
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						ICommand cmd = getPresentation().getCommandFactory().createCommand(
							source.LocalName, source.NamespaceURI);
						if (!(cmd is T)) 
						{
							throw new exception.XukException(
								String.Format("Could not create a {2} matching XUK QName {1}:{0}", source.LocalName, source.NamespaceURI, typeof(T).Name));
						}
						stack.Push((T)cmd);
						cmd.xukIn(source);
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
		protected override void xukOutChildren(XmlWriter destination, Uri baseUri)
		{
			destination.WriteStartElement("mUndoStack", ToolkitSettings.XUK_NS);
			foreach (ICommand cmd in mUndoStack)
			{
				cmd.xukOut(destination, baseUri);
			}
			destination.WriteEndElement();
			destination.WriteStartElement("mRedoStack", ToolkitSettings.XUK_NS);
			foreach (ICommand cmd in mRedoStack)
			{
				cmd.xukOut(destination, baseUri);
			}
			destination.WriteEndElement();
			destination.WriteStartElement("mActiveTransactions");
			foreach (CompositeCommand cmd in mActiveTransactions)
			{
				cmd.xukOut(destination, baseUri);
			}
			destination.WriteEndElement();
			base.xukOutChildren(destination, baseUri);
		}

		#endregion

	}
}
