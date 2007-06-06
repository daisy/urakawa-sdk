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
		string mSrc;
		IMediaFactory mFactory;
		
		/// <summary>
		/// Constructor initializing the <see cref="ImageMedia"/> with <see cref="ISized"/> <c>(0,0)</c>, 
		/// an empty src <see cref="string"/> and a given <see cref="IMediaFactory"/>
		/// </summary>
		/// <param name="fact">The given <see cref="IMediaFactory"/></param>
		protected internal ImageMedia(IMediaFactory fact)
		{
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException("The given media factory was null");
			}
			mFactory = fact;
			mWidth = 0;
			mHeight = 0;
			mSrc = "";
		}

		/// <summary>
		/// This override is useful while debugging
		/// </summary>
		/// <returns>A <see cref="string"/> representation of the <see cref="ImageMedia"/></returns>
		public override string ToString()
		{
			return String.Format("ImageMedia ({0}-{1:0}x{2:0})", getSrc(), mWidth, mHeight);
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
			newMedia.setSrc(this.getSrc());
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
		/// <param name="width">The new width</param>
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
		/// <param name="height">The new height</param>
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
		/// Reads the <see cref="ImageMedia"/> from a ImageMedia xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		public void XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element)
			{
				throw new exception.XukException("Can not read ImageMedia from a non-element node");
			}
			try
			{
				XukInAttributes(source);
				if (!source.IsEmptyElement)
				{
					while (source.Read())
					{
						if (source.NodeType == XmlNodeType.Element)
						{
							XukInChild(source);
						}
						else if (source.NodeType == XmlNodeType.EndElement)
						{
							break;
						}
						if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
					}
				}

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukIn of ImageMedia: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a ImageMedia xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInAttributes(XmlReader source)
		{
			string height = source.GetAttribute("height");
			string width = source.GetAttribute("width");
			int h, w;
			if (height != null && height != "")
			{
				if (!Int32.TryParse(height, out h))
				{
					throw new exception.XukException(
						String.Format("height attribute of {0} element is not an integer", source.LocalName));
				}
				setHeight(h);
			}
			else
			{
				setHeight(0);
			}
			if (width != null && width != "")
			{
				if (!Int32.TryParse(width, out w))
				{
					throw new exception.XukException(
						String.Format("width attribute of {0} element is not an integer", source.LocalName));
				}
				setWidth(w);
			}
			else
			{
				setWidth(0);
			}
			string s = source.GetAttribute("src");
			if (s == null)
			{
				throw new exception.XukException(
					String.Format("src attribute of {0} element is missing", source.LocalName));
			}
			setSrc(s);
		}

		/// <summary>
		/// Reads a child of a ImageMedia xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInChild(XmlReader source)
		{
			bool readItem = false;
			if (source.NamespaceURI == ToolkitSettings.XUK_NS)
			{
				readItem = true;
				switch (source.LocalName)
				{
					default:
						readItem = false;
						break;
				}
			}
			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past unknown child 
			}
		}

		/// <summary>
		/// Write a ImageMedia element to a XUK file representing the <see cref="ImageMedia"/> instance
		/// </summary>
		/// <param localName="destination">The destination <see cref="XmlWriter"/></param>
		public void XukOut(XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not XukOut to a null XmlWriter");
			}
			try
			{
				destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
				XukOutAttributes(destination);
				XukOutChildren(destination);

				destination.WriteEndElement();

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukOut of ImageMedia: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a ImageMedia element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutAttributes(XmlWriter destination)
		{
			destination.WriteAttributeString("height", this.mHeight.ToString());
			destination.WriteAttributeString("width", this.mWidth.ToString());
			destination.WriteAttributeString("src", getSrc());
		}

		/// <summary>
		/// Write the child elements of a ImageMedia element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutChildren(XmlWriter destination)
		{

		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="ImageMedia"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="ImageMedia"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion



		#region ILocated Members


		/// <summary>
		/// Gets the src of <c>this</c>
		/// </summary>
		/// <returns>The src</returns>
		public string getSrc()
		{
			return mSrc;
		}

		/// <summary>
		/// Sets the src of <c>this</c>
		/// </summary>
		/// <param name="src">The new src</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="src"/> is <c>null</c></exception>
		public void setSrc(string src)
		{
			if (src == null)
			{
				throw new exception.MethodParameterIsNullException("The src can not be null");
			}
			mSrc = src;
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
			if (getSrc()!=otherImage.getSrc()) return false;
			if (getHeight() != otherImage.getHeight()) return false;
			if (getWidth() != otherImage.getWidth()) return false;
			return true;
		}

		#endregion
	}
}
