using System;
using System.Xml;
using urakawa.media.timing;

namespace urakawa.media
{
    /// <summary>
    /// VideoMedia is the video object.
    /// It is time-based, comes from an external source, and has a visual presence.
    /// </summary>
    public class ExternalVideoMedia : ExternalMedia, IVideoMedia
    {
        #region Event related members

        /// <summary>
        /// Event fired after the clip (clip begin or clip end) of the <see cref="ExternalAudioMedia"/> has changed
        /// </summary>
        public event EventHandler<events.media.ClipChangedEventArgs> ClipChanged;

        /// <summary>
        /// Fires the <see cref="ClipChanged"/> event
        /// </summary>
        /// <param name="source">The source, that is the <see cref="ExternalVideoMedia"/> whoose clip has changed</param>
        /// <param name="newCB">The new clip begin value</param>
        /// <param name="newCE">The new clip begin value</param>
        /// <param name="prevCB">The clip begin value prior to the change</param>
        /// <param name="prevCE">The clip end value prior to the change</param>
        protected void notifyClipChanged(ExternalVideoMedia source, Time newCB, Time newCE, Time prevCB, Time prevCE)
        {
            EventHandler<events.media.ClipChangedEventArgs> d = ClipChanged;
            if (d != null) d(this, new urakawa.events.media.ClipChangedEventArgs(source, newCB, newCE, prevCB, prevCE));
        }

        private void this_clipChanged(object sender, urakawa.events.media.ClipChangedEventArgs e)
        {
            notifyChanged(e);
        }

        /// <summary>
        /// Event fired after the size (height or width) of the <see cref="ExternalVideoMedia"/> has changed
        /// </summary>
        public event EventHandler<events.media.SizeChangedEventArgs> SizeChanged;

        /// <summary>
        /// Fires the <see cref="SizeChanged"/> event
        /// </summary>
        /// <param name="source">The source, that is the <see cref="ExternalVideoMedia"/> whoose size has changed</param>
        /// <param name="newHeight">The new height of the <see cref="ExternalVideoMedia"/></param>
        /// <param name="newWidth">The new width of the <see cref="ExternalVideoMedia"/></param>
        /// <param name="prevHeight">The height of the <see cref="ExternalVideoMedia"/> prior to the change</param>
        /// <param name="prevWidth">The width of the <see cref="ExternalVideoMedia"/> prior to the change</param>
        protected void notifySizeChanged(ExternalVideoMedia source, int newHeight, int newWidth, int prevHeight,
                                         int prevWidth)
        {
            EventHandler<events.media.SizeChangedEventArgs> d = SizeChanged;
            if (d != null)
                d(this,
                  new urakawa.events.media.SizeChangedEventArgs(source, newHeight, newWidth, prevHeight, prevWidth));
        }

        private void this_sizeChanged(object sender, urakawa.events.media.SizeChangedEventArgs e)
        {
            notifyChanged(e);
        }

        #endregion

        private int mWidth = 0;
        private int mHeight = 0;
        private Time mClipBegin;
        private Time mClipEnd;

        private void resetClipTimes()
        {
            mClipBegin = Time.Zero;
            mClipEnd = Time.MaxValue;
            this.ClipChanged += new EventHandler<urakawa.events.media.ClipChangedEventArgs>(this_clipChanged);
            this.SizeChanged += new EventHandler<urakawa.events.media.SizeChangedEventArgs>(this_sizeChanged);
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        protected internal ExternalVideoMedia() : base()
        {
            mWidth = 0;
            mHeight = 0;
            resetClipTimes();
        }

        #region IMedia Members

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
        protected override IMedia copyProtected()
        {
            return Export(MediaFactory.Presentation);
        }

        /// <summary>
        /// Copy function which returns an <see cref="ExternalVideoMedia"/> object
        /// </summary>
        /// <returns>a copy of this</returns>
        public new ExternalVideoMedia Copy()
        {
            return copyProtected() as ExternalVideoMedia;
        }

        /// <summary>
        /// Exports the external video media to a destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported external video media</returns>
        protected override IMedia exportProtected(Presentation destPres)
        {
            ExternalVideoMedia exported = base.exportProtected(destPres) as ExternalVideoMedia;
            if (exported == null)
            {
                throw new exception.FactoryCannotCreateTypeException(String.Format(
                                                                         "The MediaFactory cannot create a ExternalVideoMedia matching QName {1}:{0}",
                                                                         XukLocalName, XukNamespaceUri));
            }
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
            return exportProtected(destPres) as ExternalVideoMedia;
        }

        #endregion

        #region ISized Members

        /// <summary>
        /// Return the video width
        /// </summary>
        /// <returns>The width</returns>
        public int Width
        {
            get { return mWidth; }
            set { SetSize(Height, value); }
        }

        /// <summary>
        /// Return the video height
        /// </summary>
        /// <returns>The height</returns>
        public int Height
        {
            get { return mHeight; }
            set { SetSize(value, Width); }
        }


        /// <summary>
        /// Sets the video size
        /// </summary>
        /// <param name="height">The new height</param>
        /// <param name="width">The new width</param>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when the new width or height is negative
        /// </exception>
        public void SetSize(int height, int width)
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
                notifySizeChanged(this, mHeight, mWidth, prevHeight, prevWidth);
            }
        }

