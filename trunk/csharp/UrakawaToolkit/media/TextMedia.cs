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

		private IMediaFactory mMediaFactory;


		/// <summary>
		/// Constructor setting the associated <see cref="IMediaFactory"/>
		/// </summary>
		/// <param localName="fact">
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
		/// <param localName="text"></param>
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

		/// <summary>
		/// Return the urakawa media type
		/// </summary>
		/// <returns>always returns <see cref="MediaType.TEXT"/></returns>
		public MediaType getMediaType()
		{
			return MediaType.TEXT;
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
			IMedia newMedia = getMediaFactory().createMedia(
				getXukLocalName(), getXukNamespaceUri());
			if (newMedia is ITextMedia)
			{
				ITextMedia newTextMedia = (ITextMedia)newMedia;
				newTextMedia.setText(this.getText());
				return newTextMedia;
			}
			else
			{
				throw new exception.FactoryCanNotCreateTypeException(String.Format(
					"The media factory can not create an ITextMedia matching QName {0}:{1}",
					getXukLocalName(), getXukNamespaceUri()));
			}
		}

		#endregion

		#region IXukAble members

		/// <summary>
		/// Fill in audio data from an XML source.
		/// Assume that the XmlReader cursor is at the opening audio tag.
		/// </summary>
		/// <param localName="source">the input XML source</param>
		/// <returns>true or false, depending on whether the data could be processed</returns>
		public bool XukIn(System.Xml.XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Reader is null");
			}
			if (source.NodeType != System.Xml.XmlNodeType.Element) return false;

			if (source.IsEmptyElement)
			{
				mTextString = "";
			}
			else
			{
				mTextString = source.ReadString();
			}
			return true;
		}

		/// <summary>
		/// The opposite of <see cref="XukIn"/>, this function writes the object's data
		/// to an XML file
		/// </summary>
		/// <param localName="destination">the XML source for outputting data</param>
		/// <returns>so far, this function always returns true</returns>
		public bool XukOut(System.Xml.XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Writer is null");
			}
			destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
			destination.WriteString(this.mTextString);
			destination.WriteEndElement();
			return true;
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
		public bool ValueEquals(IMedia other)
		{
			if (!(other is ITextMedia)) return false;
			if (getText() != ((ITextMedia)other).getText()) return false;
			return true;
		}

		#endregion
	}
}