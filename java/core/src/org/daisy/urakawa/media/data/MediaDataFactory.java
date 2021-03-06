package org.daisy.urakawa.media.data;

import org.daisy.urakawa.GenericWithPresentationFactory;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.audio.codec.WavAudioMediaData;

/**
 * Extension of the generic factory to handle one or more specific types derived
 * from the base specified class, in order to provide convenience create()
 * methods.
 * 
 * @xhas - - 1 org.daisy.urakawa.Presentation
 * @depend - Create - org.daisy.urakawa.media.data.audio.codec.WavAudioMediaData
 */
public final class MediaDataFactory extends
        GenericWithPresentationFactory<WavAudioMediaData>
{
    /**
     * @param pres
     * @throws MethodParameterIsNullException
     */
    public MediaDataFactory(Presentation pres)
            throws MethodParameterIsNullException
    {
        super(pres);
    }

    /**
     * @hidden
     */
    @Override
    protected void initializeInstance(WavAudioMediaData instance)
    {
        super.initializeInstance(instance);
        try
        {
            getPresentation().getMediaDataManager().addMediaData(instance);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    /**
     * @return
     */
    public WavAudioMediaData createWavAudioMediaData()
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
