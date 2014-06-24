using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.media;
using urakawa.core;
using urakawa.progress;
using urakawa.property;
using urakawa.xuk;

namespace urakawa.property.channel
{
    [XukNameUglyPrettyAttribute("cP", "ChannelsProperty")]
    public class ChannelsProperty : Property
    {
        #region Event related members

        /// <summary>
        /// Event fired after a <see cref="Media"/> is mapped to a <see cref="Channel"/>
        /// </summary>
        public event EventHandler<urakawa.events.property.channel.ChannelMediaMapEventArgs> ChannelMediaMapOccured;

        /// <summary>
        /// Fires the <see cref="ChannelMediaMapOccured"/>
        /// </summary>
        /// <param name="src">The source, that is the <see cref="ChannelsProperty"/> at which the mapping occured</param>
        /// <param name="destChannel">The destination <see cref="Channel"/> of the mapping</param>
        /// <param name="mappedMedia">The <see cref="Media"/> that is now mapped to the <see cref="Channel"/> - may be <c>null</c></param>
        /// <param name="prevMedia">The <see cref="Media"/> was mapped to the <see cref="Channel"/> before - may be <c>null</c></param>
        protected void NotifyChannelMediaMapOccured(ChannelsProperty src, Channel destChannel, Media mappedMedia,
                                                    Media prevMedia)
        {
            EventHandler<urakawa.events.property.channel.ChannelMediaMapEventArgs> d = ChannelMediaMapOccured;
            if (d != null)
                d(this,
                  new urakawa.events.property.channel.ChannelMediaMapEventArgs(src, destChannel, mappedMedia, prevMedia));
        }

        private void this_ChannelMediaMapOccured(object sender,
                                                 urakawa.events.property.channel.ChannelMediaMapEventArgs e)
        {
            if (e.MappedMedia != null)
                e.MappedMedia.Changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(MappedMedia_changed);
            if (e.PreviousMedia != null)
                e.PreviousMedia.Changed -=
                    new EventHandler<urakawa.events.DataModelChangedEventArgs>(MappedMedia_changed);
            NotifyChanged(e);
        }

