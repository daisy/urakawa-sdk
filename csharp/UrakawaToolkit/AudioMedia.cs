using System;

namespace urakawa.media
{
	/// <summary>
	/// AudioMedia is the audio object.
	/// It is time-based and comes from an external source.
	/// </summary>
	public class AudioMedia : IAudioMedia
	{
		private Time mClipBegin = new Time();
		private Time mClipEnd = new Time();
		private MediaLocation mMediaLocation = new MediaLocation();
		
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
	
		#region IClippedMedia Members

		/// <summary>
		/// returns the difference between <see cref="mClipBegin"/> and <see cref="mClipEnd"/>
		/// </summary>
		/// <returns></returns>
		public ITimeDelta getDuration()
		{
			return mClipBegin.getTimeDelta(mClipEnd);
		}

		public ITime getClipBegin()
		{
			return mClipBegin;
		}

		public ITime getClipEnd()
		{
			return mClipEnd;
		}

		/// <summary>
		/// defines a new begin time for the clip.  changes to audio media objects are non-destructive.
		/// this function will throw exceptions <see cref="exception.MethodParameterIsNullException"/>
		/// and <see cref="exception.TimeOffsetIsNegativeException"/> if provoked.
		/// </summary>
		/// <param name="beginPoint">the new begin time</param>
		public void setClipBegin(ITime beginPoint)
		{
			if (beginPoint == null)
			{
				throw new exception.MethodParameterIsNullException("AudioMedia.setClipBegin (null) caused MethodParameterIsNullException");
			}

			if (beginPoint.isNegativeTimeOffset() == true)
			{
				throw new exception.TimeOffsetIsNegativeException("AudioMedia.setClipBegin (" + 
					beginPoint.ToString() + ") caused TimeOffsetIsNegativeException");

			}

			mClipBegin = (Time)beginPoint;
		}

		/// <summary>
		/// defines a new end time for the clip.  changes to audio media objects are non-destructive.
		/// this function will throw exceptions <see cref="exception.MethodParameterIsNullException"/>
		/// and <see cref="exception.TimeOffsetIsNegativeException"/> if provoked.
		/// </summary>
		/// <param name="endPoint">the new end time</param>
		public void setClipEnd(ITime endPoint)
		{
			if (endPoint == null)
			{
				throw new exception.MethodParameterIsNullException("AudioMedia.setClipEnd (null) caused MethodParameterIsNullException");
			}

			if (endPoint.isNegativeTimeOffset() == true)
			{
				throw new exception.TimeOffsetIsNegativeException("AudioMedia.setClipEnd (" + 
					endPoint.ToString() + ") caused TimeOffsetIsNegativeException");

			}
			mClipEnd = (Time)endPoint;
		}

		/// <summary>
		/// split the object at the time given and return the later portion
		/// this function will throw exceptions <see cref="exception.MethodParameterIsNullException"/>
		/// and <see cref="exception.TimeOffsetIsNegativeException"/> if provoked.
		/// </summary>
		/// <param name="splitPoint">the time when the audio object should be split</param>
		/// <returns>a new object representing the later portion of the recently-split media object</returns>
		public IClippedMedia split(ITime splitPoint)
		{
			if (splitPoint == null)
			{
				throw new exception.MethodParameterIsNullException("AudioMedia.split (null) caused MethodParameterIsNullException");
			}
	
			if (splitPoint.isNegativeTimeOffset() == true)
			{
				throw new exception.TimeOffsetIsNegativeException("AudioMedia.split (" + 
					splitPoint.ToString() + ") caused TimeOffsetIsNegativeException");
				
				
			}


			AudioMedia splitMedia = new AudioMedia();
			splitMedia.setClipBegin(splitPoint);
			splitMedia.setClipEnd(mClipEnd);

			this.setClipEnd(splitPoint);

			return splitMedia;
		}

		#endregion

		#region IExternalMedia Members

		/// <summary>
		/// returns the location of the physical media being referenced 
		/// (e.g., could be a file path)
		/// </summary>
		/// <returns>an <see cref="IMediaLocation"/> object which can be queried to find the media location</returns>
		public IMediaLocation getLocation()
		{
			return mMediaLocation;
		}

		/// <summary>
		/// set the media location for this object
		/// </summary>
		/// <param name="location"></param>
		public void setLocation(IMediaLocation location)
		{
			if (location == null)
			{
				throw new exception.MethodParameterIsNullException("AudioMedia.setLocation(null) caused MethodParameterIsNullException");
			}

			mMediaLocation = (MediaLocation)location;
		}

		#endregion
		
		#region IMedia Members

		/// <summary>
		/// audio media is always continuous
		/// </summary>
		/// <returns></returns>
		public bool isContinuous()
		{
			return true;
		}

		/// <summary>
		/// audio media is never discrete
		/// </summary>
		/// <returns></returns>
		public bool isDiscrete()
		{
			return false;
		}

		/// <summary>
		/// a single audio object is never a sequence by itself
		/// </summary>
		/// <returns></returns>
		public bool isSequence()
		{
			return false;
		}

		/// <summary>
		/// return the urakawa media type
		/// </summary>
		/// <returns>always returns <see cref="MediaType.AUDIO"/></returns>
		public urakawa.media.MediaType getType()
		{
			return MediaType.AUDIO;
		}
		
		/// <summary>
		/// private function to satisfy the interface requirement of returning IMedia
		/// </summary>
		/// <returns></returns>
		IMedia IMedia.copy()
		{
			return copy();
		}

		/// <summary>
		/// actually useful copy function which returns an AudioMedia object
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
		/// fill in audio data from an XML source.
		/// assume that the XmlReader cursor is at the opening audio tag
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

			this.mMediaLocation = new MediaLocation(src);

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
		public bool XUKout(System.Xml.XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Writer is null");
			}

		
			destination.WriteStartElement("Media");

			destination.WriteAttributeString("type", "AUDIO");

			destination.WriteAttributeString("src", this.mMediaLocation.mLocation);

			destination.WriteAttributeString("clipBegin", this.mClipBegin.getTimeAsString());

			destination.WriteAttributeString("clipEnd", this.mClipEnd.getTimeAsString());

			destination.WriteEndElement();
		
			return true;
			
		}
		#endregion
	}
}
