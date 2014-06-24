using urakawa.media;

namespace urakawa.events.media
{
    /// <summary>
    /// Arguments for event occuring when the <c>src</c> of a <see cref="ILocated"/> <see cref="Media"/> has changed
    /// </summary>
    public class SrcChangedEventArgs : MediaEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="ILocated"/> <see cref="Media"/> 
        /// and previous+new <c>src</c> values
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="newSrcVal">The new src value</param>
        /// <param name="prevSrcVal">The previous src value</param>
        public SrcChangedEventArgs(Media source, string newSrcVal, string prevSrcVal)
            : base(source)
        {
            SourceExternalMedia = source as ILocated;
            this.source = source;
            NewSrc = newSrcVal;
            PreviousSrc = prevSrcVal;
        }

        /// <summary>
        /// The source media
        /// </summary>
        public readonly ILocated SourceExternalMedia;

        private readonly Media source;

        /// <summary>
        /// The new value of <c>src</c>
        /// </summary>
        public readonly string NewSrc;

        /// <summary>
        /// The value of <c>src</c> before the change
        /// </summary>
        public readonly string PreviousSrc;
    }
}