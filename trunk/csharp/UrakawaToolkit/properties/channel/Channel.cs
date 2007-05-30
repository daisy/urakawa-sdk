using System;
using System.Xml;
using urakawa.media;

namespace urakawa.properties.channel
{
	/// <summary>
	/// Default implementation of the <see cref="IChannel"/> interface 
	/// - supports at most a single <see cref="MediaType"/>
	/// </summary>
	public class Channel : IChannel
	{
		private string mName = "";
		private IChannelsManager mChannelsManager;

		/// <summary>
		/// Holds the supported <see cref="MediaType"/> for the channel,
		/// the value <see cref="MediaType.EMPTY_SEQUENCE"/> signifies that 
		/// all <see cref="MediaType"/>s are supported 
		/// </summary>
		private MediaType mSupportedMediaType = MediaType.EMPTY_SEQUENCE;

		internal Channel(IChannelsManager chMgr)
		{
			mChannelsManager = chMgr;
		}

		#region IChannel Members

		/// <summary>
		/// Gets the <see cref="IChannelsManager"/> managing the <see cref="Channel"/>
		/// </summary>
		/// <returns>The <see cref="IChannelsManager"/></returns>
		public IChannelsManager getChannelsManager()
		{
			return mChannelsManager;
		}

		/// <summary>
		/// Sets the human-readable name of the <see cref="IChannel"/>
		/// </summary>
		/// <param name="name">The new human-readable name</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="name"/> is null
		/// </exception>
		public void setName(string name)
		{
			if (mName==null) 
			{
				throw new exception.MethodParameterIsNullException(
					"Can not set channel localName to null");
			}
			mName = name;
		}

		/// <summary>
		/// Gets the human-readable name of the <see cref="IChannel"/>
		/// </summary>
		/// <returns>The human-readable name</returns>
		public string getName()
		{
			return mName;
		}

		/// <summary>
		/// Checks of a given <see cref="MediaType"/> is supported by the channel
		/// </summary>
		/// <param name="type">The <see cref="MediaType"/></param>
		/// <returns>A <see cref="bool"/> indicating if the <see cref="MediaType"/>
		/// is supported</returns>
		public bool isMediaTypeSupported(MediaType type)
		{
			if (mSupportedMediaType==MediaType.EMPTY_SEQUENCE) return true;
			return (type==mSupportedMediaType);
		}

		/// <summary>
		/// Sets the <see cref="MediaType"/> supported by the <see cref="Channel"/>
		/// </summary>
		/// <param name="newType">The new <see cref="MediaType"/> supported</param>
		/// <exception cref="exception.MediaTypeIsIllegalException">
		/// Thrown when the <see cref="Channel"/> has already been assigned 
		/// a <see cref="MediaType"/> to support that is different from <paramref localName="newType"/>. 
		/// Alternatively if <paramref localName="newType"/> has the illegal 
		/// value <see cref="MediaType.EMPTY_SEQUENCE"/>
		/// </exception>
		public void addSupportedMediaType(MediaType newType)
		{
			if (newType == MediaType.EMPTY_SEQUENCE)
			{
				throw new exception.MediaTypeIsIllegalException(
					"A Channel can not support the EMPTY_SEQUENCE media type");
			}
			if (!isMediaTypeSupported(newType))
			{
				throw new exception.MediaTypeIsIllegalException(String.Format(
					"The media type {0:d} is illegal because the Channel currently "
					+ "supports the media type {0:d}",
					newType,
					mSupportedMediaType));
			}
			mSupportedMediaType = newType;
		}

		/// <summary>
		/// Gets the Xuk id of the <see cref="Channel"/>
		/// </summary>
		/// <returns>The Xuk Uid as calculated by 
		/// <c>this.getChannelsManager.getUidOfChannel(this)</c></returns>
		public string getUid()
		{
			return getChannelsManager().getUidOfChannel(this);
		}

		#endregion

		
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="Channel"/> from a Channel xuk element
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
			string name = "";
			if (!source.IsEmptyElement) name = source.ReadString();
			setName(name);
			return true;
		}

		/// <summary>
		/// Reads the attributes of a Channel xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the attributes was succefully read</returns>
		protected virtual bool XukInAttributes(XmlReader source)
		{
			// No known attributes
			return true;
		}

		/// <summary>
		/// Write a Channel element to a XUK file representing the <see cref="Channel"/> instance
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
			destination.WriteString(getName());
			destination.WriteEndElement();
			return true;
		}

		/// <summary>
		/// Writes the attributes of a Channel element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutAttributes(XmlWriter destination)
		{
			return true;
		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="Channel"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="Channel"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

		#region IValueEquatable<IChannel> Members


		/// <summary>
		/// Determines of <c>this</c> has the same value as a given other instance
		/// </summary>
		/// <param name="other">The other instance</param>
		/// <returns>A <see cref="bool"/> indicating the result</returns>
		public bool ValueEquals(IChannel other)
		{
			if (getName() != other.getName()) return false;
			if (!other.isMediaTypeSupported(mSupportedMediaType)) return false;
			return true;
		}

		#endregion
	}
}
