using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.media.data;
using urakawa.progress;
using urakawa.xuk;

namespace urakawa.command
{
    /// <summary>
    /// A composite command made of a series of sub commands. Useful for merging small commands into one such as:
    /// user typing text letter by letter (the exception/redo would work on full word or sentence, not for each character.)
    /// </summary>

    [XukNameUglyPrettyAttribute("cmpCmd", "CompositeCommand")]
    public class CompositeCommand : Command
    {
        public List<T> GetChildCommandsAllType<T>() where T : Command
        {
            List<T> list = new List<T>();
            foreach (Command childCmd in ChildCommands.ContentsAs_Enumerable)
            {
                if (childCmd is T)
                {
                    list.Add((T)childCmd);
                }
                else
                {
                    return null;
                }
            }
            return list.Count == 0 ? null : list;
        }

        public ObjectListProvider<Command> ChildCommands
        {
            get
            {
                return mCommands;
            }
        }

        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            CompositeCommand otherz = other as CompositeCommand;
            if (otherz == null)
            {
                return false;
            }

            if (otherz.ChildCommands.Count != mCommands.Count)
            {
                return false;
            }

            for (int i = 0; i < mCommands.Count; i++)
            {
                if (!mCommands.Get(i).ValueEquals(otherz.ChildCommands.Get(i)))
                {
                    //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                    return false;
                }
            }
            //foreach (Command cmd in mCommands)
            //{
            //    bool foundEqualCommand = false;
            //    foreach (Command cmdOther in otherz.Commands)
            //    {
            //        if (cmdOther.ValueEquals(cmd))
            //        {
            //            foundEqualCommand = true;
            //            break;
            //        }
            //    }
            //    if (!foundEqualCommand)
            //    {
            //        return false;
            //    }
            //}
            return true;
        }

    


        private ObjectListProvider<Command> mCommands;

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
        /// Only used when the long description has not been explicitly set using <see cref="LongDescription"/>.
        /// </summary>
        public static string LongDescriptionFormatString = "{0:0} commands: {1}";

        /// <summary>
        /// Create an empty composite command.
        /// </summary>
        public CompositeCommand()
        {
            mCommands = new ObjectListProvider<Command>(this, true);
        }

        /// <summary>
        /// Gets the long humanly-readable description of the composite command
        /// </summary>
        public override string LongDescription
        {
            set
            {
                base.LongDescription = value;
            }
            get
            {
                if (mLongDescription != null) return mLongDescription;
                string cmds = "";
                if (mCommands.Count > 0)
                {
                    for (int i = 0; i < mCommands.Count; i++)
                    {
                        cmds += "//" + mCommands.Get(0).LongDescription;
                    }
                }
                return String.Format(LongDescriptionFormatString, mCommands.Count, cmds);
            }
        }

        /// <summary>
        /// Gets the long humanly-readable description of the composite command
        /// </summary>
        public override string ShortDescription
        {
            set
            {
                base.ShortDescription = value;
            }
            get
            {
                if (mShortDescription != null) return mShortDescription;
                string cmds = "";
                if (mCommands.Count > 0)
                {
                    for (int i = 0; i < mCommands.Count; i++)
                    {
                        cmds += "//" + mCommands.Get(0).ShortDescription;
                    }
                }
                return String.Format(ShortDescriptionFormatString, mCommands.Count, cmds);
            }
        }

        #region Command Members

        /// <summary>
        /// Execute the reverse command by executing the reverse commands for all the contained commands.
        /// The commands are undone in reverse order.
        /// </summary>
        /// <exception cref="urakawa.exception.CannotUndoException">Thrown when the command cannot be reversed; either because
        /// the composite command is empty or one of its contained command cannot be undone. In the latter case, the original
        /// exception is passed as the inner exception of the thrown exception.</exception>
        public override void UnExecute()
        {
            if (mCommands.Count == 0) return; // throw new exception.CannotUndoException("Composite command is empty.");
            try
            {
                for (int i = mCommands.Count - 1; i >= 0; --i) mCommands.Get(i).UnExecute();
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
        public override void Execute()
        {
            if (mCommands.Count == 0) return; // throw new exception.CannotRedoException("Composite command is empty.");
            try
            {
                foreach (Command command in mCommands.ContentsAs_Enumerable) command.Execute();
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
        public override bool CanUnExecute
        {
            get
            {
                foreach (Command cmd in mCommands.ContentsAs_Enumerable)
                {
                    if (!cmd.CanUnExecute)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// The composite command can execute if it contains commmands, and all the contained commands can execute
        /// </summary>
        /// <returns></returns>
        public override bool CanExecute
        {
            get
            {
                foreach(Command cmd in mCommands.ContentsAs_Enumerable)
                {
                    if (!cmd.CanExecute)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Gets a list of all <see cref="urakawa.media.data.MediaData"/> used by sub-commands of the composite command
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<MediaData> UsedMediaData
        {
            get
            {
                //List<MediaData> res = new List<MediaData>();
                foreach (Command cmd in mCommands.ContentsAs_Enumerable)
                {
                    foreach (MediaData md in cmd.UsedMediaData)
                    {
                        yield return md;
                    }
                    //res.AddRange(cmd.UsedMediaData);
                }
                //return res;
                yield break;
            }
        }

        #endregion

        #region IXUKAble members

        /// <summary>
        /// Clears the <see cref="CompositeCommand"/> clearing the descriptions and the list of <see cref="Command"/>s
        /// </summary>
        protected override void Clear()
        {
            foreach (Command cmd in ChildCommands.ContentsAs_ListCopy)
            {
                ChildCommands.Remove(cmd);
            }
            mShortDescription = null;
            mLongDescription = null;
            base.Clear();
        }

        /// <summary>
        /// Reads a child of a CompositeCommand xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == XukAble.XUK_NS)
            {
                if (source.LocalName == XukStrings.Commands)
                {
                        XukInCommands(source, handler);
                        readItem = true;
                }
            }

            if (!readItem) base.XukInChild(source, handler);
        }

        private void XukInCommands(XmlReader source, IProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        Command cmd = Presentation.CommandFactory.Create(source.LocalName, source.NamespaceURI);
                        if (cmd == null)
                        {
                            throw new exception.XukException(String.Format(
                                                                 "Could not create Command matching xuk QName {1}:{0}",
                                                                 source.LocalName, source.NamespaceURI));
                        }
                        cmd.XukIn(source, handler);
                        mCommands.Insert(mCommands.Count, cmd);
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
        /// Write the child elements of a CompositeCommand element.
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            destination.WriteStartElement(XukStrings.Commands, XukAble.XUK_NS);
            foreach (Command cmd in mCommands.ContentsAs_Enumerable)
            {
                cmd.XukOut(destination, baseUri, handler);
            }
            destination.WriteEndElement();
            base.XukOutChildren(destination, baseUri, handler);
        }

        #endregion
    }
}