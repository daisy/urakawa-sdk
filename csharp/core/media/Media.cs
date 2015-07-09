using System;
using System.Diagnostics;
using System.Xml;
using urakawa.core;
using urakawa.xuk;
using urakawa.events;

namespace urakawa.media
{
    /// <summary>
    /// This is the base interface for all media-related classes and interfaces.  
    /// Media is continuous (time-based) or discrete (static), and is of a specific type.
    /// </summary>
    public abstract class Media : WithPresentation, ObjectTag, IChangeNotifier
    {
        private object m_Tag = null;
        public object Tag
        {
            set { m_Tag = value; }
            get { return m_Tag; }
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

        private void this_LanguageChanged(object sender, urakawa.events.LanguageChangedEventArgs e)
        {
            NotifyChanged(e);
        }


        private string mLanguage;

        private void Reset()
        {
            mLanguage = null;
        }

        /// <summary>
        /// Default constructor - for system use only, 
        /// <see cref="Media"/>s should only be created via. the <see cref="MediaFactory"/>
        /// </summary>
        protected Media()
        {
            Reset();
            this.LanguageChanged += new EventHandler<urakawa.events.LanguageChangedEventArgs>(this_LanguageChanged);
        }

        #region IChangeNotifier members

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

        #endregion

        /// <summary>
        /// Gets or sets the language of the media
        /// </summary>
        /// <returns>The language</returns>
        public virtual string Language
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

        /// <summary>
        /// Determines if the <see cref="Media"/> is continuous
        /// </summary>
        /// <returns><c>true</c> if the <see cref="Media"/> is continuous, <c>false</c> else</returns>
        public abstract bool IsContinuous { get; }

        /// <summary>
        /// Convenience Equivalent to <c>!IsContinuous</c>
        /// </summary>
        /// <returns><c>!isContinuous</c></returns>
        public abstract bool IsDiscrete { get; }

        /// <summary>
        /// tells you if the media object itself is a sequence
        /// does not tell you if your individual media object is part of a sequence
        /// </summary>
        /// <returns></returns>
        public abstract bool IsSequence { get; }

        /// <summary>
        /// Gets a copy of the <see cref="Media"/>
        /// </summary>
        /// <returns>The copy</returns>
        public Media Copy()
        {
//#if DEBUG
//            Debugger.Break();
//#endif //DEBUG

            return CopyProtected();
        }

        /// <summary>
        /// Gets a copy of the <see cref="Media"/>
        /// </summary>
        /// <returns>The copy</returns>
        protected virtual Media CopyProtected()
        {
            Media exp = Presentation.MediaFactory.Create(GetType());
            exp.Language = Language;
            return exp;
        }

        /// <summary>
        /// Exports the media to a destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported media</returns>
        public Media Export(Presentation destPres)
        {
//#if DEBUG
//            Debugger.Break();
//#endif //DEBUG

            return ExportProtected(destPres);
        }

        /// <summary>
        /// Exports the media to a destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported media</returns>
        protected virtual Media ExportProtected(Presentation destPres)
        {
            Media exp = destPres.MediaFactory.Create(GetType());
            exp.Language = Language;
            return exp;
        }


        #region XukAble overides

        /// <summary>
        /// Clears the data of the <see cref="Media"/>
        /// </summary>
        protected override void Clear()
        {
            Reset();
            base.Clear();
        }


        /// <summary>
        /// Reads the attributes of a AbstractMedia xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

            string lang = XukAble.ReadXukAttribute(source, XukAble.Language_NAME);
            if (lang != null) lang = lang.Trim();
            if (lang == "") lang = null;
            Language = lang;
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
            base.XukOutAttributes(destination, baseUri);

            if (Language != null)
            {
                destination.WriteAttributeString(XukAble.Language_NAME.z(PrettyFormat), Language);
            }
            
        }

        #endregion


        #region Implementation of IValueEquatable<Media>

        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            Media otherz = other as Media;
            if (otherz == null)
            {
                return false;
            }

            if (otherz.Language != Language)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            return true;
        }

        #endregion

    }
}