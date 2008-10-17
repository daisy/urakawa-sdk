package org.daisy.urakawa.property.channel;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.AbstractAudioMedia;
import org.daisy.urakawa.media.IMedia;
import org.daisy.urakawa.media.ISequenceMedia;

/**
 *
 */
public class AudioChannel extends Channel
{
    @Override
    public boolean canAccept(IMedia m) throws MethodParameterIsNullException
    {
        if (!super.canAccept(m))
            return false;
        if (m instanceof AbstractAudioMedia)
            return true;
        if (m instanceof ISequenceMedia)
        {
            for (IMedia sm : ((ISequenceMedia) m).getListOfItems())
            {
                if (!(sm instanceof AbstractAudioMedia))
                    return false;
            }
            return true;
        }
        return false;
    }
}
