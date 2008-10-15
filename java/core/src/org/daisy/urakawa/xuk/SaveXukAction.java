package org.daisy.urakawa.xuk;

import java.io.IOException;
import java.net.URI;

import org.daisy.urakawa.command.CommandCannotExecuteException;
import org.daisy.urakawa.events.CancellableEvent;
import org.daisy.urakawa.events.Event;
import org.daisy.urakawa.events.EventHandler;
import org.daisy.urakawa.events.IEventHandler;
import org.daisy.urakawa.events.IEventListener;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.nativeapi.FileStream;
import org.daisy.urakawa.nativeapi.IStream;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.nativeapi.XmlDataWriter;
import org.daisy.urakawa.progress.ProgressAction;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.ProgressInformation;

/**
 *
 */
public class SaveXukAction extends ProgressAction implements
        IEventListener<CancellableEvent>
{
    protected IEventHandler<Event> mEventNotifier = new EventHandler();
    private URI mUri;
    private IXukAble mXukAble;
    private IStream mStream;
    private IXmlDataWriter mWriter;

    private static IStream getStreamFromUri(URI uri)
    {
        return new FileStream(uri.getPath());
    }

    private void initializeXmlWriter(IStream stream)
    {
        mWriter = new XmlDataWriter(stream);
    }

    /**
     * @param xukAble
     *        cannot be null
     * @param uri
     *        can be null
     * @param writer
     *        cannot be null
     * @throws MethodParameterIsNullException
     */
    public SaveXukAction(IXukAble xukAble, URI uri, IXmlDataWriter writer)
            throws MethodParameterIsNullException
    {
        if (xukAble == null || writer == null)
        {
            throw new MethodParameterIsNullException();
        }
        mUri = uri;
        mXukAble = xukAble;
        mWriter = writer;
        mStream = mWriter.getBaseStream();
    }

    /**
     * @param xukAble
     *        cannot be null
     * @param uri
     *        can be null
     * @param stream
     *        cannot be null
     * @throws MethodParameterIsNullException
     */
    public SaveXukAction(IXukAble xukAble, URI uri, IStream stream)
            throws MethodParameterIsNullException
    {
        if (xukAble == null || stream == null)
        {
            throw new MethodParameterIsNullException();
        }
        mUri = uri;
        mXukAble = xukAble;
        mStream = stream;
        initializeXmlWriter(mStream);
    }

    /**
     * @param xukAble
     *        cannot be null
     * @param uri
     *        cannot be null
     * @throws MethodParameterIsNullException
     */
    public SaveXukAction(IXukAble xukAble, URI uri)
            throws MethodParameterIsNullException
    {
        if (uri == null || xukAble == null)
        {
            throw new MethodParameterIsNullException();
        }
        mUri = uri;
        mXukAble = xukAble;
        mStream = getStreamFromUri(uri);
        initializeXmlWriter(mStream);
    }

    @Override
    public ProgressInformation getProgressInfo()
    {
        if (mStream == null)
        {
            return null;
        }
        ProgressInformation pi = null;
        try
        {
            pi = new ProgressInformation(mStream.getLength(), mStream
                    .getPosition());
        }
        catch (MethodParameterIsOutOfBoundsException e)
        {
            e.printStackTrace();
            return null;
        }
        return pi;
    }

    public boolean canExecute()
    {
        return mWriter != null;
    }

    private void closeOutput()
    {
        mWriter.close();
        mWriter = null;
        try
        {
            mStream.close();
        }
        catch (IOException e)
        {
            e.printStackTrace();
        }
        mStream = null;
    }

    /**
     * @tagvalue Events "Cancelled-Finished"
     */
    public void execute() throws CommandCannotExecuteException
    {
        mCancelHasBeenRequested = false;
        mWriter.writeStartDocument();
        mWriter.writeStartElement("Xuk", IXukAble.XUK_NS);
        if (IXukAble.XUK_XSD_PATH != "")
        {
            if (IXukAble.XUK_NS.length() == 0)
            {
                mWriter.writeAttributeString("xsi",
                        "noNamespaceSchemaLocation",
                        "http://www.w3.org/2001/XMLSchema-instance",
                        IXukAble.XUK_XSD_PATH);
            }
            else
            {
                mWriter.writeAttributeString("xsi",
                        "noNamespaceSchemaLocation",
                        "http://www.w3.org/2001/XMLSchema-instance",
                        IXukAble.XUK_NS + " " + IXukAble.XUK_XSD_PATH);
            }
        }
        try
        {
            registerListener(this, CancellableEvent.class);
        }
        catch (MethodParameterIsNullException e1)
        {
            // Should never happen
            throw new RuntimeException("WTF ?!", e1);
        }
        boolean canceled = false;
        try
        {
            mXukAble.xukOut(mWriter, mUri, this);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ?!", e);
        }
        catch (XukSerializationFailedException e)
        {
            closeOutput();
            throw new CommandCannotExecuteException(e);
        }
        catch (ProgressCancelledException e)
        {
            canceled = true;
        }
        finally
        {
            try
            {
                unregisterListener(this, CancellableEvent.class);
            }
            catch (MethodParameterIsNullException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ?!", e);
            }
            mWriter.writeEndElement();
            mWriter.writeEndDocument();
            closeOutput();
            if (canceled)
                notifyCancelled();
            else
                notifyFinished();
        }
    }

    public String getLongDescription()
    {
        return null;
    }

    public String getShortDescription()
    {
        return null;
    }

    @SuppressWarnings("unused")
    public void setLongDescription(String str)
            throws MethodParameterIsNullException
    {
        /**
         * Does nothing.
         */
    }

    @SuppressWarnings("unused")
    public void setShortDescription(String str)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException
    {
        /**
         * Does nothing.
         */
    }

    public <K extends Event> void notifyListeners(K event)
            throws MethodParameterIsNullException
    {
        if (event == null)
        {
            throw new MethodParameterIsNullException();
        }
        mEventNotifier.notifyListeners(event);
    }

    public <K extends Event> void registerListener(IEventListener<K> listener,
            Class<K> klass) throws MethodParameterIsNullException
    {
        if (listener == null || klass == null)
        {
            throw new MethodParameterIsNullException();
        }
        mEventNotifier.registerListener(listener, klass);
    }

    public <K extends Event> void unregisterListener(
            IEventListener<K> listener, Class<K> klass)
            throws MethodParameterIsNullException
    {
        if (listener == null || klass == null)
        {
            throw new MethodParameterIsNullException();
        }
        mEventNotifier.unregisterListener(listener, klass);
    }

    public <K extends CancellableEvent> void eventCallback(K event)
            throws MethodParameterIsNullException
    {
        if (event == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (event.isCancelled())
        {
            requestCancel();
        }
    }
}
