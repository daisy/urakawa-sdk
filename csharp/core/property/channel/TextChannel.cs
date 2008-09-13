using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;

namespace urakawa.property.channel
{
    /// <summary>
    /// A <see cref="Channel"/> that only accepts <see cref="AbstractTextMedia"/>
    /// </summary>
    public class TextChannel : Channel
    {

        /// <summary>
        /// Determines if a given <see cref="Media"/> can be accepted by the channel,
        /// which it can if it implements interface <see cref="AbstractTextMedia"/>
        /// </summary>
        /// <param name="m">The given media</param>
        /// <returns>A <see cref="bool"/> indicating if the given media can be accepted</returns>
        public override bool CanAccept(urakawa.media.Media m)
        {
            if (!base.CanAccept(m)) return false;
            if (m is AbstractTextMedia) return true;
            if (m is SequenceMedia)
            {
                foreach (Media sm in ((SequenceMedia) m).ListOfItems)
                {
                    if (!(sm is AbstractTextMedia)) return false;
                }
                return true;
            }
            return false;
        }
    }
}