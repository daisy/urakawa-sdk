using System;

namespace urakawa.media
{
	/// <summary>
	/// AudioMedia is the audio object.
	/// It is time-based and comes from an external source.
	/// </summary>
	public class AudioMedia : ClippedMedia, IAudioMedia
	{
		//internal constructor encourages use of MediaFactory to create AudioMedia objects
		internal AudioMedia()
		{
		}

		/// <summary>
		/// this override is useful while debugging
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "AudioMedia";
		}
		
		#region IMedia members
		public override bool isContinuous()
		{
			return true;
		}

		public override bool isDiscrete()
		{
			return false;
		}

		public override bool isSequence()
		{
			return false;
		}

		/// <summary>
		/// return the urakawa media type
		/// </summary>
		/// <returns>always returns <see cref="MediaType.AUDIO"/></returns>
		public override urakawa.media.MediaType getType()
		{
			return MediaType.AUDIO;
		}

		IMedia IMedia.copy()
		{
			return copy();
		}

		/// <summary>
		/// actually useful copy function which returns an AudioMedia object
		/// </summary>
		/// <returns>a copy of this</returns>
		public new AudioMedia copy()
		{
			AudioMedia newMedia = new AudioMedia();
			newMedia.setClipBegin(this.getClipBegin().copy());
			newMedia.setClipEnd(this.getClipEnd().copy());
			newMedia.setLocation(this.getLocation().copy());

			return newMedia;
		}

		#endregion

		#region IXUKable members 

		/// <summary>
		/// fill in audio data from an XML source.
		/// assume that the XmlReader cursor is at the opening audio tag
		/// </summary>
		/// <param name="source">the input XML source</param>
		/// <returns>true or false, depending on whether the data could be processed</returns>
		public override bool XUKin(System.Xml.XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Reader is null");
			}

			if (!(source.Name == "Media" && source.NodeType == System.Xml.XmlNodeType.Element &&
				source.GetAttribute("type") == "AUDIO"))
			{
				return false;
			}

			
			System.Diagnostics.Debug.WriteLine("XUKin: AudioMedia");

			string cb = source.GetAttribute("clipBegin");
			string ce = source.GetAttribute("clipEnd");
			string src = source.GetAttribute("src");
		
			this.setClipBegin(new Time(cb));
			this.setClipEnd(new Time(ce));

			MediaLocation location = new MediaLocation(src);
			this.setLocation(location);

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
		/// the opposite of <see cref="XUKin"/>, this function writes the object's data
		/// to an XML file
		/// </summary>
		/// <param name="destination">the XML source for outputting data</param>
		/// <returns>so far, this function always returns true</returns>
		public override bool XUKout(System.Xml.XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Writer is null");
			}

		
			destination.WriteStartElement("Media");

			destination.WriteAttributeString("type", "AUDIO");

			destination.WriteAttributeString("src", this.getLocation().Location);

			destination.WriteAttributeString("clipBegin", this.getClipBegin().getTimeAsString());

			destination.WriteAttributeString("clipEnd", this.getClipEnd().getTimeAsString());

			destination.WriteEndElement();
		
			return true;
			
		}
		#endregion
	}
}
