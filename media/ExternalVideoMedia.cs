using System;
using System.Xml;
using urakawa.events.media;
using urakawa.media.timing;
using urakawa.xuk;

namespace urakawa.media
{
    [XukNameUglyPrettyAttribute("exVidMed", "ExternalVideoMedia")]
    public class ExternalVideoMedia : AbstractVideoMedia, IClipped, ILocated
    {
        public const string DEFAULT_SRC = "file.ext";
        
        private string mSrc;
        private int mWidth;
        private int mHeight;
        private Time mClipBegin;
        private Time mClipEnd;

        private void Reset()
        {
            mSrc = DEFAULT_SRC;
            mWidth = 0;
            mHeight = 0;
            mClipBegin = Time.Zero;
            mClipEnd = Time.MaxValue;
        }

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
            if (ClipBegin.IsNegative)
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
            if (ClipBegin.IsNegative)
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
            base.XukInAttributes(source);

            string val = source.GetAttribute(XukStrings.Src);
            if (string.IsNullOrEmpty(val)) val = DEFAULT_SRC;
            Src = val;
            string cb = source.GetAttribute(XukStrings.ClipBegin);
            string ce = source.GetAttribute(XukStrings.ClipEnd);
            try
            {
                Time ceTime = new Time(ce);
                Time cbTime = new Time(cb);
                if (cbTime.IsNegative)
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
            if (!string.IsNullOrEmpty(height))
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
            if (!string.IsNullOrEmpty(width))
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
            base.XukOutAttributes(destination, baseUri);

            if (Src != "")
            {
                Uri srcUri = new Uri(Presentation.RootUri, Src);
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
            
        }

        #endregion

        #region IContinuous Members

        /// <summary>
        /// Gets the duration of <c>this</c>
        /// </summary>
        /// <returns>The duration</returns>
        public override Time Duration
        {
            get { return ClipEnd.GetDifference(ClipBegin); }
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
        /// Gets or sets the src value. The default value is DEFAULT_SRC
        /// </summary>
        /// <exception cref="exception.MethodParameterIsEmptyStringException">
        /// Thrown when trying to set the <see cref="Src"/> value to <c>null</c></exception>
        public string Src
        {
            get { return mSrc; }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException("The src value cannot be null or empty");
                }
                if (value == "")
                {
                    throw new exception.MethodParameterIsEmptyStringException("The src value cannot be null or empty");
                }
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
                return new Uri(Presentation.RootUri, Src);
            }
        }

        #endregion
        
        #region IValueEquatable<Media> Members

        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            ExternalVideoMedia otherz = other as ExternalVideoMedia;
            if (otherz == null)
            {
                return false;
            }

            if (Src != otherz.Src)
            {
                return false;
            }

            if (!ClipBegin.IsEqualTo(otherz.ClipBegin))
            {
                return false;
            }
            if (!ClipEnd.IsEqualTo(otherz.ClipEnd))
            {
                return false;
            }

            return true;
        }

        #endregion


    }
}