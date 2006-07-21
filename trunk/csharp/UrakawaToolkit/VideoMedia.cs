using System;

namespace urakawa.media
{
	/// <summary>
	/// VideoMedia is the video object.
	/// It is time-based, comes from an external source, and has a visual presence.
	/// </summary>
	public class VideoMedia : ClippedMedia, IVideoMedia
	{
		int mWidth;
		int mHeight;

		/// <summary>
		/// Default constructor
		/// </summary>
		protected VideoMedia()
		{
		}

		internal static VideoMedia create()
		{
			return new VideoMedia();
		}

		/// <summary>
		/// Set the visual media's width
		/// </summary>
		/// <param name="width"></param>
		public void setWidth(int width)
		{
			mWidth = width;
		}

		/// <summary>
		/// Set the visual media's height
		/// </summary>
		/// <param name="height"></param>
		public void setHeight(int height)
		{
			mHeight = height;
		}

		#region IMedia Members

		/// <summary>
		/// This always returns true, because
		/// video media is always considered continuous
		/// </summary>
		/// <returns></returns>
		public override bool isContinuous()
		{
			return true;
		}

		/// <summary>
		/// This always returns false, because
		/// video media is never considered discrete
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
		/// <returns>always returns <see cref="MediaType.VIDEO"/></returns>
		public override urakawa.media.MediaType getType()
		{
			return MediaType.VIDEO;
		}

		IMedia IMedia.copy()
		{
			return copy();
		}

		/// <summary>
		/// Copy function which returns an <see cref="VideoMedia"/> object
		/// </summary>
		/// <returns>a copy of this</returns>
		public VideoMedia copy()
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

		/// <summary>
		/// Return the visual media's width
		/// </summary>
		/// <returns></returns>
		public int getWidth()
		{
			return mWidth;
		}

		/// <summary>
		/// Return the visual media's height.
		/// </summary>
		/// <returns></returns>
		public int getHeight()
		{
			return mHeight;
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
				source.GetAttribute("type") == "VIDEO"))
			{
				return false;
			}

			
			//System.Diagnostics.Debug.WriteLine("XUKin: VideoMedia");

			string cb = source.GetAttribute("clipBegin");
			string ce = source.GetAttribute("clipEnd");
			string src = source.GetAttribute("src");
			string height = source.GetAttribute("height");
			string width = source.GetAttribute("width");

			this.setClipBegin(new Time(cb));
			this.setClipEnd(new Time(ce));
			this.setLocation(new MediaLocation(src));
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

			destination.WriteAttributeString("type", "VIDEO");

			destination.WriteAttributeString("src", this.getLocation().Location);

			destination.WriteAttributeString("clipBegin", this.getClipBegin().getTimeAsString());

			destination.WriteAttributeString("clipEnd", this.getClipEnd().getTimeAsString());

			destination.WriteAttributeString("height", this.getHeight().ToString());

			destination.WriteAttributeString("width", this.getWidth().ToString());

			destination.WriteEndElement();

			return true;
		}
		#endregion
	}
}
