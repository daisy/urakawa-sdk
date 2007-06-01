package org.daisy.urakawa.media.data;

/**
 * @todo verify / add comments and exceptions
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface FileDataProvider extends DataProvider,
		WithFileDataProviderManager {
	public String getDataFileRelativePath();

	public String getDataFileFullPath();
}
