package org.daisy.urakawa.media.data.audio;

import java.io.IOException;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.InvalidDataFormatException;
import org.daisy.urakawa.media.timing.ITimeDelta;
import org.daisy.urakawa.nativeapi.IStream;

/**
 * An extension of IPCMFormatInfo with support for RIFF WAV header, and actual
 * data length.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface IPCMDataInfo extends IPCMFormatInfo
{
    /**
     * @return the data length
     */
    public int getDataLength();

    /**
     * sets the data length
     * 
     * @param newValue
     */
    public void setDataLength(int newValue);

    /**
     * @return the duration
     */
    public ITimeDelta getDuration();

    /**
     * parses the RIFF header from the given stream
     * 
     * @param riffWaveStream
     * @return the data info
     * @throws InvalidDataFormatException
     * @throws MethodParameterIsNullException
     * @throws IOException
     */
    public IPCMDataInfo parseRiffWaveHeader(IStream riffWaveStream)
            throws InvalidDataFormatException, MethodParameterIsNullException,
            IOException;

    /**
     * Compares 2 streams for equality
     * 
     * @param thisData
     * @param otherdata
     * @param length
     * @return true or false
     * @throws IOException
     */
    public boolean compareStreamData(IStream thisData, IStream otherdata,
            int length) throws IOException;

    /**
     * writes a RIFF header corresponding to this instance, to the given stream
     * 
     * @param output
     * @throws IOException
     * @throws MethodParameterIsNullException
     */
    public void writeRiffWaveHeader(IStream output) throws IOException,
            MethodParameterIsNullException;
}
