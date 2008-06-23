package org.daisy.urakawa.media.data;

/**
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Clone - org.daisy.urakawa.media.data.FileDataProvider
 * @depend - Aggregation 1 org.daisy.urakawa.media.data.FileDataProviderManager
 */
public interface FileDataProvider extends DataProvider,
		WithFileDataProviderManager {
	/**
	 * Gets the path of the file storing the data of the instance, relative to
	 * the path of data file directory of the owning FileDataProviderManager
	 * 
	 * @return path
	 */
	public String getDataFileRelativePath();

	/**
	 * Gets the full path of the file storing the data the instance
	 * 
	 * @return path
	 */
	public String getDataFileFullPath();

	/**
	 * <p>
	 * Cloning method
	 * </p>
	 * 
	 * @return a copy.
	 */
	public FileDataProvider copy();
}
