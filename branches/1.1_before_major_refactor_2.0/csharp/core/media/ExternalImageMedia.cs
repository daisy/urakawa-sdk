using System;
using System.Xml;
using urakawa.progress;


namespace urakawa.media
{
	/// <summary>
	/// ImageMedia is the image object. 
	/// It has width, height, and an external source.
	/// </summary>
	public class ExternalImageMedia : ExternalMedia, IImageMedia
	{
		#region Event related members
		/// <summary>
		/// Event fired after the size (height or width) of the <see cref="ExternalImageMedia"/> has changed
		/// </summary>
		public event EventHandler<events.media.SizeChangedEventArgs> sizeChanged;
		/// <summary>
		/// Fires the <see cref="sizeChanged"/> event
		/// </summary>
		/// <param name="source">The source, that is the <see cref="ExternalImageMedia"/> whoose size has changed</param>
		/// <param name="newHeight">The new height of the <see cref="ExternalImageMedia"/></param>
		/// <param name="newWidth">The new width of the <see cref="ExternalImageMedia"/></param>
		/// <param name="prevHeight">The height of the <see cref="ExternalImageMedia"/> prior to the change</param>
		/// <param name="prevWidth">The width of the <see cref="ExternalImageMedia"/> prior to the change</param>
		protected void notifySizeChanged(ExternalImageMedia source, int newHeight, int newWidth, int prevHeight, int prevWidth)
		{
			EventHandler<events.media.SizeChangedEventArgs> d = sizeChanged;
			if (d != null) d(this, new urakawa.events.media.SizeChangedEventArgs(source, newHeight, newWidth, prevHeight, prevWidth));
		}

		void this_sizeChanged(object sender, urakawa.events.media.SizeChangedEventArgs e)
		{
			notifyChanged(e);
		}
		#endregion
		int mWidth;
		int mHeight;
		
		/// <summary>
		/// Constructor initializing the <see cref="ExternalImageMedia"/> with <see cref="ISized"/> <c>(0,0)</c>, 
		/// an empty src <see cref="string"/> and a given <see cref="IMediaFactory"/>
		/// </summary>
		protected internal ExternalImageMedia() : base()
		{
			mWidth = 0;
			mHeight = 0;
			this.sizeChanged += new EventHandler<urakawa.events.media.SizeChangedEventArgs>(this_sizeChanged);
		}

		/// <summary>
		/// This override is useful while debugging
		/// </summary>
		/// <returns>A <see cref="string"/> representation of the <see cref="ExternalImageMedia"/></returns>
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
		public override bool isContinuous()
		{
			return false;
		}

		/// <summary>
		/// This always returns <c>true</c>, because
		/// image media is always considered discrete
		/// </summary>
		/// <returns><c>true</c></returns>
		public override bool isDiscrete()
		{
			return true;
		}

		/// <summary>
		/// This always returns <c>false</c>, because
		/// a single media object is never considered to be a sequence
		/// </summary>
		/// <returns><c>false</c></returns>
		public override bool isSequence()
		{
			return false;
		}

		/// <summary>
		/// Creates a copy of the <c>this</c>
		/// </summary>
		/// <returns>The copy</returns>
		public new ExternalImageMedia copy()
		{
			return copyProtected() as ExternalImageMedia;
		}

		/// <summary>
		/// Exports <c>this</c> to a destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The destination <see cref="Presentation"/></param>
		/// <returns>The export</returns>
		public new ExternalImageMedia export(Presentation destPres)
		{
			return exportProtected(destPres) as ExternalImageMedia;
		}

		/// <summary>
		/// Exports the external image media to a destination <see cref="Presentation"/>
		/// - part of a construct allowing the <see cref="export"/> method to return <see cref="ExternalImageMedia"/>
		/// </summary>
		/// <param name="destPres">The destination presentation</param>
		/// <returns>The exported external video media</returns>
		protected override IMedia exportProtected(Presentation destPres)
		{
			ExternalImageMedia exported = base.exportProtected(destPres) as ExternalImageMedia;
			if (exported == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The MediaFactory of the destination Presentation of the export cannot create a ExternalImageMedia matching QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			exported.setHeight(this.getHeight());
			exported.setWidth(this.getWidth());
			return exported;
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
			setSize(getHeight(), width);
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
			setSize(height, getWidth());
		}


		/// <summary>
		/// Sets the image size
		/// </summary>
		/// <param name="height">The new height</param>
		/// <param name="width">The new width</param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the new width or height is negative
		/// </exception>
		public void setSize(int height, int width)
		{
			if (width < 0)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The width of an image can not be negative");
			}
			if (height < 0)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The height of an image can not be negative");
			}
			int prevWidth = mWidth;
			mWidth = width;
			int prevHeight = mHeight;
			mHeight = height;
			if (mWidth != prevWidth || mHeight != prevHeight)
			{
				notifySizeChanged(this, mHeight, mWidth, prevHeight, prevWidth);
			}
		}

		#endregion
				
		#region IXUKAble members

		/// <summary>
		/// Reads the attributes of a ImageMedia xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected override void xukInAttributes(XmlReader source)
		{
			base.xukInAttributes(source);
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
		}

		/// <summary>
		/// Reads a child of a ImageMedia xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        protected override void xukInChild(XmlReader source, ProgressHandler handler)
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
		/// Writes the attributes of a ImageMedia element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected override void xukOutAttributes(XmlWriter destination, Uri baseUri)
		{
			destination.WriteAttributeString("height", this.mHeight.ToString());
			destination.WriteAttributeString("width", this.mWidth.ToString());
			base.xukOutAttributes(destination, baseUri);
		}

		#endregion
		
		#region IValueEquatable<IMedia> Members

		/// <summary>
		/// Conpares <c>this</c> with a given other <see cref="IMedia"/> for equality
		/// </summary>
		/// <param name="other">The other <see cref="IMedia"/></param>
		/// <returns><c>true</c> if equal, otherwise <c>false</c></returns>
		public override bool valueEquals(IMedia other)
		{
			if (!base.valueEquals(other)) return false;
			IImageMedia otherImage = (IImageMedia)other;
			if (getHeight() != otherImage.getHeight()) return false;
			if (getWidth() != otherImage.getWidth()) return false;
			return true;
		}

		#endregion
	}
}
