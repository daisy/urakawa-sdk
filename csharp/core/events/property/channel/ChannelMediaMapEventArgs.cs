using System;
using System.Collections.Generic;
using System.Text;
using urakawa.property.channel;
using urakawa.media;

namespace urakawa.events.property.channel
{
    /// <summary>
    /// Arguments for the <see cref="urakawa.property.channel.ChannelsProperty.channelMediaMapOccured"/> event
    /// </summary>
	public class ChannelMediaMapEventArgs : ChannelsPropertyEventArgs
	{
        /// <summary>
        /// Constructor setting the fields of the event
        /// </summary>
        /// <param name="src">The source of the event</param>
        /// <param name="destCh">The destination <see cref="urakawa.property.channel.Channel"/> of the mapping</param>
        /// <param name="mapdMedia">The <see cref="urakawa.media.IMedia"/> being mapped</param>
        /// <param name="prevMedia">The <see cref="urakawa.media.IMedia"/> previously mapped to the destination <see cref="urakawa.property.channel.Channel"/></param>
		public ChannelMediaMapEventArgs(ChannelsProperty src, Channel destCh, IMedia mapdMedia, IMedia prevMedia) : base(src)
		{
			DestinationChannel = destCh;
			MappedMedia = mapdMedia;
			PreviousMedia = prevMedia;
		}
        /// <summary>
        /// The destination <see cref="urakawa.property.channel.Channel"/> of the mapping
        /// </summary>
		public readonly Channel DestinationChannel;
        /// <summary>
        /// The <see cref="urakawa.media.IMedia"/> being mapped
        /// - may be <c>null</c>
        /// </summary>
		public readonly IMedia MappedMedia;
        /// <summary>
        /// The <see cref="urakawa.media.IMedia"/> previously mapped to the destination <see cref="urakawa.property.channel.Channel"/> 
        /// - may be <c>null</c>
        /// </summary>
		public readonly IMedia PreviousMedia;
	}
}
