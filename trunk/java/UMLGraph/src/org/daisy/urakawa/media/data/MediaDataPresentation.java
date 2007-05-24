package org.daisy.urakawa.media.data;

import org.daisy.urakawa.media.MediaPresentation;

/**
 * @depend - Aggregation 1 MediaDataManager
 * @depend - Aggregation 1 MediaDataFactory
 */
public interface MediaDataPresentation extends MediaPresentation {
    /**
     * @return
     */
    public MediaDataManager getMediaDataManager();

    /**
     * @param man
     * @stereotype initialize
     */
    public void setMediaDataManager(MediaDataManager man);

    /**
     * @return
     */
    public MediaDataFactory getMediaAssetFactory();

    /**
     * @param man
     * @stereotype initialize
     */
    public void setMediaAssetFactory(MediaDataFactory fact);

    /**
     * @return
     */
    public DataProviderManager getDataProviderManager();

    /**
     * @param man
     * @stereotype initialize
     */
    public void setDataProviderManager(DataProviderManager man);
}
