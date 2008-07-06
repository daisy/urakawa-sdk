using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.media.data;
using urakawa.progress;
using CommandAddedEventArgs = urakawa.events.command.CommandAddedEventArgs;
using ExecutedEventArgs = urakawa.events.command.ExecutedEventArgs;
using UnExecutedEventArgs = urakawa.events.command.UnExecutedEventArgs;

namespace urakawa.command
{
    /// <summary>
    /// A composite command made of a series of sub commands. Useful for merging small commands into one such as:
    /// user typing text letter by letter (the exception/redo would work on full word or sentence, not for each character.)
    /// </summary>
    public class CompositeCommand : WithPresentation, ICommand
    {
        #region Event related members

        /// <summary>
        /// Event fired after the <see cref="CompositeCommand"/> has changed. 
        /// The event fire before any change specific event 
        /// </summary>
        public event EventHandler<urakawa.events.DataModelChangedEventArgs> Changed;

        /// <summary>
        /// Fires the <see cref="Changed"/> event 
        /// </summary>
        /// <param name="args">The arguments of the event</param>
        protected void NotifyChanged(urakawa.events.DataModelChangedEventArgs args)
        {
            EventHandler<urakawa.events.DataModelChangedEventArgs> d = Changed;
            if (d != null) d(this, args);
        }

        /// <summary>
        /// Event fired after a <see cref="ICommand"/> has been added to the <see cref="CompositeCommand"/>
        /// </summary>
        public event EventHandler<CommandAddedEventArgs> CommandAdded;

        /// <summary>
        /// Fires the <see cref="CommandAdded"/> event
        /// </summary>
        /// <param name="addedCmd">
        /// The <see cref="ICommand"/> that was added
        /// </param>
        /// <param name="index">The index of the added <see cref="ICommand"/></param>
        protected void NotifyCommandAdded(ICommand addedCmd, int index)
        {
            EventHandler<CommandAddedEventArgs> d = CommandAdded;
            if (d != null) d(this, new CommandAddedEventArgs(this, addedCmd, index));
        }

        /// <summary>
        /// Event fired after the <see cref="CompositeCommand"/> has been executed
        /// </summary>
        public event EventHandler<ExecutedEventArgs> Executed;

        /// <summary>
        /// Fires the <see cref="Executed"/> event
        /// </summary>
        protected void NotifyExecuted()
        {
            EventHandler<ExecutedEventArgs> d = Executed;
            if (d != null) d(this, new ExecutedEventArgs(this));
        }

        /// <summary>
        /// Event fired after the <see cref="CompositeCommand"/> has been un-executed
        /// </summary>
        public event EventHandler<UnExecutedEventArgs> UnExecuted;

        /// <summary>
        /// Fires the <see cref="UnExecuted"/> event
        /// </summary>
        protected void NotifyUnExecuted()
        {
            EventHandler<UnExecutedEventArgs> d = UnExecuted;
            if (d != null) d(this, new UnExecutedEventArgs(this));
        }

        #endregion

        private List<ICommand> mCommands;
        private string mLongDescription = null;
        private string mShortDescription = null;

        /// <summary>
        /// Format string for the short description of the composite command. 
        /// Format parameter {0:0} is replaced by the number of commands, 
        /// and format parameter {1} is replaced by the short descriptions of the first and last command in the composite command.
        /// Only used when the short description has not been explicitly set using <see cref="ShortDescription"/>.
        /// </summary>
        public static string ShortDescriptionFormatString = "{0:0} commands: {1}";

        /// <summary>
        /// Format string for the short description of the composite command. 
        /// Format parameter {0:0} is replaced by the number of commands, 
        /// and format parameter {1} is replaced by the long descriptions of the sub-commands in the composite command.
        /// Only used when the short description has not been explicitly set using <see cref="LongDescription"/>.
        /// </summary>
        public static string LongDescriptionFormatString = "{0:0} commands:\n{1}";

        /// <summary>
        /// Create an empty composite command.
        /// </summary>
        public CompositeCommand()
        {
            mCommands = new List<ICommand>();
        }

