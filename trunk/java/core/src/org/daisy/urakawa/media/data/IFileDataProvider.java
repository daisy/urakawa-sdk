package org.daisy.urakawa.media.data;

/**
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Clone - org.daisy.urakawa.media.data.IFileDataProvider
 * @depend - Aggregation 1 org.daisy.urakawa.media.data.IDataProviderManager
 */
public interface IFileDataProvider extends IDataProvider
{
    /**
     * Initialize the file data provider with a given manager and relative path
     * 
     * @param relPath The relative path of the data file of the constructed
     *        instance
     * @param mimeType The MIME type of the data to store in the constructed
     *        instance
     */
    public void initialize(String relPath, String mimeType);

    /**
     * Gets the path of the file storing the data of the instance, relative to
     * the path of data file directory of the owning IDataProviderManager
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
    public IFileDataProvider copy();
}
