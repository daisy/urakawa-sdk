using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.events.progress
{
    /// <summary>
    /// Base class for events that are cancellable
    /// </summary>
    public class CancellableEventArgs : EventArgs
    {
        private bool mIsCancelled = false;

        /// <summary>
        /// Gets a <see cref="bool"/> indicating if the event has been cancelled
        /// </summary>
        public bool IsCancelled
        {
            get { return mIsCancelled; }
        }

        /// <summary>
        /// Cancels the event
        /// </summary>
        public void Cancel()
        {
            mIsCancelled = true;
        }
    }
}