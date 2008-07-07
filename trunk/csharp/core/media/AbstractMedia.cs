using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace urakawa.media
{
    /// <summary>
    /// Abstract implementation of <see cref="IMedia"/> - used as bvase of all <see cref="IMedia"/>
    /// </summary>
    public abstract class AbstractMedia : WithPresentation, IMedia
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
        protected void NotifyLanguageChanged(AbstractMedia source, string newLang, string prevLang)
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
        public AbstractMedia()
        {
            mLanguage = null;
            this.LanguageChanged += new EventHandler<urakawa.events.LanguageChangedEventArgs>(this_languageChanged);
        }

        private string mLanguage;

        #region IMedia Members

        /// <summary>
        /// Gets the <see cref="IMediaFactory"/> associated with the <see cref="AbstractMedia"/> (via. the owning <see cref="Presentation"/>
        /// </summary>
        /// <returns>The <see cref="IMediaFactory"/></returns>
        public IMediaFactory MediaFactory
        {
            get { return Presentation.MediaFactory; }
        }

        /// <summary>
        /// Determines if the <see cref="AbstractMedia"/> is continuous
        /// </summary>
        /// <returns>A <see cref="bool"/> indicating if the <see cref="AbstractMedia"/> is continuous</returns>
        public abstract bool IsContinuous { get; }

        /// <summary>
        /// Determines if the <see cref="AbstractMedia"/> is discrete
        /// </summary>
        /// <returns>A <see cref="bool"/> indicating if the <see cref="AbstractMedia"/> is discrete</returns>
        public abstract bool IsDiscrete { get; }

        /// <summary>
        /// Determines if the <see cref="AbstractMedia"/> is a <see cref="SequenceMedia"/>
        /// </summary>
        /// <returns>A <see cref="bool"/> indicating if the <see cref="AbstractMedia"/> is a <see cref="SequenceMedia"/></returns>
        public abstract bool IsSequence { get; }

        IMedia IMedia.Copy()
        {
            return CopyProtected();
        }

        /// <summary>
        /// Creates a copy of the <see cref="AbstractMedia"/>
        /// </summary>
        /// <returns>The copy</returns>
        public AbstractMedia Copy()
        {
            return CopyProtected() as AbstractMedia;
        }

        /// <summary>
        /// Creates a copy of the <see cref="AbstractMedia"/>
        /// </summary>
        /// <returns>The copy</returns>
        protected virtual IMedia CopyProtected()
        {
            return ExportProtected(Presentation);
        }

        IMedia IMedia.Export(Presentation destPres)
        {
            return ExportProtected(destPres);
        }

        /// <summary>
        /// Exports the <see cref="AbstractMedia"/> to a given destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination <see cref="Presentation"/></param>
        /// <returns>The exported <see cref="AbstractMedia"/></returns>
        public AbstractMedia Export(Presentation destPres)
        {
            return ExportProtected(destPres) as AbstractMedia;
        }

        /// <summary>
        /// Exports the <see cref="AbstractMedia"/> to a given destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination <see cref="Presentation"/></param>
        /// <returns>The exported <see cref="AbstractMedia"/></returns>
        protected virtual IMedia ExportProtected(Presentation destPres)
        {
            AbstractMedia expMedia = destPres.MediaFactory.CreateMedia(XukLocalName, XukNamespaceUri) as AbstractMedia;
            if (expMedia == null)
            {
                throw new exception.FactoryCannotCreateTypeException(String.Format(
                                                                         "The factory of the destination Presentation cannot create a AbstractMedia matching Xuk QName {1}:{0}",
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
        /// Clears the data of the <see cref="AbstractMedia"/>
        /// </summary>
        protected override void Clear()
        {
            mLanguage = null;
            base.Clear();
        }


        /// <summary>
        /// Reads the attributes of a AbstractMedia xuk element.
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
        /// Writes the attributes of a AbstractMedia element
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

        #region IValueEquatable<IMedia> Members

        /// <summary>
        /// Compares <c>this</c> with a given other <see cref="IMedia"/> for equality
        /// </summary>
        /// <param name="other">The other <see cref="IMedia"/></param>
        /// <returns><c>true</c> if equal, otherwise <c>false</c></returns>
        public virtual bool ValueEquals(IMedia other)
        {
            AbstractMedia amOther = other as AbstractMedia;
            if (amOther == null) return false;
            if (this.GetType() != amOther.GetType()) return false;
            if (this.Language != amOther.Language) return false;
            return true;
        }

        #endregion
    }
}