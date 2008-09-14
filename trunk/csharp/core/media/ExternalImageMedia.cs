using System;
using System.Xml;
using urakawa.events.media;
using urakawa.progress;


namespace urakawa.media
{
    /// <summary>
    /// Implementation of <see cref="AbstractImageMedia"/> based on an external image file
    /// </summary>
    public class ExternalImageMedia : AbstractImageMedia, ILocated
    {
        private string mSrc;
        private int mWidth;
        private int mHeight;

        private void Reset()
        {
            mSrc = ".";
            mWidth = 0;
            mHeight = 0;
        }

        /// <summary>
        /// Default constructor - for system use only, 
        /// <see cref="ExternalImageMedia"/>s should only be created via. the <see cref="MediaFactory"/>
        /// </summary>
        public ExternalImageMedia()
        {
            Reset();
            SrcChanged += this_SrcChanged;
        }

        /// <summary>
        /// This override is useful while debugging
        /// </summary>
        /// <returns>A <see cref="string"/> representation of the <see cref="ExternalImageMedia"/></returns>
        public override string ToString()
        {
            return String.Format("{0} ({1}-{2:0}x{3:0})", GetType().FullName, Src, mWidth, mHeight);
        }

        #region Media Members

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

        /// <summary>
        /// Creates a copy of the <c>this</c>
        /// </summary>
        /// <returns>The copy</returns>
        public new ExternalImageMedia Copy()
        {
            return CopyProtected() as ExternalImageMedia;
        }

        /// <summary>
        /// Exports <c>this</c> to a destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination <see cref="Presentation"/></param>
        /// <returns>The export</returns>
        public new ExternalImageMedia Export(Presentation destPres)
        {
            return ExportProtected(destPres) as ExternalImageMedia;
        }

        /// <summary>
        /// Exports the external image media to a destination <see cref="Presentation"/>
        /// - part of a construct allowing the <see cref="Export"/> method to return <see cref="ExternalImageMedia"/>
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported external video media</returns>
        protected override Media ExportProtected(Presentation destPres)
        {
            ExternalImageMedia exported = (ExternalImageMedia) base.ExportProtected(destPres);
            exported.Src = this.Src;
            exported.Height = this.Height;
            exported.Width = this.Width;
            return exported;
        }

        #endregion

        #region ISized Members

        /// <summary>
        /// Return the image width
        /// </summary>
        /// <returns>The width</returns>
        public override int Width
        {
            get { return mWidth; }
            set { SetSize(Height, value); }
        }

        /// <summary>
        /// Return the image height
        /// </summary>
        /// <returns>The height</returns>
        public override int Height
        {
            get { return mHeight; }
            set { SetSize(value, Width); }
        }


