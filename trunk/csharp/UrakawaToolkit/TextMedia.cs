using System;

namespace urakawa.media
{
	/// <summary>
	/// TextMedia represents a text string
	/// </summary>
	public class TextMedia : ITextMedia
	{
		private string mTextString;

		
		//internal constructor encourages use of MediaFactory to create TextMedia objects
		internal TextMedia()
		{
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

		#region IXUKable members 

		/// <summary>
		/// Fill in audio data from an XML source.
		/// Assume that the XmlReader cursor is at the opening audio tag.
		/// </summary>
		/// <param name="source">the input XML source</param>
		/// <returns>true or false, depending on whether the data could be processed</returns>
		public bool XUKin(System.Xml.XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Reader is null");
			}

			if (!(source.Name == "Media" && source.NodeType == System.Xml.XmlNodeType.Element &&
				source.GetAttribute("type") == "TEXT"))
			{
				return false;
			}

			
			System.Diagnostics.Debug.WriteLine("XUKin: TextMedia");

			//the next element should be the text data
			if (source.IsEmptyElement == false)
			{
				source.Read();
				if (source.NodeType == System.Xml.XmlNodeType.Text)
				{
					string src = source.Value;
					this.setText(src);
				}
			}

			//move the cursor to the closing tag
			if (source.IsEmptyElement == false)
			{
				while (!(source.Name == "Media" && 
					source.NodeType == System.Xml.XmlNodeType.EndElement)
					&&
					source.EOF == false)
				{
					source.Read();
				}
			}

			return true;

		}

		/// <summary>
		/// The opposite of <see cref="XUKin"/>, this function writes the object's data
		/// to an XML file
		/// </summary>
		/// <param name="destination">the XML source for outputting data</param>
		/// <returns>so far, this function always returns true</returns>
		public bool XUKout(System.Xml.XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Writer is null");
			}

			destination.WriteStartElement("Media");

			destination.WriteAttributeString("type", "TEXT");

			destination.WriteString(this.mTextString);

			destination.WriteEndElement();

			return true;
		}
		#endregion
	}
}
