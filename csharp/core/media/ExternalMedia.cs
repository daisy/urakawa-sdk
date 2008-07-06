using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using urakawa.events.media;

namespace urakawa.media
{
    /// <summary>
    /// Common abstract base class for external (ie. <see cref="ILocated"/> <see cref="IMedia"/>
    /// </summary>
    public abstract class ExternalMedia : AbstractMedia, ILocated
    {
        #region Event related members

        /// <summary>
        /// Event fired after the src has changed
        /// </summary>
        public event EventHandler<SrcChangedEventArgs> SrcChanged;

        /// <summary>
        /// Fires the <see cref="SrcChanged"/> event
        /// </summary>
        /// <param name="source">The source, that is the <see cref="ExternalMedia"/> whoose src value changed</param>
        /// <param name="newVal">The new src value</param>
        /// <param name="prevVal">The src value prior to the change</param>
        protected void notifySrcChanged(ExternalMedia source, string newVal, string prevVal)
        {
            EventHandler<SrcChangedEventArgs> d = SrcChanged;
            if (d != null) d(this, new SrcChangedEventArgs(source, newVal, prevVal));
        }

        private void this_srcChanged(object sender, SrcChangedEventArgs e)
        {
            notifyChanged(e);
        }

        #endregion

        private string mSrc;

        internal ExternalMedia()
        {
            mSrc = ".";
            this.SrcChanged += new EventHandler<SrcChangedEventArgs>(this_srcChanged);
        }

        #region IMedia Members

        /// <summary>
        /// Creates a copy of the <see cref="ExternalMedia"/>
        /// </summary>
        /// <returns>The copy</returns>
        public new ExternalMedia Copy()
        {
            return copyProtected() as ExternalMedia;
        }

        /// <summary>
        /// Exports the <see cref="ExternalMedia"/> to a given destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination <see cref="Presentation"/></param>
        /// <returns>The exported <see cref="ExternalMedia"/></returns>
        /// <remarks>The current instance is left untouched to the export</remarks>
        public new ExternalMedia Export(Presentation destPres)
        {
            return exportProtected(destPres) as ExternalMedia;
        }

        /// <summary>
        /// Exports the <see cref="ExternalMedia"/> to a given destination <see cref="Presentation"/>
        /// - part of a technical solution to have the <see cref="Export"/> method return the correct <see cref="Type"/>
        /// </summary>
        /// <param name="destPres">The destination <see cref="Presentation"/></param>
        /// <returns>The exported <see cref="ExternalMedia"/></returns>
        /// <remarks>The current instance is left untouched to the export</remarks>
        protected override IMedia exportProtected(Presentation destPres)
        {
            ExternalMedia expEM = base.exportProtected(destPres) as ExternalMedia;
            if (expEM == null)
            {
                throw new exception.FactoryCannotCreateTypeException(String.Format(
                                                                         "The MediaFactory cannot create a ExternalMedia matching QName {1}:{0}",
                                                                         XukLocalName, XukNamespaceUri));
            }
            if (Uri.IsWellFormedUriString(Src, UriKind.Relative))
            {
                string destSrc = destPres.RootUri.MakeRelativeUri(Uri).ToString();
                if (destSrc == "") destSrc = ".";
                expEM.Src = destSrc;
            }
            else
            {
                expEM.Src = Src;
            }
            return expEM;
        }

        #endregion

        #region IXUKAble members

        /// <summary>
        /// Clears to <see cref="ExternalMedia"/>, resetting the src value
        /// </summary>
        protected override void clear()
        {
            mSrc = ".";
            base.clear();
        }

        /// <summary>
        /// Reads the attributes of a ExternalMedia xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void xukInAttributes(XmlReader source)
        {
            string val = source.GetAttribute("src");
            if (val == null || val == "") val = ".";
            Src = val;
            base.xukInAttributes(source);
        }

        /// <summary>
        /// Writes the attributes of a ExternalMedia element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void xukOutAttributes(XmlWriter destination, Uri baseUri)
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
            base.xukOutAttributes(destination, baseUri);
        }

        #endregion

        #region IValueEquatable<IMedia> Members

        /// <summary>
        /// Determines if the <see cref="ExternalMedia"/> has the same value as a given other <see cref="IMedia"/>
        /// </summary>
        /// <param name="other">The other <see cref="IMedia"/></param>
        /// <returns>A <see cref="bool"/> indicating value equality</returns>
        public override bool ValueEquals(IMedia other)
        {
            if (!base.ValueEquals(other)) return false;
            ExternalMedia emOther = (ExternalMedia) other;
            if (Uri != emOther.Uri) return false;
            return true;
        }

        #endregion

        #region ILocated Members

        /// <summary>
        /// Gets the src value. The default value is "."
        /// </summary>
        /// <returns>The src value</returns>
        public string Src
        {
            get { return mSrc; }
            set
            {
                if (value == null) throw new exception.MethodParameterIsNullException("The src value can not be null");
                if (value == "")
                    throw new exception.MethodParameterIsEmptyStringException("The src value can not be an empty string");
                string prevSrc = mSrc;
                mSrc = value;
                if (mSrc != prevSrc) notifySrcChanged(this, mSrc, prevSrc);
            }
        }

        /// <summary>
        /// Gets the <see cref="Uri"/> of the <see cref="ExternalMedia"/> 
        /// - uses <c>getMediaFactory().getPresentation().getRootUri()</c> as base <see cref="Uri"/>
        /// </summary>
        /// <returns>The <see cref="Uri"/></returns>
        /// <exception cref="exception.InvalidUriException">
        /// Thrown when the value <see cref="Src"/> is not a well-formed <see cref="Uri"/>
        /// </exception>
        public Uri Uri
        {
            get
            {
                if (!Uri.IsWellFormedUriString(Src, UriKind.RelativeOrAbsolute))
                {
                    throw new exception.InvalidUriException(String.Format(
                                                                "The src value '{0}' is not a well-formed Uri", Src));
                }
                return new Uri(MediaFactory.Presentation.RootUri, Src);
            }
        }

        #endregion
    }
}