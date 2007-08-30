using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.media;
using urakawa.core;
using urakawa.property;

namespace urakawa.property.channel
{
	/// <summary>
	/// Default implementation of <see cref="ChannelsProperty"/>
	/// </summary>
	public class ChannelsProperty : Property
	{
		private IDictionary<Channel, IMedia> mMapChannelToMediaObject;

		/// <summary>
		/// Constructor using a given <see cref="IDictionary{Channel, IMedia}"/> for channels to media mapping
		/// </summary>
		/// <param name="chToMediaMapper">
		/// The <see cref="IDictionary{Channel, IMedia}"/> used to map channels and media</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="potentialOwner"/> is <c>null</c>
		/// </exception>
		internal ChannelsProperty(IDictionary<Channel, IMedia> chToMediaMapper) : base()
		{
			mMapChannelToMediaObject = chToMediaMapper;
			mMapChannelToMediaObject.Clear();
		}

		/// <summary>
		/// Constructor using a <see cref="System.Collections.Generic.Dictionary{Channel, IMedia}"/>
		/// for mapping channels to media
		/// </summary>
		internal ChannelsProperty()
			: this(new System.Collections.Generic.Dictionary<Channel, IMedia>())
		{
		}

		/// <summary>
		/// Tests if the channels property can be added to a given potential owning <see cref="TreeNode"/>, 
		/// which it can if the potential new owner does not already have a channels property
		/// </summary>
		/// <param name="potentialOwner">The potential new owner</param>
		/// <returns>A <see cref="bool"/> indicating if the property can be added</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="potentialOwner"/> is <c>null</c>
		/// </exception>
		public override bool canBeAddedTo(TreeNode potentialOwner)
		{
			if (!base.canBeAddedTo(potentialOwner)) return false;
			if (potentialOwner.hasProperties(this.GetType())) return false;
			return true;
		}

		/// <summary>
		/// Retrieves the <see cref="IMedia"/> of a given <see cref="Channel"/>
		/// </summary>
		/// <param name="channel">The given <see cref="Channel"/></param>
		/// <returns>The <see cref="IMedia"/> associated with the given channel, 
		/// <c>null</c> if no <see cref="IMedia"/> is associated</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="channel"/> is null
		/// </exception>
		/// <exception cref="exception.ChannelDoesNotExistException">
		/// Thrown when <paramref localName="channel"/> is not managed by the associated <see cref="ChannelsManager"/>
		/// </exception>
		public IMedia getMedia(Channel channel)
		{
			if (channel == null)
			{
				throw new exception.MethodParameterIsNullException(
					"channel parameter is null");
			}
			if (!getPresentation().getChannelsManager().getListOfChannels().Contains(channel))
			{
				throw new exception.ChannelDoesNotExistException(
					"The given channel is not managed by the ChannelManager associated with the ChannelsProperty");
			}
			if (!mMapChannelToMediaObject.ContainsKey(channel)) return null;
			return (IMedia)mMapChannelToMediaObject[channel];
		}

		/// <summary>
		/// Associates a given <see cref="IMedia"/> with a given <see cref="Channel"/>
		/// </summary>
		/// <param name="channel">The given <see cref="Channel"/></param>
		/// <param name="media">The given <see cref="IMedia"/>, 
		/// pass <c>null</c> if you want to remove <see cref="IMedia"/>
		/// from the given <see cref="Channel"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameters <paramref localName="channel"/> is null
		/// </exception>
		/// <exception cref="exception.ChannelDoesNotExistException">
		/// Thrown when <paramref localName="channel"/> is not managed by the associated <see cref="ChannelsManager"/>
		/// </exception>
		/// <exception cref="exception.MediaNotAcceptable">
		/// Thrown when <paramref localName="channel"/> does not support the <see cref="MediaType"/> 
		/// of <paramref localName="media"/>
		/// </exception>
		public void setMedia(Channel channel, IMedia media)
		{
			if (channel == null)
			{
				throw new exception.MethodParameterIsNullException(
					"channel parameter is null");
			}
			if (!getPresentation().getChannelsManager().getListOfChannels().Contains(channel))
			{
				throw new exception.ChannelDoesNotExistException(
					"The given channel is not managed by the ChannelManager associated with the ChannelsProperty");
			}
			if (media != null)
			{
				if (!channel.canAccept(media))
				{
					throw new exception.MediaNotAcceptable(
						"The given media type is not supported by the given channel");
				}
			}
			mMapChannelToMediaObject[channel] = media;
		}