        private void MappedMedia_changed(object sender, urakawa.events.DataModelChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        #endregion

        private Dictionary<Channel, Media> mMapChannelToMediaObject = new Dictionary<Channel, Media>();

        /// <summary>
        /// Default constructor - for system use only, 
        /// <see cref="Property"/>s should only be created via. the <see cref="PropertyFactory"/>
        /// </summary>
        public ChannelsProperty()
        {
            ChannelMediaMapOccured += new EventHandler<urakawa.events.property.channel.ChannelMediaMapEventArgs>(this_ChannelMediaMapOccured);
        }

        /// <summary>
        /// Tests if the channels property can be added to a given potential owning <see cref="TreeNode"/>, 
        /// which it can if the potential new owner does not already have a channels property
        /// </summary>
        /// <param name="potentialOwner">The potential new owner</param>
        /// <returns>A <see cref="bool"/> indicating if the property can be added</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="potentialOwner"/> is <c>null</c>
        /// </exception>
        public override bool CanBeAddedTo(TreeNode potentialOwner)
        {
            if (!base.CanBeAddedTo(potentialOwner)) return false;
            if (potentialOwner.HasProperties(this.GetType())) return false;
            return true;
        }

        /// <summary>
        /// Retrieves the <see cref="Media"/> of a given <see cref="Channel"/>
        /// </summary>
        /// <param name="channel">The given <see cref="Channel"/></param>
        /// <returns>The <see cref="Media"/> associated with the given channel, 
        /// <c>null</c> if no <see cref="Media"/> is associated</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref localName="channel"/> is null
        /// </exception>
        /// <exception cref="exception.ChannelDoesNotExistException">
        /// Thrown when <paramref localName="channel"/> is not managed by the associated <see cref="ChannelsManager"/>
        /// </exception>
        public Media GetMedia(Channel channel)
        {
            if (channel == null)
            {
                throw new exception.MethodParameterIsNullException(
                    "channel parameter is null");
            }
            if (Presentation.ChannelsManager.ManagedObjects.IndexOf(channel) == -1)
            {
                throw new exception.ChannelDoesNotExistException(
                    "The given channel is not managed by the ChannelManager associated with the ChannelsProperty");
            }

            Media obj;
            mMapChannelToMediaObject.TryGetValue(channel, out obj);
            return obj;

            //if (!mMapChannelToMediaObject.ContainsKey(channel)) return null;
            //return (Media) mMapChannelToMediaObject[channel];
        }

        /// <summary>
        /// Associates a given <see cref="Media"/> with a given <see cref="Channel"/>
        /// </summary>
        /// <param name="channel">The given <see cref="Channel"/></param>
        /// <param name="media">The given <see cref="Media"/>, 
        /// pass <c>null</c> if you want to remove <see cref="Media"/>
        /// from the given <see cref="Channel"/></param>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when parameters <paramref localName="channel"/> is null
        /// </exception>
        /// <exception cref="exception.ChannelDoesNotExistException">
        /// Thrown when <paramref localName="channel"/> is not managed by the associated <see cref="ChannelsManager"/>
        /// </exception>
        /// <exception cref="exception.MediaNotAcceptable">
        /// Thrown when <paramref localName="channel"/> does not accept the given <see cref="Media"/>,
        /// see <see cref="Channel.CanAccept"/> for more information.
        /// </exception>
        public void SetMedia(Channel channel, Media media)
        {
            if (channel == null)
            {
                throw new exception.MethodParameterIsNullException(
                    "channel parameter is null");
            }
            if (Presentation.ChannelsManager.ManagedObjects.IndexOf(channel) == -1)
            {
                throw new exception.ChannelDoesNotExistException(
                    "The given channel is not managed by the ChannelManager associated with the ChannelsProperty");
            }
            if (media != null)
            {
                if (!channel.CanAccept(media))
                {
                    throw new exception.MediaNotAcceptable(
                        "The given media type is not supported by the given channel");
                }
            }
            Media prevMedia = null;

            Media obj;
            mMapChannelToMediaObject.TryGetValue(channel, out obj);
            if (obj != null)
            {
                prevMedia = obj;
            }

            //if (mMapChannelToMediaObject.ContainsKey(channel)) prevMedia = mMapChannelToMediaObject[channel];

            mMapChannelToMediaObject[channel] = media;
            NotifyChannelMediaMapOccured(this, channel, media, prevMedia);
        }

        /// <summary>
        /// Gets the list of <see cref="Channel"/>s used by this instance of <see cref="ChannelsProperty"/>
        /// </summary>
        /// <returns>The list of used <see cref="Channel"/>s</returns>
        public IEnumerable<Channel> UsedChannels
        {
            get
            {
                //List<Channel> res = new List<Channel>();
                foreach (Channel ch in Presentation.ChannelsManager.ManagedObjects.ContentsAs_Enumerable)
                {
                    if (GetMedia(ch) != null)
                    {
                        //res.Add(ch);
                        yield return ch;
                    }
                }
                yield break;
                //return res;
            }
        }

        /// <summary>
        /// Creates a "deep" copy of the <see cref="ChannelsProperty"/> instance 
        /// - deep meaning that all associated <see cref="Media"/> are copies and not just referenced
        /// </summary>
        /// <returns>The deep copy</returns>
        /// <exception cref="exception.FactoryCannotCreateTypeException">
        /// TODO: Explain exception
        /// </exception>
        public new ChannelsProperty Copy()
        {
            return CopyProtected() as ChannelsProperty;
        }

        /// <summary>
        /// Creates a "deep" copy of the <see cref="ChannelsProperty"/> instance 
        /// - deep meaning that all associated are copies and not just referenced
        /// </summary>
        /// <returns>The deep copy</returns>
        /// <exception cref="exception.FactoryCannotCreateTypeException">
        /// TODO: Explain exception
        /// </exception>
        protected override Property CopyProtected()
        {
            ChannelsProperty theCopy = base.CopyProtected() as ChannelsProperty;
            if (theCopy == null)
            {
                throw new exception.FactoryCannotCreateTypeException(String.Format(
                                                                         "The property factory can not create a ChannelsProperty matching QName {0}:{1}",
                                                                         GetXukNamespace(),
                                                                         GetXukName()));
            }
            foreach (Channel ch in UsedChannels)
            {
                theCopy.SetMedia(ch, GetMedia(ch).Copy());
            }
            return theCopy;
        }

        /// <summary>
        /// Exports the channels property to a given destination <see cref="Presentation"/>, 
        /// including exports of any attachedx <see cref="Media"/>
        /// </summary>
        /// <param name="destPres">Thre destination presentation of the export</param>
        /// <returns>The exported channels property</returns>
        public new ChannelsProperty Export(Presentation destPres)
        {
            return ExportProtected(destPres) as ChannelsProperty;
        }

        /// <summary>
        /// Exports the channels property to a given destination <see cref="Presentation"/>, 
        /// including exports of any attachedx <see cref="Media"/>
        /// </summary>
        /// <param name="destPres">Thre destination presentation of the export</param>
        /// <returns>The exported channels property</returns>
        protected override Property ExportProtected(Presentation destPres)
        {
            ChannelsProperty chExport = base.ExportProtected(destPres) as ChannelsProperty;
            if (chExport == null)
            {
                throw new exception.OperationNotValidException(
                    "The ExportProtected method of the base class unexpectedly did not return a ChannelsProperty");
            }
            foreach (Channel ch in UsedChannels)
            {
                Channel exportDestCh = null;
                foreach (Channel dCh in destPres.ChannelsManager.ManagedObjects.ContentsAs_Enumerable)
                {
                    if (ch.IsEquivalentTo(dCh))
                    {
                        exportDestCh = dCh;
                        break;
                    }
                }
                if (exportDestCh == null)
                {
                    exportDestCh = ch.Export(destPres);
                }
                chExport.SetMedia(exportDestCh, GetMedia(ch).Export(destPres));
            }
            return chExport;
        }

        #region IXukAble Members

        /// <summary>
        /// Reads a child of a ChannelsProperty xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <returns>A <see cref="bool"/> indicating if the child was succefully read</returns>
        /// <param name="handler">The handler for progress</param>
        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == XukAble.XUK_NS)
            {
                readItem = true;
                if (PrettyFormat && source.LocalName == XukStrings.ChannelMappings)
                {
                    XukInChannelMappings(source, handler);
                }
                else if (!PrettyFormat)
                {
                    XukInChannelMapping(source, handler);
                }
                else
                {
                    readItem = false;
                }
            }
            if (!(readItem || source.IsEmptyElement))
            {
                source.ReadSubtree().Close(); //Read past invalid MediaDataItem element
            }
        }

