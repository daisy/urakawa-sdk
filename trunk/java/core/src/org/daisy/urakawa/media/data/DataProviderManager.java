package org.daisy.urakawa.media.data;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.net.URI;
import java.net.URISyntaxException;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.daisy.urakawa.IValueEquatable;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.events.IEventListener;
import org.daisy.urakawa.events.presentation.RootUriChangedEvent;
import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsAlreadyManagerOfException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.IsNotManagerOfException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.nativeapi.IStream;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.xuk.AbstractXukAble;
import org.daisy.urakawa.xuk.IXukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 * @depend - Composition 0..n org.daisy.urakawa.media.data.IDataProvider
 */
public final class DataProviderManager extends AbstractXukAble implements
        IValueEquatable<DataProviderManager>
{
    private Presentation mPresentation;

    /**
     * @return the Presentation owner
     */
    public Presentation getPresentation()
    {
        return mPresentation;
    }

    private Map<String, IDataProvider> mDataProvidersDictionary = new HashMap<String, IDataProvider>();
    private Map<IDataProvider, String> mReverseLookupDataProvidersDictionary = new HashMap<IDataProvider, String>();
    private List<String> mXukedInFilDataProviderPaths = new LinkedList<String>();
    private String mDataFileDirectory;
    protected IEventListener<RootUriChangedEvent> mRootUriChangedEventListener = new IEventListener<RootUriChangedEvent>()
    {
        public <K extends RootUriChangedEvent> void eventCallback(K event)
                throws MethodParameterIsNullException
        {
            if (event == null)
            {
                throw new MethodParameterIsNullException();
            }
            if (event.getPreviousUri() != null)
            {
                // TODO: copy media data
                String prevDataDirFullPath;
                try
                {
                    prevDataDirFullPath = DataProviderManager.this.getDataFileDirectoryFullPath(event.getPreviousUri());
                }
                catch (URISyntaxException e)
                {
                    // Should never happen
                    throw new RuntimeException("WTF ??!", e);
                }
                if (new File(prevDataDirFullPath).exists())
                {
                    try
                    {
                        DataProviderManager.this.copyDataFiles(prevDataDirFullPath, getDataFileDirectoryFullPath());
                    }
                    catch (MethodParameterIsEmptyStringException e)
                    {
                        // Should never happen
                        throw new RuntimeException("WTF ??!", e);
                    }
                    catch (IOException e)
                    {
                        // Should never happen
                        throw new RuntimeException("WTF ??!", e);
                    }
                    catch (DataIsMissingException e)
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

            }
        }
    };

    /**
     * @param pres
     * @throws MethodParameterIsNullException
     */
    public DataProviderManager(Presentation pres)
            throws MethodParameterIsNullException
    {
        if (pres == null)
        {
            throw new MethodParameterIsNullException();
        }
        mPresentation = pres;
        mDataFileDirectory = null;
        mPresentation.registerListener(mRootUriChangedEventListener,
                RootUriChangedEvent.class);
    }

    /**
     * Appends data from a given input IStream to a given IDataProvider
     * 
     * @param data
     * @param count
     * @param provider
     * @throws MethodParameterIsNullException
     * @throws DataIsMissingException
     * @throws InputStreamIsOpenException
     * @throws OutputStreamIsOpenException
     * @throws InputStreamIsTooShortException
     * @throws IOException
     */
    public void appendDataToProvider(IStream data, int count,
            IDataProvider provider) throws MethodParameterIsNullException,
            OutputStreamIsOpenException, InputStreamIsOpenException,
            DataIsMissingException, IOException, InputStreamIsTooShortException
    {
        if (data == null || provider == null)
        {
            throw new MethodParameterIsNullException();
        }
        IStream provOutputStream = provider.getOutputStream();
        try
        {
            provOutputStream.seek(data.getLength());
            int bytesAppended = 0;
            byte[] buf = new byte[1024];
            while (bytesAppended < count)
            {
                if (bytesAppended + buf.length >= count)
                {
                    buf = new byte[count - bytesAppended];
                }
                if (data.read(buf, 0, buf.length) != buf.length)
                {
                    throw new InputStreamIsTooShortException();
                }
                provOutputStream.write(buf, 0, buf.length);
                bytesAppended += buf.length;
            }
        }
        finally
        {
            provOutputStream.close();
        }
    }

    /**
     * Compares the data content of two data providers to check for value
     * equality
     * 
     * @param dp1
     * @param dp2
     * @return true or false
     * @throws MethodParameterIsNullException
     * @throws OutputStreamIsOpenException
     * @throws DataIsMissingException
     * @throws IOException
     */
    public boolean compareDataProviderContent(IDataProvider dp1,
            IDataProvider dp2) throws MethodParameterIsNullException,
            DataIsMissingException, OutputStreamIsOpenException, IOException
    {
        if (dp1 == null || dp2 == null)
        {
            throw new MethodParameterIsNullException();
        }
        IStream s1 = null;
        IStream s2 = null;
        boolean allEq = true;
        try
        {
            s1 = dp1.getInputStream();
            s2 = dp2.getInputStream();
            allEq = ((s1.getLength() - s1.getPosition()) == (s2.getLength() - s2
                    .getPosition()));
            while (allEq && (s1.getPosition() < s1.getLength()))
            {
                if (s1.readByte() != s2.readByte())
                {
                    allEq = false;
                    break;
                }
            }
        }
        finally
        {
            if (s1 != null)
                s1.close();
            if (s2 != null)
                s2.close();
        }
        return allEq;
    }

    /**
     * Gets the path of the data file directory used by the FileDataProviders
     * managed by this, relative to the base uri of the MediaDataPresentation
     * owning the file data provider manager. The DataFileDirectory is
     * initialized lazily: If the DataFileDirectory has not been explicitly
     * initialized using the setDataFileDirectory() method, calling
     * getDataFileDirectory() will assign it the default value "Data"
     * 
     * @return directory
     */
    public String getDataFileDirectory()
    {
        if (mDataFileDirectory == null)
            mDataFileDirectory = "Data";
        return mDataFileDirectory;
    }

    /**
     * Initializes the IFileDataProvider with a DataFileDirectory
     * 
     * @param dataDir
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws MethodParameterIsEmptyStringException
     *         Empty string '' method parameters are forbidden
     * @throws IsAlreadyInitializedException
     * @throws URISyntaxException
     */
    public void setDataFileDirectory(String dataDir)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException,
            IsAlreadyInitializedException, URISyntaxException
    {
        if (dataDir == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (dataDir.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        if (mDataFileDirectory != null)
        {
            throw new IsAlreadyInitializedException();
        }
        @SuppressWarnings("unused")
        URI tmp = new URI(mDataFileDirectory);
        mDataFileDirectory = dataDir;
    }

    /**
     * Moves the data file directory of the manager
     * 
     * @param newDataFileDir
     * @param deleteSource
     * @param overwriteDestDir
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws MethodParameterIsEmptyStringException
     *         Empty string '' method parameters are forbidden
     * @throws MethodParameterIsOutOfBoundsException
     * @throws DataIsMissingException
     * @throws IOException
     */
    public void moveDataFiles(String newDataFileDir, boolean deleteSource,
            boolean overwriteDestDir) throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException,
            MethodParameterIsOutOfBoundsException, IOException,
            DataIsMissingException
    {
        if (newDataFileDir == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (newDataFileDir.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        File file = new File(newDataFileDir);
        if (file.isAbsolute())
        {
            throw new MethodParameterIsOutOfBoundsException();
        }
        String oldPath = null;
        String newPath = null;
        try
        {
            oldPath = getDataFileDirectoryFullPath();
            newPath = getDataFileDirectoryFullPath();
        }
        catch (URISyntaxException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        mDataFileDirectory = newDataFileDir;
        File fileNew = new File(newPath);
        if (fileNew.exists())
        {
            if (overwriteDestDir)
                fileNew.delete();
        }
        copyDataFiles(oldPath, newPath);
        File fileOld = new File(oldPath);
        if (deleteSource && fileOld.exists())
        {
            fileOld.delete();
        }
    }

    /**
     * @hidden
     */
    private void createDirectory(String path) throws IOException,
            MethodParameterIsNullException,
            MethodParameterIsEmptyStringException
    {
        if (path == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (path.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        File file = new File(path);
        if (!file.exists())
        {
            File parent = file.getParentFile();
            if (!parent.exists())
                createDirectory(parent.getAbsolutePath());
            file.createNewFile();
        }
    }

    /**
     * @hidden
     */
    protected void copyDataFiles(String source, String dest)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException, IOException,
            DataIsMissingException
    {
        if (source == null || dest == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (source.length() == 0 || dest.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        createDirectory(dest);
        for (IFileDataProvider fdp : getListOfManagedFileDataProviders())
        {
            File file = new File(source, fdp.getDataFileRelativePath());
            if (!file.exists())
            {
                throw new DataIsMissingException();
            }
            File file2 = new File(dest, fdp.getDataFileRelativePath());
            file2.createNewFile();
            FileInputStream fis = new FileInputStream(file);
            FileOutputStream fos = new FileOutputStream(file2);
            byte[] b = new byte[4096];
            @SuppressWarnings("unused")
            int read = 0;
            while ((read = fis.read(b)) > 0)
            {
                fos.write(b);
            }
            fis.close();
            fos.close();
        }
    }

    /**
     * @hidden
     */
    protected String getDataFileDirectoryFullPath(URI baseUri)
            throws URISyntaxException
    {
        if (baseUri.getScheme() != "file")
        {
            throw new URISyntaxException(
                    baseUri.toString(),
                    "The base Uri of the presentation to which the DataProviderManager belongs must be a file Uri");
        }
        URI dataFileDirUri = new URI(getDataFileDirectory());
        dataFileDirUri.relativize(baseUri);
        return dataFileDirUri.getPath();
    }

    /**
     * Gets the full path of the data file directory
     * 
     * @return path
     * @throws URISyntaxException
     */
    public String getDataFileDirectoryFullPath() throws URISyntaxException
    {
        return getDataFileDirectoryFullPath(getPresentation().getRootURI());
    }

    /**
     * Initializer that sets the path of the data file directory used by
     * FileDataProviders managed by this
     * 
     * @param path
     * @throws MethodParameterIsNullException
     * @throws MethodParameterIsEmptyStringException
     * @throws IsAlreadyInitializedException
     * @throws IOException
     */
    public void setDataFileDirectoryPath(String path)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException,
            IsAlreadyInitializedException, IOException
    {
        if (path == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (path.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        if (mDataFileDirectory != null)
        {
            throw new IsAlreadyInitializedException();
        }
        File file = new File(path);
        if (!file.exists())
        {
            file.createNewFile();
        }
        mDataFileDirectory = path;
    }

    /**
     * Gets a new data file path relative to the path of the data file directory
     * of the manager
     * 
     * @param extension
     * @return the relative path
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws MethodParameterIsEmptyStringException
     *         Empty string '' method parameters are forbidden
     */
    public String getNewDataFileRelPath(String extension)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException
    {
        if (extension == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (extension.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        String res;
        while (true)
        {
            res = generateRandomFileName(extension);
            for (IFileDataProvider prov : getListOfManagedFileDataProviders())
            {
                if (res.toLowerCase() == prov.getDataFileRelativePath()
                        .toLowerCase())
                {
                    continue;
                }
            }
            break;
        }
        return res;
    }

    /**
     * @hidden
     */
    private String generateRandomFileName(String extension)
    {
        // TODO: generate random string
        return "test." + extension;
    }

    /**
     * Gets a list of the FileDataProviders managed by the manager
     * 
     * @return a non-null but potentially empty list
     */
    public List<IFileDataProvider> getListOfManagedFileDataProviders()
    {
        List<IFileDataProvider> res = new LinkedList<IFileDataProvider>();
        for (IDataProvider prov : getListOfDataProviders())
        {
            if (prov instanceof IFileDataProvider)
            {
                res.add((IFileDataProvider) prov);
            }
        }
        return res;
    }

    /**
     * Removes one of the IDataProvider managed by the manager
     * 
     * @param provider
     * @param delete
     * @throws MethodParameterIsNullException
     * @throws IsNotManagerOfException
     */
    public void removeDataProvider(IDataProvider provider, boolean delete)
            throws MethodParameterIsNullException, IsNotManagerOfException
    {
        if (provider == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (delete)
        {
            try
            {
                provider.delete();
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
        }
        else
        {
            String uid = getUidOfDataProvider(provider);
            try
            {
                removeDataProvider(uid, provider);
            }
            catch (MethodParameterIsEmptyStringException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
    }

    /**
     * Removes the IDataProvider with a given UID from the // manager
     * 
     * @param uid
     * @param delete
     * @throws MethodParameterIsNullException
     * @throws IsNotManagerOfException
     * @throws MethodParameterIsEmptyStringException
     */
    public void removeDataProvider(String uid, boolean delete)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException, IsNotManagerOfException
    {
        if (uid == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (uid.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        IDataProvider provider = getDataProvider(uid);
        if (delete)
        {
            try
            {
                provider.delete();
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
        }
        else
        {
            removeDataProvider(uid, provider);
        }
    }

    /**
     * @hidden
     */
    private void removeDataProvider(String uid, IDataProvider provider)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException
    {
        if (uid == null || provider == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (uid.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        mDataProvidersDictionary.remove(uid);
        mReverseLookupDataProvidersDictionary.remove(provider);
    }

    /**
     * Gets the UID of a given IDataProvider
     * 
     * @param provider
     * @return the UID
     * @throws MethodParameterIsNullException
     * @throws IsNotManagerOfException
     */
    public String getUidOfDataProvider(IDataProvider provider)
            throws MethodParameterIsNullException, IsNotManagerOfException
    {
        if (provider == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (!mReverseLookupDataProvidersDictionary.containsKey(provider))
        {
            throw new IsNotManagerOfException();
        }
        return mReverseLookupDataProvidersDictionary.get(provider);
    }

    /**
     * Gets the IDataProvider with a given UID
     * 
     * @param uid
     * @return the provider
     * @throws IsNotManagerOfException
     * @throws MethodParameterIsNullException
     * @throws MethodParameterIsEmptyStringException
     */
    public IDataProvider getDataProvider(String uid)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException, IsNotManagerOfException
    {
        if (uid == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (uid.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        if (!mDataProvidersDictionary.containsKey(uid))
        {
            throw new IsNotManagerOfException();
        }
        return mDataProvidersDictionary.get(uid);
    }

    /**
     * @hidden
     */
    private void addDataProvider(IDataProvider provider, String uid)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException, IsNotManagerOfException,
            IsAlreadyManagerOfException
    {
        if (provider == null || uid == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (uid.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        if (mReverseLookupDataProvidersDictionary.containsKey(provider))
        {
            throw new IsAlreadyManagerOfException();
        }
        if (mDataProvidersDictionary.containsKey(uid))
        {
            throw new IsAlreadyManagerOfException();
        }
        try
        {
            if (provider.getPresentation().getDataProviderManager() != this)
            {
                throw new IsNotManagerOfException();
            }
        }
        catch (IsNotInitializedException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        mDataProvidersDictionary.put(uid, provider);
        mReverseLookupDataProvidersDictionary.put(provider, uid);
    }

    /**
     * Adds a IDataProvider to the DataProviderManager
     * 
     * @param provider
     * @throws MethodParameterIsNullException
     * @throws IsAlreadyManagerOfException
     * @throws IsNotManagerOfException
     * @throws MethodParameterIsEmptyStringException
     */
    public void addDataProvider(IDataProvider provider)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException, IsNotManagerOfException,
            IsAlreadyManagerOfException
    {
        if (provider == null)
        {
            throw new MethodParameterIsNullException();
        }
        addDataProvider(provider, getNextUid());
    }

    /**
     * @hidden
     */
    @SuppressWarnings("boxing")
    private String getNextUid()
    {
        long i = 0;
        while (i < Integer.MAX_VALUE)
        {
            String newId = String.format("DPID{0:0000}", i);
            if (!mDataProvidersDictionary.containsKey(newId))
                return newId;
            i++;
        }
        // Should never happen
        throw new RuntimeException("WTF ??!");
    }

    /**
     * Determines if the manager manages a IDataProvider with a given uid
     * 
     * @param uid
     * @return true or false
     * @throws MethodParameterIsNullException
     * @throws MethodParameterIsEmptyStringException
     */
    public boolean isManagerOf(String uid)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException
    {
        if (uid == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (uid.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        return mDataProvidersDictionary.containsKey(uid);
    }

    /**
     * Sets the uid of a given managed IDataProvider to a given value
     * 
     * @param provider
     * @param uid
     * @throws MethodParameterIsNullException
     * @throws IsAlreadyManagerOfException
     * @throws IsNotManagerOfException
     * @throws MethodParameterIsEmptyStringException
     */
    public void setDataProviderUid(IDataProvider provider, String uid)
            throws MethodParameterIsNullException, IsNotManagerOfException,
            MethodParameterIsEmptyStringException, IsAlreadyManagerOfException
    {
        if (provider == null || uid == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (uid.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        removeDataProvider(provider, false);
        addDataProvider(provider, uid);
    }

    /**
     * Gets a list of the DataProviders that are managed by the
     * DataProviderManager
     * 
     * @return a non-null but potentially empty list
     */
    public List<IDataProvider> getListOfDataProviders()
    {
        return new LinkedList<IDataProvider>(mDataProvidersDictionary.values());
    }

    /**
     * Removes any DataProviders "not used", that is all IDataProvider that are
     * not used by a IMediaData of the Presentation
     * 
     * @param delete
     */
    public void removeUnusedDataProviders(boolean delete)
    {
        List<IDataProvider> usedDataProviders = new LinkedList<IDataProvider>();
        for (IMediaData md : getPresentation().getMediaDataManager()
                .getListOfMediaData())
        {
            for (IDataProvider prov : md.getListOfUsedDataProviders())
            {
                if (!usedDataProviders.contains(prov))
                    usedDataProviders.add(prov);
            }
        }
        for (IDataProvider prov : getListOfDataProviders())
        {
            if (!usedDataProviders.contains(prov))
            {
                try
                {
                    removeDataProvider(prov, delete);
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
            }
        }
    }

    /**
     * @hidden
     */
    @Override
    protected void clear()
    {
        mDataProvidersDictionary.clear();
        mDataFileDirectory = null;
        mReverseLookupDataProvidersDictionary.clear();
        mXukedInFilDataProviderPaths.clear();
        // super.clear();
    }

    /**
     * @hidden
     */
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
        String dataFileDirectoryPath = source
                .getAttribute("dataFileDirectoryPath");
        if (dataFileDirectoryPath == null
                || dataFileDirectoryPath.length() == 0)
        {
            throw new XukDeserializationFailedException();
        }
        try
        {
            setDataFileDirectoryPath(dataFileDirectoryPath);
        }
        catch (MethodParameterIsEmptyStringException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (IsAlreadyInitializedException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (IOException e)
        {
            throw new XukDeserializationFailedException();
        }
    }

    /**
     * @hidden
     */
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
        if (source.getNamespaceURI() == IXukAble.XUK_NS)
        {
            readItem = true;
            if (source.getLocalName() == "mDataProviders")
            {
                xukInDataProviders(source, ph);
            }
            else
            {
                readItem = false;
            }
        }
        if (!readItem)
        {
            super.xukInChild(source, ph);
        }
    }

    /**
     * @hidden
     */
    private void xukInDataProviders(IXmlDataReader source, IProgressHandler ph)
            throws MethodParameterIsNullException,
            XukDeserializationFailedException, ProgressCancelledException
    {
        if (source == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (ph != null && ph.notifyProgress())
        {
            throw new ProgressCancelledException();
        }
        if (!source.isEmptyElement())
        {
            while (source.read())
            {
                if (source.getNodeType() == IXmlDataReader.ELEMENT)
                {
                    if (source.getLocalName() == "mDataProviderItem"
                            && source.getNamespaceURI() == IXukAble.XUK_NS)
                    {
                        xukInDataProviderItem(source, ph);
                    }
                    else
                    {
                        super.xukInChild(source, ph);
                    }
                }
                else
                    if (source.getNodeType() == IXmlDataReader.END_ELEMENT)
                    {
                        break;
                    }
                if (source.isEOF())
                    throw new XukDeserializationFailedException();
            }
        }
    }

    /**
     * @hidden
     */
    private void xukInDataProviderItem(IXmlDataReader source,
            IProgressHandler ph) throws MethodParameterIsNullException,
            XukDeserializationFailedException, ProgressCancelledException
    {
        if (source == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (ph != null && ph.notifyProgress())
        {
            throw new ProgressCancelledException();
        }
        String uid = source.getAttribute("uid");
        if (!source.isEmptyElement())
        {
            boolean addedProvider = false;
            while (source.read())
            {
                if (source.getNodeType() == IXmlDataReader.ELEMENT)
                {
                    IDataProvider prov;
                    try
                    {
                        prov = getPresentation().getDataProviderFactory()
                                .createFileDataProvider("",
                                        source.getLocalName(),
                                        source.getNamespaceURI());
                    }
                    catch (MethodParameterIsEmptyStringException e)
                    {
                        // Should never happen
                        throw new RuntimeException("WTF ??!", e);
                    }
                    if (prov != null)
                    {
                        if (addedProvider)
                        {
                            throw new XukDeserializationFailedException();
                        }
                        prov.xukIn(source, ph);
                        if (prov instanceof IFileDataProvider)
                        {
                            IFileDataProvider fdProv = (IFileDataProvider) prov;
                            if (mXukedInFilDataProviderPaths.contains(fdProv
                                    .getDataFileRelativePath().toLowerCase()))
                            {
                                throw new XukDeserializationFailedException();
                            }
                            mXukedInFilDataProviderPaths.add(fdProv
                                    .getDataFileRelativePath().toLowerCase());
                        }
                        if (uid == null || uid.length() == 0)
                        {
                            throw new XukDeserializationFailedException();
                        }
                        try
                        {
                            if (isManagerOf(uid))
                            {
                                if (getDataProvider(uid) != prov)
                                {
                                    throw new XukDeserializationFailedException();
                                }
                            }
                            else
                            {
                                setDataProviderUid(prov, uid);
                            }
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
                        catch (IsAlreadyManagerOfException e)
                        {
                            // Should never happen
                            throw new RuntimeException("WTF ??!", e);
                        }
                        addedProvider = true;
                    }
                    else
                    {
                        super.xukInChild(source, ph);
                    }
                }
                else
                    if (source.getNodeType() == IXmlDataReader.END_ELEMENT)
                    {
                        break;
                    }
                if (source.isEOF())
                    throw new XukDeserializationFailedException();
            }
        }
    }

    /**
     * @hidden
     */
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
        URI presBaseUri;
        presBaseUri = getPresentation().getRootURI();
        URI dfdUri;
        try
        {
            dfdUri = new URI(getDataFileDirectory());
        }
        catch (URISyntaxException e)
        {
            throw new XukSerializationFailedException();
        }
        dfdUri = dfdUri.relativize(presBaseUri);
        destination.writeAttributeString("dataFileDirectoryPath", presBaseUri
                .relativize(dfdUri).toString());
        // super.xukOutAttributes(destination, baseUri, ph);
    }

    /**
     * @hidden
     */
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
        destination.writeStartElement("mDataProviders", IXukAble.XUK_NS);
        for (IDataProvider prov : getListOfDataProviders())
        {
            destination.writeStartElement("mDataProviderItem", IXukAble.XUK_NS);
            destination.writeAttributeString("uid", prov.getUid());
            prov.xukOut(destination, baseUri, ph);
            destination.writeEndElement();
        }
        destination.writeEndElement();
        // super.xukOutChildren(destination, baseUri, ph);
    }

    /**
     * @hidden
     */
    public boolean ValueEquals(DataProviderManager other)
            throws MethodParameterIsNullException
    {
        if (other == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (other.getDataFileDirectory() != getDataFileDirectory())
            return false;
        List<IDataProvider> oDP = getListOfDataProviders();
        if (other.getListOfDataProviders().size() != oDP.size())
            return false;
        for (IDataProvider dp : oDP)
        {
            String uid = dp.getUid();
            try
            {
                if (!other.isManagerOf(uid))
                    return false;
            }
            catch (MethodParameterIsEmptyStringException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
            try
            {
                if (!other.getDataProvider(uid).ValueEquals(dp))
                    return false;
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
        }
        return true;
    }
}
