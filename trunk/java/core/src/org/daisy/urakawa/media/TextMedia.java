package org.daisy.urakawa.media;

import java.net.URI;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.events.DataModelChangedEvent;
import org.daisy.urakawa.events.Event;
import org.daisy.urakawa.events.EventHandler;
import org.daisy.urakawa.events.IEventHandler;
import org.daisy.urakawa.events.IEventListener;
import org.daisy.urakawa.events.media.TextChangedEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.xuk.IXukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class TextMedia extends AbstractMedia implements ITextMedia
{
    @Override
    public <K extends DataModelChangedEvent> void notifyListeners(K event)
            throws MethodParameterIsNullException
    {
        if (TextChangedEvent.class.isAssignableFrom(event.getClass()))
        {
            mTextChangedEventNotifier.notifyListeners(event);
        }
        super.notifyListeners(event);
    }

    @Override
    public <K extends DataModelChangedEvent> void registerListener(
            IEventListener<K> listener, Class<K> klass)
            throws MethodParameterIsNullException
    {
        if (TextChangedEvent.class.isAssignableFrom(klass))
        {
            mTextChangedEventNotifier.registerListener(listener, klass);
        }
        else
        {
            super.registerListener(listener, klass);
        }
    }

    @Override
    public <K extends DataModelChangedEvent> void unregisterListener(
            IEventListener<K> listener, Class<K> klass)
            throws MethodParameterIsNullException
    {
        if (TextChangedEvent.class.isAssignableFrom(klass))
        {
            mTextChangedEventNotifier.unregisterListener(listener, klass);
        }
        else
        {
            super.unregisterListener(listener, klass);
        }
    }

    protected IEventHandler<Event> mTextChangedEventNotifier = new EventHandler();

    /**
	 * 
	 */
    public TextMedia()
    {
        mText = "";
    }

    private String mText;

    @Override
    public String toString()
    {
        return mText;
    }

    public String getText()
    {
        return mText;
    }

    public void setText(String text) throws MethodParameterIsNullException
    {
        if (text == null)
        {
            throw new MethodParameterIsNullException();
        }
        String prevTxt = mText;
        mText = text;
        notifyListeners(new TextChangedEvent(this, mText, prevTxt));
    }

    @Override
    public boolean isContinuous()
    {
        return false;
    }

    @Override
    public boolean isDiscrete()
    {
        return true;
    }

    @Override
    public boolean isSequence()
    {
        return false;
    }

    @Override
    public ITextMedia copy()
    {
        return (ITextMedia) copyProtected();
    }

    @Override
    protected IMedia copyProtected()
    {
        try
        {
            return export(getPresentation());
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (FactoryCannotCreateTypeException e)
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

    @Override
    public ITextMedia export(Presentation destPres)
            throws FactoryCannotCreateTypeException,
            MethodParameterIsNullException
    {
        if (destPres == null)
        {
            throw new MethodParameterIsNullException();
        }
        return (ITextMedia) exportProtected(destPres);
    }

    @Override
    protected IMedia exportProtected(Presentation destPres)
            throws FactoryCannotCreateTypeException,
            MethodParameterIsNullException
    {
        if (destPres == null)
        {
            throw new MethodParameterIsNullException();
        }
        ITextMedia exported;
        try
        {
            exported = (ITextMedia) destPres.getMediaFactory().create(
                    getXukLocalName(), getXukNamespaceURI());
        }
        catch (MethodParameterIsEmptyStringException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        if (exported == null)
        {
            throw new FactoryCannotCreateTypeException();
        }
        exported.setText(this.getText());
        return exported;
    }

    @Override
    protected void clear()
    {
        mText = "";
        super.clear();
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
        if (source.getLocalName() == "mText"
                && source.getNamespaceURI() == IXukAble.XUK_NS)
        {
            if (!source.isEmptyElement())
            {
                IXmlDataReader subtreeReader = source.readSubtree();
                subtreeReader.read();
                try
                {
                    setText(subtreeReader.readElementContentAsString());
                }
                finally
                {
                    subtreeReader.close();
                }
            }
            return;
        }
        super.xukInChild(source, ph);
    }

    @SuppressWarnings("unused")
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
        destination.writeStartElement("mText", IXukAble.XUK_NS);
        destination.writeString(getText());
        destination.writeEndElement();
        // super.xukOutChildren(destination, baseUri, ph);
    }

    @Override
    public boolean ValueEquals(IMedia other)
            throws MethodParameterIsNullException
    {
        if (other == null)
        {
            throw new MethodParameterIsNullException();
        }
        try
        {
            if (!super.ValueEquals(other))
                return false;
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        if (getText() != ((ITextMedia) other).getText())
            return false;
        return true;
    }
}
