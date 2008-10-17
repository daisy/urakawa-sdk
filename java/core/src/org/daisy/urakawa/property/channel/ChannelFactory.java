package org.daisy.urakawa.property.channel;

import org.daisy.urakawa.GenericWithPresentationFactory;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Extension of the generic factory to handle one or more specific types derived
 * from the base specified class, in order to provide convenience create()
 * methods.
 * 
 * @xhas - - 1 org.daisy.urakawa.Presentation
 * @depend - Create - org.daisy.urakawa.property.channel.Channel
 * @depend - Create - org.daisy.urakawa.property.channel.AudioChannel
 * @depend - Create - org.daisy.urakawa.property.channel.ManagedAudioChannel
 * @depend - Create - org.daisy.urakawa.property.channel.TextChannel
 */
public final class ChannelFactory extends GenericWithPresentationFactory<Channel>
{
    /**
     * @param pres
     * @throws MethodParameterIsNullException
     */
    public ChannelFactory(Presentation pres)
            throws MethodParameterIsNullException
    {
        super(pres);
    }

    /**
     * @return
     */
    public Channel createChannel()
    {
        try
        {
            return create(Channel.class);
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
    public AudioChannel createAudioChannel()
    {
        try
        {
            return create(AudioChannel.class);
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
    public ManagedAudioChannel createManagedAudioChannel()
    {
        try
        {
            return create(ManagedAudioChannel.class);
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
    public TextChannel createTextChannel()
    {
        try
        {
            return create(TextChannel.class);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }
}