        /// <summary>
        /// Sets the image size
        /// </summary>
        /// <param name="height">The new height</param>
        /// <param name="width">The new width</param>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when the new width or height is negative
        /// </exception>
        public override void SetSize(int height, int width)
        {
            if (width < 0)
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The width of an image can not be negative");
            }
            if (height < 0)
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The height of an image can not be negative");
            }
            int prevWidth = mWidth;
            mWidth = width;
            int prevHeight = mHeight;
            mHeight = height;
            if (mWidth != prevWidth || mHeight != prevHeight)
            {
                NotifySizeChanged(this, mHeight, mWidth, prevHeight, prevWidth);
            }
        }



        #endregion

        #region ILocated Members

        /// <summary>
        /// Event fired after <see cref="Src"/> of the <see cref="ILocated"/> has changed
        /// </summary>
        public event EventHandler<SrcChangedEventArgs> SrcChanged;

        /// <summary>
        /// Fires the <see cref="SrcChanged"/> event
        /// </summary>
        /// <param name="newSrc">The new <see cref="Src"/> value</param>
        /// <param name="prevSrc">The <see cref="Src"/> value prior to the change</param>
        protected void NotifySrcChanged(string newSrc, string prevSrc)
        {
            EventHandler<SrcChangedEventArgs> d = SrcChanged;
            if (d != null) d(this, new SrcChangedEventArgs(this, newSrc, prevSrc));
        }

        private void this_SrcChanged(object sender, SrcChangedEventArgs e)
        {
            NotifyChanged(e);
        }


        /// <summary>
        /// Gets or sets the src value. The default value is "."
        /// </summary>
        /// <exception cref="exception.MethodParameterIsEmptyStringException">
        /// Thrown when trying to set the <see cref="Src"/> value to <c>null</c></exception>
        public string Src
        {
            get { return mSrc; }
            set
            {
                if (value == null)
                    throw new exception.MethodParameterIsNullException("The src value cannot be null");
                if (value == "")
                    throw new exception.MethodParameterIsEmptyStringException("The src value cannot be an empty string");
                string prevSrc = mSrc;
                mSrc = value;
                if (mSrc != prevSrc) NotifySrcChanged(mSrc, prevSrc);
            }
        }

        /// <summary>
        /// Gets the <see cref="Uri"/> of the <see cref="ExternalAudioMedia"/> 
        /// - uses <c>getMediaFactory().getPresentation().getRootUri()</c> as base <see cref="Uri"/>
        /// </summary>
        /// <returns>The <see cref="Uri"/> - <c>null</c> if <see cref="Src"/> is <c>null</c></returns>
        /// <exception cref="exception.InvalidUriException">
        /// Thrown when the value <see cref="Src"/> is not a well-formed <see cref="Uri"/>
        /// </exception>
        public Uri Uri
        {
            get
            {
                if (Src == null) return null;
                if (!Uri.IsWellFormedUriString(Src, UriKind.RelativeOrAbsolute))
                {
                    throw new exception.InvalidUriException(String.Format(
                                                                "The src value '{0}' is not a well-formed Uri", Src));
                }
                return new Uri(MediaFactory.Presentation.RootUri, Src);
            }
        }

        #endregion

        #region IXUKAble members

        /// <summary>
        /// Clears the data of the <see cref="Media"/>
        /// </summary>
        protected override void Clear()
        {
            Reset();
            base.Clear();
        }

        /// <summary>
        /// Reads the attributes of a ExternalImageMedia xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void XukInAttributes(XmlReader source)
        {
            string val = source.GetAttribute("src");
            if (val == null || val == "") val = ".";
            Src = val;
            string height = source.GetAttribute("height");
            string width = source.GetAttribute("width");
            if (height != null && height != "")
            {
                int h;
                if (!Int32.TryParse(height, out h))
                {
                    throw new exception.XukException(
                        String.Format("height attribute of {0} element is not an integer", source.LocalName));
                }
                Height = h;
            }
            else
            {
                Height = 0;
            }
            if (width != null && width != "")
            {
                int w;
                if (!Int32.TryParse(width, out w))
                {
                    throw new exception.XukException(
                        String.Format("width attribute of {0} element is not an integer", source.LocalName));
                }
                Width = w;
            }
            else
            {
                Width = 0;
            }
            base.XukInAttributes(source);
        }

        /// <summary>
        /// Writes the attributes of a ExternalImageMedia element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            if (Src != "")
            {
                Uri srcUri = new Uri(MediaFactory.Presentation.RootUri, Src);
                if (baseUri == null)
                {
                    destination.WriteAttributeString("src", srcUri.AbsoluteUri);
                }
                else
                {
                    destination.WriteAttributeString("src", baseUri.MakeRelativeUri(srcUri).ToString());
                }
            }
            destination.WriteAttributeString("height", this.mHeight.ToString());
            destination.WriteAttributeString("width", this.mWidth.ToString());
            base.XukOutAttributes(destination, baseUri);
        }

        #endregion

        #region IValueEquatable<Media> Members

        /// <summary>
        /// Conpares <c>this</c> with a given other <see cref="Media"/> for equality
        /// </summary>
        /// <param name="other">The other <see cref="Media"/></param>
        /// <returns><c>true</c> if equal, otherwise <c>false</c></returns>
        public override bool ValueEquals(Media other)
        {
            if (!base.ValueEquals(other)) return false;
            ExternalImageMedia otherImage = (ExternalImageMedia)other;
            if (Src != otherImage.Src) return false;
            if (Height != otherImage.Height) return false;
            if (Width != otherImage.Width) return false;
            return true;
        }

        #endregion
    }
}