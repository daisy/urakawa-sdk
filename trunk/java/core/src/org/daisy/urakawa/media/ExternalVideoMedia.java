package org.daisy.urakawa.media;

import java.net.URI;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.IPresentation;
import org.daisy.urakawa.events.DataModelChangedEvent;
import org.daisy.urakawa.events.Event;
import org.daisy.urakawa.events.EventHandler;
import org.daisy.urakawa.events.IEventHandler;
import org.daisy.urakawa.events.IEventListener;
import org.daisy.urakawa.events.media.ClipChangedEvent;
import org.daisy.urakawa.events.media.SizeChangedEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.timing.ITime;
import org.daisy.urakawa.media.timing.ITimeDelta;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;
import org.daisy.urakawa.media.timing.TimeStringRepresentationIsInvalidException;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 *
 */
public class ExternalVideoMedia extends AbstractExternalMedia implements
        IVideoMedia
{
    int mWidth = 0;
    int mHeight = 0;
    ITime mClipBegin;
    ITime mClipEnd;

    @Override
    public <K extends DataModelChangedEvent> void notifyListeners(K event)
            throws MethodParameterIsNullException
    {
        if (ClipChangedEvent.class.isAssignableFrom(event.getClass()))
        {
            mClipChangedEventNotifier.notifyListeners(event);
        }
        else
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
        if (ClipChangedEvent.class.isAssignableFrom(klass))
        {
            mClipChangedEventNotifier.registerListener(listener, klass);
        }
        else
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
        if (ClipChangedEvent.class.isAssignableFrom(klass))
        {
            mClipChangedEventNotifier.unregisterListener(listener, klass);
        }
        else
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
    protected IEventHandler<Event> mClipChangedEventNotifier = new EventHandler();

    private void resetClipTimes()
    {
        mClipBegin = new Time().getZero();
        mClipEnd = new Time().getMaxValue();
    }

    /**
	 * 
	 */
    public ExternalVideoMedia()
    {
        mWidth = 0;
        mHeight = 0;
        resetClipTimes();
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
    public ExternalVideoMedia copy()
    {
        return (ExternalVideoMedia) copyProtected();
    }

    @Override
    protected IMedia exportProtected(IPresentation destPres)
            throws FactoryCannotCreateTypeException,
            MethodParameterIsNullException
    {
        if (destPres == null)
        {
            throw new MethodParameterIsNullException();
        }
        ExternalVideoMedia exported = (ExternalVideoMedia) super
                .exportProtected(destPres);
        if (exported == null)
        {
            throw new FactoryCannotCreateTypeException();
        }
        if (getClipBegin().isNegativeTimeOffset())
        {
            try
            {
                exported.setClipBegin(getClipBegin().copy());
                exported.setClipEnd(getClipEnd().copy());
            }
            catch (TimeOffsetIsOutOfBoundsException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
        else
        {
            try
            {
                exported.setClipEnd(getClipEnd().copy());
                exported.setClipBegin(getClipBegin().copy());
            }
            catch (TimeOffsetIsOutOfBoundsException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
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

    @Override
    public ExternalVideoMedia export(IPresentation destPres)
            throws FactoryCannotCreateTypeException,
            MethodParameterIsNullException
    {
        if (destPres == null)
        {
            throw new MethodParameterIsNullException();
        }
        return (ExternalVideoMedia) exportProtected(destPres);
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
        String cb = source.getAttribute("clipBegin");
        String ce = source.getAttribute("clipEnd");
        resetClipTimes();
        try
        {
            ITime ceTime = new Time(ce);
            ITime cbTime = new Time(cb);
            if (cbTime.isNegativeTimeOffset())
            {
                setClipBegin(cbTime);
                setClipEnd(ceTime);
            }
            else
            {
                setClipEnd(ceTime);
                setClipBegin(cbTime);
            }
        }
        catch (TimeStringRepresentationIsInvalidException e)
        {
            throw new XukDeserializationFailedException();
        }
        catch (MethodParameterIsOutOfBoundsException e)
        {
            throw new XukDeserializationFailedException();
        }
        catch (MethodParameterIsEmptyStringException e)
        {
            throw new XukDeserializationFailedException();
        }
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
        destination.writeAttributeString("clipBegin", this.getClipBegin()
                .toString());
        destination.writeAttributeString("clipEnd", this.getClipEnd()
                .toString());
        destination.writeAttributeString("height", Integer.toString(this
                .getHeight()));
        destination.writeAttributeString("width", Integer.toString(this
                .getWidth()));
        super.xukOutAttributes(destination, baseUri, ph);
    }

    public ITimeDelta getDuration()
    {
        try
        {
            return getClipEnd().getTimeDelta(getClipBegin());
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    public ITime getClipBegin()
    {
        return mClipBegin;
    }

    public ITime getClipEnd()
    {
        return mClipEnd;
    }

    public void setClipBegin(ITime beginPoint)
            throws MethodParameterIsNullException,
            TimeOffsetIsOutOfBoundsException
    {
        if (beginPoint == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (beginPoint.isLessThan(new Time().getZero()))
        {
            throw new TimeOffsetIsOutOfBoundsException();
        }
        if (beginPoint.isGreaterThan(getClipEnd()))
        {
            throw new TimeOffsetIsOutOfBoundsException();
        }
        if (!mClipBegin.isEqualTo(beginPoint))
        {
            ITime prevCB = getClipBegin();
            mClipBegin = beginPoint.copy();
            notifyListeners(new ClipChangedEvent(this, getClipBegin(),
                    getClipEnd(), prevCB, getClipEnd()));
        }
    }

    public void setClipEnd(ITime endPoint)
            throws MethodParameterIsNullException,
            TimeOffsetIsOutOfBoundsException
    {
        if (endPoint == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (endPoint.isLessThan(getClipBegin()))
        {
            throw new TimeOffsetIsOutOfBoundsException();
        }
        if (!mClipEnd.isEqualTo(endPoint))
        {
            ITime prevCE = getClipEnd();
            mClipEnd = endPoint.copy();
            notifyListeners(new ClipChangedEvent(this, getClipBegin(),
                    getClipEnd(), getClipBegin(), prevCE));
        }
    }

    public ExternalVideoMedia split(ITime splitPoint)
            throws MethodParameterIsNullException,
            TimeOffsetIsOutOfBoundsException
    {
        if (splitPoint == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (getClipBegin().isGreaterThan(splitPoint)
                || splitPoint.isGreaterThan(getClipEnd()))
        {
            throw new TimeOffsetIsOutOfBoundsException();
        }
        ExternalVideoMedia secondPart = copy();
        secondPart.setClipBegin(splitPoint.copy());
        setClipEnd(splitPoint.copy());
        return secondPart;
    }

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
        ExternalVideoMedia otherVideo = (ExternalVideoMedia) other;
        if (!getClipBegin().isEqualTo(otherVideo.getClipBegin()))
            return false;
        if (!getClipEnd().isEqualTo(otherVideo.getClipEnd()))
            return false;
        if (getWidth() != otherVideo.getWidth())
            return false;
        if (getHeight() != otherVideo.getHeight())
            return false;
        return true;
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
