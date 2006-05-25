using System;
using System.Collections;

namespace urakawa.core
{
  /// <summary>
  /// Arguments for the <see cref="ChannelsManager.Removed"/> event
  /// </summary>
  internal class ChannelsManagerRemovedEventArgs : EventArgs
  {
    /// <summary>
    /// The removed <see cref="IChannel"/>
    /// </summary>
    public IChannel RemovedChannel;

    /// <summary>
    /// Constructor - sets member 
    /// </summary>
    /// <param name="removedCh">The value for member <see cref="RemovedChannel"/></param>
    public ChannelsManagerRemovedEventArgs(IChannel removedCh)
    {
      RemovedChannel = removedCh;
    }
  }

  /// <summary>
  /// Delegate for the <see cref="ChannelsManager.Removed"/> event
  /// </summary>
  internal delegate void ChannelsManagerRemovedEventDelegate(
    ChannelsManager o, ChannelsManagerRemovedEventArgs e);

	/// <summary>
	/// Default implementation of <see cref="IChannelsManager"/>
	/// </summary>
	public class ChannelsManager : IChannelsManager
	{
    /// <summary>
    /// The list of channels managed by the manager
    /// </summary>
    private IList mChannels;

    /// <summary>
    /// Event fired when a <see cref="IChannel"/> is removed from the list of <see cref="IChannel"/> 
    /// mamaged by the <see cref="ChannelsManager"/>
    /// </summary>
    internal event ChannelsManagerRemovedEventDelegate Removed;

    private void FireRemoved(IChannel removedChannel)
    {
      if (Removed!=null) Removed(this, new ChannelsManagerRemovedEventArgs(removedChannel));
    }

    /// <summary>
    /// Default constructor
    /// </summary>
	public ChannelsManager()
	{
		mChannels = new ArrayList();
    }
    #region IChannelsManager Members

    /// <summary>
    /// Adds an existing  <see cref="IChannel"/> to the list of <see cref="IChannel"/>s 
    /// managed by the <see cref="ChannelsManager"/>
    /// </summary>
    /// <param name="channel">The <see cref="IChannel"/> to add</param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref name="channel"/> is null
    /// </exception>
    /// <exception cref="exception.ChannelAlreadyExistsException">
    /// Thrown when <paramref name="channel"/> is already in the managers list of channels
    /// </exception>
    public void addChannel(IChannel channel)
    {
      if (channel==null)
      {
        throw new exception.MethodParameterIsNullException(
          "channel parameter is null");
      }
      if (mChannels.IndexOf(channel)!=-1)
      {
        throw new exception.ChannelAlreadyExistsException(
          "The given channel is already managed by the ChannelsManager");
      }
      mChannels.Add(channel);
    }

    /// <summary>
    /// Removes an <see cref="IChannel"/> from the list
    /// </summary>
    /// <param name="channel">The <see cref="IChannel"/> to remove</param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref name="channel"/> is null
    /// </exception>
    /// <exception cref="exception.ChannelDoesNotExistException">
    /// Thrown when <paramref name="channel"/> is not in the managers list of channels
    /// </exception>
    public void removeChannel(IChannel channel)
    {
      if (channel==null)
      {
        throw new exception.MethodParameterIsNullException(
          "channel parameter is null");
      }
      int index = mChannels.IndexOf(channel);
      if (index==-1)
      {
        throw new exception.ChannelDoesNotExistException(
          "The given channel is not managed by the ChannelsManager");
      }
      FireRemoved(channel);
      mChannels.RemoveAt(index);
    }

    /// <summary>
    /// Gets a lists of the <see cref="IChannel"/>s managed by the <see cref="IChannelsManager"/>
    /// </summary>
    /// <returns>Teh list</returns>
    public System.Collections.IList getListOfChannels()
    {
      // ArrayList(ICollection c) constructs a new ArrayList with the items of the given ICollection,
      // items are not cloned
      return new ArrayList(mChannels);
    }
    #endregion

	#region IXUKable members 

	public bool XUKin(System.Xml.XmlReader source)
	{
		if (source == null)
		{
			throw new exception.MethodParameterIsNullException("XML Reader is null");
		}

		//if we are not at the opening tag for the ChannelsManager element, return false
		if (!(source.Name == "ChannelsManager" && 
			source.NodeType == System.Xml.XmlNodeType.Element))
		{
			return false;
		}

		System.Diagnostics.Debug.WriteLine("XUKin: ChannelsManager");

		bool bChannelsAdded = false;
		bool bChannelsFound = false;

		//read until the end of the ChannelsManager element
		while (!(source.NodeType == System.Xml.XmlNodeType.EndElement && 
			source.Name == "ChannelsManager")
			&&
			source.EOF == false)
		{
			//look at the next element
			source.Read();

			//are we in a Channel element?
			if (source.Name == "Channel" && source.NodeType == System.Xml.XmlNodeType.Element)
			{
				//if this is the first channel, just record its value
				if (bChannelsFound == false)
				{
					bChannelsAdded = this.XUKin_Channel(source);
				}

				//otherwise, keep a cumulative record of any error that has happened
				else
				{
					bChannelsAdded = bChannelsAdded && this.XUKin_Channel(source);
				}

				bChannelsFound = true;
			}
		}

		return bChannelsAdded;
	}

	public bool XUKout(System.Xml.XmlWriter destination)
	{
		if (destination == null)
		{
			throw new exception.MethodParameterIsNullException("Xml Writer is null");
		}

		destination.WriteStartElement("ChannelsManager");

		bool bWroteChannels = true;

		for (int i=0; i<mChannels.Count; i++)
		{
			Channel tmpChannel = (Channel)mChannels[i];

			bool bTmp = tmpChannel.XUKout(destination);

			bWroteChannels = bWroteChannels && bTmp;
		}

		destination.WriteEndElement();

		return bWroteChannels;
	}
	#endregion

	/// <summary>
	/// helper function to create a new channel and add it to this channels manager
	/// </summary>
	/// <param name="source"></param>
	private bool XUKin_Channel(System.Xml.XmlReader source)
	{
		if (!(source.Name == "Channel" && 
			source.NodeType == System.Xml.XmlNodeType.Element))
		{
			return false;
		}

		System.Diagnostics.Debug.WriteLine("XUKin: ChannelsManager::Channel");

		string id = source.GetAttribute("id");

		if (source.IsEmptyElement == true)
		{
			Channel channel = new Channel("");
			channel.setId(id);

			this.addChannel(channel);
		}
		else
		{
			//the text node should come next
			source.Read();
			if (source.NodeType == System.Xml.XmlNodeType.Text)
			{
				Channel channel = new Channel(source.Value);
				channel.setId(id);

				//add a channel
				this.addChannel(channel);
			}
		}

		return true;
	}

		//note: this function assumes mChannel contains Channel objects, not just anything using IChannel
		public IChannel getChannelById(string id)
		{
			Channel tmpChannel = null;

			bool bFound = false;
			for (int i = 0; i<mChannels.Count; i++)
			{
				if (mChannels[i].GetType() == typeof(Channel))
				{
					tmpChannel = (Channel)mChannels[i];
					if (tmpChannel.getId() == id)
					{
						bFound = true;
						break;
					}
				}
			}

			if (bFound == true)
			{
				return tmpChannel;
			}
			else
			{
				return null;
			}
		}
  }
}
