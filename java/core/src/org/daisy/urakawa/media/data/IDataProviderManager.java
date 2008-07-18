package org.daisy.urakawa.media.data;

import java.io.IOException;
import java.net.URISyntaxException;
import java.util.List;

import org.daisy.urakawa.IValueEquatable;
import org.daisy.urakawa.IWithPresentation;
import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsAlreadyManagerOfException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.IsNotManagerOfException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.nativeapi.IStream;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * @depend - Aggregation 1 org.daisy.urakawa.IPresentation
 * @depend - Composition 0..n org.daisy.urakawa.media.data.IDataProvider
 * 
 */
public interface IDataProviderManager extends IWithPresentation, IXukAble,
		IValueEquatable<IDataProviderManager> {

	/**
	 * Gets the UID of a given IDataProvider
	 * 
	 * @param provider
	 * @return the UID
	 * @throws MethodParameterIsNullException
	 * @throws IsNotManagerOfException
	 */
	public String getUidOfDataProvider(IDataProvider provider)
			throws MethodParameterIsNullException, IsNotManagerOfException;

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
			throws IsNotManagerOfException, MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

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
			MethodParameterIsEmptyStringException;

	/**
	 * Removes one of the IDataProvider managed by the manager
	 * 
	 * @param provider
	 * @param delete
	 * @throws MethodParameterIsNullException
	 * @throws IsNotManagerOfException
	 */
	public void removeDataProvider(IDataProvider provider, boolean delete)
			throws MethodParameterIsNullException, IsNotManagerOfException;

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
			throws MethodParameterIsNullException, IsNotManagerOfException,
			MethodParameterIsEmptyStringException;

	/**
	 * Adds a IDataProvider to the IDataProviderManager
	 * 
	 * @param provider
	 * @throws MethodParameterIsNullException
	 * @throws IsAlreadyManagerOfException
	 * @throws IsNotManagerOfException
	 * @throws MethodParameterIsEmptyStringException
	 */
	public void addDataProvider(IDataProvider provider)
			throws MethodParameterIsNullException, IsAlreadyManagerOfException,
			MethodParameterIsEmptyStringException, IsNotManagerOfException;

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
			throws MethodParameterIsNullException, IsAlreadyManagerOfException,
			IsNotManagerOfException, MethodParameterIsEmptyStringException;

	/**
	 * Gets a list of the DataProviders that are managed by the
	 * IDataProviderManager
	 * 
	 * @return a non-null but potentially empty list
	 */
	public List<IDataProvider> getListOfDataProviders();

	/**
	 * Removes any DataProviders "not used", that is all IDataProvider that are
	 * not used by a IMediaData of the IPresentation
	 * 
	 * @param delete
	 */
	public void removeUnusedDataProviders(boolean delete);

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
	public String getDataFileDirectory();

	/**
	 * Moves the data file directory of the manager
	 * 
	 * @param newDataFileDir
	 * @param deleteSource
	 * @param overwriteDestDir
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 * @throws MethodParameterIsOutOfBoundsException
	 * @throws DataIsMissingException
	 * @throws IOException
	 */
	public void moveDataFiles(String newDataFileDir, boolean deleteSource,
			boolean overwriteDestDir) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException,
			MethodParameterIsOutOfBoundsException, IOException,
			DataIsMissingException;

	/**
	 * Gets the full path of the data file directory
	 * 
	 * @return path
	 * @throws IsNotInitializedException
	 * @throws URISyntaxException
	 */
	public String getDataFileDirectoryFullPath()
			throws IsNotInitializedException, URISyntaxException;

	/**
	 * Gets a new data file path relative to the path of the data file directory
	 * of the manager
	 * 
	 * @param extension
	 * @return the relative path
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 */
	public String getNewDataFileRelPath(String extension)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * Gets a list of the FileDataProviders managed by the manager
	 * 
	 * @return a non-null but potentially empty list
	 */
	public List<IFileDataProvider> getListOfManagedFileDataProviders();

	/**
	 * Initializes the IFileDataProvider with a DataFileDirectory
	 * 
	 * @param dataDir
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 * @throws IsAlreadyInitializedException
	 * @throws URISyntaxException
	 */
	public void setDataFileDirectory(String dataDir)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException,
			IsAlreadyInitializedException, URISyntaxException;

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
			DataIsMissingException, IOException, InputStreamIsTooShortException;

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
			DataIsMissingException, OutputStreamIsOpenException, IOException;

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
			IsAlreadyInitializedException, IOException;
}
