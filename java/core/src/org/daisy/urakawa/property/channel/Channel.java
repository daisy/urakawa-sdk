package org.daisy.urakawa.property.channel;

import java.net.URI;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.IPresentation;
import org.daisy.urakawa.AbstractXukAbleWithPresentation;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.IMedia;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class Channel extends AbstractXukAbleWithPresentation implements IChannel
{
    private String mName = "";
    private String mLanguage = null;

    public boolean isEquivalentTo(IChannel otherChannel)
            throws MethodParameterIsNullException
    {
        if (otherChannel == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (this.getClass() != otherChannel.getClass())
            return false;
        try
        {
            if (this.getName() != otherChannel.getName())
                return false;
        }
        catch (IsNotInitializedException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        if (this.getLanguage() != otherChannel.getLanguage())
            return false;
        return true;
    }

    public IChannel export(IPresentation destPres)
            throws FactoryCannotCreateTypeException, IsNotInitializedException,
            MethodParameterIsNullException
    {
        return exportProtected(destPres);
    }

    protected IChannel exportProtected(IPresentation destPres)
            throws FactoryCannotCreateTypeException, IsNotInitializedException,
            MethodParameterIsNullException
    {
        IChannel exportedCh;
        try
        {
            exportedCh = destPres.getChannelFactory().create(getXukLocalName(),
                    getXukNamespaceURI());
        }
        catch (MethodParameterIsEmptyStringException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        if (exportedCh == null)
        {
            throw new FactoryCannotCreateTypeException();
        }
        try
        {
            exportedCh.setName(getName());
            exportedCh.setLanguage(getLanguage());
        }
        catch (MethodParameterIsEmptyStringException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        return exportedCh;
    }

    @SuppressWarnings("unused")
    public boolean canAccept(IMedia iMedia)
            throws MethodParameterIsNullException
    {
        return true;
    }

    public void setName(String name) throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException
    {
        if (name == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (name.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        mName = name;
    }

    public String getName() throws IsNotInitializedException
    {
        if (mName == null)
        {
            throw new IsNotInitializedException();
        }
        return mName;
    }

    public void setLanguage(String lang)
            throws MethodParameterIsEmptyStringException
    {
        if (lang.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        mLanguage = lang;
    }

    public String getLanguage()
    {
        return mLanguage;
    }

    public String getUid()
    {
        try
        {
            return getPresentation().getChannelsManager().getUidOfChannel(this);
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
        catch (IsNotInitializedException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    @SuppressWarnings("unused")
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
        String name = source.getAttribute("name");
        if (name == null)
            name = "UNDEFINED_NAME";
        try
        {
            setName(name);
        }
        catch (MethodParameterIsEmptyStringException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        String lang = source.getAttribute("language");
        if (lang != null)
        {
            lang = lang.trim();
            if (lang.length() == 0)
            {
                lang = null;
            }
        }
        try
        {
            setLanguage(lang);
        }
        catch (MethodParameterIsEmptyStringException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

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
        if (!readItem)
        {
            super.xukInChild(source, ph);
        }
    }

    @SuppressWarnings("unused")
    @Override
    protected void xukOutAttributes(IXmlDataWriter destination, URI baseUri,
            IProgressHandler ph) throws MethodParameterIsNullException,
            XukSerializationFailedException, ProgressCancelledException
    {
        if (ph != null && ph.notifyProgress())
        {
            throw new ProgressCancelledException();
        }
        try
        {
            destination.writeAttributeString("name", getName());
        }
        catch (IsNotInitializedException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        destination.writeAttributeString("language", getLanguage());
    }

    public boolean ValueEquals(IChannel other)
            throws MethodParameterIsNullException
    {
        if (other == null)
            throw new MethodParameterIsNullException();
        if (getClass() != other.getClass())
            return false;
        try
        {
            if (getName() != other.getName())
                return false;
        }
        catch (IsNotInitializedException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        if (getLanguage() != other.getLanguage())
            return false;
        return true;
    }

    @Override
    protected void clear()
    {
        /**
         * Does nothing.
         */
    }

    @SuppressWarnings("unused")
    @Override
    protected void xukOutChildren(IXmlDataWriter destination, URI baseUri,
            IProgressHandler ph) throws XukSerializationFailedException,
            MethodParameterIsNullException, ProgressCancelledException
    {
        /**
         * Does nothing.
         */
    }
}
