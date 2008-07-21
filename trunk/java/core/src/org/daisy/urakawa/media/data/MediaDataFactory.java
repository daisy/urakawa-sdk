package org.daisy.urakawa.media.data;

import org.daisy.urakawa.GenericFactory;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.audio.codec.WavAudioMediaData;

/**
 * Extension of the generic factory to handle one or more specific types derived
 * from the base specified class, in order to provide convenience create()
 * methods.
 * 
 * @depend - Create - org.daisy.urakawa.media.data.audio.codec.WavAudioMediaData
 */
public final class MediaDataFactory extends GenericFactory<WavAudioMediaData>
{
    /**
     * @return
     */
    public WavAudioMediaData create()
    {
        try
        {
            return create(WavAudioMediaData.class);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }
}
