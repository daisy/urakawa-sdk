using System;
using System.Xml;
using urakawa.events.media;
using urakawa.media.timing;
using urakawa.xuk;

namespace urakawa.media
{
    /// <summary>
    /// Implementation of <see cref="AbstractVideoMedia"/> based on an external file
    /// </summary>
    public class ExternalVideoMedia : AbstractVideoMedia, IClipped, ILocated
    {

        public override string GetTypeNameFormatted()
        {
            return XukStrings.ExternalVideoMedia;
        }
        private string mSrc;
        private int mWidth;
        private int mHeight;
        private Time mClipBegin;
        private Time mClipEnd;

        private void Reset()
        {
            mSrc = ".";
            mHeight = 0;
            mWidth = 0;
            mClipBegin = Time.Zero;
            mClipEnd = Time.MaxValue;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ExternalVideoMedia()
        {
            Reset();
            SrcChanged += this_SrcChanged;
            ClipChanged += this_ClipChanged;
        }

        #region Media Members

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

        /// <summary>
        /// Copy function which returns an <see cref="ExternalVideoMedia"/> object
        /// </summary>
        /// <returns>a copy of this</returns>
        public new ExternalVideoMedia Copy()
        {
            return CopyProtected() as ExternalVideoMedia;
        }

        ///<summary>
        ///
        ///</summary>
        ///<returns></returns>
        protected override Media CopyProtected()
        {
            ExternalVideoMedia copy = (ExternalVideoMedia)base.CopyProtected();
            copy.Src = Src;
            if (ClipBegin.IsNegativeTimeOffset)
            {
                copy.ClipBegin = ClipBegin.Copy();
                copy.ClipEnd = ClipEnd.Copy();
            }
            else
            {
                copy.ClipEnd = ClipEnd.Copy();
                copy.ClipBegin = ClipBegin.Copy();
            }
            copy.Width = Width;
            copy.Height = Height;
            return copy;
        }


        /// <summary>
        /// Exports the external video media to a destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported external video media</returns>
        protected override Media ExportProtected(Presentation destPres)
        {
            ExternalVideoMedia exported = (ExternalVideoMedia)base.ExportProtected(destPres);
            exported.Src = Src;
            if (ClipBegin.IsNegativeTimeOffset)
            {
                exported.ClipBegin = ClipBegin.Copy();
                exported.ClipEnd = ClipEnd.Copy();
            }
            else
            {
                exported.ClipEnd = ClipEnd.Copy();
                exported.ClipBegin = ClipBegin.Copy();
            }
            exported.Width = Width;
            exported.Height = Height;
            return exported;
        }

        /// <summary>
        /// Exports the external video media to a destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported external video media</returns>
        public new ExternalVideoMedia Export(Presentation destPres)
        {
            return ExportProtected(destPres) as ExternalVideoMedia;
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
                NotifySizeChanged(mHeight, mWidth, prevHeight, prevWidth);
            }
        }



        #endregion

        #region IXUKAble overides

        /// <summary>
        /// Clears the data of the <see cref="ExternalVideoMedia"/>
        /// </summary>
        protected override void Clear()
        {
            Reset();
            base.Clear();
        }

        /// <summary>
        /// Reads the attributes of a ExternalVideoMedia xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void XukInAttributes(XmlReader source)
        {
            string val = source.GetAttribute(XukStrings.Src);
            if (val == null || val == "") val = ".";
            Src = val;
            string cb = source.GetAttribute(XukStrings.ClipBegin);
            string ce = source.GetAttribute(XukStrings.ClipEnd);
            try
            {
                Time ceTime = new Time(ce);
                Time cbTime = new Time(cb);
                if (cbTime.IsNegativeTimeOffset)
                {
                    ClipBegin = cbTime;
                    ClipEnd = ceTime;
                }
                else
                {
                    ClipEnd = ceTime;
                    ClipBegin = cbTime;
                }
            }
            catch (exception.TimeStringRepresentationIsInvalidException e)
            {
                throw new exception.XukException("Invalid time string encountered", e);
            }
            catch (exception.MethodParameterIsOutOfBoundsException e)
            {
                throw new exception.XukException("Out of bounds time encountered", e);
            }
            string height = source.GetAttribute(XukStrings.Height);
            string width = source.GetAttribute(XukStrings.Width);
            int h, w;
            if (height != null && height != "")
            {
                if (!Int32.TryParse(height, out h))
                {
                    throw new exception.XukException("height attribute is not an integer");
                }
                Height = h;
            }
            else
            {
                Height = 0;
            }
            if (width != null && width != "")
            {
                if (!Int32.TryParse(width, out w))
                {
                    throw new exception.XukException("width attribute is not an integer");
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
        /// Writes the attributes of a ExternalVideoMedia element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        /// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            if (Src != "")
            {
                Uri srcUri = new Uri(MediaFactory.Presentation.RootUri, Src);
                if (baseUri == null)
                {
                    destination.WriteAttributeString(XukStrings.Src, srcUri.AbsoluteUri);
                }
                else
                {
                    destination.WriteAttributeString(XukStrings.Src, baseUri.MakeRelativeUri(srcUri).ToString());
                }
            }
            destination.WriteAttributeString(XukStrings.ClipBegin, this.ClipBegin.ToString());
            destination.WriteAttributeString(XukStrings.ClipEnd, this.ClipEnd.ToString());
            destination.WriteAttributeString(XukStrings.Height, this.Height.ToString());
            destination.WriteAttributeString(XukStrings.Width, this.Width.ToString());
            base.XukOutAttributes(destination, baseUri);
        }

        #endregion

        #region IContinuous Members

        /// <summary>
        /// Gets the duration of <c>this</c>
        /// </summary>
        /// <returns>The duration</returns>
        public override TimeDelta Duration
        {
            get { return ClipEnd.GetTimeDelta(ClipBegin); }
        }

        /// <summary>
        /// Splits <c>this</c> at a given <see cref="Time"/>
        /// </summary>
        /// <param name="splitPoint">The <see cref="Time"/> at which to split - 
        /// must be between clip begin and clip end <see cref="Time"/>s</param>
        /// <returns>
        /// A newly created <see cref="ExternalAudioMedia"/> containing the audio after <paramref localName="splitPoint"/>,
        /// <c>this</c> retains the audio before <paramref localName="splitPoint"/>.
        /// </returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="splitPoint"/> is <c>null</c>
        /// </exception>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when <paramref name="splitPoint"/> is not between clip begin and clip end
        /// </exception>
        public new ExternalVideoMedia Split(Time splitPoint)
        {
            return SplitProtected(splitPoint) as ExternalVideoMedia;
        }

        /// <summary>
        /// Splits <c>this</c> at a given <see cref="Time"/>
        /// </summary>
        /// <param name="splitPoint">The <see cref="Time"/> at which to split - 
        /// must be between clip begin and clip end <see cref="Time"/>s</param>
        /// <returns>
        /// A newly created <see cref="AbstractAudioMedia"/> containing the audio after <paramref localName="splitPoint"/>,
        /// <c>this</c> retains the audio before <paramref localName="splitPoint"/>.
        /// </returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="splitPoint"/> is <c>null</c>
        /// </exception>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when <paramref name="splitPoint"/> is not between clip begin and clip end
        /// </exception>
        protected override AbstractVideoMedia SplitProtected(Time splitPoint)
        {

            if (splitPoint == null)
            {
                throw new exception.MethodParameterIsNullException(
                    "The time at which to split can not be null");
            }
            if (splitPoint.IsLessThan(ClipBegin))
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The split time can not be before ClipBegin");
            }
            if (splitPoint.IsGreaterThan(ClipEnd))
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The split time can not be after ClipEnd");
            }
            ExternalVideoMedia splitAM = Copy();
            ClipEnd = splitPoint;
            splitAM.ClipBegin = splitPoint;
            return splitAM;
        }


