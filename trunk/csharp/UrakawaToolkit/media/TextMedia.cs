using System;
using System.Xml;

namespace urakawa.media
{
	/// <summary>
	/// TextMedia represents a text string
	/// </summary>
	public class TextMedia : ITextMedia
	{
		private string mTextString;
		private string mLanguage;

		private IMediaFactory mMediaFactory;


		/// <summary>
		/// Constructor setting the associated <see cref="IMediaFactory"/>
		/// </summary>
		/// <param name="fact">
		/// The <see cref="IMediaFactory"/> to associate the <see cref="TextMedia"/> with
		/// </param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="fact"/> is <c>null</c>
		/// </exception>
		protected internal TextMedia(IMediaFactory fact)
		{
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException("Factory is null");
			}
			mMediaFactory = fact;
			mLanguage = null;
		}

		/// <summary>
		/// This override is useful while debugging
		/// </summary>
		/// <returns>The textual content of the <see cref="ITextMedia"/></returns>
		public override string ToString()
		{
			return mTextString;
		}

		#region ITextMedia Members

		/// <summary>
		/// Return the text string
		/// </summary>
		/// <returns></returns>
		public string getText()
		{
			return mTextString;
		}

		/// <summary>
		/// Set the text string
		/// </summary>
		/// <param name="text"></param>
		public void setText(string text)
		{
			if (text == null)
			{
				throw new exception.MethodParameterIsNullException("TextMedia.setText(null) caused MethodParameterIsNullException");
			}

			if (text.Length == 0)
			{
				throw new exception.MethodParameterIsEmptyStringException("TextMedia.setText(" +
					text + ") caused MethodParameterIsEmptyStringException");

				//causing a return here might be too oppositional, what if you are using an empty string?
				//(assuming it even matters in c#)
			}

			mTextString = text;
		}

		#endregion

		#region IMedia Members


		/// <summary>
		/// Sets the language of the text media
		/// </summary>
		/// <param name="lang">The new language, can be null but not an empty string</param>
		public void setLanguage(string lang)
		{
			if (lang == "")
			{
				throw new exception.MethodParameterIsEmptyStringException(
					"The language can not be an empty string");
			}
			mLanguage = lang;
		}

		/// <summary>
		/// Gets the language of the text media
		/// </summary>
		/// <returns>The language</returns>
		public string getLanguage()
		{
			return mLanguage;
		}


		/// <summary>
		/// Gets the <see cref="IMediaFactory"/> associated with the <see cref="IMedia"/>
		/// </summary>
		/// <returns>The <see cref="IMediaFactory"/></returns>
		public IMediaFactory getMediaFactory()
		{
			return mMediaFactory;
		}

		/// <summary>
		/// This always returns false, because
		/// text media is never considered continuous
		/// </summary>
		/// <returns></returns>
		public bool isContinuous()
		{
			return false;
		}

		/// <summary>
		/// This always returns true, because
		/// text media is always considered discrete
		/// </summary>
		/// <returns></returns>
		public bool isDiscrete()
		{
			return true;
		}


		/// <summary>
		/// This always returns false, because
		/// a single media object is never considered to be a sequence
		/// </summary>
		/// <returns></returns>
		public bool isSequence()
		{
			return false;
		}

		IMedia IMedia.copy()
		{
			return copy();
		}

		/// <summary>
		/// Make a copy of this text object
		/// </summary>
		/// <returns>The copy</returns>
		public ITextMedia copy()
		{
			return export(getMediaFactory().getPresentation());
		}


		IMedia IMedia.export(Presentation destPres)
		{
			return export(destPres);
		}

		/// <summary>
		/// Exports the text media to a destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The destination presentation</param>
		/// <returns>The exported external text media</returns>
		public TextMedia export(Presentation destPres)
		{
			TextMedia exported = destPres.getMediaFactory().createMedia(
				getXukLocalName(), getXukNamespaceUri()) as TextMedia;
			if (exported == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The MediaFactory cannot create a TextMedia matching QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			exported.setText(this.getText());
			return exported;
		}


		#endregion

		#region IXukAble members

		/// <summary>
		/// Fill in audio data from an XML source.
		/// Assume that the XmlReader cursor is at the opening audio tag.
		/// </summary>
		/// <param name="source">the input XML source</param>
		public void XukIn(System.Xml.XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Reader is null");
			}
			if (source.NodeType != XmlNodeType.Element)
			{
				throw new exception.XukException("Can not read TextMedia from a non-element node");
			}
			try
			{
				string text = "";
				if (!source.IsEmptyElement)
				{
					text = source.ReadString();
				}
				setText(text);
			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukIn of TextMedia: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a TextMedia xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInAttributes(XmlReader source)
		{
			string lang = source.GetAttribute("Language").Trim();
			if (lang != "") lang = null;
			setLanguage(lang);
		}

		/// <summary>
		/// The opposite of <see cref="XukIn"/>, this function writes the object's data
		/// to an XML file
		/// </summary>
		/// <param name="destination">the XML source for outputting data</param>
		public void XukOut(System.Xml.XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Writer is null");
			}
			try
			{
				destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
				destination.WriteString(getText());
				destination.WriteEndElement();

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukOut of TextMedia: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a TextMedia element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual void XukOutAttributes(XmlWriter destination)
		{
			if (getLanguage() != null) destination.WriteAttributeString("Language", getLanguage());
		}
		
		/// <summary>
		/// Gets the local localName part of the QName representing a <see cref="TextMedia"/> in Xuk
		/// </summary>
		/// <returns>The local localName part</returns>
		public string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="TextMedia"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

		#region IValueEquatable<IMedia> Members

		/// <summary>
		/// Conpares <c>this</c> with a given other <see cref="IMedia"/> for equality
		/// </summary>
		/// <param name="other">The other <see cref="IMedia"/></param>
		/// <returns><c>true</c> if equal, otherwise <c>false</c></returns>
		public bool valueEquals(IMedia other)
		{
			if (other == null) return false;
			if (getLanguage() != other.getLanguage()) return false;
			if (GetType() != other.GetType()) return false;
			if (getText() != ((ITextMedia)other).getText()) return false;
			return true;
		}

		#endregion
	}
}