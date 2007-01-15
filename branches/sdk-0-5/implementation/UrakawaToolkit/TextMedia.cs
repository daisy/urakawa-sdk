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

		
		/// <summary>
		/// Default constructor
		/// </summary>
		protected TextMedia()
		{
		}

		internal static TextMedia create()
		{
			return new TextMedia();
		}

		/// <summary>
		/// this override is useful while debugging
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "TextMedia";
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
		/// Set the text <see cref="string"/>
		/// </summary>
		/// <param name="text">The new text <see cref="string"/></param>
		public void setText(string text)
		{
			if (text == null)
			{
				throw new exception.MethodParameterIsNullException("TextMedia.setText(null) caused MethodParameterIsNullException");
			}
		
			mTextString = text;
		}

		#endregion

		#region IMedia Members

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
		public urakawa.media.MediaType getType()
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
		/// <returns></returns>
		public TextMedia copy()
		{
			TextMedia newMedia = new TextMedia();
			newMedia.setText(this.getText());
			return newMedia;
		}

		#endregion

		#region IXUKAble members 

		/// <summary>
		/// Fill in audio data from an XML source.
		/// Assume that the XmlReader cursor is at the opening audio tag.
		/// </summary>
		/// <param name="source">the input XML source</param>
		/// <returns>true or false, depending on whether the data could be processed</returns>
		public bool XUKIn(System.Xml.XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Reader is null");
			}
			if (source.Name != "TextMedia") return false;
			if (source.NamespaceURI != urakawa.ToolkitSettings.XUK_NS) return false;
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
		/// The opposite of <see cref="XUKIn"/>, this function writes the object's data
		/// to an XML file
		/// </summary>
		/// <param name="destination">the XML source for outputting data</param>
		/// <returns>so far, this function always returns true</returns>
		public bool XUKOut(System.Xml.XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Writer is null");
			}
			destination.WriteStartElement("TextMedia", urakawa.ToolkitSettings.XUK_NS);
			destination.WriteString(this.mTextString);
			destination.WriteEndElement();
			return true;
		}
		#endregion
	}
}
