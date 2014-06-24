using System;
using System.Collections.Generic;
using System.Text;
using urakawa.property.channel;

namespace urakawa.events.property.channel
{
    /// <summary>
    /// Base class arguments of <see cref="ChannelsProperty"/> sourced events
    /// </summary>
    public class ChannelsPropertyEventArgs : PropertyEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="ChannelsProperty"/> of the event
        /// </summary>
        /// <param name="src">The source <see cref="ChannelsProperty"/> of the event</param>
        public ChannelsPropertyEventArgs(ChannelsProperty src)
            : base(src)
        {
            SourceChannelsProperty = src;
        }

        /// <summary>
        /// The source <see cref="ChannelsProperty"/> of the event
        /// </summary>
        public readonly ChannelsProperty SourceChannelsProperty;
    }
}