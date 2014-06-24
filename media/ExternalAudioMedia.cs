using System;
using System.Xml;
using urakawa.events.media;
using urakawa.media.timing;
using urakawa.xuk;

namespace urakawa.media
{
    [XukNameUglyPrettyAttribute("exAuMed", "ExternalAudioMedia")]
    public class ExternalAudioMedia : AbstractAudioMedia, ILocated, IClipped
    {

        #region IValueEquatable<Media> Members

        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            ExternalAudioMedia otherz = other as ExternalAudioMedia;
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

        public const string DEFAULT_SRC = "file.ext";
        
        private string mSrc;
        private Time mClipBegin;
        private Time mClipEnd;

        private void Reset()
        {
            mSrc = DEFAULT_SRC;
            mClipBegin = Time.Zero;
            mClipEnd = Time.MaxValue;
        }

        /// <summary>
        /// Default constructor - for system use only, 
        /// <see cref="ExternalAudioMedia"/>s should only be created via. the <see cref="MediaFactory"/>
        /// </summary>
        public ExternalAudioMedia()
        {
            Reset();
            SrcChanged += this_SrcChanged;
            ClipChanged += this_ClipChanged;
        }

        #region Media members


        /// <summary>
        /// Copy function which returns an <see cref="AbstractAudioMedia"/> object
        /// </summary>
        /// <returns>A copy of this</returns>
        /// <exception cref="exception.FactoryCannotCreateTypeException">
        /// Thrown when the <see cref="MediaFactory"/> associated with this 
        /// can not create an <see cref="ExternalAudioMedia"/> matching the QName of <see cref="ExternalAudioMedia"/>
        /// </exception>
        public new ExternalAudioMedia Copy()
        {
            return CopyProtected() as ExternalAudioMedia;
        }

   
        /// <summary>
        /// Exports the external audio media to a destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported external audio media</returns>
        public new ExternalAudioMedia Export(Presentation destPres)
        {
            return ExportProtected(destPres) as ExternalAudioMedia;
        }

        ///<summary>
        ///
        ///</summary>
        ///<returns></returns>
        protected override Media CopyProtected()
        {
            ExternalAudioMedia copy = (ExternalAudioMedia)base.CopyProtected();
            copy.Src = Src;
            copy.ClipBegin = ClipBegin.Copy();
            copy.ClipEnd = ClipEnd.Copy();
            return copy;
        }

        /// <summary>
        /// Exports the external audio media to a destination <see cref="Presentation"/>
        /// - part of technical construct to have <see cref="Export"/> return <see cref="ExternalAudioMedia"/>
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported external audio media</returns>
        protected override Media ExportProtected(Presentation destPres)
        {
            ExternalAudioMedia exported = (ExternalAudioMedia)base.ExportProtected(destPres);
            exported.Src = Src;
            exported.ClipBegin = ClipBegin.Copy();
            exported.ClipEnd = ClipEnd.Copy();
            return exported;
        }

        #endregion

        #region IXUKAble members

        /// <summary>
        /// Clears the data of the <see cref="ExternalAudioMedia"/>
        /// </summary>
        protected override void Clear()
        {
            Reset();
            base.Clear();
        }

        /// <summary>
        /// Reads the attributes of a ExternalAudioMedia xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

            string val = source.GetAttribute(XukStrings.Src);
            if (string.IsNullOrEmpty(val)) val = DEFAULT_SRC;
            Src = val;
            Time cbTime, ceTime;
            try
            {
                cbTime = new Time(source.GetAttribute(XukStrings.ClipBegin));
            }
            catch (exception.CheckedException e)
            {
                throw new exception.XukException(
                    String.Format("clipBegin attribute is missing or has invalid value: {0}", e.Message),
                    e);
            }
            try
            {
                ceTime = new Time(source.GetAttribute(XukStrings.ClipEnd));
            }
            catch (exception.CheckedException e)
            {
                throw new exception.XukException(
                    String.Format("clipEnd attribute is missing or has invalid value: {0}", e.Message),
                    e);
            }
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

        /// <summary>
        /// Writes the attributes of a ExternalAudioMedia element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
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
            
        }


        #endregion

        #region IContinuous Members

        /// <summary>
        /// Gets the duration of <c>this</c>
        /// </summary>
        /// <returns>A <see cref="Time"/> representing the duration</returns>
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
        public new ExternalAudioMedia Split(Time splitPoint)
        {
            return SplitProtected(splitPoint) as ExternalAudioMedia;
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
        protected override AbstractAudioMedia SplitProtected(Time splitPoint)
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
            ExternalAudioMedia splitAM = Copy();
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
                if (value.IsGreaterThan(ClipEnd) )
                {
                    throw new exception.MethodParameterIsOutOfBoundsException(
                        "ClipBegin can not be after ClipEnd");
                }
                if (!mClipBegin.IsEqualTo(value))
                {
                    Time prevCB = ClipBegin;
                    mClipBegin = value.Copy();
                    NotifyClipChanged(this, ClipBegin, ClipEnd, prevCB, ClipEnd);
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
                    NotifyClipChanged(this, ClipBegin, ClipEnd, ClipBegin, prevCE);
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
        /// <param name="source">The source, that is the <see cref="ExternalAudioMedia"/> whoose clip has changed</param>
        /// <param name="newCB">The new clip begin value</param>
        /// <param name="newCE">The new clip begin value</param>
        /// <param name="prevCB">The clip begin value prior to the change</param>
        /// <param name="prevCE">The clip end value prior to the change</param>
        protected void NotifyClipChanged(ExternalAudioMedia source, Time newCB, Time newCE, Time prevCB, Time prevCE)
        {
            EventHandler<events.media.ClipChangedEventArgs> d = ClipChanged;
            if (d != null) d(this, new urakawa.events.media.ClipChangedEventArgs(source, newCB, newCE, prevCB, prevCE));
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
                if(value == null)
                {
                    throw new exception.MethodParameterIsNullException("The src value cannot be null or empty");
                }
                if(value == "")
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
                //if (!Uri.IsWellFormedUriString(Src, UriKind.RelativeOrAbsolute))
                //{
                //    throw new exception.InvalidUriException(String.Format(
                //                                                "The src value '{0}' is not a well-formed Uri", Src));
                //}
                return new Uri(Presentation.RootUri, Src);
            }
        }

        #endregion


    }
}