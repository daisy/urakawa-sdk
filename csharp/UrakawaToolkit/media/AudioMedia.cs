using System;
using System.Xml;
using urakawa.media.data;
using urakawa.media.timing;

namespace urakawa.media
{
	/// <summary>
	/// Implementation of <see cref="IAudioMedia"/> based on <see cref="IAudioMediaData"/>
	/// </summary>
	public class AudioMedia : IAudioMedia
	{
		internal AudioMedia(IMediaFactory fact, IAudioMediaData amd)
		{
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException("The MediaFactory of a AudioMedia can not be null");
			}
			mFactory = fact;
			setAudioMediaData(amd);
		}

		private IMediaFactory mFactory;
		private IMediaDataManager mMediaDataManager;
		private IAudioMediaData mAudioMediaData;

		/// <summary>
		/// Gets the <see cref="IAudioMediaData"/> instance on which this <see cref="AudioMedia"/> instance is based
		/// </summary>
		/// <returns>The audio media data instance</returns>
		public IAudioMediaData getAudioMediaData()
		{
			return mAudioMediaData;
		}

		/// <summary>
		/// Sets the <see cref="IAudioMediaData"/> instance on which this <see cref="AudioMedia"/> instance is based
		/// </summary>
		/// <param name="newAMD">The new audio media data instance</param>
		public void setAudioMediaData(IAudioMediaData newAMD)
		{
			if (newAMD == null)
			{
				throw new exception.MethodParameterIsNullException("The AudioMediaData of a AudioMedia can not be null");
			}
			if (newAMD.getMediaDataManager().getPresentation() != getMediaFactory().getPresentation())
			{
				throw new exception.OperationNotValidException(
					"The AudioMediaData must belong to the same Presentation as the AudioMedia to which it is associated");
			}
			mAudioMediaData = newAMD;
		}

		#region IMedia Members


		/// <summary>
		/// Gets the <see cref="IMediaFactory"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IMediaFactory"/></returns>
		public IMediaFactory getMediaFactory()
		{
			return mFactory;
		}

		/// <summary>
		/// Determines if the audio media is continuous (which is always the case)
		/// </summary>
		/// <returns><c>true</c></returns>
		public bool isContinuous()
		{
			return true;
		}

		/// <summary>
		/// Determines if the audio media is descrete (which is never the case)
		/// </summary>
		/// <returns><c>false</c></returns>
		public bool isDiscrete()
		{
			return false;
		}

		/// <summary>
		/// Determines if the audio media is a seqeunce (which is never the case)
		/// </summary>
		/// <returns><c>false</c></returns>
		public bool isSequence()
		{
			return false;
		}

		/// <summary>
		/// Gets the type of the the audio media
		/// </summary>
		/// <returns><c>MediaType.AUDIO</c></returns>
		public MediaType getMediaType()
		{
			return MediaType.AUDIO;
		}

		IMedia IMedia.copy()
		{
			return copy();
		}

		/// <summary>
		/// Creates a copy of the <see cref="AudioMedia"/> instance
		/// </summary>
		/// <returns>The copy</returns>
		public AudioMedia copy()
		{
			IMedia oCopy = getMediaFactory().createMedia(getXukLocalName(), getXukNamespaceUri());
			if (!(oCopy is AudioMedia))
			{
				throw new exception.FactoryCanNotCreateTypeException(String.Format(
					"The MediaFactory can not create an AudioMedia matching Xuk QName {0}:{1}",
					getXukNamespaceUri(), getXukLocalName()));
			}
			AudioMedia theCopy = (AudioMedia)oCopy;
			theCopy.setAudioMediaData(getAudioMediaData().copy());
			return theCopy;
		}

