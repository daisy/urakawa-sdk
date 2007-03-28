using System;
using System.Xml;
using urakawa.media.timing;

namespace urakawa.media
{
	/// <summary>
	/// VideoMedia is the video object.
	/// It is time-based, comes from an external source, and has a visual presence.
	/// </summary>
	public class VideoMedia : IVideoMedia
	{
		IMediaFactory mFactory;
		int mWidth = 0;
		int mHeight= 0;
		ITime mClipBegin = new Time();
		ITime mClipEnd = new Time(TimeSpan.MaxValue);
		IMediaLocation mLocation;

		private void resetClipTimes()
		{
			mClipBegin = new Time();
			mClipEnd = new Time();
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		protected internal VideoMedia(IMediaFactory fact)
		{
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException("A video media must have a media factory");
			}
			mFactory = fact;
			mLocation = fact.createMediaLocation();
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
			if (getClipBegin().isNegativeTimeOffset())
			{
				copyVM.setClipBegin(getClipBegin().copy());
				copyVM.setClipEnd(getClipEnd().copy());
			}
			else
			{
				copyVM.setClipEnd(getClipEnd().copy());
				copyVM.setClipBegin(getClipBegin().copy());
			}
			copyVM.setLocation(getLocation().copy());
			copyVM.setWidth(getWidth());
			copyVM.setHeight(getHeight());

			return copyVM;
		}

		#endregion

		#region ISized Members

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

		#endregion

		#region IXUKAble members 

		/// <summary>
		/// Reads data from the attributes of a xuk xml element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the attributes were succesfully read</returns>
		protected virtual bool XukInAttributes(XmlReader source)
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
				Time ceTime = new Time(ce);
				Time cbTime = new Time(cb);
				if (cbTime.isNegativeTimeOffset())
				{
					setClipBegin(cbTime);
					setClipEnd(ceTime);
				}
				else
				{
					setClipEnd(ceTime);
					setClipBegin(cbTime);
				}
			}
			catch (exception.TimeStringRepresentationIsInvalidException)
			{
				return false;
			}
			catch (exception.MethodParameterIsOutOfBoundsException)
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

