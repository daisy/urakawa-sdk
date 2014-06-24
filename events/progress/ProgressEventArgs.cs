namespace urakawa.events.progress
{
    ///<summary>
    /// Arguments for progress events
    ///</summary>
    public class ProgressEventArgs : CancellableEventArgs
    {
        ///<summary>
        /// Constructor initializing the <see cref="Current"/> and <see cref="Total"/> field values
        ///</summary>
        ///<param name="c">The value for <see cref="Current"/></param>
        ///<param name="t">The value for <see cref="Total"/></param>
        public ProgressEventArgs(long c, long t)
        {
            Current = c;
            Total = t;
        }

        /// <summary>
        /// The current progress value
        /// </summary>
        public readonly long Current;

        /// <summary>
        /// The estimated total progress value
        /// </summary>
        public readonly long Total;
    }
}