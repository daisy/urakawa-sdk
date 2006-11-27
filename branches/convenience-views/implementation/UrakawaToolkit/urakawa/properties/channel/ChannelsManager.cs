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

		/// <summary>
    /// Default constructor
    /// </summary>
	  public ChannelsManager()
	  {
			mChannels = new Dictionary<string, IChannel>();
    }

    #region IChannelsManager Members

		/// <summary>
		/// Gets the <see cref="IChannelsManager"/> associated with <c>this</c>.
		/// Convenience for <c>getPresentation().getChannelFactory()</c>
		/// </summary>
		/// <returns>The <see cref="IChannelsManager"/></returns>
		public IChannelFactory getChannelFactory()
		{
			return getPresentation().getChannelFactory();
		}

		/// <summary>
		/// Sets the <see cref="IChannelPresentation"/> of the <see cref="ChannelsManager"/>
		/// </summary>
		/// <param localName="newPres"></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// The associated <see cref="IChannelPresentation"/> can not be null
		/// </exception>
		public void setPresentation(IChannelPresentation newPres)
		{
			if (newPres == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The associated presentation can not be null");
			}
			mPresentation = newPres;
		}

		/// <summary>
		/// Gets the <see cref="IChannelPresentation"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IChannelPresentation"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// When no <see cref="IChannelPresentation"/> has been associated with <c>this</c>
		/// </exception>
		public IChannelPresentation getPresentation()
		{
			if (mPresentation == null)
			{
				throw new exception.IsNotInitializedException(
					"The ChannelsManager has not been initialized with a IChannelPresentation");
			}
			return mPresentation;
		}

    /// <summary>
    /// Adds an existing  <see cref="IChannel"/> to the list of <see cref="IChannel"/>s 
    /// managed by the <see cref="ChannelsManager"/>
    /// </summary>
    /// <param localName="channel">The <see cref="IChannel"/> to add</param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref localName="channel"/> is null
    /// </exception>
    /// <exception cref="exception.ChannelAlreadyExistsException">
    /// Thrown when <paramref localName="channel"/> is already in the managers list of channels
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
    /// <param localName="channel">The <see cref="IChannel"/> to remove</param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref localName="channel"/> is null
    /// </exception>
    /// <exception cref="exception.ChannelDoesNotExistException">
    /// Thrown when <paramref localName="channel"/> is not in the managers list of channels
    /// </exception>
    public void removeChannel(IChannel channel)
    {
      if (channel==null)
      {
        throw new exception.MethodParameterIsNullException(
          "channel parameter is null");
      }
			string xukId = getUidOfChannel(channel);
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

		/// <summary>
		/// Gets the <see cref="IChannel"/> with a given xuk id
		/// </summary>
		/// <param localName="Id">The given xuk id</param>
		/// <returns>The <see cref="IChannel"/> with the given xuk id</returns>
		/// <exception cref="exception.ChannelDoesNotExistException">
		/// Thrown when <c>this</c> does not manage a <see cref="IChannel"/> with the given xuk id
		/// </exception>
		public IChannel getChannel(string Id)
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
		/// <param localName="ch">The given channel</param>
		/// <returns>The Xuk Id of the given channel</returns>
		/// <exception cref="exception.ChannelDoesNotExistException">
		/// Thrown when the given channel is not managed by <c>this</c>
		/// </exception>
		public string getUidOfChannel(IChannel ch)
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


		/// <summary>
	  /// this is a helper function for getting one or more channels by its localName
	  /// </summary>
	  /// <param localName="channelName">The localName of the channel to get</param>
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
		/// <param localName="source">A <see cref="XmlReader"/> with which to read the ChannelsManager element</param>
		/// <returns>A <see cref="bool"/> indicating if the read was succesful</returns>
		/// <remarks>The cursor of the <paramref localName="source"/> must be positioned 
		/// at the start of the ChannelsManager element</remarks>
		public bool XukIn(System.Xml.XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("XML Reader is null");
			}
			if (source.NodeType != XmlNodeType.Element) return false;

			if (source.IsEmptyElement) return true;
			while (source.Read())
			{
				if (source.NodeType == XmlNodeType.Element)
				{
					IChannel newCh = getChannelFactory().createChannel(source.LocalName, source.NamespaceURI);
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
		/// <param localName="destination"></param>
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
		/// Gets the local localName part of the QName representing a <see cref="ChannelsManager"/> in Xuk
		/// </summary>
		/// <returns>The local localName part</returns>
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
	}
}
