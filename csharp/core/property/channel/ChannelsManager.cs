using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.progress;
using urakawa.xuk;

namespace urakawa.property.channel
{
    /// <summary>
    /// Default implementation of <see cref="ChannelsManager"/>
    /// Can only manage channels that inherit <see cref="Channel"/>
    /// TODO: Check XUKIn/XukOut implementation
    /// </summary>
    public sealed class ChannelsManager : XukAble, IValueEquatable<ChannelsManager>
    {
        
        private Presentation mPresentation;

        /// <summary>
        /// Gets the <see cref="Presentation"/> associated with <c>this</c>
        /// </summary>
        /// <returns>The owning <see cref="Presentation"/></returns>
        public Presentation Presentation
        {
            get
            {
                return mPresentation;
            }
        }

        public ChannelsManager(Presentation pres)
        {
            mPresentation = pres;
            
            mChannels = new Dictionary<string, Channel>();
        }


        /// <summary>
        /// A dictionary of the <see cref="Channel"/>s managed by the manager, sorted by their uid
        /// </summary>
        private IDictionary<string, Channel> mChannels;


        #region ChannelsManager Members

        /// <summary>
        /// Gets the <see cref="ChannelsManager"/> associated with <c>this</c>.
        /// Convenience for <c>getPresentation().getChannelFactory()</c>
        /// </summary>
        /// <returns>The <see cref="ChannelsManager"/></returns>
        public ChannelFactory ChannelFactory
        {
            get { return Presentation.ChannelFactory; }
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
        public void AddChannel(Channel channel)
        {
            AddChannel(channel, GetNewId());
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
        public void AddChannel(Channel channel, string uid)
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

        private string GetNewId()
        {
            ulong i = 0;
            while (i < UInt64.MaxValue)
            {
                string newId = String.Format(
                    "CHID{0:0000}", i);
                if (!mChannels.ContainsKey(newId)) return newId;
                i++;
            }
            throw new OverflowException("YOU HAVE WAY TOO MANY CHANNELS!!!");
        }

        /// <summary>
        /// Removes an <see cref="Channel"/> from the manager
        /// </summary>
        /// <param name="channel">The <see cref="Channel"/> to remove</param>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref localName="channel"/> is null
        /// </exception>
        /// <exception cref="exception.ChannelDoesNotExistException">
        /// Thrown when <paramref localName="channel"/> is not in the managers list of channels
        /// </exception>
        public void RemoveChannel(Channel channel)
        {
            RemoveChannel(GetUidOfChannel(channel));
        }


        /// <summary>
        /// Removes an <see cref="Channel"/> from the manager by uid
        /// </summary>
        /// <param name="uid">The uid of the <see cref="Channel"/> to remove</param>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref localName="uid"/> is null
        /// </exception>
        /// <exception cref="exception.MethodParameterIsEmptyStringException">
        /// Thrown when <paramref localName="uid"/> is an empty string
        /// </exception>
        /// <exception cref="exception.ChannelDoesNotExistException">
        /// Thrown when <paramref localName="uid"/> is the uid of any managed channel
        /// </exception>
        public void RemoveChannel(string uid)
        {
            Channel channel = GetChannel(uid);
            ClearChannelTreeNodeVisitor clChVisitor = new ClearChannelTreeNodeVisitor(channel);
            Presentation.RootNode.AcceptDepthFirst(clChVisitor);
            mChannels.Remove(uid);
        }

        /// <summary>
        /// Gets a lists of the <see cref="Channel"/>s managed by the <see cref="ChannelsManager"/>
        /// </summary>
        /// <returns>The list</returns>
        public List<Channel> ListOfChannels
        {
            get { return new List<Channel>(mChannels.Values); }
        }

        /// <summary>
        /// Gets a list of the uids of <see cref="Channel"/>s managed by the <see cref="ChannelsManager"/>
        /// </summary>
        /// <returns>The list</returns>
        public List<string> ListOfUids
        {
            get { return new List<string>(mChannels.Keys); }
        }

        /// <summary>
        /// Gets the <see cref="Channel"/> with a given xuk uid
        /// </summary>
        /// <param name="uid">The given xuk uid</param>
        /// <returns>The <see cref="Channel"/> with the given xuk uid</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="uid"/> is <c>null</c>
        /// </exception>
        /// <exception cref="exception.MethodParameterIsEmptyStringException">
        /// Thrown when <paramref name="uid"/> is an empty string
        /// </exception>
        /// <exception cref="exception.ChannelDoesNotExistException">
        /// Thrown when <c>this</c> does not manage a <see cref="Channel"/> with the given xuk uid
        /// </exception>
        public Channel GetChannel(string uid)
        {
            if (uid == null)
            {
                throw new exception.MethodParameterIsNullException("Can not get a Channel with null uid");
            }
            if (uid == "")
            {
                throw new exception.MethodParameterIsEmptyStringException("Can not get a Channel with empty uid");
            }
            if (!mChannels.Keys.Contains(uid))
            {
                throw new exception.ChannelDoesNotExistException(String.Format(
                                                                     "The channels manager does not manage a channel with xuk uid {0}",
                                                                     uid));
            }
            return mChannels[uid];
        }


        /// <summary>
        /// Gets the Xuk id of a given channel
        /// </summary>
        /// <param name="ch">The given channel</param>
        /// <returns>The Xuk uid of the given channel</returns>
        /// <exception cref="exception.ChannelDoesNotExistException">
        /// Thrown when the given channel is not managed by <c>this</c>
        /// </exception>
        public string GetUidOfChannel(Channel ch)
        {
		if (ch == null)
            {
                throw new exception.MethodParameterIsNullException("channel parameter is null");
            }
	    
            foreach (string Id in mChannels.Keys)
            {
                if (mChannels[Id] == ch)
                {
                    return Id;
                }
            }
            throw new exception.ChannelDoesNotExistException("The given channel is not managed by this");
        }
	public void SetUidOfChannel(Channel ch, string uid)
        {
		if (ch == null)
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
        
            foreach (string Id in mChannels.Keys)
            {
                if (mChannels[Id] == ch)
                {
                mChannels.Remove(Id);
                mChannels.Add(uid, ch);
                return;
                }
            }
            throw new exception.ChannelDoesNotExistException("The given channel is not managed by this");
    }

        /// <summary>
        /// Removes all <see cref="Channel"/>s from the manager
        /// </summary>
        public void ClearChannels()
        {
            foreach (Channel ch in ListOfChannels)
            {
                RemoveChannel(ch);
            }
        }


        /// <summary>
        /// this is a helper function for getting one or more channels by its localName
        /// </summary>
        /// <param name="channelName">The localName of the channel to get</param>
        /// <returns>An array of the </returns>
        public List<Channel> GetChannelsByName(string channelName)
        {
            List<Channel> res = new List<Channel>();
            foreach (Channel ch in mChannels.Values)
            {
                if (ch.Name == channelName) res.Add(ch);
            }
            return res;
        }


        /// <summary>
        /// Determines if the manager manages a <see cref="Channel"/> with a given uid
        /// </summary>
        /// <param name="uid">The given uid</param>
        /// <returns>
        /// A <see cref="bool"/> indicating if the manager manages a <see cref="Channel"/> with the given uid
        /// </returns>
        public bool IsManagerOf(string uid)
        {
            return mChannels.ContainsKey(uid);
        }

        #endregion

        #region IXukAble Members

        /// <summary>
        /// Clears the <see cref="ChannelsManager"/>, disassociating any <see cref="Channel"/>s
        /// </summary>
        protected override void Clear()
        {
            mChannels.Clear();
            base.Clear();
        }

        /// <summary>
        /// Reads a child of a ChannelsManager xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukInChild(XmlReader source, ProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == XUK_NS && source.LocalName == "mChannels")
            {
                readItem = true;
                if (!source.IsEmptyElement)
                {
                    while (source.Read())
                    {
                        if (source.NodeType == XmlNodeType.Element)
                        {
                            if (source.LocalName == "mChannelItem" && source.NamespaceURI == XUK_NS)
                            {
                                XukInChannelItem(source, handler);
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
            if (!readItem) base.XukInChild(source, handler);
        }

        private void XukInChannelItem(XmlReader source, ProgressHandler handler)
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
                        Channel newCh = ChannelFactory.Create(source.LocalName, source.NamespaceURI);
                        if (newCh != null)
                        {
                            try
                            {
                                SetUidOfChannel(newCh, uid);
                            }
                            catch (exception.CheckedException e)
                            {
                                throw new exception.XukException(
                                    String.Format("Could not add Xuked In channel: {0}", e.Message),
                                    e);
                            }
                            newCh.XukIn(source, handler);
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
        /// Write the child elements of a ChannelsManager element.
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, ProgressHandler handler)
        {
            List<string> uids = ListOfUids;
            if (uids.Count > 0)
            {
                destination.WriteStartElement("mChannels");
                foreach (string uid in uids)
                {
                    destination.WriteStartElement("mChannelItem");
                    destination.WriteAttributeString("uid", uid);
                    GetChannel(uid).XukOut(destination, baseUri, handler);
                    destination.WriteEndElement();
                }
                destination.WriteEndElement();
            }
            base.XukOutChildren(destination, baseUri, handler);
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
            List<string> thisUids = ListOfUids;
            List<string> otherUids = other.ListOfUids;
            if (thisUids.Count != otherUids.Count) return false;
            foreach (string uid in thisUids)
            {
                if (!otherUids.Contains(uid)) return false;
                if (!GetChannel(uid).ValueEquals(other.GetChannel(uid))) return false;
            }
            return true;
        }

        #endregion
    }
}