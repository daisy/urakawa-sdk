package org.daisy.urakawa.media;

import java.net.URI;

import org.daisy.urakawa.AbstractXukAbleWithPresentation;
import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.events.DataModelChangedEvent;
import org.daisy.urakawa.events.Event;
import org.daisy.urakawa.events.EventHandler;
import org.daisy.urakawa.events.IEventHandler;
import org.daisy.urakawa.events.IEventListener;
import org.daisy.urakawa.events.LanguageChangedEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * 
 *
 */
public abstract class AbstractMedia extends AbstractXukAbleWithPresentation
        implements IMedia
{
    protected IEventListener<DataModelChangedEvent> mBubbleEventListener = new IEventListener<DataModelChangedEvent>()
    {
        public <K extends DataModelChangedEvent> void eventCallback(K event)
                throws MethodParameterIsNullException
        {
            if (event == null)
            {
                throw new MethodParameterIsNullException();
            }
            notifyListeners(event);
        }
    };
    protected IEventHandler<Event> mLanguageChangedEventNotifier = new EventHandler();
    protected IEventHandler<Event> mDataModelEventNotifier = new EventHandler();

    public <K extends DataModelChangedEvent> void notifyListeners(K event)
            throws MethodParameterIsNullException
    {
        if (LanguageChangedEvent.class.isAssignableFrom(event.getClass()))
        {
            mLanguageChangedEventNotifier.notifyListeners(event);
        }
        mDataModelEventNotifier.notifyListeners(event);
    }

    public <K extends DataModelChangedEvent> void registerListener(
            IEventListener<K> listener, Class<K> klass)
            throws MethodParameterIsNullException
    {
        if (LanguageChangedEvent.class.isAssignableFrom(klass))
        {
            mLanguageChangedEventNotifier.registerListener(listener, klass);
        }
        else
        {
            mDataModelEventNotifier.registerListener(listener, klass);
        }
    }

    public <K extends DataModelChangedEvent> void unregisterListener(
            IEventListener<K> listener, Class<K> klass)
            throws MethodParameterIsNullException
    {
        if (LanguageChangedEvent.class.isAssignableFrom(klass))
        {
            mLanguageChangedEventNotifier.unregisterListener(listener, klass);
        }
        else
        {
            mDataModelEventNotifier.unregisterListener(listener, klass);
        }
    }

    /**
	 * 
	 */
    public AbstractMedia()
    {
        super();
        mLanguage = null;
    }

    private String mLanguage;

    public abstract boolean isContinuous();

    public abstract boolean isDiscrete();

    public abstract boolean isSequence();

    public AbstractMedia copy()
    {
        return (AbstractMedia) copyProtected();
    }

    protected IMedia copyProtected()
    {
        IMedia expMedia;
        try
        {
            expMedia = getPresentation().getMediaFactory().create(
                    getXukLocalName(), getXukNamespaceURI());
        }
        catch (MethodParameterIsEmptyStringException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (IsNotInitializedException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        try
        {
            expMedia.setLanguage(getLanguage());
        }
        catch (MethodParameterIsEmptyStringException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        return expMedia;
    }

    public IMedia export(Presentation destPres)
            throws FactoryCannotCreateTypeException,
            MethodParameterIsNullException
    {
        if (destPres == null)
        {
            throw new MethodParameterIsNullException();
        }
        return exportProtected(destPres);
    }

    protected IMedia exportProtected(Presentation destPres)
            throws FactoryCannotCreateTypeException,
            MethodParameterIsNullException
    {
        if (destPres == null)
        {
            throw new MethodParameterIsNullException();
        }
        IMedia expMedia;
        try
        {
            expMedia = destPres.getMediaFactory().create(getXukLocalName(),
                    getXukNamespaceURI());
        }
        catch (MethodParameterIsEmptyStringException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        if (expMedia == null)
        {
            throw new FactoryCannotCreateTypeException();
        }
        try
        {
            expMedia.setLanguage(getLanguage());
        }
        catch (MethodParameterIsEmptyStringException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        return expMedia;
    }

    public void setLanguage(String lang)
            throws MethodParameterIsEmptyStringException
    {
        if (lang.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        String prevlang = mLanguage;
        mLanguage = lang;
        if (prevlang != mLanguage)
            try
            {
                notifyListeners(new LanguageChangedEvent(this, mLanguage,
                        prevlang));
            }
            catch (MethodParameterIsNullException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
    }

    public String getLanguage()
    {
        return mLanguage;
    }

    @Override
    protected void clear()
    {
        mLanguage = null;
        // super.clear();
    }

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
        // super.xukInAttributes(source, ph);
    }

    @Override
    protected void xukOutAttributes(IXmlDataWriter destination, URI baseUri,
            IProgressHandler ph) throws MethodParameterIsNullException,
            XukSerializationFailedException, ProgressCancelledException
    {
        if (ph != null && ph.notifyProgress())
        {
            throw new ProgressCancelledException();
        }
        if (getLanguage() != null)
            destination.writeAttributeString("language", getLanguage());
        // super.xukOutAttributes(destination, baseUri, ph);
    }

    public boolean ValueEquals(IMedia other)
            throws MethodParameterIsNullException
    {
        if (other == null)
            throw new MethodParameterIsNullException();
        if (this.getClass() != other.getClass())
            return false;
        if (this.getLanguage() != other.getLanguage())
            return false;
        return true;
    }
}
