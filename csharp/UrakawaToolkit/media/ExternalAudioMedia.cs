using System;
using System.Xml;
using urakawa.media.timing;

namespace urakawa.media
{
	/// <summary>
	/// AudioMedia is the audio object.
	/// It is time-based and comes from an external source.
	/// </summary>
	public class ExternalAudioMedia : IAudioMedia, IClipped, ILocated
	{
		private string mSrc;
		private Time mClipBegin;
		private Time mClipEnd;
		private IMediaFactory mFactory;
		private string mLanguage;

		private void resetClipTimes()
		{
			mClipBegin = new Time();
			mClipEnd = new Time(TimeSpan.MaxValue);
			mSrc = "";
		}

		/// <summary>
		/// Constructor setting the associated <see cref="IMediaFactory"/>
		/// </summary>
		/// <param name="fact">The <see cref="IMediaFactory"/> with which to associate</param>
		protected internal ExternalAudioMedia(IMediaFactory fact)
		{
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException("Factory is null");
			}
			mFactory = fact;
			mSrc = "";
			resetClipTimes();
			mLanguage = null;
		}
		
		#region IMedia members

		/// <summary>
		/// Sets the language of the external audio media
		/// </summary>
		/// <param name="lang">The new language, can be null but not an empty string</param>
		public void setLanguage(string lang)
		{
			if (lang == "")
			{
				throw new exception.MethodParameterIsEmptyStringException(
					"The language can not be an empty string");
			}
			mLanguage = lang;
		}

		/// <summary>
		/// Gets the language of the external audio media
		/// </summary>
		/// <returns>The language</returns>
		public string getLanguage()
		{
			return mLanguage;
		}
		/// <summary>
		/// This always returns true, because
		/// audio media is always considered continuous
		/// </summary>
		/// <returns></returns>
		public bool isContinuous()
		{
			return true;
		}

		/// <summary>
		/// This always returns false, because
		/// audio media is never considered discrete
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
		
		IMedia IMedia.copy()
		{
			return copy();
		}

		/// <summary>
		/// Copy function which returns an <see cref="IAudioMedia"/> object
		/// </summary>
		/// <returns>A copy of this</returns>
		/// <exception cref="exception.FactoryCannotCreateTypeException">
		/// Thrown when the <see cref="IMediaFactory"/> associated with this 
		/// can not create an <see cref="ExternalAudioMedia"/> matching the QName of <see cref="ExternalAudioMedia"/>
		/// </exception>
		public ExternalAudioMedia copy()
		{
			return export(getMediaFactory().getPresentation());
		}

		IMedia IMedia.export(Presentation destPres)
		{
			return export(destPres);
		}

