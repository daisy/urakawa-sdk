package org.daisy.urakawa.media.data;

/**
 * @todo verify / add comments and exceptions
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
	public FileDataProvider copy();
}