		#endregion
		
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="AudioMedia"/> from a AudioMedia xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the read was succesful</returns>
		public bool XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element) return false;
			if (!XukInAttributes(source)) return false;
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						if (!XukInChild(source)) return false;
					}
					else if (source.NodeType == XmlNodeType.EndElement)
					{
						break;
					}
					if (source.EOF) break;
				}
			}
			return true;
		}

		/// <summary>
		/// Reads the attributes of a AudioMedia xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the attributes was succefully read</returns>
		protected virtual bool XukInAttributes(XmlReader source)
		{
			string audioMediaData = source.GetAttribute("audioMediaData");
			IMediaData oAMD = getAudioMediaData().getMediaDataManager().getMediaData(audioMediaData);
			if (!(oAMD is IAudioMediaData)) return false;
			setAudioMediaData((IAudioMediaData)oAMD);
			return true;
		}

		/// <summary>
		/// Reads a child of a AudioMedia xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the child was succefully read</returns>
		protected virtual bool XukInChild(XmlReader source)
		{
			bool readItem = false;

			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past unknown child 
			}
			return true;
		}

		/// <summary>
		/// Write a AudioMedia element to a XUK file representing the <see cref="AudioMedia"/> instance
		/// </summary>
		/// <param localName="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		public bool XukOut(XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not XukOut to a null XmlWriter");
			}
			destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
			if (!XukOutAttributes(destination)) return false;
			if (!XukOutChildren(destination)) return false;
			destination.WriteEndElement();
			return true;
		}

		/// <summary>
		/// Writes the attributes of a AudioMedia element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutAttributes(XmlWriter destination)
		{
			destination.WriteAttributeString("audioMediaData", getAudioMediaData().getUid());
			return true;
		}

		/// <summary>
		/// Write the child elements of a AudioMedia element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutChildren(XmlWriter destination)
		{
			// No children to write
			return true;
		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="AudioMedia"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="AudioMedia"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

		#region IValueEquatable<IMedia> Members


		/// <summary>
		/// Determines of <c>this</c> has the same value as a given other instance
		/// </summary>
		/// <param name="other">The other instance</param>
		/// <returns>A <see cref="bool"/> indicating the result</returns>
		public bool ValueEquals(IMedia other)
		{
			if (!(other is AudioMedia)) return false;
			return getAudioMediaData().ValueEquals(((AudioMedia)other).getAudioMediaData());
		}

		#endregion

		#region ILocated Members

		/// <summary>
		/// Gets a <see cref="MediaDataLocation"/> pointing to the underlying <see cref="IAudioMediaData"/> of <c>this</c>
		/// </summary>
		/// <returns>The <see cref="MediaDataLocation"/></returns>
		public IMediaLocation getLocation()
		{
			IMediaDataLocation loc = new MediaDataLocation(
				getMediaFactory(),
				getAudioMediaData().getMediaDataManager().getMediaDataFactory());
			loc.setMediaData(getAudioMediaData());
			return loc;
		}

		/// <summary>
		/// Sets the <see cref="IMediaLocation"/> of the audio media. The new location must be a <see cref="IMediaDataLocation"/>
		/// pointing to a <see cref="IAudioMediaData"/>
		/// </summary>
		/// <param name="location">The new media location</param>
		public void setLocation(IMediaLocation location)
		{
			if (!(location is IMediaDataLocation))
			{
				throw new exception.MethodParameterIsWrongTypeException(
					"The MediaDataLocation of an AudioMedia must be a MediaDataLocation");
			}
			IMediaData newData = ((IMediaDataLocation)location).getMediaData();
			if (!(newData is IAudioMediaData))
			{
				throw new exception.OperationNotValidException(
					"The MediaData pointed to by the MediaDataLocation of an AudioMedia must be a AudioMediaData");
			}
			setAudioMediaData((IAudioMediaData)newData);
		}

		#endregion

		#region IContinuous Members

		/// <summary>
		/// Gets the duration of <c>this</c>
		/// </summary>
		/// <returns>The duration</returns>
		public urakawa.media.timing.ITimeDelta getDuration()
		{
			return getAudioMediaData().getAudioDuration();
		}

		IContinuous IContinuous.split(ITime splitPoint)
		{
			return split(splitPoint);
		}

		/// <summary>
		/// Splits the audio media at a given split point, the currrent instance retaining the audio before the the split point
		/// and a newly created audio media gets the audio after.
		/// </summary>
		/// <param name="splitPoint">The given split point</param>
		/// <returns>The audio media with the audio after the given split point</returns>
		public AudioMedia split(ITime splitPoint)
		{
			//TODO: Implement split
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
