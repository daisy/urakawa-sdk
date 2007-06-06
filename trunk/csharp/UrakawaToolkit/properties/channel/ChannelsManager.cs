using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.core;
using urakawa.core.visitor;
using urakawa.xuk;

namespace urakawa.properties.channel
{
	/// <summary>
	/// Default implementation of <see cref="ChannelsManager"/>
	/// Can only manage channels that inherit <see cref="Channel"/>
	/// TODO: Check XUKIn/XukOut implementation
	/// </summary>
	public class ChannelsManager : IXukAble, IValueEquatable<ChannelsManager>
	{
    /// <summary>
    /// The list of channels managed by the manager
    /// </summary>
    private IDictionary<string, Channel> mChannels;

		private IChannelPresentation mPresentation;

		/// <summary>
    /// Default constructor
    /// </summary>
	  public ChannelsManager()
	  {
			mChannels = new Dictionary<string, Channel>();
    }

    #region ChannelsManager Members

		/// <summary>
		/// Gets the <see cref="ChannelsManager"/> associated with <c>this</c>.
		/// Convenience for <c>getPresentation().getChannelFactory()</c>
		/// </summary>
		/// <returns>The <see cref="ChannelsManager"/></returns>
		public ChannelFactory getChannelFactory()
		{
			return getPresentation().getChannelFactory();
		}

		/// <summary>
		/// Sets the <see cref="IChannelPresentation"/> of the <see cref="ChannelsManager"/>
		/// </summary>
		/// <param name="newPres"></param>
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
    /// Adds an existing  <see cref="Channel"/> to the list of <see cref="Channel"/>s 
    /// managed by the <see cref="ChannelsManager"/>
    /// </summary>
    /// <param name="channel">The <see cref="Channel"/> to add</param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref localName="channel"/> is null
    /// </exception>
    /// <exception cref="exception.ChannelAlreadyExistsException">
    /// Thrown when <paramref localName="channel"/> is already in the managers list of channels
    /// </exception>
    public void addChannel(Channel channel)
    {
			addChannel(channel, getNewId());
    }

