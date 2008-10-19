package org.daisy.urakawa.property.channel;

import java.net.URI;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.daisy.urakawa.IValueEquatable;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.xuk.AbstractXukAble;
import org.daisy.urakawa.xuk.IXukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * @depend - Composition 0..n org.daisy.urakawa.property.channel.Channel
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 */
public final class ChannelsManager extends AbstractXukAble implements
        IValueEquatable<ChannelsManager>
{
    private Presentation mPresentation;

    /**
     * @return the Presentation owner
     */
    public Presentation getPresentation()
    {
        return mPresentation;
    }

    private Map<String, IChannel> mChannels;

    /**
     * @param pres
     * @throws MethodParameterIsNullException
     */
    public ChannelsManager(Presentation pres)
            throws MethodParameterIsNullException
    {
        if (pres == null)
        {
            throw new MethodParameterIsNullException();
        }
        mPresentation = pres;
        mChannels = new HashMap<String, IChannel>();
    }

    /**
     * Adds an existing IChannel to the list.
     * 
     * @param iChannel
     *        cannot be null, channel must not already exist in the list.
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws ChannelAlreadyExistsException
     *         when the given channel is already managed by this manager
     */
    public void addChannel(IChannel iChannel)
            throws MethodParameterIsNullException,
            ChannelAlreadyExistsException
    {
        try
        {
            addChannel(iChannel, getNewId());
        }
        catch (MethodParameterIsEmptyStringException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    /**
     * Adds an existing IChannel to the list.
     * 
     * @param iChannel
     * @param uid
     * @throws MethodParameterIsNullException
     * @throws ChannelAlreadyExistsException
     * @throws MethodParameterIsEmptyStringException
     */
    public void addChannel(IChannel iChannel, String uid)
            throws MethodParameterIsNullException,
            ChannelAlreadyExistsException,
            MethodParameterIsEmptyStringException
    {
        if (iChannel == null || uid == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (uid.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        if (mChannels.values().contains(iChannel))
        {
            throw new ChannelAlreadyExistsException();
        }
        if (mChannels.containsKey(uid))
        {
            throw new ChannelAlreadyExistsException();
        }
        mChannels.put(uid, iChannel);
    }

    /**
     * @hidden
     */
    @SuppressWarnings("boxing")
    private String getNewId()
    {
        long i = 0;
        while (i < Integer.MAX_VALUE)
        {
            String newId = String.format("CHID{0:0000}", i);
            if (!mChannels.containsKey(newId))
                return newId;
            i++;
        }
        throw new RuntimeException("TOO MANY CHANNELS!!!");
    }

    /**
     * Removes a given channel from the Presentation instance.
     * 
     * @param iChannel
     *        cannot be null, the channel must exist in the list of current
     *        channel
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws ChannelDoesNotExistException
     *         When the given channel is not managed by this manager
     */
    public void removeChannel(IChannel iChannel)
            throws MethodParameterIsNullException, ChannelDoesNotExistException
    {
        if (iChannel == null)
        {
            throw new MethodParameterIsNullException();
        }
        try
        {
            removeChannel(getUidOfChannel(iChannel));
        }
        catch (MethodParameterIsEmptyStringException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    /**
     * Removes a given channel from the Presentation instance given its UID.
     * 
     * @param uid
     *        the unique ID of the channel to remove
     *        "MethodParameterIsEmptyString-MethodParameterIsNull-ChannelDoesNotExist"
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws MethodParameterIsEmptyStringException
     *         Empty string '' method parameters are forbidden
     * @throws ChannelDoesNotExistException
     *         When the given channel is not managed by this manager
     */
    public void removeChannel(String uid)
            throws MethodParameterIsNullException,
            ChannelDoesNotExistException, MethodParameterIsEmptyStringException
    {
        IChannel iChannel = getChannel(uid);
        ClearChannelTreeNodeVisitor clChVisitor = new ClearChannelTreeNodeVisitor(
                iChannel);
        getPresentation().getRootNode().acceptDepthFirst(clChVisitor);
        mChannels.remove(uid);
    }

    /**
     * @return the list of channel that are used in the presentation. Cannot
     *         return null (no channel = returns an empty list).
     */
    public List<IChannel> getListOfChannels()
    {
        return new LinkedList<IChannel>(mChannels.values());
    }

    /**
     * @return list
     */
    public List<String> getListOfUids()
    {
        return new LinkedList<String>(mChannels.keySet());
    }

    /**
     * @param uid
     * @return channel that matches the uid
     * @throws MethodParameterIsEmptyStringException
     *         Empty string '' method parameters are forbidden
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws ChannelDoesNotExistException
     */
    public IChannel getChannel(String uid)
            throws MethodParameterIsNullException,
            ChannelDoesNotExistException, MethodParameterIsEmptyStringException
    {
        if (uid == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (uid.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        if (!mChannels.keySet().contains(uid))
        {
            throw new ChannelDoesNotExistException();
        }
        return mChannels.get(uid);
    }

    /**
     * There is no IChannel::setUid() method because the manager maintains the
     * uid<->channel mapping, the channel object does not know about its UID
     * directly.
     * 
     * @param ch
     * @return channel uid
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws ChannelDoesNotExistException
     */
    public String getUidOfChannel(IChannel ch)
            throws MethodParameterIsNullException, ChannelDoesNotExistException
    {
        if (ch == null)
        {
            throw new MethodParameterIsNullException();
        }
        for (String Id : mChannels.keySet())
        {
            if (mChannels.get(Id) == ch)
            {
                return Id;
            }
        }
        throw new ChannelDoesNotExistException();
    }

    /**
     * @param ch
     * @param uid
     * @throws MethodParameterIsNullException
     * @throws ChannelDoesNotExistException
     * @throws MethodParameterIsEmptyStringException
     */
    public void setUidOfChannel(IChannel ch, String uid)
            throws MethodParameterIsNullException,
            ChannelDoesNotExistException, MethodParameterIsEmptyStringException
    {
        if (ch == null || uid == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (uid.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        for (String Id : mChannels.keySet())
        {
            if (mChannels.get(Id) == ch)
            {
                mChannels.remove(Id);
                mChannels.put(uid, ch);
                return;
            }
        }
        throw new ChannelDoesNotExistException();
    }

    /**
     * 
     */
    public void clearChannels()
    {
        for (IChannel ch : getListOfChannels())
        {
            try
            {
                removeChannel(ch);
            }
            catch (MethodParameterIsNullException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
            catch (ChannelDoesNotExistException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
    }

    /**
     * @param channelName
     * @return list
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws MethodParameterIsEmptyStringException
     *         Empty string '' method parameters are forbidden
     */
    public List<IChannel> getListOfChannels(String channelName)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException
    {
        if (channelName == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (channelName.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        List<IChannel> res = new LinkedList<IChannel>();
        for (IChannel ch : mChannels.values())
        {
            try
            {
                if (ch.getName() == channelName)
                    res.add(ch);
            }
            catch (IsNotInitializedException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
        return res;
    }

    /**
     * @param uid
     * @return true or false
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
        return mChannels.containsKey(uid);
    }

    /**
     * @hidden
     */
    @Override
    protected void clear()
    {
        mChannels.clear();
        // super.clear();
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
        if (source.getNamespaceURI() == IXukAble.XUK_NS
                && source.getLocalName() == "mChannels")
        {
            readItem = true;
            if (!source.isEmptyElement())
            {
                while (source.read())
                {
                    if (source.getNodeType() == IXmlDataReader.ELEMENT)
                    {
                        if (source.getLocalName() == "mChannelItem"
                                && source.getNamespaceURI() == IXukAble.XUK_NS)
                        {
                            xukInChannelItem(source, ph);
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
        if (!readItem)
        {
            super.xukInChild(source, ph);
        }
    }

    /**
     * @hidden
     */
    private void xukInChannelItem(IXmlDataReader source, IProgressHandler ph)
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
        if (uid == null || uid.length() == 0)
        {
            throw new XukDeserializationFailedException();
        }
        boolean foundChannel = false;
        if (!source.isEmptyElement())
        {
            while (source.read())
            {
                if (source.getNodeType() == IXmlDataReader.ELEMENT)
                {
                    IChannel newCh;
                    try
                    {
                        newCh = getPresentation().getChannelFactory()
                                .create(source.getLocalName(),
                                        source.getNamespaceURI());
                    }
                    catch (MethodParameterIsEmptyStringException e1)
                    {
                        // Should never happen
                        throw new RuntimeException("WTF ??!", e1);
                    }
                    if (newCh != null)
                    {
                        try
                        {
                            setUidOfChannel(newCh, uid);
                        }
                        catch (MethodParameterIsEmptyStringException e)
                        {
                            // Should never happen
                            throw new RuntimeException("WTF ??!", e);
                        }
                        catch (ChannelDoesNotExistException e)
                        {
                            // Should never happen
                            throw new RuntimeException("WTF ??!", e);
                        }
                        newCh.xukIn(source, ph);
                        foundChannel = true;
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
        if (!foundChannel)
        {
            throw new XukDeserializationFailedException();
        }
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
        List<String> uids = getListOfUids();
        if (uids.size() > 0)
        {
            destination.writeStartElement("mChannels", IXukAble.XUK_NS);
            for (String uid : uids)
            {
                destination.writeStartElement("mChannelItem", IXukAble.XUK_NS);
                destination.writeAttributeString("uid", uid);
                try
                {
                    getChannel(uid).xukOut(destination, baseUri, ph);
                }
                catch (MethodParameterIsEmptyStringException e)
                {
                    // Should never happen
                    throw new RuntimeException("WTF ??!", e);
                }
                catch (ChannelDoesNotExistException e)
                {
                    // Should never happen
                    throw new RuntimeException("WTF ??!", e);
                }
                destination.writeEndElement();
            }
            destination.writeEndElement();
        }
        // super.xukOutChildren(destination, baseUri, ph);
    }

    /**
     * @hidden
     */
    public boolean ValueEquals(ChannelsManager other)
            throws MethodParameterIsNullException
    {
        if (other == null)
        {
            throw new MethodParameterIsNullException();
        }
        List<String> thisUids = getListOfUids();
        List<String> otherUids = other.getListOfUids();
        if (thisUids.size() != otherUids.size())
            return false;
        for (String uid : thisUids)
        {
            if (!otherUids.contains(uid))
                return false;
            try
            {
                if (!getChannel(uid).ValueEquals(other.getChannel(uid)))
                    return false;
            }
            catch (MethodParameterIsEmptyStringException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
            catch (ChannelDoesNotExistException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
        return true;
    }

    /**
     * @hidden
     */
    @SuppressWarnings("unused")
    @Override
    protected void xukInAttributes(IXmlDataReader source, IProgressHandler ph)
            throws MethodParameterIsNullException,
            XukDeserializationFailedException, ProgressCancelledException
    {
        /**
         * Does nothing.
         */
    }

    /**
     * @hidden
     */
    @SuppressWarnings("unused")
    @Override
    protected void xukOutAttributes(IXmlDataWriter destination, URI baseUri,
            IProgressHandler ph) throws XukSerializationFailedException,
            MethodParameterIsNullException, ProgressCancelledException
    {
        /**
         * Does nothing.
         */
    }
}
