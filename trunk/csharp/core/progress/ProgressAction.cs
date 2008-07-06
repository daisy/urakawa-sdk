using System;
using urakawa.command;
using urakawa.events.command;
using urakawa.events.progress;

namespace urakawa.progress
{
    /// <summary>
    /// An <see cref="IAction"/> that also handles progress
    /// </summary>
    public abstract class ProgressAction : ProgressHandler, IAction
    {
        /// <summary>
        /// Indicates if a request has been made to cancel the action
        /// </summary>
        protected bool mHasCancelBeenRequested = false;

        ///<summary>
        /// Gets a <see cref="bool"/> indicating if a request has been made to cancel the action
        ///</summary>
        public bool HasCancelBeenRequested
        {
            get { return mHasCancelBeenRequested; }
        }

        /// <summary>
        /// Request that the action be cancelled
        /// </summary>
        public void RequestCancel()
        {
            mHasCancelBeenRequested = true;
        }

        #region Implementation of ProgressHandler

        /// <summary>
        /// Event fired to indicate progress
        /// </summary>
        public event EventHandler<ProgressEventArgs> Progress;

        /// <summary>
        /// Gets the current and estimated total progress values
        /// </summary>
        /// <param name="cur">A <see cref="long"/> in which the current progress is returned</param>
        /// <param name="tot">A <see cref="long"/> in which the estimated total progress is returned</param>
        protected abstract void getCurrentProgress(out long cur, out long tot);

        /// <summary>
        /// Notifies the handler of progress
        /// </summary>
        /// <returns>A <see cref="bool"/> indicating if the progress was cancelled</returns>
        public virtual bool NotifyProgress()
        {
            EventHandler<ProgressEventArgs> d = Progress;
            if (d != null)
            {
                long c, t;
                getCurrentProgress(out c, out t);
                ProgressEventArgs e = new ProgressEventArgs(c, t);
                d(this, e);
                if (e.IsCancelled) return true;
            }
            return false;
        }

        /// <summary>
        /// Event fired to indicate that the progress has finished
        /// </summary>
        public event EventHandler<FinishedEventArgs> Finished;

        /// <summary>
        /// Notifies the handler that the progress is finished
        /// </summary>
        public void NotifyFinished()
        {
            EventHandler<FinishedEventArgs> d = Finished;
            if (d != null) d(this, new FinishedEventArgs());
        }

        /// <summary>
        /// Event fired to indicate that the progress has been cancelled
        /// </summary>
        public event EventHandler<CancelledEventArgs> Cancelled;

        ///<summary>
        /// Notofies the handler that the progress has been cancelled
        ///</summary>
        public void NotifyCancelled()
        {
            EventHandler<CancelledEventArgs> d = Cancelled;
            if (d != null) d(this, new CancelledEventArgs());
        }

        #endregion

        #region Implementation of IAction

        /// <summary>
        /// Gets a <c>bool</c> indicating if the <see cref="IAction"/> can execute
        /// </summary>
        /// <returns>The <c>bool</c></returns>
        public abstract bool CanExecute { get; }

        /// <summary>
        /// Get a long uman-readable description of the command
        /// </summary>
        public abstract string LongDescription { get; }

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <exception cref="urakawa.exception.CannotExecuteException">Thrown when the command cannot be reversed.</exception>
        public abstract void Execute();

        /// <summary>
        /// Gets a short humanly readable description of the command
        /// </summary>
        public abstract string ShortDescription { get; }

        #endregion
    }
}