using System;
using System.Xml;

namespace urakawa.media
{
	/// <summary>
	/// VideoMedia is the video object.
	/// It is time-based, comes from an external source, and has a visual presence.
	/// </summary>
	public class VideoMedia : IVideoMedia
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
		public bool isContinuous()
		{
			return true;
		}

		/// <summary>
		/// This always returns false, because
		/// video media is never considered discrete
		/// </summary>
		/// <returns></returns>
		public bool isDiscrete()
		{
			return false;
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
		/// <returns>always returns <see cref="MediaType.VIDEO"/></returns>
		public MediaType getMediaType()
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
		public IVideoMedia copy()
		{
			IMedia copyM = getMediaFactory().createMedia(getXukLocalName(), getXukNamespaceUri());
			if (copyM == null || !(copyM is IVideoMedia))
			{
				throw new exception.FactoryCanNotCreateTypeException(
					"The media factory could not create an IVideoMedia");
			}
			IVideoMedia copyVM = (IVideoMedia)copyM;
			copyVM.setClipBegin(getClipBegin().copy());
			copyVM.setClipEnd(setClipEnd().copy());
			copyVM.setLocation(setLocation().copy());
			copyVM.setWidth(setWidth());
			copyVM.setHeight(setHeight());

			return copyVM;
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

		#region IXUKAble members 

		/// <summary>
		/// Fill in audio data from an XML source.
		/// Assume that the XmlReader cursor is at the opening audio tag.
		/// </summary>
		/// <param name="source">the input XML source</param>
		/// <returns>true or false, depending on whether the data could be processed</returns>
		public bool XukIn(System.Xml.XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Reader is null");
			}

			if (source.NodeType != System.Xml.XmlNodeType.Element) return false;

			string cb = source.GetAttribute("clipBegin");
			string ce = source.GetAttribute("clipEnd");
			string height = source.GetAttribute("height");
			string width = source.GetAttribute("width");
			try
			{
				this.setClipBegin(new Time(cb));
				this.setClipEnd(new Time(ce));
			}
			catch (exception.TimeStringRepresentationIsInvalidException)
			{
				return false;
			}
			try
			{
				this.setHeight(int.Parse(height));
				this.setWidth(int.Parse(width));
			}
			catch (Exception)
			{
				return false;
			}


			IMediaLocation loc;

			if (!source.IsEmptyElement)
			{
				while(source.Read())
				{
				}
			}
			if (loc==null) return false;
			setLocation(loc);
			return true;
		}

		/// <summary>
		/// The opposite of <see cref="XUKIn"/>, this function writes the object's data
		/// to an XML file
		/// </summary>
		/// <param name="destination">the XML source for outputting data</param>
		/// <returns>so far, this function always returns true</returns>
		public bool XukOut(System.Xml.XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Writer is null");
			}

		
			destination.WriteStartElement("VideoMedia", urakawa.ToolkitSettings.XUK_NS);

			destination.WriteAttributeString("src", this.getLocation().Location);

			destination.WriteAttributeString("clipBegin", this.getClipBegin().getTimeAsString());

			destination.WriteAttributeString("clipEnd", this.getClipEnd().getTimeAsString());

			destination.WriteAttributeString("height", this.getHeight().ToString());

			destination.WriteAttributeString("width", this.getWidth().ToString());

			destination.WriteEndElement();

			return true;
		}

		
		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="VideoMedia"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="VideoMedia"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

		#region IMedia Members

		public IMediaFactory getMediaFactory()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IExternalLocation Members

		public IMediaLocation getLocation()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void setLocation(IMediaLocation location)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IClipTimes Members

		public ITimeDelta getDuration()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public ITime getClipBegin()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public ITime getClipEnd()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void setClipBegin(ITime beginPoint)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void setClipEnd(ITime endPoint)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IMedia split(ITime splitPoint)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
