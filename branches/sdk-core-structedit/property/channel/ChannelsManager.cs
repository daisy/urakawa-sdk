using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.progress;
using urakawa.xuk;

namespace urakawa.property.channel
{
    [XukNameUglyPrettyAttribute("chsMan", "ChannelsManager")]
    public sealed class ChannelsManager : XukAbleManager<Channel>
    {
        public ChannelsManager(Presentation pres) : base(pres, "CH")
        {
        }

        #region ChannelsManager Members


        public override void RemoveManagedObject(Channel channel)
        {
            ClearChannelTreeNodeVisitor clChVisitor = new ClearChannelTreeNodeVisitor(channel);
            Presentation.RootNode.AcceptDepthFirst(clChVisitor);

            base.RemoveManagedObject(channel);
        }

        public override bool CanAddManagedObject(Channel managedObject)
        {
            return true;
        }

        public bool HasChannel<T>() where T : Channel, new()
        {
            foreach (Channel ch in Presentation.ChannelsManager.ManagedObjects.ContentsAs_Enumerable)
            {
                if (ch is T)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasAudioChannel
        {
            get { return HasChannel<AudioChannel>(); }
        }
        public bool HasAudioXChannel
        {
            get { return HasChannel<AudioXChannel>(); }
        }
        public bool HasImageChannel
        {
            get { return HasChannel<ImageChannel>(); }
        }
        public bool HasTextChannel
        {
            get { return HasChannel<TextChannel>(); }
        }

        public T GetOrCreateChannel<T>() where T : Channel, new()
        {
            T channel = null;

            foreach (Channel ch in Presentation.ChannelsManager.ManagedObjects.ContentsAs_Enumerable)
            {
                if (ch is T)
                {
                    channel = ch as T;
                    break;
                }
            }
            if (channel == null)
            {
                channel = Presentation.ChannelFactory.Create<T>();
                //channel = Presentation.ChannelFactory.Create(typeof(T));
            }
            return channel;
        }

        public AudioChannel GetOrCreateAudioChannel()
        {
            return GetOrCreateChannel<AudioChannel>();
        }
        public AudioXChannel GetOrCreateAudioXChannel()
        {
            return GetOrCreateChannel<AudioXChannel>();
        }
        public TextChannel GetOrCreateTextChannel()
        {
            return GetOrCreateChannel<TextChannel>();
        }
        public ImageChannel GetOrCreateImageChannel()
        {
            return GetOrCreateChannel<ImageChannel>();
        }
        public VideoChannel GetOrCreateVideoChannel()
        {
            return GetOrCreateChannel<VideoChannel>();
        }

        /// <summary>
        /// this is a helper function for getting one or more channels by its localName
        /// </summary>
        /// <param name="channelName">The localName of the channel to get</param>
        /// <returns>An array of the </returns>
        public List<Channel> GetChannelsByName(string channelName)
        {
            List<Channel> res = new List<Channel>();
            foreach (Channel ch in ManagedObjects.ContentsAs_Enumerable)
            {
                if (ch.Name == channelName) res.Add(ch);
            }
            return res;
        }

        #endregion

        #region IXukAble Members

        /// <summary>
        /// Clears the <see cref="ChannelsManager"/>, disassociating any <see cref="Channel"/>s
        /// </summary>
        protected override void Clear()
        {
            foreach (Channel ch in ManagedObjects.ContentsAs_ListCopy)
            {
                ManagedObjects.Remove(ch);
            }
            base.Clear();
        }

        /// <summary>
        /// Reads a child of a ChannelsManager xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == XukAble.XUK_NS)
            {
                readItem = true;
                if (source.LocalName == XukStrings.Channels)
                {
                    XukInChannels(source, handler);
                }
                else if (true || !Presentation.Project.PrettyFormat
                    // && source.LocalName == XukStrings.ChannelItem
                    )
                {
                    //XukInChannelItem(source, handler);
                    XukInChannel(source, handler);
                }
                else
                {
                    readItem = false;
                }
            }
            if (!(readItem || source.IsEmptyElement))
            {
                source.ReadSubtree().Close();
            }
        }

        private void XukInChannels(XmlReader source, IProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        if (source.LocalName == XukStrings.ChannelItem && source.NamespaceURI == XukAble.XUK_NS)
                        {
                            XukInChannelItem(source, handler);
                        }
                        else
                        {
                            XukInChannel(source, handler);
                        }
                        //else if (!source.IsEmptyElement)
                        //    {
                        //        source.ReadSubtree().Close();
                        //    }
                    }
                    else if (source.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }
                    if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
                }
            }
        }

        private void XukInChannel(XmlReader source, IProgressHandler handler)
        {
            if (source.NodeType == XmlNodeType.Element)
            {
                Channel newCh = Presentation.ChannelFactory.Create_SkipManagerInitialization(source.LocalName, source.NamespaceURI);
                if (newCh != null)
                {
                    newCh.XukIn(source, handler);

                    //string uid = source.GetAttribute(XukStrings.Uid);
                    if (string.IsNullOrEmpty(newCh.Uid))
                    {
                        throw new exception.XukException("mChannelItem element has no uid attribute");
                    }
                    
                    Presentation.ChannelsManager.AddManagedObject_NoSafetyChecks(newCh, newCh.Uid);
                            
                    //if (IsManagerOf(newCh.Uid))
                    //{
                    //    if (GetManagedObject(newCh.Uid) != newCh)
                    //    {
                    //        throw new exception.XukException(
                    //            String.Format("Another MediaData exists in the manager with uid {0}", newCh.Uid));
                    //    }
                    //}
                    //else
                    //{
                    //    SetUidOfManagedObject(newCh, newCh.Uid);
                    //}
                }
                else if (!source.IsEmptyElement)
                {
                    source.ReadSubtree().Close();
                }
            }
        }

        private void XukInChannelItem(XmlReader source, IProgressHandler handler)
        {
            bool foundChannel = false;
            if (!source.IsEmptyElement)
            {
                string uid = XukAble.ReadXukAttribute(source, XukAble.Uid_NAME);

                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        Channel newCh = Presentation.ChannelFactory.Create_SkipManagerInitialization(source.LocalName, source.NamespaceURI);
                        if (newCh != null)
                        {
                            string uid_ = XukAble.ReadXukAttribute(source, XukAble.Uid_NAME);

                            newCh.XukIn(source, handler);

                            if (string.IsNullOrEmpty(uid_) && !string.IsNullOrEmpty(uid))
                            {
                                newCh.Uid = uid;
                            }
                            if (string.IsNullOrEmpty(newCh.Uid))
                            {
                                throw new exception.XukException("mChannelItem element has no uid attribute");
                            }

                            Presentation.ChannelsManager.AddManagedObject_NoSafetyChecks(newCh, newCh.Uid);
                            
                            //if (IsManagerOf(newCh.Uid))
                            //{
                            //    if (GetManagedObject(newCh.Uid) != newCh)
                            //    {
                            //        throw new exception.XukException(
                            //            String.Format("Another MediaData exists in the manager with uid {0}", newCh.Uid));
                            //    }
                            //}
                            //else
                            //{
                            //    SetUidOfManagedObject(newCh, newCh.Uid);
                            //}
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
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            if (ManagedObjects.Count > 0)
            {
                if (Presentation.Project.PrettyFormat)
                {
                    destination.WriteStartElement(XukStrings.Channels);
                }
                foreach (Channel ch in ManagedObjects.ContentsAs_Enumerable)
                {
                    if (false && Presentation.Project.PrettyFormat)
                    {
                        destination.WriteStartElement(XukStrings.ChannelItem);
                        //destination.WriteAttributeString(XukStrings.Uid, uid);
                    }

                    ch.XukOut(destination, baseUri, handler);

                    if (false && Presentation.Project.PrettyFormat)
                    {
                        destination.WriteEndElement();
                    }
                }
                if (Presentation.Project.PrettyFormat)
                {
                    destination.WriteEndElement();
                }
            }
            base.XukOutChildren(destination, baseUri, handler);
        }

        #endregion
    }
}