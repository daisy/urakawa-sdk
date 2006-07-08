using System;

namespace urakawa.media
{
	/// <summary>
	/// AudioMedia is the audio object.
	/// It is time-based and comes from an external source.
	/// </summary>
	public class AudioMedia : ClippedMedia, IAudioMedia
	{
		//Internal constructor encourages use of MediaFactory to create AudioMedia objects
		protected AudioMedia()
		{
		}

		static internal AudioMedia create()
		{
			return new AudioMedia();
		}
		
		#region IMedia members
		/// <summary>
		/// This always returns true, because
		/// audio media is always considered continuous
		/// </summary>
		/// <returns></returns>
		public override bool isContinuous()
		{
			return true;
		}

		/// <summary>
		/// This always returns false, because
		/// audio media is never considered discrete
		/// </summary>
		/// <returns></returns>
		public override bool isDiscrete()
		{
			return false;
		}

		/// <summary>
		/// This always returns false, because
		/// a single media object is never considered to be a sequence
		/// </summary>
		/// <returns></returns>
		public override bool isSequence()
		{
			return false;
		}

		/// <summary>
		/// Return the urakawa media type
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
		/// Copy function which returns an <see cref="AudioMedia"/> object
		/// </summary>
		/// <returns>a copy of this</returns>
		public AudioMedia copy()
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
		/// Fill in audio data from an XML source.
		/// Assume that the XmlReader cursor is at the opening audio tag.
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

			
			//System.Diagnostics.Debug.WriteLine("XUKin: AudioMedia");

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
		/// The opposite of <see cref="XUKin"/>, this function writes the object's data
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
