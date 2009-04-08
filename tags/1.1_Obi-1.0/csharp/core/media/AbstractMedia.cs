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
		public event EventHandler<urakawa.events.DataModelChangedEventArgs> changed;
		/// <summary>
		/// Fires the <see cref="changed"/> event 
		/// </summary>
		/// <param name="args">The arguments of the event</param>
		protected void notifyChanged(urakawa.events.DataModelChangedEventArgs args)
		{
			EventHandler<urakawa.events.DataModelChangedEventArgs> d = changed;
			if (d != null) d(this, args);
		}

		/// <summary>
		/// Event fired after the language of the <see cref="TextMedia"/> has changed
		/// </summary>
		public event EventHandler<urakawa.events.LanguageChangedEventArgs> languageChanged;
		/// <summary>
		/// Fires the <see cref="languageChanged"/> event
		/// </summary>
		/// <param name="source">The source, that is the <see cref="TextMedia"/> whoose language changed</param>
		/// <param name="newLang">The new value for the language</param>
		/// <param name="prevLang">The value for the language prior to the change</param>
		protected void notifyLanguageChanged(AbstractMedia source, string newLang, string prevLang)
		{
			EventHandler<urakawa.events.LanguageChangedEventArgs> d = languageChanged;
			if (d != null) d(this, new urakawa.events.LanguageChangedEventArgs(source, newLang, prevLang));
		}

		void this_languageChanged(object sender, urakawa.events.LanguageChangedEventArgs e)
		{
			notifyChanged(e);
		}
		#endregion

		/// <summary>
		/// Default constructor
		/// </summary>
		public AbstractMedia()
		{
			mLanguage = null;
			this.languageChanged += new EventHandler<urakawa.events.LanguageChangedEventArgs>(this_languageChanged);
		}

		private string mLanguage;

		#region IMedia Members


		/// <summary>
		/// Gets the <see cref="IMediaFactory"/> associated with the <see cref="AbstractMedia"/> (via. the owning <see cref="Presentation"/>
		/// </summary>
		/// <returns>The <see cref="IMediaFactory"/></returns>
		public IMediaFactory getMediaFactory()
		{
			return getPresentation().getMediaFactory();
		}

		/// <summary>
		/// Determines if the <see cref="AbstractMedia"/> is continuous
		/// </summary>
		/// <returns>A <see cref="bool"/> indicating if the <see cref="AbstractMedia"/> is continuous</returns>
		public abstract bool isContinuous();

		/// <summary>
		/// Determines if the <see cref="AbstractMedia"/> is discrete
		/// </summary>
		/// <returns>A <see cref="bool"/> indicating if the <see cref="AbstractMedia"/> is discrete</returns>
		public abstract bool isDiscrete();

		/// <summary>
		/// Determines if the <see cref="AbstractMedia"/> is a <see cref="SequenceMedia"/>
		/// </summary>
		/// <returns>A <see cref="bool"/> indicating if the <see cref="AbstractMedia"/> is a <see cref="SequenceMedia"/></returns>
		public abstract bool isSequence();

		IMedia IMedia.copy()
		{
			return copyProtected();
		}

		/// <summary>
		/// Creates a copy of the <see cref="AbstractMedia"/>
		/// </summary>
		/// <returns>The copy</returns>
		public AbstractMedia copy()
		{
			return copyProtected() as AbstractMedia;
		}

		/// <summary>
		/// Creates a copy of the <see cref="AbstractMedia"/>
		/// </summary>
		/// <returns>The copy</returns>
		protected virtual IMedia copyProtected()
        {
            IMedia expMedia = getPresentation().getMediaFactory().createMedia(getXukLocalName(), getXukNamespaceUri());
            if (expMedia == null)
            {
                throw new exception.FactoryCannotCreateTypeException(String.Format(
                    "The factory of the destination Presentation cannot create a AbstractMedia matching Xuk QName {1}:{0}",
                    getXukLocalName(), getXukNamespaceUri()));
            }
            expMedia.setLanguage(getLanguage());
            return expMedia;
		}

		IMedia IMedia.export(Presentation destPres)
		{
			return exportProtected(destPres);
		}

		/// <summary>
		/// Exports the <see cref="AbstractMedia"/> to a given destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The destination <see cref="Presentation"/></param>
		/// <returns>The exported <see cref="AbstractMedia"/></returns>
		public AbstractMedia export(Presentation destPres)
		{
			return exportProtected(destPres) as AbstractMedia;
		}

		/// <summary>
		/// Exports the <see cref="AbstractMedia"/> to a given destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The destination <see cref="Presentation"/></param>
		/// <returns>The exported <see cref="AbstractMedia"/></returns>
		protected virtual IMedia exportProtected(Presentation destPres)
		{
			AbstractMedia expMedia = destPres.getMediaFactory().createMedia(getXukLocalName(), getXukNamespaceUri()) as AbstractMedia;
			if (expMedia == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The factory of the destination Presentation cannot create a AbstractMedia matching Xuk QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			expMedia.setLanguage(getLanguage());
			return expMedia;
		}

		/// <summary>
		/// Sets the language of the <see cref="AbstractMedia"/>
		/// </summary>
		/// <param name="lang">The new language, can be null but not an empty string</param>
		/// <exception cref="exception.MethodParameterIsEmptyStringException">
		/// Thrown if the new language is an empty <see cref="String"/></exception>
		public void setLanguage(string lang)
		{
			if (lang == "")
			{
				throw new exception.MethodParameterIsEmptyStringException(
					"The language can not be an empty string");
			}
			string prevlang = mLanguage;
			mLanguage = lang;
			if (prevlang != mLanguage) notifyLanguageChanged(this, mLanguage, prevlang);
		}

		/// <summary>
		/// Gets the language of the <see cref="AbstractMedia"/>
		/// </summary>
		/// <returns>The language</returns>
		public string getLanguage()
		{
			return mLanguage;
		}

		#endregion

		/// <summary>
		/// Clears the data of the <see cref="AbstractMedia"/>
		/// </summary>
		protected override void clear()
		{
			mLanguage = null;
			base.clear();
		}


		/// <summary>
		/// Reads the attributes of a AbstractMedia xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected override void xukInAttributes(XmlReader source)
		{
			string lang = source.GetAttribute("language");
			if (lang != null) lang = lang.Trim();
			if (lang == "") lang = null;
			setLanguage(lang);
			base.xukInAttributes(source);
		}

		/// <summary>
		/// Writes the attributes of a AbstractMedia element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected override void xukOutAttributes(XmlWriter destination, Uri baseUri)
		{
			if (getLanguage()!=null) destination.WriteAttributeString("language", getLanguage());

			base.xukOutAttributes(destination, baseUri);
		}

		#region IValueEquatable<IMedia> Members

		/// <summary>
		/// Compares <c>this</c> with a given other <see cref="IMedia"/> for equality
		/// </summary>
		/// <param name="other">The other <see cref="IMedia"/></param>
		/// <returns><c>true</c> if equal, otherwise <c>false</c></returns>
		public virtual bool valueEquals(IMedia other)
		{
			AbstractMedia amOther = other as AbstractMedia;
			if (amOther == null) return false;
			if (this.GetType() != amOther.GetType()) return false;
			if (this.getLanguage() != amOther.getLanguage()) return false;
			return true;
		}

		#endregion
	}
}
