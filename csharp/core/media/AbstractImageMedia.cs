using System;
using urakawa.events.media;

namespace urakawa.media
{
    /// <summary>
    /// Represents images which are external media and have a height and width
    /// </summary>
    public abstract class AbstractImageMedia : Media, ISized
    {
        private ResizeMode m_ResizeMode;
        public ResizeMode ResizeMode
        {
            get { return m_ResizeMode; }
            set { m_ResizeMode = value; }
        }

        /// <summary>
        /// This always returns <c>false</c>, because
        /// image media is never considered continuous
        /// </summary>
        /// <returns><c>false</c></returns>
        public override bool IsContinuous
        {
            get { return false; }
        }

        /// <summary>
        /// This always returns <c>true</c>, because
        /// image media is always considered discrete
        /// </summary>
        /// <returns><c>true</c></returns>
        public override bool IsDiscrete
        {
            get { return true; }
        }

        /// <summary>
        /// This always returns <c>false</c>, because
        /// a single media object is never considered to be a sequence
        /// </summary>
        /// <returns><c>false</c></returns>
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

            AbstractImageMedia otherz = other as AbstractImageMedia;
            if (otherz == null)
            {
                return false;
            }

            if (ResizeMode != otherz.ResizeMode)
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
        /// <see cref="AbstractImageMedia"/>s should only be created via. the <see cref="MediaFactory"/>
        /// </summary>
        protected AbstractImageMedia()
        {
            SizeChanged += this_SizeChanged;
        }



        #region Implementation of ISized

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


        /// <summary>
        /// Event fired after the size (height or width) of the <see cref="ISized"/> has changed
        /// </summary>
        public event EventHandler<SizeChangedEventArgs> SizeChanged;

        /// <summary>
        /// Fires the <see cref="SizeChanged"/> event
        /// </summary>
        /// <param name="source">The source, that is the <see cref="ExternalImageMedia"/> whoose size has changed</param>
        /// <param name="newHeight">The new height of the <see cref="ExternalImageMedia"/></param>
        /// <param name="newWidth">The new width of the <see cref="ExternalImageMedia"/></param>
        /// <param name="prevHeight">The height of the <see cref="ExternalImageMedia"/> prior to the change</param>
        /// <param name="prevWidth">The width of the <see cref="ExternalImageMedia"/> prior to the change</param>
        protected void NotifySizeChanged(ExternalImageMedia source, int newHeight, int newWidth, int prevHeight,
                                         int prevWidth)
        {
            EventHandler<events.media.SizeChangedEventArgs> d = SizeChanged;
            if (d != null)
                d(this,
                  new urakawa.events.media.SizeChangedEventArgs(source, newHeight, newWidth, prevHeight, prevWidth));
        }

        private void this_SizeChanged(object sender, urakawa.events.media.SizeChangedEventArgs e)
        {
            NotifyChanged(e);
        }


        #endregion
    }
}