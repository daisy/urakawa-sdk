package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * 
 * @depend - Composition 0..n org.daisy.urakawa.media.data.FileDataProvider
 * @depend - Composition 0..n org.daisy.urakawa.media.data.DataProvider
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 * @depend - Aggregation 1 org.daisy.urakawa.media.data.DataProviderFactory
 */
public interface FileDataProviderManager extends DataProviderManager {
	public String getDataFileDirectory();

	/**
	 * 
	 * @param newDataFileDir
	 * @param deleteSource
	 * @param overwriteDestDir
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 */
	public void moveDataFiles(String newDataFileDir, boolean deleteSource,
			boolean overwriteDestDir)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

	public String getDataFileDirectoryFullPath();

	/**
	 * 
	 * @param extension
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 */
	public String getNewDataFileRelPath(String extension)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

	public List<FileDataProvider> getListOfFileDataProviders();

	/**
	 * 
	 * @param newPath
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 */
	public void setDataFileDirectoryPath(String newPath)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;
}
