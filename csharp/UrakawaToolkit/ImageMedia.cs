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
		
		//internal constructor encourages use of MediaFactory to create ImageMedia objects
		internal ImageMedia()
		{
			mWidth = 0;
			mHeight = 0;
		}

		/// <summary>
		/// this override is useful while debugging
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
			return false;
		}

		public override bool isDiscrete()
		{
			return true;
		}

		public override bool isSequence()
		{
			return false;
		}

		public override urakawa.media.MediaType getType()
		{
			return MediaType.IMAGE;
		}

		IMedia IMedia.copy()
		{
			return copy();
		}

		public new ImageMedia copy()
		{
			ImageMedia newMedia = new ImageMedia();
			newMedia.setHeight(this.getHeight());
			newMedia.setWidth(this.getWidth());
			newMedia.setLocation(this.getLocation().copy());
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
				source.GetAttribute("type") == "IMAGE"))
			{
				return false;
			}

			
			System.Diagnostics.Debug.WriteLine("XUKin: ImageMedia");


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
