using System;
using System.Xml;


namespace urakawa.media
{
	/// <summary>
	/// ImageMedia is the image object. 
	/// It has width, height, and an external source.
	/// </summary>
	public class ImageMedia : IImageMedia
	{
		int mWidth;
		int mHeight;
		IMediaLocation mLocation;
		IMediaFactory mFactory;
		
		/// <summary>
		/// Constructor initializing the <see cref="ImageMedia"/> with <see cref="ISized"/> <c>(0,0)</c>, 
		/// an empty <see cref="SrcMediaLocation"/> and a given <see cref="IMediaFactory"/>
		/// </summary>
		/// <param localName="fact">The given <see cref="IMediaFactory"/></param>
		protected internal ImageMedia(IMediaFactory fact)
		{
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException("The given media factory was null");
			}
			mFactory = fact;
			mWidth = 0;
			mHeight = 0;
			mLocation = mFactory.createMediaLocation();
		}

		/// <summary>
		/// This override is useful while debugging
		/// </summary>
		/// <returns>A <see cref="string"/> representation of the <see cref="ImageMedia"/></returns>
		public override string ToString()
		{
			IMediaLocation l = getLocation();
			return String.Format("ImageMedia ({0}-{1:0}x{2:0})", l.ToString(), mWidth, mHeight);
		}

		#region IMedia Members

		/// <summary>
		/// This always returns <c>false</c>, because
		/// image media is never considered continuous
		/// </summary>
		/// <returns><c>false</c></returns>
		public bool isContinuous()
		{
			return false;
		}

		/// <summary>
		/// This always returns <c>true</c>, because
		/// image media is always considered discrete
		/// </summary>
		/// <returns><c>true</c></returns>
		public bool isDiscrete()
		{
			return true;
		}

		/// <summary>
		/// This always returns <c>false</c>, because
		/// a single media object is never considered to be a sequence
		/// </summary>
		/// <returns><c>false</c></returns>
		public bool isSequence()
		{
			return false;
		}

		/// <summary>
		/// Return the urakawa media type
		/// </summary>
		/// <returns>always returns <see cref="MediaType.IMAGE"/></returns>
		public MediaType getMediaType()
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
		/// <returns>The copy</returns>
		public IImageMedia copy()
		{
			IMedia copyM = getMediaFactory().createMedia(getXukLocalName(), getXukNamespaceUri());
			if (copyM == null || !(copyM is IImageMedia))
			{
				throw new exception.FactoryCanNotCreateTypeException(String.Format(
					"The media factory does not create IImageMedia when passed QName {0}:{1}",
					getXukNamespaceUri(), getXukLocalName()));
			}
			IImageMedia newMedia = (IImageMedia)copyM;
			newMedia.setHeight(this.getHeight());
			newMedia.setWidth(this.getWidth());
			newMedia.setLocation(this.getLocation().copy());
			return newMedia;
		}


		/// <summary>
		/// Gets the <see cref="IMediaFactory"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IMediaFactory"/></returns>
		public IMediaFactory getMediaFactory()
		{
			return mFactory;
		}


		#endregion

		#region ISized Members

		/// <summary>
		/// Return the image width
		/// </summary>
		/// <returns>The width</returns>
		public int getWidth()
		{
			return mWidth;
		}

		/// <summary>
		/// Return the image height
		/// </summary>
		/// <returns>The height</returns>
		public int getHeight()
		{
			return mHeight;
		}

		/// <summary>
		/// Sets the image width
		/// </summary>
		/// <param localName="width">The new width</param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the new width is negative
		/// </exception>
		public void setWidth(int width)
		{
			if (width < 0)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The width of an image can not be negative");
			}
			mWidth = width;
		}

		/// <summary>
		/// Sets the image height
		/// </summary>
		/// <param localName="height">The new height</param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the new height is negative
		/// </exception>
		public void setHeight(int height)
		{
			if (height < 0)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The height of an image can not be negative");
			}
			mHeight = height;
		}

		#endregion

		#region IXUKAble members 

		/// <summary>
		/// Fill in audio data from an XML source.
		/// Assume that the XmlReader cursor is at the opening audio tag.
		/// </summary>
		/// <param localName="source">the input XML source</param>
		/// <returns>true or false, depending on whether the data could be processed</returns>
		public bool XukIn(System.Xml.XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Reader is null");
			}
			if (source.NodeType != System.Xml.XmlNodeType.Element) return false;


			string height = source.GetAttribute("height");
			string width = source.GetAttribute("width");

			try
			{
				this.setHeight(int.Parse(height));
				this.setWidth(int.Parse(width));
			}
			catch (Exception)
			{
				return false;
			}
			IMediaLocation location = null;
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						location = getMediaFactory().createMediaLocation(source.LocalName, source.NamespaceURI);
						if (location == null)
						{
							//Read past unrecognized element
							source.ReadSubtree().Close();
						}
						else
						{
							if (!location.XukIn(source)) return false;
						}
					}
					else if (source.NodeType == XmlNodeType.EndElement)
					{
						break;
					}
					if (source.EOF) break;
				}
			}
			if (location == null) return false;
			setLocation(location);
			return true;
		}

		/// <summary>
		/// The opposite of <see cref="XukIn"/>, this function writes the object's data
		/// to an XML file
		/// </summary>
		/// <param localName="destination">the XML source for outputting data</param>
		/// <returns>so far, this function always returns true</returns>
		public bool XukOut(System.Xml.XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Writer is null");
			}

			destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());

			destination.WriteAttributeString("type", "IMAGE");

			destination.WriteAttributeString("height", this.mHeight.ToString());

			destination.WriteAttributeString("width", this.mWidth.ToString());

			if (!getLocation().XukOut(destination)) return false;

			destination.WriteEndElement();

			return true;
		}

		
		/// <summary>
		/// Gets the local localName part of the QName representing a <see cref="ImageMedia"/> in Xuk
		/// </summary>
		/// <returns>The local localName part</returns>
		public string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="ImageMedia"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

		#region ILocated Members

		/// <summary>
		/// Gets the <see cref="IMediaLocation"/> of <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IMediaLocation"/></returns>
		public IMediaLocation getLocation()
		{
			return mLocation;
		}

		/// <summary>
		/// Sets the <see cref="IMediaLocation"/> of <c>this</c>
		/// </summary>
		/// <param localName="location">The new <see cref="IMediaLocation"/></param>
		public void setLocation(IMediaLocation location)
		{
			if (location == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The media location of an image can not be null");
			}
			mLocation = location;
		}

		#endregion

		#region IValueEquatable<IMedia> Members

		/// <summary>
		/// Conpares <c>this</c> with a given other <see cref="IMedia"/> for equality
		/// </summary>
		/// <param name="other">The other <see cref="IMedia"/></param>
		/// <returns><c>true</c> if equal, otherwise <c>false</c></returns>
		public bool ValueEquals(IMedia other)
		{
			if (!(other is IImageMedia)) return false;
			IImageMedia otherImage = (IImageMedia)other;
			if (!getLocation().ValueEquals(otherImage.getLocation())) return false;
			if (getHeight() != otherImage.getHeight()) return false;
			if (getWidth() != otherImage.getWidth()) return false;
			return true;
		}

		#endregion
	}
}
