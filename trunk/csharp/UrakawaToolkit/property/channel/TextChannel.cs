using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;

namespace urakawa.property.channel
{
	/// <summary>
	/// <see cref="Channel"/> that
	/// </summary>
	public class TextChannel : Channel
	{
		internal TextChannel(ChannelsManager chMgr) : base(chMgr) { }

		/// <summary>
		/// Determines if a given <see cref="IMedia"/> can be accepted by the channel,
		/// which it can if it implements interface <see cref="ITextMedia"/>
		/// </summary>
		/// <param name="m">The given media</param>
		/// <returns>A <see cref="bool"/> indicating if the given media can be accepted</returns>
		public override bool canAccept(urakawa.media.IMedia m)
		{
			if (!base.canAccept(m)) return false;
			if (m is ITextMedia) return true;
			return false;
		}
	}
}