		/// <summary>
		/// Exports the external audio media to a destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The destination presentation</param>
		/// <returns>The exported external audio media</returns>
		public ExternalAudioMedia export(Presentation destPres)
		{
			ExternalAudioMedia exported = destPres.getMediaFactory().createMedia(
				getXukLocalName(), getXukNamespaceUri()) as ExternalAudioMedia;
			if (exported==null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The MediaFacotry cannot create a ExternalAudioMedia matching QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			exported.setClipBegin(getClipBegin().copy());
			exported.setClipEnd(getClipEnd().copy());
			exported.setSrc(getSrc());
			return exported;
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

		#region Old IXUKAble members

		///// <summary>
		///// Fill in audio data from an XML source.
		///// Assume that the XmlReader cursor is at the opening audio tag.
		///// </summary>
		///// <param name="source">the input XML source</param>
		///// <returns>true or false, depending on whether the data could be processed</returns>
		//public bool XukIn(System.Xml.XmlReader source)
		//{
		//  if (source == null)
		//  {
		//    throw new exception.MethodParameterIsNullException("Xml Reader is null");
		//  }
		//  if (source.NodeType != System.Xml.XmlNodeType.Element) return false;

		//  string cb = source.GetAttribute("clipBegin");
		//  string ce = source.GetAttribute("clipEnd");

		//  resetClipTimes();

		//  try
		//  {
		//    Time ceTime = new Time(ce);
		//    Time cbTime = new Time(cb);
		//    if (cbTime.isNegativeTimeOffset())
		//    {
		//      setClipBegin(cbTime);
		//      setClipEnd(ceTime);
		//    }
		//    else
		//    {
		//      setClipEnd(ceTime);
		//      setClipBegin(cbTime);
		//    }
		//  }
		//  catch (exception.TimeStringRepresentationIsInvalidException)
		//  {
		//    return false;
		//  }
		//  catch (exception.MethodParameterIsOutOfBoundsException)
		//  {
		//    return false;
		//  }
		//  IMediaLocation loc = null;

		//  if (!source.IsEmptyElement)
		//  {
		//    while (source.Read())
		//    {
		//      if (source.NodeType == XmlNodeType.Element)
		//      {
		//        loc = getMediaFactory().createMediaLocation(source.LocalName, source.NamespaceURI);
		//        if (loc == null)
		//        {
		//          if (!source.IsEmptyElement)
		//          {
		//            //Read past unrecognized element
		//            source.ReadSubtree().Close();
		//          }
		//        }
		//        else
		//        {
		//          if (!loc.XukIn(source)) return false;
		//        }
		//      }
		//      else if (source.NodeType == XmlNodeType.EndElement)
		//      {
		//        break;
		//      }
		//      if (source.EOF) break;
		//    }
		//  }
		//  if (loc == null) return false;
		//  setSrc(loc);
		//  return true;
		//}
		#endregion


		#region IXUKAble members



		/// <summary>
		/// Reads the <see cref="ExternalAudioMedia"/> from a ExternalAudioMedia xuk element
		/// </summary>
		/// <param name="source">The source <see cref="System.Xml.XmlReader"/></param>
		public void XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element)
			{
				throw new exception.XukException("Can not read ExternalAudioMedia from a non-element node");
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
					String.Format("An exception occured during XukIn of ExternalAudioMedia: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a ExternalAudioMedia xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInAttributes(XmlReader source)
		{
			resetClipTimes();
			Time cbTime, ceTime;
			try
			{
				cbTime = new Time(source.GetAttribute("clipBegin"));
			}
			catch (exception.CheckedException e)
			{
				throw new exception.XukException(
					String.Format("clipBegin attribute is missing or has invalid value: {0}", e.Message),
					e);
			}
			try
			{
				ceTime = new Time(source.GetAttribute("clipEnd"));
			}
			catch (exception.CheckedException e)
			{
				throw new exception.XukException(
					String.Format("clipEnd attribute is missing or has invalid value: {0}", e.Message),
					e);
			}
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
			string s = source.GetAttribute("src");
			if (s == null)
			{
				throw new exception.XukException(
					"src attribute is missing from ExternamAudioMedia element");
			}
			setSrc(s);
			string lang = source.GetAttribute("Language").Trim();
			if (lang != "") lang = null;
			setLanguage(lang);
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
		/// Write a ExternalAudioMedia element to a XUK file representing the <see cref="ExternalAudioMedia"/> instance
		/// </summary>
		/// <param localName="destination">The destination <see cref="System.Xml.XmlWriter"/></param>
		public void XukOut(System.Xml.XmlWriter destination)
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
					String.Format("An exception occured during XukOut of ExternalAudioMedia: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a ExternalAudioMedia element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutAttributes(XmlWriter destination)
		{
			destination.WriteAttributeString("clipBegin", this.getClipBegin().ToString());
			destination.WriteAttributeString("clipEnd", this.getClipEnd().ToString());
			destination.WriteAttributeString("src", getSrc());
			if (getLanguage() != null) destination.WriteAttributeString("Language", getLanguage());
		}

		/// <summary>
		/// Write the child elements of a ExternalAudioMedia element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutChildren(XmlWriter destination)
		{

		}

		
		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="ExternalAudioMedia"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="ExternalAudioMedia"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public string getXukNamespaceUri()
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

		#region IContinuous Members

		/// <summary>
		/// Gets the duration of <c>this</c>
		/// </summary>
		/// <returns>A <see cref="TimeDelta"/> representing the duration</returns>
		public TimeDelta getDuration()
		{
			return getClipEnd().getTimeDelta(getClipBegin());
		}

		#endregion

		#region IClipped Members

		/// <summary>
		/// Gets the clip begin <see cref="Time"/> of <c>this</c>
		/// </summary>
		/// <returns>Clip begin</returns>
		public Time getClipBegin()
		{
			return mClipBegin;
		}

		/// <summary>
		/// Gets the clip end <see cref="Time"/> of <c>this</c>
		/// </summary>
		/// <returns>Clip end</returns>
		public Time getClipEnd()
		{
			return mClipEnd;
		}

		/// <summary>
		/// Sets the clip begin <see cref="Time"/>
		/// </summary>
		/// <param name="beginPoint">The new clip begin <see cref="Time"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="beginPoint"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref localName="beginPoint"/> is beyond clip end of <c>this</c>
		/// </exception>
		public void setClipBegin(Time beginPoint)
		{
			if (beginPoint==null)
			{
				throw new exception.MethodParameterIsNullException("ClipBegin can not be null");
			}
			if (beginPoint.isGreaterThan(getClipEnd()))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"ClipBegin can not be after ClipEnd"); 
			}
			mClipBegin = beginPoint;
		}

		/// <summary>
		/// Sets the clip end <see cref="Time"/>
		/// </summary>
		/// <param name="endPoint">The new clip end <see cref="Time"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="endPoint"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref localName="endPoint"/> is before clip begin of <c>this</c>
		/// </exception>
		public void setClipEnd(Time endPoint)
		{
			if (endPoint == null)
			{
				throw new exception.MethodParameterIsNullException("ClipEnd can not be null");
			}
			if (endPoint.isLessThan(getClipBegin()))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"ClipEnd can not be before ClipBegin");
			}
			mClipEnd = endPoint;
		}

		IContinuous IContinuous.split(Time splitPoint)
		{
			return split(splitPoint);
		}

		/// <summary>
		/// Splits <c>this</c> at a given <see cref="Time"/>
		/// </summary>
		/// <param name="splitPoint">The <see cref="Time"/> at which to split - 
		/// must be between clip begin and clip end <see cref="Time"/>s</param>
		/// <returns>
		/// A newly created <see cref="IAudioMedia"/> containing the audio after,
		/// <c>this</c> retains the audio before <paramref localName="splitPoint"/>.
		/// </returns>
		public ExternalAudioMedia split(Time splitPoint)
		{
			if (splitPoint==null)
			{
				throw new exception.MethodParameterIsNullException(
					"The time at which to split can not be null");
			}
			if (splitPoint.isLessThan(getClipBegin()))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The split time can not be before ClipBegin");
			}
			if (splitPoint.isGreaterThan(getClipEnd()))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The split time can not be after ClipEnd");
			}
			ExternalAudioMedia splitAM = (ExternalAudioMedia)copy();
			setClipEnd(splitPoint);
			splitAM.setClipBegin(splitPoint);
			return splitAM;

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
			if (other == null) return false;
			if (getLanguage() != other.getLanguage()) return false;
			if (GetType() != other.GetType()) return false;
			ExternalAudioMedia otherAudio = (ExternalAudioMedia)other;
			if (getSrc()!=otherAudio.getSrc()) return false;
			if (!getClipBegin().isEqualTo(otherAudio.getClipBegin())) return false;
			if (!getClipEnd().isEqualTo(otherAudio.getClipEnd())) return false;
			return true;
		}

		#endregion

	}
}
