package org.daisy.urakawa.media.data.audio;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * Getting and Setting the media data.
 * </p>
 * <p>
 * When using this interface (e.g. by using "extend" or "implement"), the host
 * object type should explicitly declare the UML aggregation or composition
 * relationship, in order to clearly state the rules for object instance
 * ownership.
 * <p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface IWithAudioMediaData
{
    /**
     * @return the data object. Cannot be null.
     */
    public IAudioMediaData getAudioMediaData();

    /**
     * @param data
     *        cannot be null
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @stereotype Initialize
     */
    public void setAudioMediaData(IAudioMediaData data)
            throws MethodParameterIsNullException;
}