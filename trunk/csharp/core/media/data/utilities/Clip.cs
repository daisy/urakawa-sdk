using urakawa.media.timing;

namespace urakawa.media.data.utilities
{
    /// <summary>
    /// Represents a generic media clip
    /// </summary>
    public abstract class Clip
    {
        private Time mClipBegin = new Time();

        /// <summary>
        /// Gets (a copy of) the clip begin <see cref="Time"/> of <c>this</c>
        /// </summary>
        /// <returns>
        /// The clip begin <see cref="Time"/> - can not be <c>null</c>
        /// </returns>
        public Time ClipBegin
        {
            get { return mClipBegin.Copy(); }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException("Clip begin of a WavClip can not be null");
                }
                if (value.IsGreaterThan(ClipEnd))
                {
                    throw new exception.MethodParameterIsOutOfBoundsException(
                        "The new clip begin is beyond the current clip end");
                }
                mClipBegin = value.Copy();
            }
        }

        private Time mClipEnd = null;

        /// <summary>
        /// Gets (a copy of) the clip end <see cref="Time"/> of <c>this</c>
        /// </summary>
        /// <returns>The clip end <see cref="Time"/></returns>
        public Time ClipEnd
        {
            get
            {
                if (mClipEnd == null) return Time.Zero.AddTimeDelta(MediaDuration);
                return mClipEnd.Copy();
            }
            set
            {
                if (value == null)
                {
                    mClipEnd = null;
                }
                else
                {
                    if (value.IsLessThan(ClipBegin))
                    {
                        throw new exception.MethodParameterIsOutOfBoundsException(
                            "The new clip end time is before current clip begin");
                    }
                    mClipEnd = value.Copy();
                }
            }
        }

        /// <summary>
        /// Determines if clip end is tied to the end of the underlying media
        /// </summary>
        /// <returns>
        /// A <see cref="bool"/> indicating if clip end is tied to the end of the underlying media
        /// </returns>
        public bool IsClipEndTiedToEOM
        {
            get { return (mClipEnd == null); }
        }

        /// <summary>
        /// Gets the duration of the clip
        /// </summary>
        /// <returns>The duration of as a <see cref="TimeDelta"/></returns>
        public TimeDelta Duration
        {
            get { return ClipEnd.GetTimeDelta(ClipBegin); }
        }

        /// <summary>
        /// Gets the duration of the underlying media
        /// </summary>
        /// <returns>The duration of the underlying media</returns>
        public abstract TimeDelta MediaDuration { get; }
    }
}