        #endregion
        
        #region IClipped Members

        /// <summary>
        /// Gets the clip begin <see cref="Time"/> of <c>this</c>
        /// </summary>
        /// <returns>Clip begin</returns>
        public Time ClipBegin
        {
            get { return mClipBegin; }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException("ClipBegin can not be null");
                }
                if (value.IsLessThan(Time.Zero))
                {
                    throw new exception.MethodParameterIsOutOfBoundsException(
                        "ClipBegin is a negative time offset");
                }
                if (value.IsGreaterThan(ClipEnd))
                {
                    throw new exception.MethodParameterIsOutOfBoundsException(
                        "ClipBegin can not be after ClipEnd");
                }
                if (!mClipBegin.IsEqualTo(value))
                {
                    Time prevCB = ClipBegin;
                    mClipBegin = value.Copy();
                    NotifyClipChanged(ClipBegin, ClipEnd, prevCB, ClipEnd);
                }
            }
        }

        /// <summary>
        /// Gets the clip end <see cref="Time"/> of <c>this</c>
        /// </summary>
        /// <returns>Clip end</returns>
        public Time ClipEnd
        {
            get { return mClipEnd; }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException("ClipEnd can not be null");
                }
                if (value.IsLessThan(ClipBegin))
                {
                    throw new exception.MethodParameterIsOutOfBoundsException(
                        "ClipEnd can not be before ClipBegin");
                }
                if (!mClipEnd.IsEqualTo(value))
                {
                    Time prevCE = ClipEnd;
                    mClipEnd = value.Copy();
                    NotifyClipChanged(ClipBegin, ClipEnd, ClipBegin, prevCE);
                }
            }
        }

        /// <summary>
        /// Event fired after the clip (clip begin or clip end) of the <see cref="ExternalAudioMedia"/> has changed
        /// </summary>
        public event EventHandler<events.media.ClipChangedEventArgs> ClipChanged;

        /// <summary>
        /// Fires the <see cref="ClipChanged"/> event
        /// </summary>
        /// <param name="newCB">The new clip begin value</param>
        /// <param name="newCE">The new clip begin value</param>
        /// <param name="prevCB">The clip begin value prior to the change</param>
        /// <param name="prevCE">The clip end value prior to the change</param>
        protected void NotifyClipChanged(Time newCB, Time newCE, Time prevCB, Time prevCE)
        {
            EventHandler<events.media.ClipChangedEventArgs> d = ClipChanged;
            if (d != null) d(this, new urakawa.events.media.ClipChangedEventArgs(this, newCB, newCE, prevCB, prevCE));
        }

        private void this_ClipChanged(object sender, urakawa.events.media.ClipChangedEventArgs e)
        {
            NotifyChanged(e);
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
        
        #region IValueEquatable<Media> Members

        /// <summary>
        /// Conpares <c>this</c> with a given other <see cref="Media"/> for equality
        /// </summary>
        /// <param name="other">The other <see cref="Media"/></param>
        /// <returns><c>true</c> if equal, otherwise <c>false</c></returns>
        public override bool ValueEquals(Media other)
        {
            if (!base.ValueEquals(other))
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            ExternalVideoMedia otherVideo = (ExternalVideoMedia) other;
            if (Src != otherVideo.Src)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            if (!ClipBegin.IsEqualTo(otherVideo.ClipBegin))
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            if (!ClipEnd.IsEqualTo(otherVideo.ClipEnd))
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            if (Width != otherVideo.Width)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            if (Height != otherVideo.Height)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !");
                return false;
            }
            return true;
        }

        #endregion


    }
}