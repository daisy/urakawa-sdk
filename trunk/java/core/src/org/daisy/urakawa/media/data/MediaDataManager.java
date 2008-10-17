package org.daisy.urakawa.media.data;

import java.net.URI;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.daisy.urakawa.IValueEquatable;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.IsAlreadyManagerOfException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.IsNotManagerOfException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.data.audio.IAudioMediaData;
import org.daisy.urakawa.media.data.audio.IPCMFormatInfo;
import org.daisy.urakawa.media.data.audio.PCMFormatInfo;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.xuk.AbstractXukAble;
import org.daisy.urakawa.xuk.IXukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * @depend - Composition 0..n org.daisy.urakawa.media.data.MediaData
 * @depend - Clone - org.daisy.urakawa.media.data.MediaData
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 */
public final class MediaDataManager extends AbstractXukAble implements
        IValueEquatable<MediaDataManager>
{
    private Presentation mPresentation;

    /**
     * @return the Presentation owner
     */
    public Presentation getPresentation()
    {
        return mPresentation;
    }

    private static final String DEFAULT_UID_PREFIX = "UID";
    private Map<String, IMediaData> mMediaDataDictionary = new HashMap<String, IMediaData>();
    private Map<IMediaData, String> mReverseLookupMediaDataDictionary = new HashMap<IMediaData, String>();
    private long mUidNo = 0;
    private String mUidPrefix = DEFAULT_UID_PREFIX;
    private IPCMFormatInfo mDefaultPCMFormat;
    private boolean mEnforceSinglePCMFormat;

    /**
     * @param pres
     * @throws MethodParameterIsNullException
     */
    public MediaDataManager(Presentation pres)
            throws MethodParameterIsNullException
    {
        if (pres == null)
        {
            throw new MethodParameterIsNullException();
        }
        mPresentation = pres;
        mDefaultPCMFormat = new PCMFormatInfo();
        mEnforceSinglePCMFormat = false;
    }

    /**
     * @hidden
     */
    private boolean isNewDefaultPCMFormatOk(IPCMFormatInfo newDefault)
            throws MethodParameterIsNullException
    {
        if (newDefault == null)
        {
            throw new MethodParameterIsNullException();
        }
        for (IMediaData md : getListOfMediaData())
        {
            if (md instanceof IAudioMediaData)
            {
                IAudioMediaData amd = (IAudioMediaData) md;
                try
                {
                    if (!amd.getPCMFormat().ValueEquals(newDefault))
                        return false;
                }
                catch (MethodParameterIsNullException e)
                {
                    // Should never happen
                    throw new RuntimeException("WTF ??!", e);
                }
            }
        }
        return true;
    }

    /**
     * @return
     */
    public IPCMFormatInfo getDefaultPCMFormat()
    {
        return mDefaultPCMFormat.copy();
    }

    /**
     * @param newDefault
     * @throws MethodParameterIsNullException
     * @throws InvalidDataFormatException
     */
    public void setDefaultPCMFormat(IPCMFormatInfo newDefault)
            throws MethodParameterIsNullException, InvalidDataFormatException
    {
        if (newDefault == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (!newDefault.ValueEquals(mDefaultPCMFormat))
        {
            if (getEnforceSinglePCMFormat())
            {
                if (!isNewDefaultPCMFormatOk(newDefault))
                {
                    throw new InvalidDataFormatException();
                }
            }
            mDefaultPCMFormat = newDefault.copy();
        }
    }

    /**
     * @param numberOfChannels
     * @throws MethodParameterIsOutOfBoundsException
     */
    public void setDefaultNumberOfChannels(short numberOfChannels)
            throws MethodParameterIsOutOfBoundsException
    {
        IPCMFormatInfo newFormat = getDefaultPCMFormat();
        newFormat.setNumberOfChannels(numberOfChannels);
        try
        {
            setDefaultPCMFormat(newFormat);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (InvalidDataFormatException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    /**
     * @param sampleRate
     * @throws MethodParameterIsOutOfBoundsException
     */
    public void setDefaultSampleRate(int sampleRate)
            throws MethodParameterIsOutOfBoundsException
    {
        IPCMFormatInfo newFormat = getDefaultPCMFormat();
        newFormat.setSampleRate(sampleRate);
        try
        {
            setDefaultPCMFormat(newFormat);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (InvalidDataFormatException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    /**
     * @param bitDepth
     * @throws MethodParameterIsOutOfBoundsException
     */
    public void setDefaultBitDepth(short bitDepth)
            throws MethodParameterIsOutOfBoundsException
    {
        IPCMFormatInfo newFormat = getDefaultPCMFormat();
        newFormat.setBitDepth(bitDepth);
        try
        {
            setDefaultPCMFormat(newFormat);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (InvalidDataFormatException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    /**
     * @param numberOfChannels
     * @param sampleRate
     * @param bitDepth
     * @throws MethodParameterIsOutOfBoundsException
     */
    public void setDefaultPCMFormat(short numberOfChannels, int sampleRate,
            short bitDepth) throws MethodParameterIsOutOfBoundsException
    {
        IPCMFormatInfo newDefault = new PCMFormatInfo();
        newDefault.setNumberOfChannels(numberOfChannels);
        newDefault.setSampleRate(sampleRate);
        newDefault.setBitDepth(bitDepth);
        try
        {
            setDefaultPCMFormat(newDefault);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (InvalidDataFormatException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    /**
     * @return
     */
    public boolean getEnforceSinglePCMFormat()
    {
        return mEnforceSinglePCMFormat;
    }

    /**
     * @param newValue
     * @throws InvalidDataFormatException
     */
    public void setEnforceSinglePCMFormat(boolean newValue)
            throws InvalidDataFormatException
    {
        if (newValue)
        {
            try
            {
                if (!isNewDefaultPCMFormatOk(getDefaultPCMFormat()))
                {
                    throw new InvalidDataFormatException();
                }
            }
            catch (MethodParameterIsNullException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
        mEnforceSinglePCMFormat = newValue;
    }

    /**
     * @param uid
     * @return
     * @throws MethodParameterIsNullException
     * @throws MethodParameterIsEmptyStringException
     */
    public IMediaData getMediaData(String uid)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException
    {
        if (uid == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (uid.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        if (mMediaDataDictionary.containsKey(uid))
        {
            return mMediaDataDictionary.get(uid);
        }
        return null;
    }

    /**
     * @param data
     * @return
     * @throws MethodParameterIsNullException
     * @throws IsNotManagerOfException
     */
    public String getUidOfMediaData(IMediaData data)
            throws MethodParameterIsNullException, IsNotManagerOfException
    {
        if (data == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (!mReverseLookupMediaDataDictionary.containsKey(data))
        {
            throw new IsNotManagerOfException();
        }
        return mReverseLookupMediaDataDictionary.get(data);
    }

    /**
     * @hidden
     */
    @SuppressWarnings("boxing")
    private String getNewUid()
    {
        while (true)
        {
            if (mUidNo < Integer.MAX_VALUE)
            {
                mUidNo++;
            }
            else
            {
                mUidPrefix += "X";
            }
            String key = String.format("{0}{1:00000000}", mUidPrefix, mUidNo);
            if (!mMediaDataDictionary.containsKey(key))
            {
                return key;
            }
        }
    }

    /**
     * @param data
     * @throws MethodParameterIsNullException
     */
    public void addMediaData(IMediaData data)
            throws MethodParameterIsNullException
    {
        if (data == null)
        {
            throw new MethodParameterIsNullException();
        }
        String uid = getNewUid();
        try
        {
            addMediaData(data, uid);
        }
        catch (IsAlreadyManagerOfException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (InvalidDataFormatException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (MethodParameterIsEmptyStringException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    protected void addMediaData(IMediaData data, String uid)
            throws IsAlreadyManagerOfException, InvalidDataFormatException,
            MethodParameterIsEmptyStringException,
            MethodParameterIsNullException
    {
        if (data == null || uid == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (uid.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        if (mMediaDataDictionary.containsKey(uid))
        {
            throw new IsAlreadyManagerOfException();
        }
        if (getEnforceSinglePCMFormat())
        {
            if (data instanceof IAudioMediaData)
            {
                IAudioMediaData amdata = (IAudioMediaData) data;
                try
                {
                    if (!amdata.getPCMFormat().ValueEquals(
                            getDefaultPCMFormat()))
                    {
                        throw new InvalidDataFormatException();
                    }
                }
                catch (MethodParameterIsNullException e)
                {
                    // Should never happen
                    throw new RuntimeException("WTF ??!", e);
                }
            }
        }
        mMediaDataDictionary.put(uid, data);
        mReverseLookupMediaDataDictionary.put(data, uid);
    }

    /**
     * @param data
     * @param uid
     * @throws MethodParameterIsEmptyStringException
     * @throws MethodParameterIsNullException
     */
    public void setDataMediaDataUid(IMediaData data, String uid)
            throws MethodParameterIsEmptyStringException,
            MethodParameterIsNullException
    {
        if (data == null || uid == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (uid.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        removeMediaData(data);
        try
        {
            addMediaData(data, uid);
        }
        catch (IsAlreadyManagerOfException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (InvalidDataFormatException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    /**
     * @param uid
     * @return
     * @throws MethodParameterIsNullException
     * @throws MethodParameterIsEmptyStringException
     */
    public boolean isManagerOf(String uid)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException
    {
        if (uid == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (uid.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        return mMediaDataDictionary.containsKey(uid);
    }

    /**
     * @param data
     * @throws MethodParameterIsNullException
     */
    public void removeMediaData(IMediaData data)
            throws MethodParameterIsNullException
    {
        if (data == null)
        {
            throw new MethodParameterIsNullException();
        }
        try
        {
            removeMediaData(getUidOfMediaData(data));
        }
        catch (IsNotManagerOfException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (MethodParameterIsEmptyStringException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    /**
     * @param uid
     * @throws MethodParameterIsNullException
     * @throws MethodParameterIsEmptyStringException
     */
    public void removeMediaData(String uid)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException
    {
        if (uid == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (uid.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        IMediaData data = getMediaData(uid);
        mMediaDataDictionary.remove(uid);
        mReverseLookupMediaDataDictionary.remove(data);
    }

    /**
     * @param data
     * @return
     * @throws MethodParameterIsNullException
     * @throws IsNotManagerOfException
     */
    public IMediaData copyMediaData(IMediaData data)
            throws MethodParameterIsNullException, IsNotManagerOfException
    {
        if (data == null)
        {
            throw new MethodParameterIsNullException();
        }
        try
        {
            if (data.getPresentation().getMediaDataManager() != this)
            {
                throw new IsNotManagerOfException();
            }
        }
        catch (IsNotInitializedException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        return data.copy();
    }

    /**
     * @param uid
     * @return
     * @throws MethodParameterIsNullException
     * @throws MethodParameterIsEmptyStringException
     * @throws IsNotManagerOfException
     */
    public IMediaData copyMediaData(String uid)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException, IsNotManagerOfException
    {
        if (uid == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (uid.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        IMediaData data = getMediaData(uid);
        if (data == null)
        {
            throw new IsNotManagerOfException();
        }
        return copyMediaData(data);
    }

    /**
     * @return
     */
    public List<IMediaData> getListOfMediaData()
    {
        return new LinkedList<IMediaData>(mMediaDataDictionary.values());
    }

    /**
     * @return
     */
    public List<String> getListOfUids()
    {
        return new LinkedList<String>(mMediaDataDictionary.keySet());
    }

    /**
     * @hidden
     */
    @Override
    protected void clear()
    {
        mMediaDataDictionary.clear();
        mReverseLookupMediaDataDictionary.clear();
        // super.clear();
    }

    /**
     * @hidden
     */
    @Override
    protected void xukInAttributes(IXmlDataReader source, IProgressHandler ph)
            throws MethodParameterIsNullException,
            XukDeserializationFailedException, ProgressCancelledException
    {
        if (source == null)
        {
            throw new MethodParameterIsNullException();
        }
        // To avoid event notification overhead, we bypass this:
        if (false && ph != null && ph.notifyProgress())
        {
            throw new ProgressCancelledException();
        }
        String attr = source.getAttribute("enforceSinglePCMFormat");
        if (attr == "true" || attr == "1")
        {
            try
            {
                setEnforceSinglePCMFormat(true);
            }
            catch (InvalidDataFormatException e)
            {
                throw new XukDeserializationFailedException();
            }
        }
        else
        {
            try
            {
                setEnforceSinglePCMFormat(false);
            }
            catch (InvalidDataFormatException e)
            {
                throw new XukDeserializationFailedException();
            }
        }
    }

    /**
     * @hidden
     */
    @Override
    protected void xukInChild(IXmlDataReader source, IProgressHandler ph)
            throws MethodParameterIsNullException,
            XukDeserializationFailedException, ProgressCancelledException
    {
        if (source == null)
        {
            throw new MethodParameterIsNullException();
        }
        // To avoid event notification overhead, we bypass this:
        if (false && ph != null && ph.notifyProgress())
        {
            throw new ProgressCancelledException();
        }
        boolean readItem = false;
        if (source.getNamespaceURI() == IXukAble.XUK_NS)
        {
            readItem = true;
            String str = source.getLocalName();
            if (str == "mDefaultPCMFormat")
            {
                xukInDefaultPCMFormat(source, ph);
            }
            else
                if (str == "mMediaData")
                {
                    xukInMediaData(source, ph);
                }
                else
                {
                    readItem = false;
                }
        }
        if (!readItem)
        {
            super.xukInChild(source, ph);
        }
    }

    /**
     * @hidden
     */
    private void xukInDefaultPCMFormat(IXmlDataReader source,
            IProgressHandler ph) throws MethodParameterIsNullException,
            XukDeserializationFailedException, ProgressCancelledException
    {
        if (source == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (ph != null && ph.notifyProgress())
        {
            throw new ProgressCancelledException();
        }
        if (!source.isEmptyElement())
        {
            while (source.read())
            {
                if (source.getNodeType() == IXmlDataReader.ELEMENT)
                {
                    if (source.getLocalName() == "PCMFormatInfo"
                            && source.getNamespaceURI() == IXukAble.XUK_NS)
                    {
                        IPCMFormatInfo newInfo = new PCMFormatInfo();
                        newInfo.xukIn(source, ph);
                        boolean enf = getEnforceSinglePCMFormat();
                        if (enf)
                            try
                            {
                                setEnforceSinglePCMFormat(false);
                            }
                            catch (InvalidDataFormatException e)
                            {
                                // Should never happen
                                throw new RuntimeException("WTF ??!", e);
                            }
                        try
                        {
                            setDefaultPCMFormat(newInfo);
                        }
                        catch (InvalidDataFormatException e)
                        {
                            // Should never happen
                            throw new RuntimeException("WTF ??!", e);
                        }
                        if (enf)
                            try
                            {
                                setEnforceSinglePCMFormat(true);
                            }
                            catch (InvalidDataFormatException e)
                            {
                                // Should never happen
                                throw new RuntimeException("WTF ??!", e);
                            }
                    }
                    else
                    {
                        super.xukInChild(source, ph);
                    }
                }
                else
                    if (source.getNodeType() == IXmlDataReader.END_ELEMENT)
                    {
                        break;
                    }
                if (source.isEOF())
                    throw new XukDeserializationFailedException();
            }
        }
    }

    /**
     * @hidden
     */
    private void xukInMediaData(IXmlDataReader source, IProgressHandler ph)
            throws MethodParameterIsNullException,
            XukDeserializationFailedException, ProgressCancelledException
    {
        if (source == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (ph != null && ph.notifyProgress())
        {
            throw new ProgressCancelledException();
        }
        if (!source.isEmptyElement())
        {
            while (source.read())
            {
                if (source.getNodeType() == IXmlDataReader.ELEMENT)
                {
                    if (source.getLocalName() == "mMediaDataItem"
                            && source.getNamespaceURI() == IXukAble.XUK_NS)
                    {
                        xukInMediaDataItem(source, ph);
                    }
                    else
                    {
                        super.xukInChild(source, ph);
                    }
                }
                else
                    if (source.getNodeType() == IXmlDataReader.END_ELEMENT)
                    {
                        break;
                    }
                if (source.isEOF())
                    throw new XukDeserializationFailedException();
            }
        }
    }

    /**
     * @hidden
     */
    private void xukInMediaDataItem(IXmlDataReader source, IProgressHandler ph)
            throws MethodParameterIsNullException,
            XukDeserializationFailedException, ProgressCancelledException
    {
        if (source == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (ph != null && ph.notifyProgress())
        {
            throw new ProgressCancelledException();
        }
        String uid = source.getAttribute("uid");
        IMediaData data = null;
        if (!source.isEmptyElement())
        {
            while (source.read())
            {
                if (source.getNodeType() == IXmlDataReader.ELEMENT)
                {
                    try
                    {
                        data = getPresentation().getMediaDataFactory()
                                .create(source.getLocalName(),
                                        source.getNamespaceURI());
                    }
                    catch (MethodParameterIsEmptyStringException e)
                    {
                        // Should never happen
                        throw new RuntimeException("WTF ??!", e);
                    }
                    if (data != null)
                    {
                        data.xukIn(source, ph);
                    }
                }
                else
                    if (source.getNodeType() == IXmlDataReader.END_ELEMENT)
                    {
                        break;
                    }
                if (source.isEOF())
                    throw new XukDeserializationFailedException();
            }
        }
        if (data != null)
        {
            if (uid == null || uid.length() == 0)
            {
                throw new XukDeserializationFailedException();
            }
            try
            {
                setDataMediaDataUid(data, uid);
            }
            catch (MethodParameterIsEmptyStringException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
    }

    /**
     * @hidden
     */
    @SuppressWarnings("unused")
    @Override
    protected void xukOutAttributes(IXmlDataWriter destination, URI baseUri,
            IProgressHandler ph) throws MethodParameterIsNullException,
            XukSerializationFailedException, ProgressCancelledException
    {
        if (destination == null || baseUri == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (ph != null && ph.notifyProgress())
        {
            throw new ProgressCancelledException();
        }
        destination.writeAttributeString("enforceSinglePCMFormat",
                getEnforceSinglePCMFormat() ? "true" : "false");
        // super.xukOutAttributes(destination, baseUri, ph);
    }

    /**
     * @hidden
     */
    @Override
    protected void xukOutChildren(IXmlDataWriter destination, URI baseUri,
            IProgressHandler ph) throws MethodParameterIsNullException,
            XukSerializationFailedException, ProgressCancelledException
    {
        if (destination == null || baseUri == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (ph != null && ph.notifyProgress())
        {
            throw new ProgressCancelledException();
        }
        destination.writeStartElement("mDefaultPCMFormat", IXukAble.XUK_NS);
        try
        {
            getDefaultPCMFormat().xukOut(destination, baseUri, ph);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (XukSerializationFailedException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        destination.writeEndElement();
        destination.writeStartElement("mMediaData", IXukAble.XUK_NS);
        for (String uid : mMediaDataDictionary.keySet())
        {
            destination.writeStartElement("mMediaDataItem", IXukAble.XUK_NS);
            destination.writeAttributeString("uid", uid);
            mMediaDataDictionary.get(uid).xukOut(destination, baseUri, ph);
            destination.writeEndElement();
        }
        destination.writeEndElement();
        // super.xukOutChildren(destination, baseUri, ph);
    }

    /**
     * @hidden
     */
    public boolean ValueEquals(MediaDataManager other)
            throws MethodParameterIsNullException
    {
        if (other == null)
        {
            throw new MethodParameterIsNullException();
        }
        List<IMediaData> otherMediaData = other.getListOfMediaData();
        if (mMediaDataDictionary.size() != otherMediaData.size())
            return false;
        for (IMediaData oMD : otherMediaData)
        {
            try
            {
                if (!oMD
                        .ValueEquals(getMediaData(other.getUidOfMediaData(oMD))))
                    return false;
            }
            catch (MethodParameterIsEmptyStringException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
            catch (IsNotManagerOfException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
        return true;
    }
}