        /// <summary>
        /// Gets or sets the long humanly-readable description of the composite command
        /// </summary>
        public string LongDescription
        {
            set { mLongDescription = value; }
            get
            {
                if (mLongDescription != null) return mLongDescription;
                string cmds = "-";
                if (mCommands.Count > 0)
                {
                    cmds = mCommands[0].LongDescription;
                    for (int i = 1; i < mCommands.Count; i++)
                    {
                        cmds += "\n" + LongDescription;
                    }
                }
                return String.Format(LongDescriptionFormatString, mCommands.Count, cmds);
            }
        }

        /// <summary>
        /// Gets or ets the long humanly-readable description of the composite command
        /// </summary>
        public string ShortDescription
        {
            set { mShortDescription = value; }
            get
            {
                if (mShortDescription != null) return mShortDescription;
                string cmds = "-";
                if (mCommands.Count > 0)
                {
                    cmds = mCommands[0].ShortDescription;
                    if (mCommands.Count > 1)
                    {
                        cmds += "..." + mCommands[mCommands.Count - 1];
                    }
                }
                return String.Format(ShortDescriptionFormatString, mCommands.Count, cmds);
            }
        }

        /// <summary>
        /// Insert the given command as a child of this node, at the given index. Does NOT replace the existing child,
        /// but increments (+1) the indices of the all children which index >= insertIndex. If insertIndex == children.size
        /// (no following children), then the given node is appended at the end of the existing children list.
        /// </summary>
        /// <param name="command">Cannot be null.</param>
        /// <param name="index">Must be within bounds [0 .. children.size]</param>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">Thrown when the index is out of bounds.</exception>
        /// <exception cref="exception.MethodParameterIsNullException">Thrown when a null command is given.</exception>
        public void Insert(ICommand command, int index)
        {
            if (command == null) throw new exception.MethodParameterIsNullException("Cannot insert a null command.");
            if (index < 0 || index > mCommands.Count)
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    String.Format("Cannot insert at index {0}; expected index in range [0 .. {1}]", index,
                                  mCommands.Count));
            }
            mCommands.Insert(index, command);
            NotifyCommandAdded(command, index);
        }

        /// <summary>
        /// Appends the given command as a child of this node, at the given index. Does NOT replace the existing child,
        /// but increments (+1) the indices of the all children which index >= insertIndex. If insertIndex == children.size
        /// (no following children), then the given node is appended at the end of the existing children list.
        /// </summary>
        /// <param name="command">Cannot be null.</param>
        public void Append(ICommand command)
        {
            Insert(command, Count);
        }

        /// <summary>
        /// Gets a list of the <see cref="ICommand"/>s making up the composite command
        /// </summary>
        /// <returns>The list</returns>
        public List<ICommand> ListOfCommands
        {
            get { return new List<ICommand>(mCommands); }
        }

        /// <summary>
        /// Gets the number of <see cref="ICommand"/>s in <c>this</c>
        /// </summary>
        /// <returns></returns>
        public int Count
        {
            get { return mCommands.Count; }
        }

        #region ICommand Members

        /// <summary>
        /// Execute the reverse command by executing the reverse commands for all the contained commands.
        /// The commands are undone in reverse order.
        /// </summary>
        /// <exception cref="urakawa.exception.CannotUndoException">Thrown when the command cannot be reversed; either because
        /// the composite command is empty or one of its contained command cannot be undone. In the latter case, the original
        /// exception is passed as the inner exception of the thrown exception.</exception>
        public void UnExecute()
        {
            if (mCommands.Count == 0) throw new exception.CannotUndoException("Composite command is empty.");
            try
            {
                for (int i = mCommands.Count - 1; i >= 0; --i) mCommands[i].UnExecute();
            }
            catch (exception.CannotUndoException e)
            {
                throw new exception.CannotUndoException("Contained command could not be undone", e);
            }
            finally
            {
                NotifyUnExecuted();
            }
        }

        /// <summary>
        /// Execute all contained commands in order.
        /// </summary>
        /// <exception cref="urakawa.exception.CannotRedoException">Thrown when the command cannot be executed; either because
        /// the composite command is empty or one of its contained command cannot be executed. In the latter case, the original
        /// exception is passed as the inner exception of the thrown exception.</exception>
        public void Execute()
        {
            if (mCommands.Count == 0) throw new exception.CannotRedoException("Composite command is empty.");
            try
            {
                foreach (ICommand command in mCommands) command.Execute();
            }
            catch (exception.CannotRedoException e)
            {
                throw new exception.CannotRedoException(
                    String.Format("Contained command could not be executed: {0}", e.Message), e);
            }
            finally
            {
                NotifyExecuted();
            }
        }

        /// <summary>
        /// The composite command is reversible if it contains commmands, and all of its contained command are.
        /// </summary>
        public bool CanUnExecute
        {
            get { return mCommands.Count > 0 && mCommands.TrueForAll(delegate(ICommand c) { return c.CanUnExecute; }); }
        }

        /// <summary>
        /// The composite command can execute if it contains commmands, and all the contained commands can execute
        /// </summary>
        /// <returns></returns>
        public bool CanExecute
        {
            get { return mCommands.Count > 0 && mCommands.TrueForAll(delegate(ICommand c) { return c.CanExecute; }); }
        }

        /// <summary>
        /// Gets a list of all <see cref="urakawa.media.data.MediaData"/> used by sub-commands of the composite command
        /// </summary>
        /// <returns></returns>
        public List<MediaData> ListOfUsedMediaData
        {
            get
            {
                List<media.data.MediaData> res = new List<urakawa.media.data.MediaData>();
                foreach (ICommand cmd in mCommands)
                {
                    res.AddRange(cmd.ListOfUsedMediaData);
                }
                return res;
            }
        }

        #endregion

        #region IXUKAble members

        /// <summary>
        /// Clears the <see cref="CompositeCommand"/> clearing the descriptions and the list of <see cref="ICommand"/>s
        /// </summary>
        protected override void clear()
        {
            mCommands.Clear();
            mShortDescription = null;
            mLongDescription = null;
            base.clear();
        }

        /// <summary>
        /// Reads the attributes of a CompositeCommand xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void xukInAttributes(XmlReader source)
        {
            mShortDescription = source.GetAttribute("shortDescription");
            mLongDescription = source.GetAttribute("longDescription");
            base.xukInAttributes(source);
        }

        /// <summary>
        /// Reads a child of a CompositeCommand xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        protected override void xukInChild(XmlReader source, ProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == ToolkitSettings.XUK_NS)
            {
                switch (source.LocalName)
                {
                    case "mCommands":
                        xukInCommands(source, handler);
                        readItem = true;
                        break;
                }
            }

            if (!readItem) base.xukInChild(source, handler);
        }

        private void xukInCommands(XmlReader source, ProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        ICommand cmd = Presentation.CommandFactory.CreateCommand(
                            source.LocalName, source.NamespaceURI);
                        if (cmd == null)
                        {
                            throw new exception.XukException(String.Format(
                                                                 "Could not create ICommand matching xuk QName {1}:{0}",
                                                                 source.LocalName, source.NamespaceURI));
                        }
                        Append(cmd);
                        cmd.XukIn(source, handler);
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
        /// Writes the attributes of a CompositeCommand element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void xukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            if (mShortDescription != null)
            {
                destination.WriteAttributeString("shortDescription", mShortDescription);
            }
            if (mLongDescription != null)
            {
                destination.WriteAttributeString("longDescription", mLongDescription);
            }
            base.xukOutAttributes(destination, baseUri);
        }

        /// <summary>
        /// Write the child elements of a CompositeCommand element.
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        /// <param name="handler">The handler for progress</param>
        protected override void xukOutChildren(XmlWriter destination, Uri baseUri, ProgressHandler handler)
        {
            destination.WriteStartElement("mCommands", ToolkitSettings.XUK_NS);
            foreach (ICommand cmd in ListOfCommands)
            {
                cmd.XukOut(destination, baseUri, handler);
            }
            destination.WriteEndElement();
            base.xukOutChildren(destination, baseUri, handler);
        }

        #endregion
    }
}