		/// <summary>
		/// Adds an existing  <see cref="Channel"/> to the list of <see cref="Channel"/>s 
		/// managed by the <see cref="ChannelsManager"/> with a given UID
		/// </summary>
		/// <param name="channel">The <see cref="Channel"/> to add</param>
		/// <param name="uid">The UID assigned to the added channel</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="channel"/> or <paramref name="uid"/> are <c>null</c>
		/// </exception>
		/// <exception cref="exception.MethodParameterIsEmptyStringException">
		/// Thrown when <paramref name="uid"/> is an empty string</exception>
		/// <exception cref="exception.ChannelAlreadyExistsException">
		/// Thrown when <paramref name="channel"/> is already in the managers list of channels
		/// or when another channel exists with the given uid.
		/// </exception>
		protected void addChannel(Channel channel, string uid)
		{
			if (channel == null)
			{
				throw new exception.MethodParameterIsNullException("channel parameter is null");
			}
			if (uid == null)
			{
				throw new exception.MethodParameterIsNullException("uid parameter is null");
			}
			if (uid == "")
			{
				throw new exception.MethodParameterIsEmptyStringException("uid parameter is empty string");
			}
			if (mChannels.Values.Contains(channel))
			{
				throw new exception.ChannelAlreadyExistsException(
					"The given channel is already managed by the ChannelsManager");
			}
			if (mChannels.ContainsKey(uid))
			{
				throw new exception.ChannelAlreadyExistsException(
					String.Format("Another channel exists with uid {0}", uid));
			}
			mChannels.Add(uid, channel);

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
    /// Removes an <see cref="Channel"/> from the list
    /// </summary>
    /// <param name="channel">The <see cref="Channel"/> to remove</param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref localName="channel"/> is null
    /// </exception>
    /// <exception cref="exception.ChannelDoesNotExistException">
    /// Thrown when <paramref localName="channel"/> is not in the managers list of channels
    /// </exception>
    public void detachChannel(Channel channel)
    {
      if (channel==null)
      {
        throw new exception.MethodParameterIsNullException(
          "channel parameter is null");
      }
			string xukId = getUidOfChannel(channel);
			ClearChannelTreeNodeVisitor clChVisitor = new ClearChannelTreeNodeVisitor(channel);
			getPresentation().getRootNode().acceptDepthFirst(clChVisitor);
			mChannels.Remove(xukId);
    }

    /// <summary>
    /// Gets a lists of the <see cref="Channel"/>s managed by the <see cref="ChannelsManager"/>
    /// </summary>
    /// <returns>The list</returns>
    public List<Channel> getListOfChannels()
    {
      return new List<Channel>(mChannels.Values);
    }

		/// <summary>
		/// Gets a list of the uids of <see cref="Channel"/>s managed by the <see cref="ChannelsManager"/>
		/// </summary>
		/// <returns>The list</returns>
		public List<string> getListOfUids()
		{
			return new List<string>(mChannels.Keys);
		}

		/// <summary>
		/// Gets the <see cref="Channel"/> with a given xuk uid
		/// </summary>
		/// <param name="Uid">The given xuk uid</param>
		/// <returns>The <see cref="Channel"/> with the given xuk uid</returns>
		/// <exception cref="exception.ChannelDoesNotExistException">
		/// Thrown when <c>this</c> does not manage a <see cref="Channel"/> with the given xuk uid
		/// </exception>
		public Channel getChannel(string Uid)
		{
			if (!mChannels.Keys.Contains(Uid))
			{
				throw new exception.ChannelDoesNotExistException(String.Format(
					"The channels manager does not manage a channel with xuk uid {0}",
					Uid));
					
			}
			return mChannels[Uid];
		}


		/// <summary>
		/// Gets the Xuk id of a given channel
		/// </summary>
		/// <param name="ch">The given channel</param>
		/// <returns>The Xuk Uid of the given channel</returns>
		/// <exception cref="exception.ChannelDoesNotExistException">
		/// Thrown when the given channel is not managed by <c>this</c>
		/// </exception>
		public string getUidOfChannel(Channel ch)
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

		/// <summary>
		/// Removes all <see cref="Channel"/>s from the manager
		/// </summary>
		public void clearChannels()
		{
			foreach (Channel ch in getListOfChannels())
			{
				detachChannel(ch);
			}
		}


		/// <summary>
		/// this is a helper function for getting one or more channels by its localName
		/// </summary>
		/// <param name="channelName">The localName of the channel to get</param>
		/// <returns>An array of the </returns>
		public List<Channel> getChannelByName(string channelName)
		{
			List<Channel> res = new List<Channel>();
			foreach (Channel ch in mChannels.Values)
			{
				if (ch.getName() == channelName) res.Add(ch);
			}
			return res;
		}

		#endregion


		#region IXukAble Members

		/// <summary>
		/// Reads the <see cref="ChannelsManager"/> from a ChannelsManager xuk element
		/// </summary>
		/// <param name="source">The source <see cref="System.Xml.XmlReader"/></param>
		public void XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element)
			{
				throw new exception.XukException("Can not read ChannelsManager from a non-element node");
			}
			mChannels.Clear();
			try
			{
				XukInAttributes(source);
				if (!source.IsEmptyElement)
				{
					while (source.Read())
					{
						if (source.NodeType == XmlNodeType.Element)
						{
							XukInChild(source);
						}
						else if (source.NodeType == XmlNodeType.EndElement)
						{
							break;
						}
						if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
					}
				}

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukIn of ChannelsManager: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a ChannelsManager xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInAttributes(XmlReader source)
		{
		}

		/// <summary>
		/// Reads a child of a ChannelsManager xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual bool XukInChild(XmlReader source)
		{
			if (source.NamespaceURI == ToolkitSettings.XUK_NS && source.LocalName == "mChannels")
			{
				if (!source.IsEmptyElement)
				{
					while (source.Read())
					{
						if (source.NodeType == XmlNodeType.Element)
						{
							if (source.LocalName == "mChannelItem" && source.NamespaceURI==ToolkitSettings.XUK_NS)
							{
								XukInChannelItem(source);
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
			else if (!source.IsEmptyElement)
			{
				source.ReadSubtree().Close();
			}
			return true;
		}

		private void XukInChannelItem(XmlReader source)
		{
			string uid = source.GetAttribute("uid");
			if (uid == "" || uid == null)
			{
				throw new exception.XukException("mChannelItem element has no uid attribute");
			}
			bool foundChannel = false;
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						Channel newCh = getChannelFactory().createChannel(source.LocalName, source.NamespaceURI);
						if (newCh != null)
						{
							newCh.XukIn(source);
							try
							{
								addChannel(newCh, uid);
							}
							catch (exception.CheckedException e)
							{
								throw new exception.XukException(
									String.Format("Could not add Xuked In channel: {0}", e.Message),
									e);
							}
							foundChannel = true;
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
			if (!foundChannel)
			{
				throw new exception.XukException("Fould no Channel inside mChannelItem");
			}
		}

		/// <summary>
		/// Write a ChannelsManager element to a XUK file representing the <see cref="ChannelsManager"/> instance
		/// </summary>
		/// <param name="destination">The destination <see cref="System.Xml.XmlWriter"/></param>
		public void XukOut(System.Xml.XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not XukOut to a null XmlWriter");
			}
			try
			{
				destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
				XukOutAttributes(destination);
				XukOutChildren(destination);
				destination.WriteEndElement();
			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukOut of ChannelsManager: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a ChannelsManager element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutAttributes(XmlWriter destination)
		{

		}

		/// <summary>
		/// Write the child elements of a ChannelsManager element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutChildren(XmlWriter destination)
		{
			List<string> uids = getListOfUids();
			if (uids.Count > 0)
			{
				destination.WriteStartElement("mChannels");
				foreach (string uid in uids)
				{
					destination.WriteStartElement("mChannelItem");
					destination.WriteAttributeString("uid", uid);
					getChannel(uid).XukOut(destination);
					destination.WriteEndElement();
				}
				destination.WriteEndElement();
			}
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

		#region IValueEquatable<ChannelsManager> Members


		/// <summary>
		/// Determines of <c>this</c> has the same value as a given other instance
		/// </summary>
		/// <param name="other">The other instance</param>
		/// <returns>A <see cref="bool"/> indicating the result</returns>
		public bool ValueEquals(ChannelsManager other)
		{
			List<string> thisUids = getListOfUids();
			List<string> otherUids = other.getListOfUids();
			if (thisUids.Count != otherUids.Count) return false;
			foreach (string uid in thisUids)
			{
				if (!otherUids.Contains(uid)) return false;
				if (!getChannel(uid).ValueEquals(other.getChannel(uid))) return false;
			}
			return true;
		}

		#endregion
	}
}
