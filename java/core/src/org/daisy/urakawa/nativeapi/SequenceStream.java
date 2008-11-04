package org.daisy.urakawa.nativeapi;

import java.io.IOException;
import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Represents an ordered list of Streams. This class should be replaced by an
 * equivalent IStream API in the implementing language. The methods exposed here
 * mimic the System.IO.Stream C# API.
 * 
 * @stereotype Language-Dependent
 */
public class SequenceStream implements IStream
{
    private List<IStream> mSources;
    private int mCurrentIndex;

    /**
     * @param ss
     * @throws MethodParameterIsNullException
     */
    public SequenceStream(List<IStream> ss)
            throws MethodParameterIsNullException
    {
        if (ss == null)
        {
            throw new MethodParameterIsNullException();
        }
        mSources = new LinkedList<IStream>(ss);
        if (mSources.size() == 0)
        {
            throw new MethodParameterIsNullException();
        }
        mCurrentIndex = 0;
        // mSources.get(0).Seek(0, SeekOrigin.Begin);
    }

    /**
     * @return int
     */
    public int getLength()
    {
        int len = 0;
        for (IStream subS : mSources)
        {
            len += subS.getLength();
        }
        return len;
    }

    public int getPosition()
    {
        return getBytesBeforeIndex(mCurrentIndex)
                + mSources.get(mCurrentIndex).getPosition();
    }

    public void setPosition(int pos)
    {
        mCurrentIndex = 0;
        int bytesBefore = 0;
        while (mCurrentIndex < mSources.size())
        {
            if (pos < bytesBefore + mSources.get(mCurrentIndex).getLength())
            {
                mSources.get(mCurrentIndex).setPosition(pos - bytesBefore);
                return;
            }
            bytesBefore += mSources.get(mCurrentIndex).getLength();
            mCurrentIndex++;
        }
        mCurrentIndex = mSources.size() - 1;
        mSources.get(mCurrentIndex).setPosition(
                mSources.get(mCurrentIndex).getLength());
    }

    private int getBytesBeforeIndex(int index)
    {
        int i = 0, index_ = index;
        if (index_ >= mSources.size())
            index_ = mSources.size() - 1;
        int bytesBefore = 0;
        while (i < index_)
        {
            bytesBefore += mSources.get(i).getLength();
            i++;
        }
        return bytesBefore;
    }

    public int read(byte[] buffer, int offset, int count) throws IOException
    {
        int offset_ = offset;
        int count_ = count;
        if (count_ == 0)
            return 0;
        int totalBytesRead = 0;
        int bytesRead = 0;
        while (true)
        {
            if (mSources.get(mCurrentIndex).getPosition() < mSources.get(
                    mCurrentIndex).getLength())
            {
                bytesRead = mSources.get(mCurrentIndex).read(buffer, offset_,
                        count_);
            }
            else
            {
                bytesRead = 0;
            }
            totalBytesRead += bytesRead;
            count_ -= bytesRead;
            offset_ += bytesRead;
            if (count_ == 0)
                break;
            if (mCurrentIndex + 1 < mSources.size())
            {
                mCurrentIndex++;
                mSources.get(mCurrentIndex).setPosition(0);
            }
            else
            {
                break;
            }
        }
        return totalBytesRead;
    }

    public void close() throws IOException
    {
        for (IStream subS : mSources)
        {
            subS.close();
        }
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
