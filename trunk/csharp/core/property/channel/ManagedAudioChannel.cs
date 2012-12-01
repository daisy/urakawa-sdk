using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;
using urakawa.media.data.audio;

namespace urakawa.property.channel
{
    ///// <summary>
    ///// A <see cref="Channel"/> that only accepts <see cref="ManagedAudioMedia"/>
    ///// </summary>
    //public class ManagedAudioChannel : AudioChannel
    //{
    //    /// <summary>
    //    /// Determines if a given <see cref="Media"/> can be accepted by the channel,
    //    /// which it can if it is a <see cref="ManagedAudioMedia"/>
    //    /// </summary>
    //    /// <param name="m">The given media</param>
    //    /// <returns>A <see cref="bool"/> indicating if the given media can be accepted</returns>
    //    public override bool CanAccept(urakawa.media.Media m)
    //    {
    //        if (!base.CanAccept(m)) return false;
    //        if (!(m is ManagedAudioMedia)) return false;
    //        return true;
    //    }
    //}
}