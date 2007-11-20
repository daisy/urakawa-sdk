using System;
using System.Collections.Generic;
using System.Text;
using urakawa.property.channel;

namespace urakawa.events
{
	public class ChannelsPropertyEvent : PropertyEventArgs
	{
		public ChannelsPropertyEvent(ChannelsProperty src)
			: base(src)
		{
			SourceChannelsProperty = src;
		}

		public readonly ChannelsProperty SourceChannelsProperty;
	}
}
