using System;
using urakawa.events.progress;

namespace urakawa.progress
{
    /// <summary>
    /// Handles progress
    /// </summary>
    public interface IProgressHandler
    {
        /// <summary>
        /// Event fired to indicate progress
        /// </summary>
        event EventHandler<ProgressEventArgs> Progress;

        /// <summary>
        /// Notifies the handler of progress
        /// </summary>
        /// <returns>A <see cref="bool"/> indicating if the progress was cancelled</returns>
        bool NotifyProgress();

        /// <summary>
        /// Event fired to indicate that the progress has finished
        /// </summary>
        event EventHandler<FinishedEventArgs> Finished;

        /// <summary>
        /// Notifies the handler that the progress is finished
        /// </summary>
        void NotifyFinished();

        /// <summary>
        /// Event fired to indicate that the progress has been cancelled
        /// </summary>
        event EventHandler<CancelledEventArgs> Cancelled;

        ///<summary>
        /// Notofies the handler that the progress has been cancelled
        ///</summary>
        void NotifyCancelled();
    }
}