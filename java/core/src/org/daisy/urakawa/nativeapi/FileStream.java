package org.daisy.urakawa.nativeapi;

import java.io.IOException;

/**
 * This is a wrapper for a IStream based on an File. This class should be
 * replaced by an equivalent IStream API in the implementing language. The
 * methods exposed here mimic the System.IO.Stream C# API.
 * 
 * @stereotype Language-Dependent
 */
public class FileStream implements IStream
{
    /**
     * @param path
     */
    public FileStream(String path)
    {
        /**
         * To implement.
         */
    }

    public void close() throws IOException
    {
        /**
         * To implement.
         */
    }

    public int getLength()
    {
        return 0;
    }

    public int getPosition()
    {
        return 0;
    }

    public int read(byte[] buffer, int offset, int count) throws IOException
    {
        return 0;
    }

    public void setPosition(int pos)
    {
        /**
         * To implement.
         */
    }

    public void seek(int n)
    {
        /**
         * To implement.
         */
    }

    public void write(byte[] buffer, int offset, int count) throws IOException
    {
        /**
         * To implement.
         */
    }

    public byte readByte()
    {
        return 0;
    }

    public byte[] readBytes(int length)
    {
        return null;
    }
}
