using System;
using System.Xml;
using urakawa.media.timing;

namespace urakawa.media
{
	/// <summary>
	/// AudioMedia is the audio object.
	/// It is time-based and comes from an external source.
	/// </summary>
	public class ExternalAudioMedia : ExternalMedia, IAudioMedia, IClipped
	{
		private Time mClipBegin;
		private Time mClipEnd;

		private void resetClipTimes()
		{
			mClipBegin = Time.Zero;
			mClipEnd = Time.MaxValue;
		}

		/// <summary>
		/// Constructor setting the associated <see cref="IMediaFactory"/>
		/// </summary>
		/// <param name="fact">The <see cref="IMediaFactory"/> with which to associate</param>
		protected internal ExternalAudioMedia(IMediaFactory fact) : base(fact)
		{
			resetClipTimes();
		}
		
		#region IMedia members
		/// <summary>
		/// This always returns true, because
		/// audio media is always considered continuous
		/// </summary>
		/// <returns></returns>
		public override bool isContinuous()
		{
			return true;
		}

		/// <summary>
		/// This always returns false, because
		/// audio media is never considered discrete
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
		/// Copy function which returns an <see cref="IAudioMedia"/> object
		/// </summary>
		/// <returns>A copy of this</returns>
		/// <exception cref="exception.FactoryCannotCreateTypeException">
		/// Thrown when the <see cref="IMediaFactory"/> associated with this 
		/// can not create an <see cref="ExternalAudioMedia"/> matching the QName of <see cref="ExternalAudioMedia"/>
		/// </exception>
		public new ExternalAudioMedia copy()
		{
			return copyProtected() as ExternalAudioMedia;
		}

		/// <summary>
		/// Exports the external audio media to a destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The destination presentation</param>
		/// <returns>The exported external audio media</returns>
		public new ExternalAudioMedia export(Presentation destPres)
		{
			return exportProtected(destPres) as ExternalAudioMedia;
		}

		/// <summary>
		/// Exports the external audio media to a destination <see cref="Presentation"/>
		/// - part of technical construct to have <see cref="export"/> return <see cref="ExternalAudioMedia"/>
		/// </summary>
		/// <param name="destPres">The destination presentation</param>
		/// <returns>The exported external audio media</returns>
		protected override ExternalMedia exportProtected(Presentation destPres)
		{
			ExternalAudioMedia exported = base.exportProtected(destPres) as ExternalAudioMedia;
			if (exported==null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The MediaFactory cannot create a ExternalAudioMedia matching QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			exported.setClipBegin(getClipBegin().copy());
			exported.setClipEnd(getClipEnd().copy());
			return exported;
		}

		#endregion



		#region IXUKAble members

		/// <summary>
		/// Reads the attributes of a ExternalAudioMedia xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected override void XukInAttributes(XmlReader source)
		{
			base.XukInAttributes(source);
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
		}

		/// <summary>
		/// Writes the attributes of a ExternalAudioMedia element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
		{
			base.XukOutAttributes(destination, baseUri);
			destination.WriteAttributeString("clipBegin", this.getClipBegin().ToString());
			destination.WriteAttributeString("clipEnd", this.getClipEnd().ToString());
		}

		/// <summary>
		/// Write the child elements of a ExternalAudioMedia element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutChildren(XmlWriter destination)
		{

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
			if (beginPoint.isLessThan(Time.Zero))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"ClipBegin is a negative time offset");
			}
			if (beginPoint.isGreaterThan(getClipEnd()))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"ClipBegin can not be after ClipEnd"); 
			}
			mClipBegin = beginPoint.copy();
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
			mClipEnd = endPoint.copy();
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
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="splitPoint"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref name="splitPoint"/> is not between clip begin and clip end
		/// </exception>
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
		public override bool valueEquals(IMedia other)
		{
			if (!base.valueEquals(other)) return false;
			ExternalAudioMedia otherAudio = (ExternalAudioMedia)other;
			if (!getClipBegin().isEqualTo(otherAudio.getClipBegin())) return false;
			if (!getClipEnd().isEqualTo(otherAudio.getClipEnd())) return false;
			return true;
		}

		#endregion

	}
}
