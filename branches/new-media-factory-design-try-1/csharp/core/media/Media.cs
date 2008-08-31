using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace urakawa.media
{
    /// <summary>
    /// Abstract implementation of <see cref="IMedia"/> - used as base of all <see cref="Media"/>
    /// </summary>
    public abstract class Media : WithPresentation, IMedia
    {
        #region Event related members

        /// <summary>
        /// Event fired after the <see cref="TextMedia"/> has changed. 
        /// The event fire before any change specific event 
        /// </summary>
        public event EventHandler<urakawa.events.DataModelChangedEventArgs> Changed;

        /// <summary>
        /// Fires the <see cref="Changed"/> event 
        /// </summary>
        /// <param name="args">The arguments of the event</param>
        protected void NotifyChanged(urakawa.events.DataModelChangedEventArgs args)
        {
            EventHandler<urakawa.events.DataModelChangedEventArgs> d = Changed;
            if (d != null) d(this, args);
        }

        /// <summary>
        /// Event fired after the language of the <see cref="TextMedia"/> has changed
        /// </summary>
        public event EventHandler<urakawa.events.LanguageChangedEventArgs> LanguageChanged;

        /// <summary>
        /// Fires the <see cref="LanguageChanged"/> event
        /// </summary>
        /// <param name="source">The source, that is the <see cref="TextMedia"/> whoose language changed</param>
        /// <param name="newLang">The new value for the language</param>
        /// <param name="prevLang">The value for the language prior to the change</param>
        protected void NotifyLanguageChanged(Media source, string newLang, string prevLang)
        {
            EventHandler<urakawa.events.LanguageChangedEventArgs> d = LanguageChanged;
            if (d != null) d(this, new urakawa.events.LanguageChangedEventArgs(source, newLang, prevLang));
        }

        private void this_languageChanged(object sender, urakawa.events.LanguageChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        protected Media()
        {
            mLanguage = null;
            this.LanguageChanged += new EventHandler<urakawa.events.LanguageChangedEventArgs>(this_languageChanged);
        }

        private string mLanguage;

        #region Media Members

        /// <summary>
        /// Gets the <see cref="IMediaFactory"/> associated with the <see cref="Media"/> (via. the owning <see cref="Presentation"/>
        /// </summary>
        /// <returns>The <see cref="IMediaFactory"/></returns>
        public IMediaFactory MediaFactory
        {
            get { return Presentation.MediaFactory; }
        }

        /// <summary>
        /// Determines if the <see cref="Media"/> is continuous
        /// </summary>
        /// <returns>A <see cref="bool"/> indicating if the <see cref="Media"/> is continuous</returns>
        public abstract bool IsContinuous { get; }

        /// <summary>
        /// Determines if the <see cref="Media"/> is discrete
        /// </summary>
        /// <returns>A <see cref="bool"/> indicating if the <see cref="Media"/> is discrete</returns>
        public abstract bool IsDiscrete { get; }

        /// <summary>
        /// Determines if the <see cref="Media"/> is a <see cref="SequenceMedia"/>
        /// </summary>
        /// <returns>A <see cref="bool"/> indicating if the <see cref="Media"/> is a <see cref="SequenceMedia"/></returns>
        public abstract bool IsSequence { get; }

        Media IMedia.Copy()
        {
            return CopyProtected();
        }

        /// <summary>
        /// Creates a copy of the <see cref="Media"/>
        /// </summary>
        /// <returns>The copy</returns>
        public Media Copy()
        {
            return CopyProtected() as Media;
        }

        /// <summary>
        /// Creates a copy of the <see cref="Media"/>
        /// </summary>
        /// <returns>The copy</returns>
        protected virtual Media CopyProtected()
        {
            return ExportProtected(Presentation);
        }

        Media IMedia.Export(Presentation destPres)
        {
            return ExportProtected(destPres);
        }

        /// <summary>
        /// Exports the <see cref="Media"/> to a given destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination <see cref="Presentation"/></param>
        /// <returns>The exported <see cref="Media"/></returns>
        public Media Export(Presentation destPres)
        {
            return ExportProtected(destPres) as Media;
        }

        /// <summary>
        /// Exports the <see cref="Media"/> to a given destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination <see cref="Presentation"/></param>
        /// <returns>The exported <see cref="Media"/></returns>
        protected virtual Media ExportProtected(Presentation destPres)
        {
            Media expMedia = destPres.MediaFactory.CreateMedia(XukLocalName, XukNamespaceUri) as Media;
            if (expMedia == null)
            {
                throw new exception.FactoryCannotCreateTypeException(String.Format(
                                                                         "The factory of the destination Presentation cannot create a Media matching Xuk QName {1}:{0}",
                                                                         XukLocalName, XukNamespaceUri));
            }
            expMedia.Language = Language;
            return expMedia;
        }

        /// <summary>
        /// Gets or sets the language of the media
        /// </summary>
        /// <returns>The language</returns>
        public string Language
        {
            get { return mLanguage; }
            set
            {
                if (value == "")
                {
                    throw new exception.MethodParameterIsEmptyStringException(
                        "The language can not be an empty string");
                }
                string prevlang = mLanguage;
                mLanguage = value;
                if (prevlang != mLanguage) NotifyLanguageChanged(this, mLanguage, prevlang);
            }
        }

        #endregion

        /// <summary>
        /// Clears the data of the <see cref="Media"/>
        /// </summary>
        protected override void Clear()
        {
            mLanguage = null;
            base.Clear();
        }


        /// <summary>
        /// Reads the attributes of a Media xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void XukInAttributes(XmlReader source)
        {
            string lang = source.GetAttribute("language");
            if (lang != null) lang = lang.Trim();
            if (lang == "") lang = null;
            Language = lang;
            base.XukInAttributes(source);
        }

        /// <summary>
        /// Writes the attributes of a Media element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            if (Language != null) destination.WriteAttributeString("language", Language);

            base.XukOutAttributes(destination, baseUri);
        }

        #region IValueEquatable<Media> Members

        /// <summary>
        /// Compares <c>this</c> with a given other <see cref="IMedia"/> for equality
        /// </summary>
        /// <param name="other">The other <see cref="IMedia"/></param>
        /// <returns><c>true</c> if equal, otherwise <c>false</c></returns>
        public virtual bool ValueEquals(Media other)
        {
            if (other == null) return false;
            if (this.GetType() != other.GetType()) return false;
            if (this.Language != other.Language) return false;
            return true;
        }

        #endregion
    }
}