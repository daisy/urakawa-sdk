using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;

namespace urakawa.property.channel
{
    /// <summary>
    /// A <see cref="Channel"/> that only accepts <see cref="ITextMedia"/>
    /// </summary>
    public class TextChannel : Channel
    {
        internal TextChannel(ChannelsManager chMgr) : base(chMgr)
        {
        }

        /// <summary>
        /// Determines if a given <see cref="IMedia"/> can be accepted by the channel,
        /// which it can if it implements interface <see cref="ITextMedia"/>
        /// </summary>
        /// <param name="m">The given media</param>
        /// <returns>A <see cref="bool"/> indicating if the given media can be accepted</returns>
        public override bool CanAccept(urakawa.media.IMedia m)
        {
            if (!base.CanAccept(m)) return false;
            if (m is ITextMedia) return true;
            if (m is SequenceMedia)
            {
                foreach (IMedia sm in ((SequenceMedia) m).ListOfItems)
                {
                    if (!(sm is ITextMedia)) return false;
                }
                return true;
            }
            return false;
        }
    }
}