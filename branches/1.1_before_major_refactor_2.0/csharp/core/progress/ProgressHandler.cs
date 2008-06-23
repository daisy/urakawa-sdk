using System;
using urakawa.events.progress;

namespace urakawa.progress
{
    /// <summary>
    /// Handles progress
    /// </summary>
    public interface ProgressHandler
    {
        /// <summary>
        /// Event fired to indicate progress
        /// </summary>
        event EventHandler<ProgressEventArgs> progress;

        /// <summary>
        /// Notifies the handler of progress
        /// </summary>
        /// <returns>A <see cref="bool"/> indicating if the progress was cancelled</returns>
        bool notifyProgress();

        /// <summary>
        /// Event fired to indicate that the progress has finished
        /// </summary>
        event EventHandler<FinishedEventArgs> finished;

        /// <summary>
        /// Notifies the handler that the progress is finished
        /// </summary>
        void notifyFinished();

        /// <summary>
        /// Event fired to indicate that the progress has been cancelled
        /// </summary>
        event EventHandler<CancelledEventArgs> cancelled;

        ///<summary>
        /// Notofies the handler that the progress has been cancelled
        ///</summary>
        void notifyCancelled();
    }
}