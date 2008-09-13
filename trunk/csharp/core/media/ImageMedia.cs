using System;
using urakawa.events.media;

namespace urakawa.media
{
    /// <summary>
    /// Represents images which are external media and have a height and width
    /// </summary>
    public abstract class ImageMedia : Media, ISized
    {


        /// <summary>
        /// Default constructor - for system use only, 
        /// <see cref="ImageMedia"/>s should only be created via. the <see cref="MediaFactory"/>
        /// </summary>
        protected ImageMedia()
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