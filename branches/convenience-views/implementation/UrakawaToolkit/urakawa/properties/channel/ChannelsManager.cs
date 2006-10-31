using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.core;
using urakawa.core.visitor;

namespace urakawa.properties.channel
{
	/// <summary>
	/// Default implementation of <see cref="IChannelsManager"/>
	/// Can only manage channels that inherit <see cref="Channel"/>
	/// TODO: Check XUKIn/XukOut implementation
	/// </summary>
	public class ChannelsManager : IChannelsManager
	{
    /// <summary>
    /// The list of channels managed by the manager
    /// </summary>
    private IDictionary<string, IChannel> mChannels;

		private IChannelPresentation mPresentation;

    private IChannelFactory mChannelFactory;


		/// <summary>
    /// Default constructor
    /// </summary>
	  public ChannelsManager()
	  {
			mChannels = new Dictionary<string, IChannel>();
    }

    #region IChannelsManager Members

		/// <summary>
		/// Gets the <see cref="IChannelsManager"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IChannelsManager"/></returns>
		public IChannelFactory getChannelFactory()
		{
			return mChannelFactory;
		}

		/// <summary>
		/// Sets the <see cref="ChannelFactory"/> of the <see cref="ChannelsManager"/>
		/// </summary>
		/// <param name="newFact"></param>
		public void setChannelFactory(IChannelFactory newFact)
		{
			mChannelFactory = newFact;
		}

		/// <summary>
		/// Sets the <see cref="IChannelPresentation"/> of the <see cref="ChannelsManager"/>
		/// </summary>
		/// <param name="newPres"></param>
		public void setPresentation(IChannelPresentation newPres)
		{
			mPresentation = newPres;
		}

		/// <summary>
		/// Gets the <see cref="IChannelPresentation"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IChannelPresentation"/></returns>
		public IChannelPresentation getPresentation()
		{
			return mPresentation;
		}

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
      if (mChannels.Values.Contains(channel))
      {
        throw new exception.ChannelAlreadyExistsException(
          "The given channel is already managed by the ChannelsManager");
      }
			mChannels.Add(getNewId(), channel);
    }

    private string getNewId()
    {
      ulong i = 0;
      while (i<UInt64.MaxValue)
      {
        string newId = String.Format(
          "CHID{0:0000}", i);
				if (!mChannels.ContainsKey(newId)) return newId;
        i++;
      }
      throw new OverflowException("YOU HAVE WAY TOO MANY CHANNELS!!!");
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
			string xukId = getXukIdOfChannel(channel);
			ClearChannelCoreNodeVisitor clChVisitor = new ClearChannelCoreNodeVisitor(channel);
			getPresentation().getRootNode().acceptDepthFirst(clChVisitor);
			mChannels.Remove(xukId);
    }

    /// <summary>
    /// Gets a lists of the <see cref="IChannel"/>s managed by the <see cref="IChannelsManager"/>
    /// </summary>
    /// <returns>The list</returns>
    public System.Collections.Generic.IList<IChannel> getListOfChannels()
    {
      return new List<IChannel>(mChannels.Values);
    }

		///// <summary>
		///// Removes all <see cref="Channel"/>s from the 
		///// <see cref="ChannelsManager"/>
		///// </summary>
		//public void removeAllChannels()
		//{
		//  foreach (IChannel ch in mChannels.Values)
		//  {
		//    removeChannel(ch);
		//  }
		//}
		#endregion


		/// <summary>
	  /// this is a helper function for getting one or more channels by its name
	  /// </summary>
	  /// <param name="channelName">The name of the channel to get</param>
	  /// <returns>An array of the </returns>
	  public IChannel[] getChannelByName(string channelName)
	  {
			List<IChannel> res = new List<IChannel>();
      foreach (IChannel ch in mChannels.Values)
      {
        if (ch.getName()==channelName) res.Add(ch);
      }
			return res.ToArray();
	  }

		#region IXukAble Members
		/// <summary>
		/// Reads the <see cref="ChannelsManager"/> instance state from the ChannelsManager element 
		/// of a XUK XML document
		/// </summary>
		/// <param name="source">A <see cref="XmlReader"/> with which to read the ChannelsManager element</param>
		/// <returns>A <see cref="bool"/> indicating if the read was succesful</returns>
		/// <remarks>The cursor of the <paramref name="source"/> must be positioned 
		/// at the start of the ChannelsManager element</remarks>
		public bool XukIn(System.Xml.XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("XML Reader is null");
			}
			if (source.NodeType != XmlNodeType.Element) return false;
			if (source.LocalName != "ChannelsManager") return false;
			if (source.NamespaceURI != urakawa.ToolkitSettings.XUK_NS) return false;

			if (source.IsEmptyElement) return true;
			while (source.Read())
			{
				if (source.NodeType == XmlNodeType.Element)
				{
					IChannel newCh = mChannelFactory.createChannel(source.LocalName, source.NamespaceURI);
					if (newCh == null)//Child not recognized so skip element
					{
						if (!source.IsEmptyElement)
						{
							//Reads sub tree and places cursor at end element
							source.ReadSubtree().Close();
						}
					}
					else
					{
						string xukId = source.GetAttribute("id");
						if (mChannels.ContainsKey(xukId)) xukId = getNewId();
						if (newCh.XukIn(source))
						{
							mChannels.Add(xukId, newCh);
						}
						else
						{
							return false;
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
		/// Write the state of the <see cref="ChannelsManager"/> instance state 
		/// to a ChannelsMaanger element in a XUK XML document
		/// </summary>
		/// <param name="destination"></param>
		/// <returns></returns>
		public bool XukOut(System.Xml.XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Writer is null");
			}

			destination.WriteStartElement("ChannelsManager", urakawa.ToolkitSettings.XUK_NS);

			foreach (IChannel ch in mChannels.Values)
			{
				if (!ch.XukOut(destination)) return false;
			}

			destination.WriteEndElement();

			return true;
		}

		
		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="ChannelsManager"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="ChannelsManager"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

		#region IChannelsManager Members


		/// <summary>
		/// Gets the <see cref="IChannel"/> with a given xuk id
		/// </summary>
		/// <param name="Id">The given xuk id</param>
		/// <returns>The <see cref="IChannel"/> with the given xuk id</returns>
		/// <exception cref="exception.ChannelDoesNotExistException">
		/// Thrown when <c>this</c> does not manage a <see cref="IChannel"/> with the given xuk id
		/// </exception>
		public IChannel getChannelByXukId(string Id)
		{
			if (!mChannels.Keys.Contains(Id))
			{
				throw new exception.ChannelDoesNotExistException(String.Format(
					"The channels manager does not manage a channel with xuk id {0}",
					Id));
					
			}
			return mChannels[Id];
		}


		/// <summary>
		/// Gets the Xuk id of a given channel
		/// </summary>
		/// <param name="ch">The given channel</param>
		/// <returns>The Xuk Id of the given channel</returns>
		/// <exception cref="exception.ChannelDoesNotExistException">
		/// Thrown when the given channel is not managed by <c>this</c>
		/// </exception>
		public string getXukIdOfChannel(IChannel ch)
		{
			foreach (string Id in mChannels.Keys)
			{
				if (mChannels[Id]==ch)
				{
					return Id;
				}
			}
			throw new exception.ChannelDoesNotExistException("The given channel is not managed by this");
		}

		#endregion
	}
}
