package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Composition 0..n FileDataProvider
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface FileDataProviderManager extends DataProviderManager {
	public String getDataFileDirectory();

	/**
	 * 
	 * @param newDataFileDir
	 * @param deleteSource
	 * @param overwriteDestDir
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 */
	public void moveDataFiles(String newDataFileDir, boolean deleteSource,
			boolean overwriteDestDir)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

	public String getDataFileDirectoryFullPath();

	/**
	 * 
	 * @param extension
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 */
	public String getNewDataFileRelPath(String extension)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

	public List<FileDataProvider> getListOfManagedFileDataProviders();

	/**
	 * 
	 * @param newPath
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 */
	public void setDataFileDirectoryPath(String newPath)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;
}
