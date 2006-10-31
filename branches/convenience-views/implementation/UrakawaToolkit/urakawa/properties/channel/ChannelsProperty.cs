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
			mOwner = newOwner;
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
			//      mPresentation.getChannelsManager().Removed 
			//        += new ChannelsManagerRemovedEventDelegate(mChannelsManager_Removed);
			mMapChannelToMediaObject = chToMediaMapper;
			mMapChannelToMediaObject.Clear();
		}

		/// <summary>
		/// Constructor using a <see cref="System.Collections.Specialized.ListDictionary"/>
		/// for mapping channels to media
		/// </summary>
		/// <param name="pres">The <see cref="IChannelPresentation"/> 
		/// associated with the <see cref="ChannelsProperty"/></param>
		internal ChannelsProperty(IChannelPresentation pres)
			: this(pres, new System.Collections.Generic.Dictionary<IChannel, IMedia>())
		{
		}

		//
		//    /// <summary>
		//    /// Destructor - stops listining for the <see cref="ChannelsManager.Removed"/>
		//    /// ecent of the associated <see cref="ChannelsManager"/>
		//    /// </summary>
		//    ~ChannelsProperty()
		//    {
		//      if (mPresentation!=null)
		//      {
		//        mPresentation.getChannelsManager().Removed 
		//          -= new ChannelsManagerRemovedEventDelegate(mChannelsManager_Removed);
		//      }
		//    }

		#region IProperty Members

		IProperty IProperty.copy()
		{
			return copy();
		}


		/// <summary>
		/// Creates a "deep" copy of the <see cref="ChannelsProperty"/> instance 
		/// - deep meaning that all associated are copies and not just referenced
		/// </summary>
		/// <returns>The deep copy</returns>
		/// <exception cref="FactoryCanNotCreateTypeException">
		/// Thrown when the <see cref="IChannelsPropertyFactory"/> of the <see cref="IChannelPresentation"/>
		/// associated with <c>this</c> can not create a <see cref="ChannelsProperty"/> or sub-type
		/// </exception>
		ChannelsProperty copy()
		{
			IProperty theCopy = mPresentation.getPropertyFactory().createProperty(
				getXukLocalName(), getXukNamespaceUri());
			if (theCopy == null)
			{
				throw new urakawa.exception.FactoryCanNotCreateTypeException(String.Format(
					"The property factory can not create a IProperty matching QName {0}:{1}",
					getXukNamespaceUri(), getXukLocalName()));
			}
			if (!typeof(ChannelsProperty).IsAssignableFrom(theCopy.GetType()))
			{
				throw new urakawa.exception.FactoryCanNotCreateTypeException(String.Format(
					"The property created by the property factory to match QName {0}:{1} "
					+"is not assignable to a urakawa.properties.channels.ChannelsProperty",
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

		#region IChannelsProperty Members

		/// <summary>
		/// Retrieves the <see cref="IMedia"/> of a given <see cref="IChannel"/>
		/// </summary>
		/// <param name="channel">The given <see cref="IChannel"/></param>
		/// <returns>The <see cref="IMedia"/> associated with the given channel, 
		/// <c>null</c> if no <see cref="IMedia"/> is associated</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="channel"/> is null
		/// </exception>
		/// <exception cref="exception.ChannelDoesNotExistException">
		/// Thrown when <paramref name="channel"/> is not managed by the associated <see cref="IChannelsManager"/>
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
		/// Thrown when parameters <paramref name="channel"/> is null
		/// </exception>
		/// <exception cref="exception.ChannelDoesNotExistException">
		/// Thrown when <paramref name="channel"/> is not managed by the associated <see cref="IChannelsManager"/>
		/// </exception>
		/// <exception cref="exception.MediaTypeIsIllegalException">
		/// Thrown when <paramref name="channel"/> does not support the <see cref="MediaType"/> 
		/// of <paramref name="media"/>
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
		public IList<IChannel> getListOfUsedChannels()
		{
			IList<IChannel> res = new List<IChannel>();
			foreach (IChannel ch in mPresentation.getChannelsManager().getListOfChannels())
			{
				if (getMedia(ch) != null)
				{
					res.Add(ch);
				}
			}
			return res;
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
		/// Reads the <see cref="ChannelsProperty"/> from a ChannelsProperty element in a XUK file
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/> with cursor at the ChannelsProperty element
		/// </param>
		/// <returns>A <see cref="bool"/> indicating if the read was succesful</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="source"/> is null
		/// </exception>
		public bool XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Reader is null");
			}
			if (source.NodeType != XmlNodeType.Element) return false;
			if (source.LocalName != "ChannelsProperty") return false;
			if (source.NamespaceURI != urakawa.ToolkitSettings.XUK_NS) return false;

			if (source.IsEmptyElement) return true;

			while (source.Read())
			{
				if (source.NodeType == XmlNodeType.Element)
				{
					if (source.LocalName == "ChannelMapping" && source.NamespaceURI == urakawa.ToolkitSettings.XUK_NS)
					{
						if (!XUKInChannelMapping(source)) return false;
					}
					else
					{
						if (!source.IsEmptyElement)
						{
							//Reads sub tree and places cursor at end element
							source.ReadSubtree().Close();
						}
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
		/// helper function which is called once per ChannelMapping element
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
					IMedia newMedia = mPresentation.getMediaFactory().createMedia(source.LocalName, source.NamespaceURI);
					if (newMedia == null)
					{
						if (!source.IsEmptyElement)
						{
							//Read past unrecognized element
							source.ReadSubtree().Close();
						}
					}
					else
					{
						if (!newMedia.XukIn(source)) return false;
					}
					IChannel channel = mPresentation.getChannelsManager().getChannelByXukId(channelRef);
					if (channel == null) return false;
					setMedia(channel, newMedia);
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
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="destination"/> is null
		/// </exception>
		public bool XukOut(XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Writer is null");
			}

			destination.WriteStartElement("ChannelsProperty");

			IList<IChannel> channelsList = this.getListOfUsedChannels();


			foreach (IChannel channel in getListOfUsedChannels())
			{
				destination.WriteStartElement("ChannelMapping", urakawa.ToolkitSettings.XUK_NS);
				destination.WriteAttributeString(
					"channel", 
					channel.getChannelsManager().getXukIdOfChannel(channel));

				IMedia media = getMedia(channel);

				bool bTmp = true;
				if (media != null)
				{
					if (!media.XukOut(destination)) return false;
				}
			}

			destination.WriteEndElement();

			return true;
		}


		public string getXukLocalName()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public string getXukNamespaceUri()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
