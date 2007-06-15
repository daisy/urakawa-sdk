package org.daisy.urakawa.media.data;

/**
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Clone - org.daisy.urakawa.media.data.FileDataProvider
 * @depend - Aggregation 1 org.daisy.urakawa.media.data.FileDataProviderManager
 */
public interface FileDataProvider extends DataProvider,
		WithFileDataProviderManager {
	public String getDataFileRelativePath();

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
