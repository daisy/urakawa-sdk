package org.daisy.urakawa.media.data;

import java.io.File;
import java.io.IOException;
import java.net.URI;
import java.net.URISyntaxException;
import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.IPresentation;
import org.daisy.urakawa.AbstractXukAbleWithPresentation;
import org.daisy.urakawa.exception.IsAlreadyManagerOfException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.IsNotManagerOfException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.CloseNotifyingStream;
import org.daisy.urakawa.nativeapi.FileStream;
import org.daisy.urakawa.nativeapi.IStream;
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
public class FileDataProvider extends AbstractXukAbleWithPresentation implements
        IFileDataProvider
{
    public void initialize(String relPath, String mimeType)
    {
        try
        {
            getPresentation().getDataProviderManager().addDataProvider(this);
        }
        catch (IsAlreadyManagerOfException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (MethodParameterIsEmptyStringException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (IsNotManagerOfException e)
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
        mDataFileRelativePath = relPath;
        mMimeType = mimeType;
    }

    private String mDataFileRelativePath;
    List<CloseNotifyingStream> mOpenInputStreams = new LinkedList<CloseNotifyingStream>();
    CloseNotifyingStream mOpenOutputStream = null;

    public String getDataFileRelativePath()
    {
        return mDataFileRelativePath;
    }

    public String getDataFileFullPath()
    {
        try
        {
            return new File(getPresentation().getDataProviderManager()
                    .getDataFileDirectoryFullPath(), mDataFileRelativePath)
                    .getAbsolutePath();
        }
        catch (IsNotInitializedException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (URISyntaxException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    private boolean hasBeenInitialized = false;

    public String getUid()
    {
        try
        {
            return getPresentation().getDataProviderManager()
                    .getUidOfDataProvider(this);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (IsNotManagerOfException e)
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

    private void checkDataFile() throws DataIsMissingException
    {
        File file = new File(getDataFileFullPath());
        File dir = file.getParentFile();
        if (!dir.exists())
            try
            {
                dir.createNewFile();
            }
            catch (IOException e)
            {
                throw new DataIsMissingException();
            }
        if (file.exists())
        {
            if (!hasBeenInitialized)
            {
                file.delete();
            }
            else
            {
                return;
            }
        }
        if (hasBeenInitialized)
        {
            throw new DataIsMissingException();
        }
        try
        {
            file.createNewFile();
        }
        catch (IOException e)
        {
            throw new DataIsMissingException();
        }
        hasBeenInitialized = true;
    }

    public IStream getInputStream() throws OutputStreamIsOpenException,
            DataIsMissingException
    {
        if (mOpenOutputStream != null)
        {
            throw new OutputStreamIsOpenException();
        }
        FileStream inputFS;
        String fp = getDataFileFullPath();
        checkDataFile();
        inputFS = new FileStream(fp);
        CloseNotifyingStream res;
        try
        {
            res = new CloseNotifyingStream(inputFS);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        mOpenInputStreams.add(res);
        return res;
    }

    public IStream getOutputStream() throws OutputStreamIsOpenException,
            InputStreamIsOpenException, DataIsMissingException
    {
        FileStream outputFS;
        if (mOpenOutputStream != null)
        {
            throw new OutputStreamIsOpenException();
        }
        if (mOpenInputStreams.size() > 0)
        {
            throw new InputStreamIsOpenException();
        }
        checkDataFile();
        String fp = getDataFileFullPath();
        outputFS = new FileStream(fp);
        try
        {
            mOpenOutputStream = new CloseNotifyingStream(outputFS);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        return mOpenOutputStream;
    }

    public void delete() throws OutputStreamIsOpenException,
            InputStreamIsOpenException
    {
        if (mOpenOutputStream != null)
        {
            throw new OutputStreamIsOpenException();
        }
        if (mOpenInputStreams.size() > 0)
        {
            throw new InputStreamIsOpenException();
        }
        File file = new File(getDataFileFullPath());
        if (file.exists())
        {
            file.delete();
        }
        try
        {
            getPresentation().getDataProviderManager().removeDataProvider(this,
                    false);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (IsNotManagerOfException e)
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

    public IFileDataProvider copy()
    {
        IFileDataProvider c;
        try
        {
            c = getPresentation().getDataProviderFactory().create(
                    getMimeType(), getXukLocalName(), getXukNamespaceURI());
        }
        catch (MethodParameterIsNullException e2)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e2);
        }
        catch (MethodParameterIsEmptyStringException e2)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e2);
        }
        catch (IsNotInitializedException e2)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e2);
        }
        IStream thisData;
        try
        {
            thisData = getInputStream();
        }
        catch (OutputStreamIsOpenException e1)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e1);
        }
        catch (DataIsMissingException e1)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e1);
        }
        try
        {
            new DataProviderManager().appendDataToProvider(thisData, thisData
                    .getLength()
                    - thisData.getPosition(), c);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (OutputStreamIsOpenException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (InputStreamIsOpenException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (DataIsMissingException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (IOException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (InputStreamIsTooShortException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        finally
        {
            try
            {
                thisData.close();
            }
            catch (IOException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
        return c;
    }

    private String mMimeType;

    public String getMimeType()
    {
        return mMimeType;
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
        mDataFileRelativePath = source.getAttribute("dataFileRelativePath");
        if (mDataFileRelativePath == null
                || mDataFileRelativePath.length() == 0)
        {
            throw new XukDeserializationFailedException();
        }
        hasBeenInitialized = true;// Assume that the data file exists
        mMimeType = source.getAttribute("mimeType");
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
        try
        {
            checkDataFile();
        }
        catch (DataIsMissingException e)
        {
            throw new XukSerializationFailedException();
        }// Ensure that data file exist even if no data has yet been written
        // to it.
        destination.writeAttributeString("dataFileRelativePath",
                getDataFileRelativePath());
        destination.writeAttributeString("mimeType", getMimeType());
    }

    @Override
    @SuppressWarnings("unused")
    protected void xukOutChildren(IXmlDataWriter destination, URI baseUri,
            IProgressHandler ph) throws ProgressCancelledException
    {
        if (ph != null && ph.notifyProgress())
        {
            throw new ProgressCancelledException();
        }
    }

    public boolean ValueEquals(IDataProvider other)
            throws MethodParameterIsNullException
    {
        if (other == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (getClass() != other.getClass())
            return false;
        IFileDataProvider o = (IFileDataProvider) other;
        if (o.getMimeType() != getMimeType())
            return false;
        try
        {
            if (!new DataProviderManager().compareDataProviderContent(this, o))
                return false;
        }
        catch (DataIsMissingException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (OutputStreamIsOpenException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (IOException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        return true;
    }

    public IFileDataProvider export(IPresentation destPres)
            throws FactoryCannotCreateTypeException,
            MethodParameterIsNullException
    {
        if (destPres == null)
        {
            throw new MethodParameterIsNullException();
        }
        IFileDataProvider expFDP;
        try
        {
            expFDP = destPres.getDataProviderFactory().create(getMimeType(),
                    getXukLocalName(), getXukNamespaceURI());
        }
        catch (MethodParameterIsEmptyStringException e1)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e1);
        }
        if (expFDP == null)
        {
            throw new FactoryCannotCreateTypeException();
        }
        IStream thisStm;
        try
        {
            thisStm = getInputStream();
        }
        catch (OutputStreamIsOpenException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (DataIsMissingException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        try
        {
            try
            {
                new DataProviderManager().appendDataToProvider(thisStm, thisStm
                        .getLength(), expFDP);
            }
            catch (OutputStreamIsOpenException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
            catch (InputStreamIsOpenException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
            catch (DataIsMissingException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
            catch (IOException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
            catch (InputStreamIsTooShortException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
        finally
        {
            try
            {
                thisStm.close();
            }
            catch (IOException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
        return expFDP;
    }

    @Override
    protected void clear()
    {
        /**
         * Does nothing.
         */
    }
}