        /// <summary>
        /// Helper method to to Xuk in mChannelMappings element
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handler">The handler for progress</param>
        private void XukInChannelMappings(XmlReader source, IProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        if (source.LocalName == XukStrings.ChannelMapping && source.NamespaceURI == XukAble.XUK_NS)
                        {
                            XukInChannelMapping(source, handler);
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

        /// <summary>
        /// helper method which is called once per mChannelMapping element
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handler">The handler for progress</param>
        private void XukInChannelMapping(XmlReader source, IProgressHandler handler)
        {
            string channelRef = source.GetAttribute(XukStrings.Channel);
            while (source.Read())
            {
                if (source.NodeType == XmlNodeType.Element)
                {
                    Media newMedia = Presentation.MediaFactory.Create(source.LocalName, source.NamespaceURI);
                    if (newMedia != null)
                    {
                        //if (!Presentation.ChannelsManager.IsManagerOf(channelRef))
                        //{
                        //    throw new exception.IsNotManagerOfException(
                        //        String.Format("Found no channel with uid {0}", channelRef));
                        //}
                        Channel channel = Presentation.ChannelsManager.GetManagedObject(channelRef);
                        newMedia.XukIn(source, handler);
                        SetMedia(channel, newMedia);
                    }
                    else if (!source.IsEmptyElement)
                    {
                        //Read past unrecognized element
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

        /// <summary>
        /// Write the child elements of a ChannelsProperty element.
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            base.XukOutChildren(destination, baseUri, handler);

            if (PrettyFormat)
            {
                destination.WriteStartElement(XukStrings.ChannelMappings, XukAble.XUK_NS);
            }
            foreach (Channel channel in UsedChannels)
            {
                destination.WriteStartElement(XukStrings.ChannelMapping, XukAble.XUK_NS);
                destination.WriteAttributeString(XukStrings.Channel, channel.Uid);
                Media media = GetMedia(channel);
                if (media == null)
                {
                    throw new exception.XukException(
                        String.Format("Found no Media associated with channel {0}", channel.Uid));
                }
                media.XukOut(destination, baseUri, handler);

                destination.WriteEndElement();
            }
            if (PrettyFormat)
            {
                destination.WriteEndElement();
            }
        }

        #endregion

        #region IValueEquatable<Property> Members

        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            ChannelsProperty otherz = other as ChannelsProperty;
            if (otherz == null)
            {
                return false;
            }

            IList<Channel> chs = new List<Channel>(UsedChannels);
            IList<Channel> otherChs = new List<Channel>(otherz.UsedChannels);
            if (chs.Count != otherChs.Count)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            foreach (Channel ch in chs)
            {
                Channel otherCh = null;
                foreach (Channel ch2 in otherChs)
                {
                    if (ch.Uid == ch2.Uid)
                    {
                        otherCh = ch2;
                        break;
                    }
                }
                if (otherCh == null)
                {
                    //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                    return false;
                }
                if (!GetMedia(ch).ValueEquals(otherz.GetMedia(otherCh)))
                {
                    //System.Diagnostics.Debug.Fail("! ValueEquals !");
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}