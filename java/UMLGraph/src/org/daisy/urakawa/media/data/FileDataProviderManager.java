package org.daisy.urakawa.media.data;

import java.util.List;

/**
 * @depend - Composition 0..n FileDataProvider
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public interface FileDataProviderManager extends DataProviderManager {
	public String getDataFileDirectory();

	public void moveDataFiles(String newDataFileDir, boolean deleteSource,
			boolean overwriteDestDir);

	public String getDataFileDirectoryFullPath();

	public String getNewDataFileRelPath(String extension);

	public List<FileDataProvider> getListOfManagedFileDataProviders();

	public void setDataFileDirectoryPath(String newPath);
}