        #endregion

        #region IXUKAble members

        /// <summary>
        /// Reads the attributes of a VideoMedia xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void xukInAttributes(XmlReader source)
        {
            base.xukInAttributes(source);
            string cb = source.GetAttribute("clipBegin");
            string ce = source.GetAttribute("clipEnd");
            resetClipTimes();
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
            string height = source.GetAttribute("height");
            string width = source.GetAttribute("width");
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
        }

        /// <summary>
        /// Writes the attributes of a VideoMedia element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        /// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
        protected override void xukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            destination.WriteAttributeString("clipBegin", this.ClipBegin.ToString());
            destination.WriteAttributeString("clipEnd", this.ClipEnd.ToString());
            destination.WriteAttributeString("height", this.Height.ToString());
            destination.WriteAttributeString("width", this.Width.ToString());
            base.xukOutAttributes(destination, baseUri);
        }

        #endregion

        #region IContinuous Members

        /// <summary>
        /// Gets the duration of <c>this</c>
        /// </summary>
        /// <returns>The duration</returns>
        public TimeDelta Duration
        {
            get { return ClipEnd.GetTimeDelta(ClipBegin); }
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
                    notifyClipChanged(this, ClipBegin, ClipEnd, prevCB, ClipEnd);
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
                    notifyClipChanged(this, ClipBegin, ClipEnd, ClipBegin, prevCE);
                }
            }
        }


        IContinuous IContinuous.Split(Time splitPoint)
        {
            return Split(splitPoint);
        }

        /// <summary>
        /// Splits <c>this</c> at a given split point in <see cref="Time"/>. 
        /// The retains the clip between clip begin and the split point and a new <see cref="IVideoMedia"/>
        /// is created consisting of the clip from the split point to clip end
        /// </summary>
        /// <param name="splitPoint">The split point</param>
        /// <returns>The new <see cref="IVideoMedia"/> containing the latter prt of the clip</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="splitPoint"/> is <c>null</c>
        /// </exception>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when <paramref name="splitPoint"/> is not between clip begin and clip end
        /// </exception>
        public ExternalVideoMedia Split(Time splitPoint)
        {
            if (splitPoint == null)
            {
                throw new exception.MethodParameterIsNullException("The split point can not be null");
            }
            if (ClipBegin.IsGreaterThan(splitPoint) || splitPoint.IsGreaterThan(ClipEnd))
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The split point is not between clip begin and clip end");
            }
            ExternalVideoMedia secondPart = Copy();
            secondPart.ClipBegin = splitPoint.Copy();
            ClipEnd = splitPoint.Copy();
            return secondPart;
        }

        #endregion

        #region IValueEquatable<IMedia> Members

        /// <summary>
        /// Conpares <c>this</c> with a given other <see cref="IMedia"/> for equality
        /// </summary>
        /// <param name="other">The other <see cref="IMedia"/></param>
        /// <returns><c>true</c> if equal, otherwise <c>false</c></returns>
        public override bool ValueEquals(IMedia other)
        {
            if (!base.ValueEquals(other)) return false;
            ExternalVideoMedia otherVideo = (ExternalVideoMedia) other;
            if (!ClipBegin.IsEqualTo(otherVideo.ClipBegin)) return false;
            if (!ClipEnd.IsEqualTo(otherVideo.ClipEnd)) return false;
            if (Width != otherVideo.Width) return false;
            if (Height != otherVideo.Height) return false;
            return true;
        }

        #endregion
    }
}