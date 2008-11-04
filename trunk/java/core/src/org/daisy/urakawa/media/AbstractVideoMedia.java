package org.daisy.urakawa.media;

import java.net.URI;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.events.Event;
import org.daisy.urakawa.events.EventHandler;
import org.daisy.urakawa.events.IEventHandler;
import org.daisy.urakawa.events.media.SizeChangedEvent;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.timing.ITime;
import org.daisy.urakawa.media.timing.ITimeDelta;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 *
 */
public abstract class AbstractVideoMedia extends AbstractMedia implements
        IContinuous, ISized
{
    @Override
    public AbstractVideoMedia copy()
    {
        return (AbstractVideoMedia) copyProtected();
    }

    @Override
    public AbstractVideoMedia export(Presentation destPres)
            throws FactoryCannotCreateTypeException,
            MethodParameterIsNullException
    {
        return (AbstractVideoMedia) exportProtected(destPres);
    }

    @Override
    protected IMedia copyProtected()
    {
        AbstractVideoMedia copy = (AbstractVideoMedia) super.copyProtected();
        try
        {
            copy.setWidth(getWidth());
            copy.setHeight(getHeight());
        }
        catch (MethodParameterIsOutOfBoundsException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        return copy;
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
        AbstractVideoMedia exported = (AbstractVideoMedia) super
                .exportProtected(destPres);
        if (exported == null)
        {
            throw new FactoryCannotCreateTypeException();
        }
        try
        {
            exported.setWidth(getWidth());
            exported.setHeight(getHeight());
        }
        catch (MethodParameterIsOutOfBoundsException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        return exported;
    }

    /**
     * 
     */
    public AbstractVideoMedia()
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
    public boolean isContinuous()
    {
        return true;
    }

    @Override
    public boolean isDiscrete()
    {
        return false;
    }

    @Override
    public boolean isSequence()
    {
        return false;
    }

    public abstract ITimeDelta getDuration();

    public IContinuous split(ITime splitPoint)
            throws MethodParameterIsNullException,
            TimeOffsetIsOutOfBoundsException
    {
        if (splitPoint == null)
        {
            throw new MethodParameterIsNullException();
        }
        return splitProtected(splitPoint);
    }

    protected abstract AbstractVideoMedia splitProtected(ITime splitPoint)
            throws TimeOffsetIsOutOfBoundsException,
            MethodParameterIsNullException;

    int mWidth = 0;
    int mHeight = 0;
    protected IEventHandler<Event> mSizeChangedEventNotifier = new EventHandler();

    @Override
    public boolean ValueEquals(IMedia other)
            throws MethodParameterIsNullException
    {
        if (other == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (!super.ValueEquals(other))
            return false;
        AbstractVideoMedia otherVideo = (AbstractVideoMedia) other;
        if (getWidth() != otherVideo.getWidth())
            return false;
        if (getHeight() != otherVideo.getHeight())
            return false;
        return true;
    }

    @SuppressWarnings("boxing")
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
        int h, w;
        if (height != null && height != "")
        {
            try
            {
                h = Integer.decode(height);
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
                w = Integer.decode(width);
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
        destination.writeAttributeString("height", Integer.toString(this
                .getHeight()));
        destination.writeAttributeString("width", Integer.toString(this
                .getWidth()));
        super.xukOutAttributes(destination, baseUri, ph);
    }

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