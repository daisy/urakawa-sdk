package org.daisy.urakawa.media.data;

import java.io.IOException;
import java.net.URISyntaxException;
import java.util.List;

import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.data.utilities.Stream;

/**
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Composition 0..n org.daisy.urakawa.media.data.FileDataProvider
 */
public interface FileDataProviderManager extends DataProviderManager {
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
			MethodParameterIsOutOfBoundsException, IOException, DataIsMissingException;

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
	public List<FileDataProvider> getListOfManagedFileDataProviders();

	/**
	 * Initializes the FileDataProvider with a DataFileDirectory
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
	 * Appends data from a given input Stream to a given DataProvider
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
	public void appendDataToProvider(Stream data, int count,
			DataProvider provider) throws MethodParameterIsNullException,
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
	public boolean compareDataProviderContent(DataProvider dp1, DataProvider dp2)
			throws MethodParameterIsNullException, DataIsMissingException,
			OutputStreamIsOpenException, IOException;

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
