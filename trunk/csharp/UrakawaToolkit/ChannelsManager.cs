using System;
using System.Collections;
using System.Xml;
// TODO: Check XUKin/XUKout implementation

namespace urakawa.core
{

	/// <summary>
	/// <see cref="ICoreNodeVisitor"/> for clearing all media within a 
	/// <see cref="IChannel"/>
	/// </summary>
	public class ClearChannelCoreNodeVisitor : ICoreNodeVisitor
	{
		private IChannel mChannelToClear;

		/// <summary>
		/// Gets the <see cref="IChannel"/> within which to 
		/// clear <see cref="urakawa.media.IMedia"/>
		/// </summary>
		public IChannel ChannelToClear
		{
			get
			{
				return mChannelToClear;
			}
		}

		/// <summary>
		/// Constructor setting the <see cref="IChannel"/> to clear
		/// </summary>
		/// <param name="chToClear"></param>
		public ClearChannelCoreNodeVisitor(IChannel chToClear)
		{
			mChannelToClear = chToClear;
		}
		#region ICoreNodeVisitor Members

		/// <summary>
		/// Pre-visit action: If <see cref="urakawa.media.IMedia"/> is present in <see cref="IChannel"/> <see cref="ChannelToClear"/>,
		/// this is removed and the child <see cref="ICoreNode"/>s are not visited
		/// </summary>
		/// <param name="node">The <see cref="ICoreNode"/> to visit</param>
		/// <returns>
		/// <c>false</c> if <see cref="urakawa.media.IMedia"/> is found if <see cref="IChannel"/> <see cref="ChannelToClear"/>,
		/// <c>false</c> else
		/// </returns>
		public bool preVisit(ICoreNode node)
		{
			bool foundMedia = false;
			ChannelsProperty chProp = 
				(ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
			if (chProp!=null)
			{
				urakawa.media.IMedia m = chProp.getMedia(ChannelToClear);
				if (m!=null)
				{
          chProp.setMedia(ChannelToClear, null);
				}
			}
			return !foundMedia;
		}

		/// <summary>
		/// Post-visit action: Nothing is done here
		/// </summary>
		/// <param name="node">The <see cref="ICoreNode"/> to visit</param>
		public void postVisit(ICoreNode node)
		{
			// TODO:  Add ClearChannelCoreNodeVisitor.postVisit implementation
		}

		#endregion
	}

	/// <summary>
	/// Default implementation of <see cref="IChannelsManager"/>
	/// Can only manage channels that inherit <see cref="Channel"/>
	/// TODO: Check XUKin/XUKout implementation
	/// </summary>
	public class ChannelsManager : IChannelsManager
	{
    /// <summary>
    /// The list of channels managed by the manager
    /// </summary>
    private IList mChannels;

		private IPresentation mPresentation;
    private ChannelFactory mChannelFactory;
//
//    /// <summary>
//    /// Event fired when a <see cref="IChannel"/> is removed from the list of <see cref="IChannel"/> 
//    /// mamaged by the <see cref="ChannelsManager"/>
//    /// </summary>
//    internal event ChannelsManagerRemovedEventDelegate Removed;
//
//    private void FireRemoved(IChannel removedChannel)
//    {
//      if (Removed!=null) Removed(this, new ChannelsManagerRemovedEventArgs(removedChannel));
//    }

    /// <summary>
    /// Default constructor setting the assocuated <see cref="ChannelFactory"/>
    /// </summary>
	  public ChannelsManager(ChannelFactory chFact, IPresentation pres)
	  {
		  mChannels = new ArrayList();
      mChannelFactory = chFact;
			mPresentation = pres;
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
    /// <exception cref="exception.MethodParameterIsWrongTypeException">
    /// Thrown when <paramref name="channel"/> does not inherit <see cref="Channel"/>
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
      if (!typeof(Channel).IsAssignableFrom(channel.GetType()))
      {
        throw new exception.MethodParameterIsWrongTypeException(
          "ChannelsManager does only manage instances that inherit Channel class");
      }
      Channel ch = (Channel)channel;
      if (ch.getId()=="" || ch.getId()==null) ch.setId(getNewId());
      if (getChannelById(ch.getId())!=null) ch.setId(getNewId());
      mChannels.Add(channel);
    }

    private string getNewId()
    {
      ulong i = 0;
      while (i<UInt64.MaxValue)
      {
        string newId = String.Format(
          "CHID{0:0000}", i);
        if (getChannelById(newId)==null) return newId;
        i++;
      }
      throw new ApplicationException("YOU HAVE WAY TOO MANY CHANNELS!!!");
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
//      FireRemoved(channel);
			ClearChannelCoreNodeVisitor clChVisitor = new ClearChannelCoreNodeVisitor(channel);
			mPresentation.getRootNode().acceptDepthFirst(clChVisitor);
			
      mChannels.RemoveAt(index);
    }

