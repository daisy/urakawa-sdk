using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.media;
using urakawa.core;
using urakawa.core.property;

namespace urakawa.properties.channel
{
	/// <summary>
	/// Default implementation of <see cref="IChannelsProperty"/>
	/// </summary>
	public class ChannelsProperty : IChannelsProperty
	{
		private IDictionary<IChannel, IMedia> mMapChannelToMediaObject;

		private IChannelPresentation mPresentation;

		private ICoreNode mOwner;



		/// <summary>
		/// Gets the owner <see cref="ICoreNode"/> of the <see cref="ChannelsProperty"/>
		/// </summary>
		/// <returns>The owner</returns>
		public ICoreNode getOwner()
		{
			return mOwner;
		}

		/// <summary>
		/// Sets the owner <see cref="ICoreNode"/> of the <see cref="ChannelsProperty"/> instance
		/// </summary>
		/// <param name="newOwner">The new owner</param>
		/// <remarks>This function is intended for internal purposes only 
		/// and should not be called by users of the toolkit</remarks>
		public void setOwner(ICoreNode newOwner)
		{
			if (newOwner.getPresentation() != mPresentation)
			{
				throw new exception.NodeInDifferentPresentationException(
					"The ChannelsProperty can not have an owner CoreNode from a different presentation");
			}
			mOwner = newOwner;
		}

		/// <summary>
		/// Gets the <see cref="IChannelPresentation"/> of the channels property.
		/// If the channels property has an owner <see cref="ICoreNode"/>, this is equivalent to
		/// <c>(IChannelPresentation)getOwner().getPresentation()</c>
		/// </summary>
		/// <returns>The presentation</returns>
		public IChannelPresentation getPresentation()
		{
			return mPresentation;
		}

		/// <summary>
		/// Constructor using a given <see cref="IDictionary{IChannel, IMedia}"/> for channels to media mapping
		/// </summary>
		/// <param name="pres">The <see cref="IChannelPresentation"/> 
		/// associated with the <see cref="ChannelsProperty"/></param>
		/// <param name="chToMediaMapper">
		/// The <see cref="IDictionary{IChannel, IMedia}"/> used to map channels and media</param>
		internal ChannelsProperty(IChannelPresentation pres, IDictionary<IChannel, IMedia> chToMediaMapper)
		{
			mPresentation = pres;
			mMapChannelToMediaObject = chToMediaMapper;
			mMapChannelToMediaObject.Clear();
		}

		/// <summary>
		/// Constructor using a <see cref="System.Collections.Generic.Dictionary{IChannel, IMedia}"/>
		/// for mapping channels to media
		/// </summary>
		/// <param name="pres">The <see cref="IChannelPresentation"/> 
		/// associated with the <see cref="ChannelsProperty"/></param>
		internal ChannelsProperty(IChannelPresentation pres)
			: this(pres, new System.Collections.Generic.Dictionary<IChannel, IMedia>())
		{
		}

		#region IProperty Members

		IProperty IProperty.copy()
		{
			return copy();
		}


		#endregion

		#region IChannelsProperty Members

		/// <summary>
		/// Retrieves the <see cref="IMedia"/> of a given <see cref="IChannel"/>
		/// </summary>
		/// <param name="channel">The given <see cref="IChannel"/></param>
		/// <returns>The <see cref="IMedia"/> associated with the given channel, 
		/// <c>null</c> if no <see cref="IMedia"/> is associated</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="channel"/> is null
		/// </exception>
		/// <exception cref="exception.ChannelDoesNotExistException">
		/// Thrown when <paramref localName="channel"/> is not managed by the associated <see cref="IChannelsManager"/>
		/// </exception>
		public IMedia getMedia(IChannel channel)
		{
			if (channel == null)
			{
				throw new exception.MethodParameterIsNullException(
					"channel parameter is null");
			}
			if (!mPresentation.getChannelsManager().getListOfChannels().Contains(channel))
			{
				throw new exception.ChannelDoesNotExistException(
					"The given channel is not managed by the ChannelManager associated with the ChannelsProperty");
			}
			if (!mMapChannelToMediaObject.ContainsKey(channel)) return null;
			return (IMedia)mMapChannelToMediaObject[channel];
		}