			return true;
		}

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

			if (!XukInAttributes(source)) return false;

			IMediaLocation loc = null;

			if (!source.IsEmptyElement)
			{
				while(source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						loc = getMediaFactory().createMediaLocation(source.LocalName, source.NamespaceURI);
						if (loc == null)
						{
							if (!source.IsEmptyElement)
							{
								//Read past unrecognized element
								source.ReadSubtree().Close();
							}
						}
						else
						{
							if (!loc.XukIn(source)) return false;
						}
					}
					else if (source.NodeType == XmlNodeType.EndElement)
					{
						break;
					}
					if (source.EOF) break;
				}
			}
			if (loc==null) return false;
			setLocation(loc);
			return true;
		}

		/// <summary>
		/// Writes the attributes of the xuk xml element for <c>this</c>
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the attributes were succesfully written</returns>
		protected virtual bool XukOutAttributes(XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Writer is null");
			}
			destination.WriteAttributeString("clipBegin", this.getClipBegin().ToString());

			destination.WriteAttributeString("clipEnd", this.getClipEnd().ToString());

			destination.WriteAttributeString("height", this.getHeight().ToString());

			destination.WriteAttributeString("width", this.getWidth().ToString());

			return true;
		}

		/// <summary>
		/// The opposite of <see cref="XukIn"/>, this function writes the object's data
		/// to an XML file
		/// </summary>
		/// <param name="destination">the XML source for outputting data</param>
		/// <returns>so far, this function always returns true</returns>
		public bool XukOut(XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Writer is null");
			}
			destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
			if (!XukOutAttributes(destination)) return false;
			if (!getLocation().XukOut(destination)) return false;
			destination.WriteEndElement();
			return true;
		}

	
		/// <summary>
		/// Gets the local localName part of the QName representing a <see cref="VideoMedia"/> in Xuk
		/// </summary>
		/// <returns>The local localName part</returns>
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

		/// <summary>
		/// Gets the <see cref="IMediaFactory"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IMediaFactory"/></returns>
		public IMediaFactory getMediaFactory()
		{
			return mFactory;
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
		/// <param name="location">The new <see cref="IMediaLocation"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">Thrown when the new <see cref="IMediaLocation"/> is null</exception>
		public void setLocation(IMediaLocation location)
		{
			if (location == null)
			{
				throw new exception.MethodParameterIsNullException("The location can not be null");
			}
			mLocation = location;
		}

		#endregion


		#region IContinuous Members

		/// <summary>
		/// Gets the duration of <c>this</c>
		/// </summary>
		/// <returns>The duration</returns>
		public ITimeDelta getDuration()
		{
			return getClipEnd().getTimeDelta(getClipBegin());
		}

		#endregion


		#region IClipped Members

		/// <summary>
		/// Gets the clip begin <see cref="ITime"/> of <c>this</c>
		/// </summary>
		/// <returns>The clip begin <see cref="ITime"/></returns>
		public ITime getClipBegin()
		{
			return mClipBegin;
		}

		/// <summary>
		/// Gets the clip end <see cref="ITime"/> of <c>this</c>
		/// </summary>
		/// <returns>The clip end <see cref="ITime"/></returns>
		public ITime getClipEnd()
		{
			return mClipEnd;
		}

		/// <summary>
		/// Sets the clip begin <see cref="ITime"/>
		/// </summary>
		/// <param name="beginPoint">The new clip begin <see cref="ITime"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when te new clip begin <see cref="ITime"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the new begin point is beyond the current clip end
		/// </exception>
		public void setClipBegin(ITime beginPoint)
		{
			if (beginPoint == null)
			{
				throw new exception.MethodParameterIsNullException("The clip begin time can not be null");
			}
			if (beginPoint.isGreaterThan(getClipEnd()))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The new clip begin time can not be beyond clip end");
			}
			mClipBegin = beginPoint;
		}

		/// <summary>
		/// Sets the clip begin <see cref="ITime"/>
		/// </summary>
		/// <param name="endPoint">The new clip end <see cref="ITime"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when te new clip end <see cref="ITime"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the new end point is before the current clip begin
		/// </exception>
		public void setClipEnd(ITime endPoint)
		{
			if (endPoint == null)
			{
				throw new exception.MethodParameterIsNullException("The clip end time can not be null");
			}
			if (endPoint.isLessThan(getClipBegin()))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The new clip end time can not be before clip begin");
			}
			mClipEnd = endPoint;
		}

		IContinuous IContinuous.split(ITime splitPoint)
		{
			return split(splitPoint);
		}

		/// <summary>
		/// Splits <c>this</c> at a given split point in <see cref="ITime"/>. 
		/// The retains the clip between clip begin and the split point and a new <see cref="IVideoMedia"/>
		/// is created consisting of the clip from the split point to clip end
		/// </summary>
		/// <param name="splitPoint">The split point</param>
		/// <returns>The new <see cref="IVideoMedia"/> containing the latter prt of the clip</returns>
		public IVideoMedia split(ITime splitPoint)
		{
			if (splitPoint == null)
			{
				throw new exception.MethodParameterIsNullException("The split point can not be null");
			}
			if (getClipBegin().isGreaterThan(splitPoint) || splitPoint.isGreaterThan(getClipEnd()))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The split point is not between clip begin and clip end");
			}
			IVideoMedia secondPart = copy();
			secondPart.setClipBegin(splitPoint.copy());
			setClipEnd(splitPoint.copy());
			return secondPart;
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
			if (!(other is IVideoMedia)) return false;
			IVideoMedia otherVideo = (IVideoMedia)other;
			if (!getLocation().ValueEquals(otherVideo.getLocation())) return false;
			if (!getClipBegin().isEqualTo(otherVideo.getClipBegin())) return false;
			if (!getClipEnd().isEqualTo(otherVideo.getClipEnd())) return false;
			if (getWidth() != otherVideo.getWidth()) return false;
			if (getHeight() != otherVideo.getHeight()) return false;
			return true;
		}

		#endregion
	}
}