    /// <summary>
    /// Gets a lists of the <see cref="IChannel"/>s managed by the <see cref="IChannelsManager"/>
    /// </summary>
    /// <returns>The list</returns>
    public System.Collections.IList getListOfChannels()
    {
      // ArrayList(ICollection c) constructs a new ArrayList with the items of the given ICollection,
      // items are not cloned
      return new ArrayList(mChannels);
    }

		/// <summary>
		/// Removes all <see cref="Channel"/>s from the 
		/// <see cref="ChannelsManager"/>
		/// </summary>
		public void removeAllChannels()
		{
			foreach (IChannel ch in mChannels)
			{
				removeChannel(ch);
			}
		}
    #endregion


	  #region IXUKable members 
    /// <summary>
    /// Reads the <see cref="ChannelsManager"/> instance state from the ChannelsManager element 
    /// of a XUK XML document
    /// </summary>
    /// <param name="source">A <see cref="XmlReader"/> with which to read the ChannelsManager element</param>
    /// <returns>A <see cref="bool"/> indicating if the read was succesful</returns>
    /// <remarks>The cursor of the <paramref name="source"/> must be positioned 
    /// at the start of the ChannelsManager element</remarks>
	  public bool XUKin(System.Xml.XmlReader source)
	  {
		  if (source == null)
		  {
			  throw new exception.MethodParameterIsNullException("XML Reader is null");
		  }
			if (source.NodeType != XmlNodeType.Element) return false;
			if (source.LocalName != "ChannelsManager") return false;
			if (source.NamespaceURI != ChannelFactory.XUK_NS) return false;

      if (source.IsEmptyElement) return true;
      while (source.Read())
      {
        if (source.NodeType==XmlNodeType.Element)
        {
					IChannel newCh = mChannelFactory.createChannel(source.LocalName, source.NamespaceURI);
					if (newCh == null)
					{
						if (!source.IsEmptyElement)
						{
							//Reads sub tree and places cursor at end element
							source.ReadSubtree().Close();
						}
					}
					else
					{
						if (newCh.XUKin(source))
						{
							this.addChannel(newCh);
						}
						else
						{
							return false;
						}
					}
        }
        else if (source.NodeType==XmlNodeType.EndElement)
        {
          break;
        }
				if (source.EOF) return false;
      }
      return true;
	  }

    /// <summary>
    /// Write the state of the <see cref="ChannelsManager"/> instance state 
    /// to a ChannelsMaanger element in a XUK XML document
    /// </summary>
    /// <param name="destination"></param>
    /// <returns></returns>
	  public bool XUKout(System.Xml.XmlWriter destination)
	  {
		  if (destination == null)
		  {
			  throw new exception.MethodParameterIsNullException("Xml Writer is null");
		  }

		  destination.WriteStartElement("ChannelsManager", ChannelFactory.XUK_NS);

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
//
//	  /// <summary>
//	  /// helper function to create a new channel and add it to this channels manager
//	  /// </summary>
//	  /// <param name="source"></param>
//	  private bool XUKin_Channel(System.Xml.XmlReader source)
//	  {
//		  if (!(source.Name == "Channel" && 
//			  source.NodeType == System.Xml.XmlNodeType.Element))
//		  {
//			  return false;
//		  }
//
//		  //System.Diagnostics.Debug.WriteLine("XUKin: ChannelsManager::Channel");
//
//		  string id = source.GetAttribute("id");
//
//		  if (source.IsEmptyElement == true)
//		  {
//			  Channel channel = new Channel("");
//			  channel.setId(id);
//
//			  this.addChannel(channel);
//		  }
//		  else
//		  {
//			  //the text node should come next
//			  source.Read();
//			  if (source.NodeType == System.Xml.XmlNodeType.Text)
//			  {
//				  Channel channel = new Channel(source.Value);
//				  channel.setId(id);
//
//				  //add a channel
//				  this.addChannel(channel);
//			  }
//		  }
//
//		  return true;
//	  }
    #endregion

	  /// <summary>
	  /// this is a helper function for getting one or more channels by its name
	  /// </summary>
	  /// <param name="channelName">The name of the channel to get</param>
	  /// <returns>An array of the </returns>
	  public IChannel[] getChannelByName(string channelName)
	  {
      ArrayList res = new ArrayList();
      foreach (IChannel ch in mChannels)
      {
        if (ch.getName()==channelName) res.Add(ch);
      }
      return (IChannel[])res.ToArray(typeof(IChannel));
	  }

		//note: this function assumes mChannel contains Channel objects, not just anything using IChannel
    /// <summary>
    /// Retrieves a <see cref="IChannel"/> by it's id (in the XUK file)
    /// </summary>
    /// <param name="id">The id</param>
    /// <returns>The <see cref="IChannel"/> with the desired id if found, else <c>null</c></returns>
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
