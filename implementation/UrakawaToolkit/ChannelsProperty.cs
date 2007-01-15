using System;
using System.Collections;
using System.Xml;
using urakawa.media;

namespace urakawa.core
{
	/// <summary>
	/// Default implementation of <see cref="IChannelsProperty"/>
	/// </summary>
  public class ChannelsProperty : IChannelsProperty, IChannelsPropertyValidator
  {
	  private IDictionary mMapChannelToMediaObject;

    private Presentation mPresentation;

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
    /// Constructor using a given <see cref="IDictionary"/> for channels to media mapping
    /// </summary>
    /// <param name="pres">The <see cref="Presentation"/> 
    /// associated with the <see cref="ChannelsProperty"/></param>
    /// <param name="chToMediaMapper">
    /// The <see cref="IDictionary"/> used to map channels and media</param>
    internal ChannelsProperty(Presentation pres, IDictionary chToMediaMapper)
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
    /// <param name="pres">The <see cref="Presentation"/> 
    /// associated with the <see cref="ChannelsProperty"/></param>
    internal ChannelsProperty(Presentation pres) 
      : this(pres, new System.Collections.Specialized.ListDictionary())
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

    /// <summary>
    /// Creates a deep copy of the <see cref="ChannelsProperty"/> instance 
    /// - deep meaning that all associated are copies and not just referenced
    /// </summary>
    /// <returns>The deep copy</returns>
    ChannelsProperty copy()
    {
      ChannelsProperty theCopy = 
        mPresentation.getPropertyFactory().createChannelsProperty();
      foreach (object o in getListOfUsedChannels())
      {
        IChannel ch = (IChannel)o;
        theCopy.setMedia(ch, getMedia(ch).copy());
      }
      return theCopy;
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
    /// Thrown when <paramref name="channel"/> is null
    /// </exception>
    /// <exception cref="exception.ChannelDoesNotExistException">
    /// Thrown when <paramref name="channel"/> is not managed by the associated <see cref="IChannelsManager"/>
    /// </exception>
    public IMedia getMedia(IChannel channel)
    {
      if (channel==null)
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
      if (channel==null)
      {
        throw new exception.MethodParameterIsNullException(
          "channel parameter is null");
      }
      if (!mPresentation.getChannelsManager().getListOfChannels().Contains(channel))
      {
        throw new exception.ChannelDoesNotExistException(
          "The given channel is not managed by the ChannelManager associated with the ChannelsProperty");
      }
			if (media!=null)
			{
				if (!channel.isMediaTypeSupported(media.getType()))
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
    public IList getListOfUsedChannels()
    {
      ArrayList res = new ArrayList();
      foreach (IChannel ch in mPresentation.getChannelsManager().getListOfChannels())
      {
        if (getMedia(ch)!=null)
        {
          res.Add(ch);
        }
      }
      return res;
    }
    #endregion

	  #region IXUKAble members 

    /// <summary>
    /// Reads the <see cref="ChannelsProperty"/> from a ChannelsProperty element in a XUK file
    /// </summary>
    /// <param name="source">The source <see cref="XmlReader"/> with cursor at the ChannelsProperty element
    /// </param>
    /// <returns>A <see cref="bool"/> indicating if the read was succesful</returns>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref name="source"/> is null
    /// </exception>
    public bool XUKIn(XmlReader source)
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
    /// Write a ChannelsProperty element to a XUK file representing the <see cref="ChannelsProperty"/> instance
    /// </summary>
    /// <param name="destination">The destination <see cref="XmlWriter"/></param>
    /// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref name="destination"/> is null
    /// </exception>
	  public bool XUKOut(XmlWriter destination)
	  {
		  if (destination == null)
		  {
			  throw new exception.MethodParameterIsNullException("Xml Writer is null");
		  }

		  destination.WriteStartElement("ChannelsProperty");

		  IList channelsList = this.getListOfUsedChannels();

		  bool bRetVal = true;

		  for (int i=0; i<channelsList.Count; i++)
		  {
			  Channel channel = (Channel)channelsList[i];

				destination.WriteStartElement("ChannelMapping", urakawa.ToolkitSettings.XUK_NS);
			  destination.WriteAttributeString("channel", channel.getId());
  			
			  IMedia media = this.getMedia(channel);
  			
			  bool bTmp = true;
			  if (media != null)
			  {				
				  bTmp = media.XUKOut(destination);
			  }
			  //else, it's ok to have an empty channels property, even though it might seem a bit strange

			  destination.WriteEndElement();
			  bRetVal = bTmp && bRetVal;
		  }

		  destination.WriteEndElement();

		  return bRetVal;
	  }
	  #endregion

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
					if (!newMedia.XUKIn(source)) return false;
				}
				IChannel channel = mPresentation.getChannelsManager().getChannelById(channelRef);
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

    #region IChannelsPropertyValidator Members

    /// <summary>
    /// Determines if a given <see cref="IMedia"/> can be associated
    /// with a given <see cref="IChannel"/> 
    /// without breaking <see cref="IChannelsProperty"/> rules
    /// </summary>
    /// <param name="channel">The given <see cref="IChannel"/></param>
    /// <param name="media">The given <see cref="IMedia"/></param>
    /// <returns>A <see cref="bool"/> indicating if the given <see cref="IMedia"/>
    /// can be associated with the given <see cref="IChannel"/></returns>
    public bool canSetMedia(IChannel channel, IMedia media)
    {
      if (channel==null)
      {
        throw new exception.MethodParameterIsNullException("The given channel is null");
      }
      if (media==null)
      {
        throw new exception.MethodParameterIsNullException("The given media is null");
      }
      // The media can not be set if the channel does not support the media type
      if (!channel.isMediaTypeSupported(media.getType())) return false;
      // If any ancestors of the owner node has media in the channel, the media can not be associated
      // The media can be set if the media is already associated with the channel
      if (getMedia(channel)!=null) return true;
      if (ChannelsPropertyCoreNodeValidator.DetectMediaOfAncestors(channel, getOwner()))
      {
        return false;
      }
      // If any descendants of the owner node has media in the channel, the media can not be associated
      if (ChannelsPropertyCoreNodeValidator.DetectMediaOfSelfOrDescendants(channel, getOwner()))
      {
        return false;
      }
      return false;
    }

    #endregion
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
  }
}
