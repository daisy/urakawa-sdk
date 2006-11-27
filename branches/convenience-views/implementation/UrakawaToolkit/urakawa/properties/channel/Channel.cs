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
		/// Sets the localName of the <see cref="IChannel"/>
		/// </summary>
		/// <param localName="localName">The new localName</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="localName"/> is null
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
		/// Gets the localName of the <see cref="IChannel"/>
		/// </summary>
		/// <returns>The localName</returns>
		public string getName()
		{
			return mName;
		}

		/// <summary>
		/// Checks of a given <see cref="MediaType"/> is supported by the channel
		/// </summary>
		/// <param localName="type">The <see cref="MediaType"/></param>
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
		/// <param localName="newType">The new <see cref="MediaType"/> supported</param>
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
		/// <returns>The Xuk Id as calculated by 
		/// <c>this.getChannelsManager.getUidOfChannel(this)</c></returns>
		public string getUid()
		{
			return getChannelsManager().getUidOfChannel(this);
		}

		#endregion

		#region IXukAble Members
		/// <summary>
		/// Reads the <see cref="Channel"/> from a Channel element in a XUK document
		/// </summary>
		/// <param localName="source">An <see cref="XmlReader"/> from which to read the Channel element</param>
		/// <returns>A <see cref="bool"/> indicating if the <see cref="Channel"/> was succesfully read</returns>
		public bool XukIn(System.Xml.XmlReader source)
		{
			System.Diagnostics.Trace.WriteLine("Channel.XUKIn");
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Reader is null");
			}
			if (source.NodeType != XmlNodeType.Element) return false;
			if (source.LocalName != "Channel") return false;
			if (source.NamespaceURI != urakawa.ToolkitSettings.XUK_NS) return false;

			if (source.IsEmptyElement)
			{
				setName("");
			}
			else
			{
				string val = "";
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Text)
					{
						val += source.Value;
					}
					else if (source.NodeType == XmlNodeType.Whitespace)
					{
						val += " ";
					}
					else if (source.NodeType == XmlNodeType.Element)
					{
						return false;
					}
					else if (source.NodeType == XmlNodeType.EndElement)
					{
						break;
					}
					if (source.EOF) return false;
				}
				setName(val);
			}
			return true;
		}

		/// <summary>
		/// Writes the <see cref="Channel"/> to a Channel element in a XUK document
		/// </summary>
		/// <param localName="destination"></param>
		/// <returns></returns>
		public bool XukOut(System.Xml.XmlWriter destination)
		{
			destination.WriteStartElement("Channel", urakawa.ToolkitSettings.XUK_NS);
			string xukId = getUid();
			if (xukId == "") return false;
			destination.WriteAttributeString("id", xukId);
			destination.WriteString(this.mName);
			destination.WriteEndElement();
			return true;
		}

		/// <summary>
		/// Gets the local localName part of the QName representing a <see cref="Channel"/> in Xuk
		/// </summary>
		/// <returns>The local localName part</returns>
		public string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="Channel"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion
	}
}
