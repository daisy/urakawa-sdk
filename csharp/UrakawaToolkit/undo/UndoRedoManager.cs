using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using urakawa.xuk;

namespace urakawa.undo
{
	/// <summary>
	/// The command manager.
	/// </summary>
	public class UndoRedoManager : WithPresentation, IXukAble
	{
		private Stack<ICommand> mUndoStack;  // stack of commands to exception
		private Stack<ICommand> mRedoStack;  // stack of commands to redo
		private Stack<CompositeCommand> mActiveTransactions;

		/// <summary>
		/// Create an empty command manager.
		/// </summary>
		public UndoRedoManager()
		{
			mUndoStack = new Stack<ICommand>();
			mRedoStack = new Stack<ICommand>();
			mActiveTransactions = new Stack<CompositeCommand>();
		}

		/// <summary>
		/// Get the name of the next exception command.
		/// </summary>
		/// <exception cref="urakawa.exception.CannotUndoException">Thrown when there is no command to exception.</exception>
		public string getUndoShortDescription()
		{
			if (mUndoStack.Count == 0) throw new exception.CannotUndoException("There is no command to exception.");
			return mUndoStack.Peek().getLongDescription();
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
			mRedoStack.Push(mUndoStack.Pop());
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
			mUndoStack.Push(mRedoStack.Pop());
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
			mRedoStack.Clear();
			command.execute();
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

		public void startTransaction()
		{
			mActiveTransactions.Push(getPresentation().getCommandFactory().createCompositeCommand());
		}

		public void endTransaction()
		{
			if (!isTransactionActive())
			{
				throw new exception.UndoRedoTransactionIsNotStartedException(
					"Can not end transaction while no is active");
			}
			mUndoStack.Push(mActiveTransactions.Pop());
		}

		public void cancelTransaction()
		{
			if (!isTransactionActive())
			{
				throw new exception.UndoRedoTransactionIsNotStartedException(
					"Can not end transaction while no is active");
			}
			mActiveTransactions.Pop().unExecute();
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

		/// <summary>
		/// Reads the <see cref="UndoRedoManager"/> from a UndoRedoManager xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		public void XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element)
			{
				throw new exception.XukException("Can not read UndoRedoManager from a non-element node");
			}
			try
			{
				mUndoStack.Clear();
				mRedoStack.Clear();
				mActiveTransactions.Clear();
				XukInAttributes(source);
				if (!source.IsEmptyElement)
				{
					while (source.Read())
					{
						if (source.NodeType == XmlNodeType.Element)
						{
							XukInChild(source);
						}
						else if (source.NodeType == XmlNodeType.EndElement)
						{
							break;
						}
						if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
					}
				}

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukIn of UndoRedoManager: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a UndoRedoManager xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInAttributes(XmlReader source)
		{

		}

		/// <summary>
		/// Reads a child of a UndoRedoManager xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInChild(XmlReader source)
		{
			bool readItem = false;

			if (source.NamespaceURI == ToolkitSettings.XUK_NS)
			{
				switch (source.LocalName)
				{
					case "mUndoStack":
						XukInCommandStack<ICommand>(source, mUndoStack);
						break;
					case "mRedoStack":
						XukInCommandStack<ICommand>(source, mRedoStack);
						break;
					case "mActiveTransactions":
						XukInCommandStack<CompositeCommand>(source, mActiveTransactions);
						break;
				}
			}


			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past unknown child 
			}
		}

		private void XukInCommandStack<T>(XmlReader source, Stack<T> stack) where T : ICommand
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
						cmd.XukIn(source);
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
		/// Write a UndoRedoManager element to a XUK file representing the <see cref="UndoRedoManager"/> instance
		/// </summary>
		/// <param localName="destination">The destination <see cref="XmlWriter"/></param>
		public void XukOut(XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not XukOut to a null XmlWriter");
			}

			try
			{
				destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
				XukOutAttributes(destination);
				XukOutChildren(destination);
				destination.WriteEndElement();

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukOut of UndoRedoManager: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a UndoRedoManager element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutAttributes(XmlWriter destination)
		{

		}

		/// <summary>
		/// Write the child elements of a UndoRedoManager element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutChildren(XmlWriter destination)
		{
			destination.WriteElementString("mUndoStack", ToolkitSettings.XUK_NS);
			foreach (ICommand cmd in mUndoStack)
			{
				cmd.XukOut(destination);
			}
			destination.WriteEndElement();
			destination.WriteElementString("mRedoStack", ToolkitSettings.XUK_NS);
			foreach (ICommand cmd in mRedoStack)
			{
				cmd.XukOut(destination);
			}
			destination.WriteEndElement();
			destination.WriteStartElement("mActiveTransactions");
			foreach (CompositeCommand cmd in mActiveTransactions)
			{
				cmd.XukOut(destination);
			}
			destination.WriteEndElement();
		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="UndoRedoManager"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="UndoRedoManager"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

	}
}
