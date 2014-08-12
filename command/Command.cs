using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.core;
using urakawa.media.data;
using urakawa.progress;
using urakawa.xuk;
using urakawa.events;
using ExecutedEventArgs = urakawa.events.command.ExecutedEventArgs;
using UnExecutedEventArgs = urakawa.events.command.UnExecutedEventArgs;

namespace urakawa.command
{
    /// <summary>
    /// Classes realizing this interface must store the state of the object(s) affected by the command
    /// execution (including exception/redo). Implementations may choose various techniques suitable in terms
    /// of performance and memory usage (storage of the transition or the full object snapshot.)
    /// </summary>
    public abstract class Command : WithPresentation, ObjectTag, IUndoableAction, IUsingMediaData //IChangeNotifier
    {
        private object m_Tag = null;
        public object Tag
        {
            set { m_Tag = value; }
            get { return m_Tag; }
        }

        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            Command otherz = other as Command;
            if (otherz == null)
            {
                return false;
            }

            if (LongDescription != otherz.LongDescription)
            {
                return false;
            }
            if (ShortDescription != otherz.ShortDescription)
            {
                return false;
            }
            return true;
        }

        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

            mLongDescription = source.GetAttribute(XukStrings.LongDescription);
            mShortDescription = source.GetAttribute(XukStrings.ShortDescription);

        }

        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            //nothing new here
            base.XukInChild(source, handler);
        }

        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

            if (LongDescription != null)
            {
                destination.WriteAttributeString(XukStrings.LongDescription, LongDescription);
            }
            if (ShortDescription != null)
            {
                destination.WriteAttributeString(XukStrings.ShortDescription, ShortDescription);
            }
        }


        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            //nothing new here
            base.XukOutChildren(destination, baseUri, handler);
        }


        #region Event related

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

        ///// <summary>
        ///// Event fired after the <see cref="CompositeCommand"/> has changed. 
        ///// The event fire before any change specific event 
        ///// </summary>
        ////public event EventHandler<urakawa.events.DataModelChangedEventArgs> Changed;

        ///// <summary>
        ///// Fires the <see cref="Changed"/> event 
        ///// </summary>
        ///// <param name="args">The arguments of the event</param>
        ////protected void NotifyChanged(urakawa.events.DataModelChangedEventArgs args)
        ////{
        ////    EventHandler<urakawa.events.DataModelChangedEventArgs> d = Changed;
        ////    if (d != null) d(this, args);
        ////}
        #endregion

        /// <summary>
        /// Gets a <c>bool</c> indicating if the <see cref="IAction"/> can execute
        /// </summary>
        /// <returns>The <c>bool</c></returns>
        public abstract bool CanExecute { get; }


        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <exception cref="urakawa.exception.CannotExecuteException">Thrown when the command cannot be reversed.</exception>
        public abstract void Execute();


        /// <summary>
        /// Execute the reverse command.
        /// </summary>
        /// <exception cref="urakawa.exception.CannotUndoException">Thrown when the command cannot be reversed.</exception>
        public abstract void UnExecute();

        /// <summary>
        /// True if the command is reversible.
        /// </summary>
        public abstract bool CanUnExecute { get; }

        /// <summary>
        /// Gets a list of the <see cref="media.data.MediaData"/> used by the Command
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<MediaData> UsedMediaData
        {
            get { return new List<MediaData>(); }
        }

        protected string mLongDescription;
        protected string mShortDescription;

        /// <summary>
        /// Get a long uman-readable description of the command
        /// </summary>
        public virtual string LongDescription
        {
            get
            {
                return mLongDescription;
            }
            set
            {
                mLongDescription = value;
            }
        }

        /// <summary>
        /// Gets a short humanly readable description of the command
        /// </summary>
        public virtual string ShortDescription
        {
            get
            {
                return mShortDescription;
            }
            set
            {
                mShortDescription = value;
            }
        }
    }
}