using System;
using urakawa.events.media;
using urakawa.media.timing;

namespace urakawa.media
{
    /// <summary>
    /// Video media is both time-based and has a visual presence
    /// </summary>
    public abstract class AbstractVideoMedia : Media, IContinuous, ISized
    {
        private ResizeMode m_ResizeMode;
        public ResizeMode ResizeMode
        {
            get { return m_ResizeMode; }
            set { m_ResizeMode = value; }
        }

        /// <summary>
        /// This always returns true, because
        /// video media is always considered continuous
        /// </summary>
        /// <returns></returns>
        public override bool IsContinuous
        {
            get { return true; }
        }

        /// <summary>
        /// This always returns false, because
        /// video media is never considered discrete
        /// </summary>
        /// <returns></returns>
        public override bool IsDiscrete
        {
            get { return false; }
        }

        /// <summary>
        /// This always returns false, because
        /// a single media object is never considered to be a sequence
        /// </summary>
        /// <returns></returns>
        public override bool IsSequence
        {
            get { return false; }
        }

        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            AbstractVideoMedia otherz = other as AbstractVideoMedia;
            if (otherz == null)
            {
                return false;
            }

            if (ResizeMode != otherz.ResizeMode)
            {
                return false;
            }
            if (!Duration.IsEqualTo(otherz.Duration))
            {
                return false;
            }
            if (Height != otherz.Height)
            {
                return false;
            }
            if (Width != otherz.Width)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// Default constructor - for system use only, 
        /// <see cref="AbstractVideoMedia"/>s should only be created via. the <see cref="MediaFactory"/>
        /// </summary>
        protected AbstractVideoMedia()
        {
            SizeChanged += this_SizeChanged;
        }

        /// <summary>
        /// Get the width of the <see cref="ISized"/> object.
        /// </summary>
        /// <returns>The width</returns>
        public abstract int Width { get; set; }

        /// <summary>
        /// Get the height of the <see cref="ISized"/> object.
        /// </summary>
        /// <returns>The height</returns>
        public abstract int Height { get; set; }

        /// <summary>
        /// Sets the size of the <see cref="ISized"/> object.
        /// </summary>
        /// <param name="newWidth">The new width</param>
        /// <param name="newHeight">The new height</param>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when the new width or height is negative
        /// </exception>
        public virtual void SetSize(int newHeight, int newWidth)
        {
            Height = newHeight;
            Width = newWidth;
        }


        #region Implementation of IContinuous

        /// <summary>
        /// Gets the duration of the media
        /// </summary>
        /// <returns>The duration</returns>
        public abstract Time Duration { get; }

        IContinuous IContinuous.Split(Time splitPoint)
        {
            return Split(splitPoint);
        }

        /// <summary>
        /// Splits the continuous media at a given split point, leaving the instance with the part before the split point
        /// and creating a new media with the part after
        /// </summary>
        /// <param name="splitPoint">The given split point - must be between <c>00:00:00.000</c> and <c>GetDuration()</c></param>
        /// <returns>The media with the part of the media after the split point</returns>
        public AbstractVideoMedia Split(Time splitPoint)
        {
            return SplitProtected(splitPoint);
        }


        /// <summary>
        /// Splits <c>this</c> at a given <see cref="Time"/>
        /// </summary>
        /// <param name="splitPoint">The <see cref="Time"/> at which to split - 
        /// must be between clip begin and clip end <see cref="Time"/>s</param>
        /// <returns>
        /// A newly created <see cref="AbstractAudioMedia"/> containing the video after <paramref localName="splitPoint"/>,
        /// <c>this</c> retains the video before <paramref localName="splitPoint"/>.
        /// </returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="splitPoint"/> is <c>null</c>
        /// </exception>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when <paramref name="splitPoint"/> is not between clip begin and clip end
        /// </exception>
        protected abstract AbstractVideoMedia SplitProtected(Time splitPoint);

        #endregion

        #region Implementation of ISized


        /// <summary>
        /// Event fired after the size (height or width) of the <see cref="ISized"/> has changed
        /// </summary>
        public event EventHandler<SizeChangedEventArgs> SizeChanged;

        /// <summary>
        /// Fires the <see cref="SizeChanged"/> event
        /// </summary>
        /// <param name="newHeight">The new height of the <see cref="ExternalImageMedia"/></param>
        /// <param name="newWidth">The new width of the <see cref="ExternalImageMedia"/></param>
        /// <param name="prevHeight">The height of the <see cref="ExternalImageMedia"/> prior to the change</param>
        /// <param name="prevWidth">The width of the <see cref="ExternalImageMedia"/> prior to the change</param>
        protected void NotifySizeChanged(int newHeight, int newWidth, int prevHeight, int prevWidth)
        {
            EventHandler<events.media.SizeChangedEventArgs> d = SizeChanged;
            if (d != null) d(this, new SizeChangedEventArgs(this, newHeight, newWidth, prevHeight, prevWidth));
        }

        private void this_SizeChanged(object sender, urakawa.events.media.SizeChangedEventArgs e)
        {
            NotifyChanged(e);
        }


        #endregion
    }
}