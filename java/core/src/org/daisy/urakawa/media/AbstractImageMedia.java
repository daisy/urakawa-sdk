package org.daisy.urakawa.media;

import java.net.URI;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.events.DataModelChangedEvent;
import org.daisy.urakawa.events.Event;
import org.daisy.urakawa.events.EventHandler;
import org.daisy.urakawa.events.IEventHandler;
import org.daisy.urakawa.events.IEventListener;
import org.daisy.urakawa.events.media.SizeChangedEvent;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 *
 */
public abstract class AbstractImageMedia extends AbstractMedia implements
        ISized
{
    int mWidth;
    int mHeight;

    /**
     * 
     */
    protected AbstractImageMedia()
    {
        super();
        mWidth = 0;
        mHeight = 0;
    }

    public int getWidth()
    {
        return mWidth;
    }

    public int getHeight()
    {
        return mHeight;
    }

    public void setWidth(int width)
            throws MethodParameterIsOutOfBoundsException
    {
        setSize(getHeight(), width);
    }

    public void setHeight(int height)
            throws MethodParameterIsOutOfBoundsException
    {
        setSize(height, getWidth());
    }

    public void setSize(int height, int width)
            throws MethodParameterIsOutOfBoundsException
    {
        if (width < 0)
        {
            throw new MethodParameterIsOutOfBoundsException();
        }
        if (height < 0)
        {
            throw new MethodParameterIsOutOfBoundsException();
        }
        int prevWidth = mWidth;
        mWidth = width;
        int prevHeight = mHeight;
        mHeight = height;
        if (mWidth != prevWidth || mHeight != prevHeight)
        {
            try
            {
                notifyListeners(new SizeChangedEvent(this, mHeight, mWidth,
                        prevHeight, prevWidth));
            }
            catch (MethodParameterIsNullException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
    }

    @Override
    public boolean ValueEquals(IMedia other)
            throws MethodParameterIsNullException
    {
        if (!super.ValueEquals(other))
            return false;
        ISized otherImage = (ISized) other;
        if (getHeight() != otherImage.getHeight())
            return false;
        if (getWidth() != otherImage.getWidth())
            return false;
        return true;
    }

    @Override
    protected IMedia exportProtected(Presentation destPres)
            throws MethodParameterIsNullException,
            FactoryCannotCreateTypeException
    {
        if (destPres == null)
        {
            throw new MethodParameterIsNullException();
        }
        AbstractImageMedia exported = (AbstractImageMedia) super
                .exportProtected(destPres);
        if (exported == null)
        {
            throw new FactoryCannotCreateTypeException();
        }
        try
        {
            exported.setHeight(this.getHeight());
            exported.setWidth(this.getWidth());
        }
        catch (MethodParameterIsOutOfBoundsException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        return exported;
    }

    @Override
    public AbstractImageMedia copy()
    {
        return (AbstractImageMedia) copyProtected();
    }

    @Override
    public IMedia copyProtected()
    {
        AbstractImageMedia copy = (AbstractImageMedia) super.copyProtected();
        try
        {
            copy.setHeight(this.getHeight());
            copy.setWidth(this.getWidth());
        }
        catch (MethodParameterIsOutOfBoundsException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        return copy;
    }

    @Override
    public <K extends DataModelChangedEvent> void notifyListeners(K event)
            throws MethodParameterIsNullException
    {
        if (SizeChangedEvent.class.isAssignableFrom(event.getClass()))
        {
            mSizeChangedEventNotifier.notifyListeners(event);
        }
        super.notifyListeners(event);
    }

    @Override
    public <K extends DataModelChangedEvent> void registerListener(
            IEventListener<K> listener, Class<K> klass)
            throws MethodParameterIsNullException
    {
        if (SizeChangedEvent.class.isAssignableFrom(klass))
        {
            mSizeChangedEventNotifier.registerListener(listener, klass);
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
        if (SizeChangedEvent.class.isAssignableFrom(klass))
        {
            mSizeChangedEventNotifier.unregisterListener(listener, klass);
        }
        else
        {
            super.unregisterListener(listener, klass);
        }
    }

    protected IEventHandler<Event> mSizeChangedEventNotifier = new EventHandler();

    @SuppressWarnings("boxing")
    @Override
    public String toString()
    {
        return String.format("AbstractImageMedia ({1:0}x{2:0})", mWidth,
                mHeight);
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
        super.xukInAttributes(source, ph);
        String height = source.getAttribute("height");
        String width = source.getAttribute("width");
        int h = 0, w = 0;
        if (height != null && height != "")
        {
            try
            {
                Integer.parseInt(height);
            }
            catch (NumberFormatException e)
            {
                throw new XukDeserializationFailedException();
            }
            try
            {
                setHeight(h);
            }
            catch (MethodParameterIsOutOfBoundsException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
        else
        {
            try
            {
                setHeight(0);
            }
            catch (MethodParameterIsOutOfBoundsException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
        if (width != null && width != "")
        {
            try
            {
                Integer.parseInt(width);
            }
            catch (NumberFormatException e)
            {
                throw new XukDeserializationFailedException();
            }
            try
            {
                setWidth(w);
            }
            catch (MethodParameterIsOutOfBoundsException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
        else
        {
            try
            {
                setWidth(0);
            }
            catch (MethodParameterIsOutOfBoundsException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
    }

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
        destination.writeAttributeString("height", Integer.toString(mHeight));
        destination.writeAttributeString("width", Integer.toString(mWidth));
        super.xukOutAttributes(destination, baseUri, ph);
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
