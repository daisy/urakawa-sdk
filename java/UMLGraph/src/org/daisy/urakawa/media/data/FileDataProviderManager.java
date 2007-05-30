package org.daisy.urakawa.media.data;

import java.util.List;

public interface FileDataProviderManager extends DataProviderManager {
	public void deleteUnusedDataProviders();

	public String getDataFileDirectory();

	public void moveDataFiles(String newDataFileDir, boolean deleteSource,
			boolean overwriteDestDir);

	public String getDataFileDirectoryFullPath();

	public String getNewDataFileRelPath(String extension);

	public List<FileDataProvider> getListOfManagedFileDataProviders();

	public void setDataFileDirectoryPath(String newPath);
}
