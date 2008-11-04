package org.daisy.urakawa.media;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.MalformedURLException;
import java.net.URI;
import java.net.URISyntaxException;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.events.DataModelChangedEvent;
import org.daisy.urakawa.events.Event;
import org.daisy.urakawa.events.EventHandler;
import org.daisy.urakawa.events.IEventHandler;
import org.daisy.urakawa.events.IEventListener;
import org.daisy.urakawa.events.media.SrcChangedEvent;
import org.daisy.urakawa.events.media.TextChangedEvent;
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
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class ExternalTextMedia extends TextMedia implements ILocated
{
    // BEGIN ILocated common code (no multiple-inheritance, unfortunately)
    private String mSrc;

    @Override
    protected void clear()
    {
        mSrc = ".";
        super.clear();
    }

    @Override
    public <K extends DataModelChangedEvent> void notifyListeners(K event)
            throws MethodParameterIsNullException
    {
        if (SrcChangedEvent.class.isAssignableFrom(event.getClass()))
        {
            mSrcChangedEventNotifier.notifyListeners(event);
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
        {
            super.unregisterListener(listener, klass);
        }
    }

    protected IEventHandler<Event> mSrcChangedEventNotifier = new EventHandler();

    /**
     * 
     */
    public ExternalTextMedia()
    {
        super();
        mSrc = ".";
    }

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
    public boolean ValueEquals(IMedia other)
            throws MethodParameterIsNullException
    {
        if (other == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (!super.ValueEquals(other))
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
    public IMedia copyProtected()
    {
        ExternalImageMedia copy = (ExternalImageMedia) super.copyProtected();
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
        return copy;
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
        ExternalImageMedia expEM = (ExternalImageMedia) super
                .exportProtected(destPres);
        if (expEM == null)
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
                expEM.setSrc(destSrc);
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
                expEM.setSrc(getSrc());
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
        return expEM;
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
        super.xukInAttributes(source, ph);
    }

    @Override
    /*
     * @param baseUri can be null, in which case the raw getSrc() value is used
     * without computing the relative value again the base URI
     */
    protected void xukOutAttributes(IXmlDataWriter destination, URI baseUri,
            IProgressHandler ph) throws MethodParameterIsNullException,
            XukSerializationFailedException, ProgressCancelledException
    {
        if (destination == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (ph != null && ph.notifyProgress())
        {
            throw new ProgressCancelledException();
        }
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
            if (baseUri == null)
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

    // END ILocated common code (no multiple-inheritance, unfortunately)
    // ///
    // ///
    // //
    // //
    @Override
    public ExternalTextMedia copy()
    {
        return (ExternalTextMedia) copyProtected();
    }

    @Override
    public ExternalTextMedia export(Presentation destPres)
            throws FactoryCannotCreateTypeException,
            MethodParameterIsNullException
    {
        if (destPres == null)
        {
            throw new MethodParameterIsNullException();
        }
        return (ExternalTextMedia) exportProtected(destPres);
    }

    @Override
    public String getText()
    {
        BufferedReader reader = null;
        try
        {
            reader = new BufferedReader(new InputStreamReader(getURI().toURL()
                    .openStream()));
        }
        catch (MalformedURLException e)
        {
            e.printStackTrace();
            return "";
        }
        catch (IOException e)
        {
            e.printStackTrace();
            return "";
        }
        catch (URISyntaxException e)
        {
            e.printStackTrace();
            return "";
        }
        StringBuffer strText = new StringBuffer();
        String str = null;
        do
        {
            try
            {
                str = reader.readLine();
            }
            catch (IOException e)
            {
                e.printStackTrace();
                return "";
            }
            if (str.length() == 0)
            {
                strText.append("\n");
            }
            else
            {
                strText.append(str);
                strText.append("\n");
            }
        }
        while (str.length() != 0);
        try
        {
            reader.close();
        }
        catch (IOException e)
        {
            e.printStackTrace();
            return "";
        }
        mText = str;
        return str;
    }

    /**
     * @throws CannotWriteToExternalFileException
     *         if the URI scheme is not "file" or "ftp" (HTTP put protocol is
     *         not supported)
     */
    @Override
    public void setText(String text) throws MethodParameterIsNullException
    {
        if (text == null)
        {
            throw new MethodParameterIsNullException();
        }
        String prevTxt = mText;
        URI uri;
        try
        {
            uri = getURI();
        }
        catch (URISyntaxException e)
        {
            e.printStackTrace();
            return;
        }
        if (uri.getScheme() != "file" && uri.getScheme() != "ftp")
        {
            throw new CannotWriteToExternalFileException();
        }
        String path = uri.getPath();
        BufferedWriter writer = null;
        try
        {
            writer = new BufferedWriter(new FileWriter(path));
        }
        catch (IOException e)
        {
            e.printStackTrace();
            return;
        }
        try
        {
            writer.write(text);
        }
        catch (IOException e)
        {
            e.printStackTrace();
        }
        try
        {
            writer.close();
        }
        catch (IOException e)
        {
            e.printStackTrace();
            return;
        }
        mText = text;
        notifyListeners(new TextChangedEvent(this, mText, prevTxt));
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
