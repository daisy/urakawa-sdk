using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;

namespace urakawa.property.channel
{
	/// <summary>
	/// A <see cref="Channel"/> that only accepts <see cref="IAudioMedia"/> implementing <see cref="IMedia"/>
	/// </summary>
	public class AudioChannel : Channel
	{
		internal AudioChannel(ChannelsManager chMgr) : base(chMgr) {}

		/// <summary>
		/// Determines if a given <see cref="IMedia"/> can be accepted by the channel,
		/// which it can if it implements interface <see cref="IAudioMedia"/>
		/// </summary>
		/// <param name="m">The given media</param>
		/// <returns>A <see cref="bool"/> indicating if the given media can be accepted</returns>
		public override bool canAccept(urakawa.media.IMedia m)
		{
			if (!base.canAccept(m)) return false;
			if (m is IAudioMedia) return true;
			return false;
		}
	}
}