		/// <summary>
		/// Gets the list of <see cref="Channel"/>s used by this instance of <see cref="ChannelsProperty"/>
		/// </summary>
		/// <returns>The list of used <see cref="Channel"/>s</returns>
		public List<Channel> getListOfUsedChannels()
		{
			List<Channel> res = new List<Channel>();
			foreach (Channel ch in getPresentation().getChannelsManager().getListOfChannels())
			{
				if (getMedia(ch) != null)
				{
					res.Add(ch);
				}
			}
			return res;
		}

		/// <summary>
		/// Creates a "deep" copy of the <see cref="ChannelsProperty"/> instance 
		/// - deep meaning that all associated <see cref="IMedia"/> are copies and not just referenced
		/// </summary>
		/// <returns>The deep copy</returns>
		/// <exception cref="exception.FactoryCannotCreateTypeException">
		/// Thrown when the <see cref="IChannelsPropertyFactory"/> of the <see cref="IChannelPresentation"/>
		/// associated with <c>this</c> can not create a <see cref="ChannelsProperty"/> or sub-type
		/// </exception>
		public new ChannelsProperty copy()
		{
			ChannelsProperty theCopy = base.copyProtected() as ChannelsProperty;
			if (theCopy == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The property factory can not create a ChannelsProperty matching QName {0}:{1}",
					getXukNamespaceUri(), getXukLocalName()));
			}
			foreach (Channel ch in getListOfUsedChannels())
			{
				theCopy.setMedia(ch, getMedia(ch).copy());
			}
			return theCopy;
		}

		/// <summary>
		/// Exports the channels property to a given destination <see cref="Presentation"/>, 
		/// including exports of any attachedx <see cref="IMedia"/>
		/// </summary>
		/// <param name="destPres">Thre destination presentation of the export</param>
		/// <returns>The exported channels property</returns>
		public new ChannelsProperty export(Presentation destPres)
		{
			ChannelsProperty chExport = base.exportProtected(destPres) as ChannelsProperty;
			if (chExport==null)
			{
				throw new exception.OperationNotValidException(
					"The exportProtected method of the base class unexpectedly did not return a ChannelsProperty");
			}
			foreach (Channel ch in getListOfUsedChannels())
			{
				Channel exportDestCh = null;
				foreach (Channel dCh in destPres.getChannelsManager().getListOfChannels())
				{
					if (ch.isEquivalentTo(dCh))
					{
						exportDestCh = dCh;
						break;
					}
					if (exportDestCh==null) 
					{
						destPres.getChannelsManager().addChannel(ch.export(destPres));
					}
					chExport.setMedia(exportDestCh, getMedia(ch).export(destPres));
				}
			}
			return chExport;
		}

		/// <summary>
		/// Creates a "deep" copy of the <see cref="ChannelsProperty"/> instance 
		/// - deep meaning that all associated are copies and not just referenced
		/// </summary>
		/// <returns>The deep copy</returns>
		/// <exception cref="exception.FactoryCannotCreateTypeException">
		/// Thrown when the <see cref="IChannelsPropertyFactory"/> of the <see cref="IChannelPresentation"/>
		/// associated with <c>this</c> can not create a <see cref="ChannelsProperty"/> or sub-type
		/// </exception>
		protected override Property copyProtected()
		{
			return copy();
		}

		/// <summary>
		/// Exports the channels property to a given destination <see cref="Presentation"/>, 
		/// including exports of any attachedx <see cref="IMedia"/>
		/// </summary>
		/// <param name="destPres">Thre destination presentation of the export</param>
		/// <returns>The exported channels property</returns>
		protected override Property exportProtected(Presentation destPres)
		{
			return export(destPres);
		}

		#region IXukAble Members

		/// <summary>
		/// Reads the attributes of a ChannelsProperty xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected override void XukInAttributes(XmlReader source)
		{
			// No known attributes
		}

		/// <summary>
		/// Reads a child of a ChannelsProperty xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the child was succefully read</returns>
		protected override void XukInChild(XmlReader source)
		{
			bool readItem = false;
			if (source.NamespaceURI == urakawa.ToolkitSettings.XUK_NS)
			{
				readItem = true;
				switch (source.LocalName)
				{
					case "mChannelMappings":
						XukInChannelMappings(source);
						break;
					default:
						readItem = false;
						break;
				}
			}
			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past invalid MediaDataItem element
			}
		}

		/// <summary>
		/// Helper method to to Xuk in mChannelMappings element
		/// </summary>
		/// <param name="source"></param>
		private void XukInChannelMappings(XmlReader source)
		{
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						if (source.LocalName == "mChannelMapping" && source.NamespaceURI == ToolkitSettings.XUK_NS)
						{
							XUKInChannelMapping(source);
						}
						else if (!source.IsEmptyElement)
						{
							source.ReadSubtree().Close();
						}
					}
					else if (source.NodeType == XmlNodeType.EndElement)
					{
						break;
					}
					if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
				}
			}
		}

		/// <summary>
		/// helper method which is called once per mChannelMapping element
		/// </summary>
		/// <param name="source"></param>
		private void XUKInChannelMapping(System.Xml.XmlReader source)
		{
			string channelRef = source.GetAttribute("channel");
			while (source.Read())
			{
				if (source.NodeType == XmlNodeType.Element)
				{
					IMedia newMedia = getPresentation().getMediaFactory().createMedia(source.LocalName, source.NamespaceURI);
					if (newMedia != null)
					{
						newMedia.XukIn(source);
						Channel channel = getPresentation().getChannelsManager().getChannel(channelRef);
						if (channel == null)
						{
							throw new exception.XukException(
								String.Format("Found no channel with uid {0}", channelRef));
						}
						setMedia(channel, newMedia);
					}
					else if (!source.IsEmptyElement)
					{
						//Read past unrecognized element
						source.ReadSubtree().Close();
					}
				}
				else if (source.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
				if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
			}
		}

		/// <summary>
		/// Write the child elements of a ChannelsProperty element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected override void XukOutChildren(XmlWriter destination)
		{
			destination.WriteStartElement("mChannelMappings", ToolkitSettings.XUK_NS);
			List<Channel> channelsList = getListOfUsedChannels();
			foreach (Channel channel in channelsList)
			{
				destination.WriteStartElement("mChannelMapping", urakawa.ToolkitSettings.XUK_NS);
				destination.WriteAttributeString("channel", channel.getUid());
				IMedia media = getMedia(channel);
				if (media == null)
				{
					throw new exception.XukException(
						String.Format("Found no Media associated with channel {0}", channel.getUid()));
				}
				media.XukOut(destination);

				destination.WriteEndElement();
			}
			destination.WriteEndElement();
		}

		#endregion

		#region IValueEquatable<Property> Members

		/// <summary>
		/// Conpares <c>this</c> with a given other <see cref="Property"/> for value equality
		/// </summary>
		/// <param name="other">The other <see cref="Property"/></param>
		/// <returns><c>true</c> if equal, otherwise <c>false</c></returns>
		public override bool ValueEquals(Property other)
		{
			if (!base.ValueEquals(other)) return false;
			ChannelsProperty otherChProp = (ChannelsProperty)other;
			List<Channel> chs = getListOfUsedChannels();
			List<Channel> otherChs = otherChProp.getListOfUsedChannels();
			if (chs.Count != otherChs.Count) return false;
			foreach (Channel ch in chs)
			{
				Channel otherCh = null;
				foreach (Channel ch2 in otherChs)
				{
					if (ch.getUid() == ch2.getUid())
					{
						otherCh = ch2;
						break;
					}
				}
				if (otherCh == null) return false;
				if (!getMedia(ch).ValueEquals(otherChProp.getMedia(otherCh))) return false;
			}
			return true;
		}

		#endregion
	}
}
