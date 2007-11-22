using System;
using System.Collections.Generic;
using System.Text;
using urakawa.property.channel;

namespace urakawa.events
{
	public class ChannelsPropertyEventArgs : PropertyEventArgs
	{
		public ChannelsPropertyEventArgs(ChannelsProperty src)
			: base(src)
		{
			SourceChannelsProperty = src;
		}

		public readonly ChannelsProperty SourceChannelsProperty;
	}
}
