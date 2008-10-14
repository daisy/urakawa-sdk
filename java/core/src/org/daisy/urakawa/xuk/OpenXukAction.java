package org.daisy.urakawa.xuk;

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
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.XmlDataReader;
import org.daisy.urakawa.progress.ProgressAction;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.ProgressInformation;

/**
 *
 */
public class OpenXukAction extends ProgressAction implements
        IEventListener<CancellableEvent>
{
    protected IEventHandler<Event> mEventNotifier = new EventHandler();
    @SuppressWarnings("unused")
    private URI mUri;
    private IXukAble mXukAble;
    private IXmlDataReader mReader;
    private IStream mStream;

    private static IStream getStreamFromUri(URI uri)
    {
        return new FileStream(uri.getPath());
    }

    private void initializeXmlReader(IStream stream)
    {
        mReader = new XmlDataReader(stream);
    }

    /**
     * @param xukAble
     *        cannot be null
     * @param uri
     *        can be null
     * @param reader
     *        cannot be null
     * @throws MethodParameterIsNullException
     */
    public OpenXukAction(IXukAble xukAble, URI uri, IXmlDataReader reader)
            throws MethodParameterIsNullException
    {
        if (xukAble == null || reader == null)
        {
            throw new MethodParameterIsNullException();
        }
        mUri = uri;
        mXukAble = xukAble;
        mReader = reader;
        mStream = mReader.getBaseStream();
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
    public OpenXukAction(IXukAble xukAble, URI uri, IStream stream)
            throws MethodParameterIsNullException
    {
        if (xukAble == null || stream == null)
        {
            throw new MethodParameterIsNullException();
        }
        mUri = uri;
        mXukAble = xukAble;
        mStream = stream;
        initializeXmlReader(mStream);
    }

    /**
     * @param xukAble
     *        cannot be null
     * @param uri
     *        cannot be null
     * @throws MethodParameterIsNullException
     */
    public OpenXukAction(IXukAble xukAble, URI uri)
            throws MethodParameterIsNullException
    {
        if (uri == null || xukAble == null)
        {
            throw new MethodParameterIsNullException();
        }
        mUri = uri;
        mXukAble = xukAble;
        mStream = getStreamFromUri(uri);
        initializeXmlReader(mStream);
    }

    public boolean canExecute()
    {
        return mReader != null;
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

    private void closeInput()
    {
        mReader.close();
        mReader = null;
        mStream = null;
    }

    /**
     * @tagvalue Events "Cancelled-Finished"
     */
    public void execute() throws CommandCannotExecuteException
    {
        mCancelHasBeenRequested = false;
        if (!mReader.readToFollowing("Xuk", IXukAble.XUK_NS))
        {
            mReader.close();
            throw new CommandCannotExecuteException(
                    new XukDeserializationFailedException());
        }
        boolean foundProject = false;
        if (!mReader.isEmptyElement())
        {
            while (mReader.read())
            {
                if (mReader.getNodeType() == IXmlDataReader.ELEMENT)
                {
                    if (mReader.getLocalName() == mXukAble.getXukLocalName()
                            && mReader.getNamespaceURI() == mXukAble
                                    .getXukNamespaceURI())
                    {
                        foundProject = true;
                        try
                        {
                            registerListener(this, CancellableEvent.class);
                        }
                        catch (MethodParameterIsNullException e1)
                        {
                            // Should never happen
                            throw new RuntimeException("WTF ?!", e1);
                        }
                        try
                        {
                            mXukAble.xukIn(mReader, this);
                            
                        }
                        catch (MethodParameterIsNullException e)
                        {
                            // Should never happen
                            throw new RuntimeException("WTF ?!", e);
                        }
                        catch (XukDeserializationFailedException e)
                        {
                            closeInput();
                            throw new CommandCannotExecuteException(e);
                        }
                        catch (ProgressCancelledException e)
                        {
                            notifyCancelled();
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
                            closeInput();
                            notifyFinished();
                        }
                    }
                    else
                    {
                        if (!mReader.isEmptyElement())
                            mReader.readSubtree().close();// Read past unknown
                        // child
                    }
                }
                else
                    if (mReader.getNodeType() == IXmlDataReader.ELEMENT)
                    {
                        break;
                    }
                if (mReader != null && mReader.isEOF())
                {
                    closeInput();
                    throw new CommandCannotExecuteException(
                            new XukDeserializationFailedException());
                }
            }
        }
        if (!foundProject)
        {
            if (mReader != null) closeInput();
            throw new CommandCannotExecuteException(
                    new XukDeserializationFailedException());
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
