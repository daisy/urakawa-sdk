using System;

namespace urakawa.media
{
	/// <summary>
	/// ImageMedia is the image object. 
	/// It has width, height, and an external source.
	/// </summary>
	public class ImageMedia : ExternalMedia, IImageMedia
	{
		int mWidth;
		int mHeight;
		
		//Internal constructor encourages use of MediaFactory to create ImageMedia objects
		protected ImageMedia()
		{
			mWidth = 0;
			mHeight = 0;
		}

		internal static ImageMedia create()
		{
			return new ImageMedia();
		}

		/// <summary>
		/// This override is useful while debugging
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "ImageMedia";
		}


		//are these functions necessary, or should they be renamed something like
		//"cropImage" or "resizeImage"?
		//image width and height could be calculated from the file itself, 
		//assuming all editing is destructive, otherwise some access to "virtual" height and width 
		//will be required
		/// <summary>
		/// Set the width of the image. 
		/// </summary>
		/// <param name="width"></param>
		public void setWidth(int width)
		{
			mWidth = width;
		}

		/// <summary>
		/// Set the height of the image. 
		/// </summary>
		/// <param name="height"></param>
		public void setHeight(int height)
		{
			mHeight = height;
		}

		

		#region IMedia Members

		/// <summary>
		/// This always returns false, because
		/// image media is never considered continuous
		/// </summary>
		/// <returns></returns>
		public override bool isContinuous()
		{
			return false;
		}

		/// <summary>
		/// This always returns true, because
		/// image media is always considered discrete
		/// </summary>
		/// <returns></returns>
		public override bool isDiscrete()
		{
			return true;
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
		/// <returns>always returns <see cref="MediaType.IMAGE"/></returns>
		public override urakawa.media.MediaType getType()
		{
			return MediaType.IMAGE;
		}

		IMedia IMedia.copy()
		{
			return copy();
		}

		/// <summary>
		/// Return a copy of this <see cref="ImageMedia"/> object
		/// </summary>
		/// <returns></returns>
		public ImageMedia copy()
		{
			ImageMedia newMedia = new ImageMedia();
			newMedia.setHeight(this.getHeight());
			newMedia.setWidth(this.getWidth());
			newMedia.setLocation(this.getLocation().copy());
			return newMedia;
		}

		#endregion

		#region IImageSize Members

		/// <summary>
		/// Return the image width
		/// </summary>
		/// <returns></returns>
		public int getWidth()
		{
			return mWidth;
		}

		/// <summary>
		/// Return the image height
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
				source.GetAttribute("type") == "IMAGE"))
			{
				return false;
			}

			
			//System.Diagnostics.Debug.WriteLine("XUKin: ImageMedia");


			string height = source.GetAttribute("height");
			string width = source.GetAttribute("width");
			string src = source.GetAttribute("src");

			this.setHeight(int.Parse(height));
			this.setWidth(int.Parse(width));
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

			destination.WriteAttributeString("type", "IMAGE");

			destination.WriteAttributeString("src", this.getLocation().ToString());

			destination.WriteAttributeString("height", this.mHeight.ToString());

			destination.WriteAttributeString("width", this.mWidth.ToString());

			destination.WriteEndElement();

			return true;
		}
		#endregion
	}
}
