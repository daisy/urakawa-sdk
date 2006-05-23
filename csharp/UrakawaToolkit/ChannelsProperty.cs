using System;
using System.Collections;

using urakawa.media;

namespace urakawa.core
{
	/// <summary>
	/// Default implementation of 
	/// </summary>
  public class ChannelsProperty : IChannelsProperty
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
    /// Constructor using a given <see cref="IDictionary"/> for channels to media mapping
    /// </summary>
    /// <param name="pres">The <see cref="Presentation"/> 
    /// associated with the <see cref="ChannelsProperty"/></param>
    /// <param name="chToMediaMapper">
    /// The <see cref="IDictionary"/> used to map channels and media</param>
    internal ChannelsProperty(Presentation pres, IDictionary chToMediaMapper)
    {
      mPresentation = pres;
      mPresentation.getChannelsManager().Removed 
        += new ChannelsManagerRemovedEventDelegate(mChannelsManager_Removed);
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


    /// <summary>
    /// Destructor - stops listining for the <see cref="ChannelsManager.Removed"/>
    /// ecent of the associated <see cref="ChannelsManager"/>
    /// </summary>
    ~ChannelsProperty()
    {
      if (mChannelsManager!=null)
      {
        mPresentation.getChannelsManager().Removed 
          -= new ChannelsManagerRemovedEventDelegate(mChannelsManager_Removed);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    ChannelsProperty copy()
    {
      ChannelsProperty theCopy = mPresentation.getPropertyFactory().
      return this;
    }

    #region IProperty Members

    /// <summary>
    /// Gets the <see cref="PropertyType"/> of the <see cref="ChannelsProperty"/>
    /// </summary>
    /// <returns>Always <see cref="PropertyType.ChannelsProperty"/></returns>
    public PropertyType getPropertyType()
    {
      return PropertyType.ChannelsProperty;
    }

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
      if (!mChannelsManager.getListOfChannels().Contains(channel))
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
    /// <param name="media">The given <see cref="IMedia"/></param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when parameters <paramref name="channel"/> or <paramref name="media"/> is null
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
      if (media==null)
      {
        throw new exception.MethodParameterIsNullException(
          "media parameter is null");
      }
      if (!mChannelsManager.getListOfChannels().Contains(channel))
      {
        throw new exception.ChannelDoesNotExistException(
          "The given channel is not managed by the ChannelManager associated with the ChannelsProperty");
      }
      if (!channel.isMediaTypeSupported(media.getType()))
      {
        throw new exception.MediaTypeIsIllegalException(
          "The given media type is not supported by the given channel");
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
      foreach (IChannel ch in mChannelsManager.getListOfChannels())
      {
        if (getMedia(ch)!=null)
        {
          res.Add(ch);
        }
      }
      return res;
    }
    #endregion

		#region IXUKable members 

		public bool XUKin(System.Xml.XmlReader source)
		{
			//TODO: actual implementation, for now we return false as default, signifying that all was not done
			return false;
		}

		public bool XUKout(System.Xml.XmlWriter destination)
		{
			//TODO: actual implementation, for now we return false as default, signifying that all was not done
			return false;
		}
		#endregion



    /// <summary>
    /// Event handler for the <see cref="ChannelsManager.Removed"/> event 
    /// of the associated <see cref="ChannelsManager"/>
    /// </summary>
    /// <param name="o">The associated <see cref="ChannelsManager"/> raising the event</param>
    /// <param name="e">The event arguments passed with the event</param>
    private void mChannelsManager_Removed(ChannelsManager o, ChannelsManagerRemovedEventArgs e)
    {
      mMapChannelToMediaObject.Remove(e.RemovedChannel);
    }
  }
}
