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

		public string getText()
		{
			return mTextString;
		}

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

		public bool isContinuous()
		{
			return false;
		}

		public bool isDiscrete()
		{
			return true;
		}

		public bool isSequence()
		{
			return false;
		}

		public urakawa.media.MediaType getType()
		{
			return MediaType.TEXT;
		}

		IMedia IMedia.copy()
		{
			return copy();
		}

		public TextMedia copy()
		{
			TextMedia newMedia = new TextMedia();
			newMedia.setText(this.getText());
			return newMedia;
		}

		#endregion

		#region IXUKable members 

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
