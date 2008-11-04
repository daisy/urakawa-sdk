package org.daisy.urakawa.media;

import java.net.URI;
import java.net.URISyntaxException;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.events.DataModelChangedEvent;
import org.daisy.urakawa.events.Event;
import org.daisy.urakawa.events.EventHandler;
import org.daisy.urakawa.events.IEventHandler;
import org.daisy.urakawa.events.IEventListener;
import org.daisy.urakawa.events.media.ClipChangedEvent;
import org.daisy.urakawa.events.media.SrcChangedEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.timing.ITime;
import org.daisy.urakawa.media.timing.ITimeDelta;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;
import org.daisy.urakawa.media.timing.TimeStringRepresentationIsInvalidException;
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
public class ExternalAudioMedia extends AbstractAudioMedia implements ILocated,
        IClipped
{
    // BEGIN ILocated common code (no multiple-inheritance, unfortunately)
    private String mSrc;

    @Override
    protected void clear()
    {
        mSrc = ".";
        super.clear();
    }

    protected IEventHandler<Event> mSrcChangedEventNotifier = new EventHandler();

    public String getSrc()
    {
        return mSrc;
    }

    public void setSrc(String newSrc) throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException
    {
        if (newSrc == null)
            throw new MethodParameterIsNullException();
        if (newSrc.length() == 0)
            throw new MethodParameterIsEmptyStringException();
        String prevSrc = mSrc;
        mSrc = newSrc;
        if (mSrc != prevSrc)
            notifyListeners(new SrcChangedEvent(this, mSrc, prevSrc));
    }

    public URI getURI() throws URISyntaxException
    {
        URI uri = null;
        try
        {
            uri = new URI(getSrc()).resolve(getPresentation().getRootURI());
        }
        catch (IsNotInitializedException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        return uri;
    }

    @Override
    public IMedia copyProtected()
    {
        ExternalAudioMedia copy = (ExternalAudioMedia) super.copyProtected();
        try
        {
            URI.create(getSrc()).resolve(getPresentation().getRootURI());
            String destSrc = getPresentation().getRootURI()
                    .relativize(getURI()).toString();
            if (destSrc.length() == 0)
                destSrc = ".";
            try
            {
                copy.setSrc(destSrc);
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
        }
        catch (URISyntaxException e)
        {
            try
            {
                copy.setSrc(getSrc());
            }
            catch (MethodParameterIsEmptyStringException e1)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e1);
            }
            catch (MethodParameterIsNullException e2)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e2);
            }
        }
        catch (IsNotInitializedException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        try
        {
            copy.setClipBegin(getClipBegin().copy());
            copy.setClipEnd(getClipEnd().copy());
        }
        catch (TimeOffsetIsOutOfBoundsException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        return copy;
    }

    // END ILocated common code (no multiple-inheritance, unfortunately)
    private ITime mClipBegin;
    private ITime mClipEnd;

    @Override
    public <K extends DataModelChangedEvent> void notifyListeners(K event)
            throws MethodParameterIsNullException
    {
        if (SrcChangedEvent.class.isAssignableFrom(event.getClass()))
        {
            mSrcChangedEventNotifier.notifyListeners(event);
        }
        else
            if (ClipChangedEvent.class.isAssignableFrom(event.getClass()))
            {
                mClipChangedEventNotifier.notifyListeners(event);
            }
        super.notifyListeners(event);
    }

    @Override
    public <K extends DataModelChangedEvent> void registerListener(
            IEventListener<K> listener, Class<K> klass)
            throws MethodParameterIsNullException
    {
        if (SrcChangedEvent.class.isAssignableFrom(klass))
        {
            mSrcChangedEventNotifier.registerListener(listener, klass);
        }
        else
            if (ClipChangedEvent.class.isAssignableFrom(klass))
            {
                mClipChangedEventNotifier.registerListener(listener, klass);
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
        if (SrcChangedEvent.class.isAssignableFrom(klass))
        {
            mSrcChangedEventNotifier.unregisterListener(listener, klass);
        }
        else
            if (ClipChangedEvent.class.isAssignableFrom(klass))
            {
                mClipChangedEventNotifier.unregisterListener(listener, klass);
            }
            else
            {
                super.unregisterListener(listener, klass);
            }
    }

    protected IEventHandler<Event> mClipChangedEventNotifier = new EventHandler();

    private void resetClipTimes()
    {
        mClipBegin = new Time().getZero();
        mClipEnd = new Time().getMaxValue();
    }

    protected ExternalAudioMedia()
    {
        super();
        mSrc = ".";
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
    public ExternalAudioMedia copy()
    {
        return (ExternalAudioMedia) copyProtected();
    }

    @Override
    public ExternalAudioMedia export(Presentation destPres)
            throws MethodParameterIsNullException,
            FactoryCannotCreateTypeException
    {
        if (destPres == null)
        {
            throw new MethodParameterIsNullException();
        }
        return (ExternalAudioMedia) exportProtected(destPres);
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
        ExternalAudioMedia exported = (ExternalAudioMedia) super
                .exportProtected(destPres);
        if (exported == null)
        {
            throw new FactoryCannotCreateTypeException();
        }
        try
        {
            URI.create(getSrc()).resolve(getPresentation().getRootURI());
            String destSrc = destPres.getRootURI().relativize(getURI())
                    .toString();
            if (destSrc.length() == 0)
                destSrc = ".";
            try
            {
                exported.setSrc(destSrc);
            }
            catch (MethodParameterIsEmptyStringException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
        catch (URISyntaxException e)
        {
            try
            {
                exported.setSrc(getSrc());
            }
            catch (MethodParameterIsEmptyStringException e1)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e1);
            }
        }
        catch (IsNotInitializedException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
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
        return exported;
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
        String val = source.getAttribute("src");
        if (val == null || val.length() == 0)
            val = ".";
        try
        {
            setSrc(val);
        }
        catch (MethodParameterIsEmptyStringException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        resetClipTimes();
        ITime cbTime, ceTime;
        try
        {
            cbTime = new Time(source.getAttribute("clipBegin"));
            ceTime = new Time(source.getAttribute("clipEnd"));
        }
        catch (MethodParameterIsEmptyStringException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (TimeStringRepresentationIsInvalidException e)
        {
            throw new XukDeserializationFailedException();
        }
        try
        {
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
        catch (TimeOffsetIsOutOfBoundsException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
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
        if (getSrc() != "")
        {
            URI srcUri;
            try
            {
                srcUri = new URI(getSrc());
            }
            catch (URISyntaxException e)
            {
                throw new XukSerializationFailedException();
            }
            if (baseUri.toString() == "")
            {
                destination.writeAttributeString("src", srcUri.toString());
            }
            else
            {
                destination.writeAttributeString("src", baseUri.relativize(
                        srcUri).toString());
            }
        }
        super.xukOutAttributes(destination, baseUri, ph);
    }

    @Override
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

    @Override
    public ExternalAudioMedia splitProtected(ITime splitPoint)
            throws MethodParameterIsNullException,
            TimeOffsetIsOutOfBoundsException
    {
        if (splitPoint == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (splitPoint.isLessThan(getClipBegin()))
        {
            throw new TimeOffsetIsOutOfBoundsException();
        }
        if (splitPoint.isGreaterThan(getClipEnd()))
        {
            throw new TimeOffsetIsOutOfBoundsException();
        }
        ExternalAudioMedia splitAM = copy();
        setClipEnd(splitPoint);
        splitAM.setClipBegin(splitPoint);
        return splitAM;
    }

    @Override
    public boolean ValueEquals(IMedia other)
            throws MethodParameterIsNullException
    {
        if (!super.ValueEquals(other))
            return false;
        ExternalAudioMedia otherAudio = (ExternalAudioMedia) other;
        if (!getClipBegin().isEqualTo(otherAudio.getClipBegin()))
            return false;
        if (!getClipEnd().isEqualTo(otherAudio.getClipEnd()))
            return false;
        if (!(other instanceof ILocated))
        {
            return false;
        }
        try
        {
            if (getURI() != ((ILocated) other).getURI())
                return false;
        }
        catch (URISyntaxException e)
        {
            e.printStackTrace();
            return false;
        }
        return true;
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
