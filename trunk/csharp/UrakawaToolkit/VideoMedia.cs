using System;

namespace urakawa.media
{
	/// <summary>
	/// VideoMedia is the video object.
	/// It is time-based, comes from an external source, and has a visual presence.
	/// </summary>
	public class VideoMedia : ClippedMedia, IVideoMedia
	{
		private Time mClipBegin = new Time();
		private Time mClipEnd = new Time();
		private MediaLocation mMediaLocation = new MediaLocation();
		int mWidth;
		int mHeight;

		
		//internal constructor encourages use of MediaFactory to create VideoMedia objects
		internal VideoMedia()
		{
		}

		/// <summary>
		/// this override is useful while debugging
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "VideoMedia";
		}

		public void setWidth(int width)
		{
			mWidth = width;
		}

		public void setHeight(int height)
		{
			mHeight = height;
		}

		#region IMedia Members

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

		public override urakawa.media.MediaType getType()
		{
			return MediaType.VIDEO;
		}

		IMedia IMedia.copy()
		{
			return copy();
		}

		public new VideoMedia copy()
		{
			VideoMedia newMedia = new VideoMedia();
			newMedia.setClipBegin(this.getClipBegin().copy());
			newMedia.setClipEnd(this.getClipEnd().copy());
			newMedia.setLocation(this.getLocation().copy());
			newMedia.setWidth(this.getWidth());
			newMedia.setHeight(this.getHeight());

			return newMedia;
		}

		#endregion

		#region IImageSize Members

		public int getWidth()
		{
			return mWidth;
		}

		public int getHeight()
		{
			return mHeight;
		}

		#endregion

		#region IXUKable members 

		public override bool XUKin(System.Xml.XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Reader is null");
			}

			if (!(source.Name == "Media" && source.NodeType == System.Xml.XmlNodeType.Element &&
				source.GetAttribute("type") == "VIDEO"))
			{
				return false;
			}

			
			System.Diagnostics.Debug.WriteLine("XUKin: VideoMedia");

			string cb = source.GetAttribute("clipBegin");
			string ce = source.GetAttribute("clipEnd");
			string src = source.GetAttribute("src");
			string height = source.GetAttribute("height");
			string width = source.GetAttribute("width");

			this.setClipBegin(new Time(cb));
			this.setClipEnd(new Time(ce));
			this.mMediaLocation = new MediaLocation(src);
			this.setHeight(int.Parse(height));
			this.setWidth(int.Parse(width));

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

		public override bool XUKout(System.Xml.XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Writer is null");
			}

		
			destination.WriteStartElement("Media");

			destination.WriteAttributeString("type", "VIDEO");

			destination.WriteAttributeString("src", this.mMediaLocation.Location);

			destination.WriteAttributeString("clipBegin", this.mClipBegin.getTimeAsString());

			destination.WriteAttributeString("clipEnd", this.mClipEnd.getTimeAsString());

			destination.WriteAttributeString("height", this.mHeight.ToString());

			destination.WriteAttributeString("width", this.mWidth.ToString());

			destination.WriteEndElement();

			return true;
		}
		#endregion
	}
}