		/// <summary>
		/// Associates a given <see cref="IMedia"/> with a given <see cref="IChannel"/>
		/// </summary>
		/// <param name="channel">The given <see cref="IChannel"/></param>
		/// <param name="media">The given <see cref="IMedia"/>, 
		/// pass <c>null</c> if you want to remove <see cref="IMedia"/>
		/// from the given <see cref="IChannel"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameters <paramref localName="channel"/> is null
		/// </exception>
		/// <exception cref="exception.ChannelDoesNotExistException">
		/// Thrown when <paramref localName="channel"/> is not managed by the associated <see cref="IChannelsManager"/>
		/// </exception>
		/// <exception cref="exception.MediaTypeIsIllegalException">
		/// Thrown when <paramref localName="channel"/> does not support the <see cref="MediaType"/> 
		/// of <paramref localName="media"/>
		/// </exception>
		public void setMedia(IChannel channel, IMedia media)
		{
			if (channel == null)
			{
				throw new exception.MethodParameterIsNullException(
					"channel parameter is null");
			}
			if (!mPresentation.getChannelsManager().getListOfChannels().Contains(channel))
			{
				throw new exception.ChannelDoesNotExistException(
					"The given channel is not managed by the ChannelManager associated with the ChannelsProperty");
			}
			if (media != null)
			{
				if (!channel.isMediaTypeSupported(media.getMediaType()))
				{
					throw new exception.MediaTypeIsIllegalException(
						"The given media type is not supported by the given channel");
				}
			}
			mMapChannelToMediaObject[channel] = media;
		}

		/// <summary>
		/// Gets the list of <see cref="IChannel"/>s used by this instance of <see cref="IChannelsProperty"/>
		/// </summary>
		/// <returns>The list of used <see cref="IChannel"/>s</returns>
		public List<IChannel> getListOfUsedChannels()
		{
			List<IChannel> res = new List<IChannel>();
			foreach (IChannel ch in mPresentation.getChannelsManager().getListOfChannels())
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
		/// - deep meaning that all associated are copies and not just referenced
		/// </summary>
		/// <returns>The deep copy</returns>
		/// <exception cref="exception.FactoryCanNotCreateTypeException">
		/// Thrown when the <see cref="IChannelsPropertyFactory"/> of the <see cref="IChannelPresentation"/>
		/// associated with <c>this</c> can not create a <see cref="ChannelsProperty"/> or sub-type
		/// </exception>
		public IChannelsProperty copy()
		{
			IProperty theCopy = mPresentation.getPropertyFactory().createProperty(
				getXukLocalName(), getXukNamespaceUri());
			if (theCopy == null)
			{
				throw new exception.FactoryCanNotCreateTypeException(String.Format(
					"The property factory can not create a IProperty matching QName {0}:{1}",
					getXukNamespaceUri(), getXukLocalName()));
			}
			if (!typeof(ChannelsProperty).IsAssignableFrom(theCopy.GetType()))
			{
				throw new exception.FactoryCanNotCreateTypeException(String.Format(
					"The property created by the property factory to match QName {0}:{1} "
					+ "is not assignable to a urakawa.properties.channels.ChannelsProperty",
					getXukNamespaceUri(), getXukLocalName()));
			}
			ChannelsProperty theTypedCopy = (ChannelsProperty)theCopy;
			foreach (object o in getListOfUsedChannels())
			{
				IChannel ch = (IChannel)o;
				theTypedCopy.setMedia(ch, getMedia(ch).copy());
			}
			return theTypedCopy;
		}

		#endregion

		#region IChannelsPropertyValidator Members

		///// <summary>
		///// Determines if a given <see cref="IMedia"/> can be associated
		///// with a given <see cref="IChannel"/> 
		///// without breaking <see cref="IChannelsProperty"/> rules
		///// </summary>
		///// <param name="channel">The given <see cref="IChannel"/></param>
		///// <param name="media">The given <see cref="IMedia"/></param>
		///// <returns>A <see cref="bool"/> indicating if the given <see cref="IMedia"/>
		///// can be associated with the given <see cref="IChannel"/></returns>
		//public bool canSetMedia(IChannel channel, IMedia media)
		//{
		//  if (channel==null)
		//  {
		//    throw new exception.MethodParameterIsNullException("The given channel is null");
		//  }
		//  if (media==null)
		//  {
		//    throw new exception.MethodParameterIsNullException("The given media is null");
		//  }
		//  // The media can not be set if the channel does not support the media type
		//  if (!channel.isMediaTypeSupported(media.getType())) return false;
		//  // If any ancestors of the owner node has media in the channel, the media can not be associated
		//  // The media can be set if the media is already associated with the channel
		//  if (getMedia(channel)!=null) return true;
		//  if (ChannelsPropertyCoreNodeValidator.DetectMediaOfAncestors(channel, getOwner()))
		//  {
		//    return false;
		//  }
		//  // If any descendants of the owner node has media in the channel, the media can not be associated
		//  if (ChannelsPropertyCoreNodeValidator.DetectMediaOfSelfOrDescendants(channel, getOwner()))
		//  {
		//    return false;
		//  }
		//  return false;
		//}

		//
		//    /// <summary>
		//    /// Event handler for the <see cref="ChannelsManager.Removed"/> event 
		//    /// of the associated <see cref="ChannelsManager"/>
		//    /// </summary>
		//    /// <param name="o">The associated <see cref="ChannelsManager"/> raising the event</param>
		//    /// <param name="e">The event arguments passed with the event</param>
		//    private void mChannelsManager_Removed(ChannelsManager o, ChannelsManagerRemovedEventArgs e)
		//    {
		//      mMapChannelToMediaObject.Remove(e.RemovedChannel);
		//    }
		#endregion

		#region IXukAble Members
		
		/// <summary>
		/// Reads the <see cref="ChannelsProperty"/> from a ChannelsProperty xuk element
		/// </summary>
		/// <param name="source">The source <see cref="System.Xml.XmlReader"/></param>
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
		/// Reads the attributes of a ChannelsProperty xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the attributes was succefully read</returns>
		protected virtual bool XukInAttributes(XmlReader source)
		{
			// No known attributes


			return true;
		}

		/// <summary>
		/// Reads a child of a ChannelsProperty xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the child was succefully read</returns>
		protected virtual bool XukInChild(XmlReader source)
		{
			bool readItem = false;
			if (source.NamespaceURI == urakawa.ToolkitSettings.XUK_NS)
			{
				readItem = true;
				switch (source.LocalName)
				{
					case "mChannelMappings":
						if (!XukInChannelMappings(source)) return false;
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
			return true;
		}

		/// <summary>
		/// Helper method to to Xuk in mChannelMappings element
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		private bool XukInChannelMappings(XmlReader source)
		{
			if (source.IsEmptyElement) return true;
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
				if (source.EOF) break;
			}
			return true;
		}

		/// <summary>
		/// helper method which is called once per mChannelMapping element
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		private bool XUKInChannelMapping(System.Xml.XmlReader source)
		{
			string channelRef = source.GetAttribute("channel");
			while (source.Read())
			{
				if (source.NodeType == XmlNodeType.Element)
				{
					IMedia newMedia = getPresentation().getMediaFactory().createMedia(source.LocalName, source.NamespaceURI);
					if (newMedia != null)
					{
						if (!newMedia.XukIn(source)) return false;
						IChannel channel = getPresentation().getChannelsManager().getChannel(channelRef);
						if (channel == null) return false;
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
				if (source.EOF) return false;
			}
			return true;
		}

		/// <summary>
		/// Write a ChannelsProperty element to a XUK file representing the <see cref="ChannelsProperty"/> instance
		/// </summary>
		/// <param localName="destination">The destination <see cref="System.Xml.XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		public bool XukOut(System.Xml.XmlWriter destination)
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
		/// Writes the attributes of a ChannelsProperty element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutAttributes(XmlWriter destination)
		{
			//No attributes to Xuk out
			return true;
		}

		/// <summary>
		/// Write the child elements of a ChannelsProperty element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutChildren(XmlWriter destination)
		{
			destination.WriteStartElement("mChannelMappings", ToolkitSettings.XUK_NS);
			List<IChannel> channelsList = getListOfUsedChannels();
			foreach (IChannel channel in channelsList)
			{
				destination.WriteStartElement("mChannelMapping", urakawa.ToolkitSettings.XUK_NS);
				destination.WriteAttributeString("channel", channel.getUid());
				IMedia media = getMedia(channel);
				if (media == null) return false;//There should be media in all used channels
				if (!media.XukOut(destination)) return false;

				destination.WriteEndElement();
			}
			destination.WriteEndElement();
			return true;
		}
		
		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="ChannelsProperty"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="ChannelsProperty"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

		#region IValueEquatable<IProperty> Members

		/// <summary>
		/// Conpares <c>this</c> with a given other <see cref="IProperty"/> for value equality
		/// </summary>
		/// <param name="other">The other <see cref="IProperty"/></param>
		/// <returns><c>true</c> if equal, otherwise <c>false</c></returns>
		public bool ValueEquals(IProperty other)
		{
			if (!(other is IChannelsProperty)) return false;
			IChannelsProperty otherChProp = (IChannelsProperty)other;
			List<IChannel> chs = getListOfUsedChannels();
			List<IChannel> otherChs = otherChProp.getListOfUsedChannels();
			if (chs.Count != otherChs.Count) return false;
			foreach (IChannel ch in chs)
			{
				IChannel otherCh = null;
				foreach (IChannel ch2 in otherChs)
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
