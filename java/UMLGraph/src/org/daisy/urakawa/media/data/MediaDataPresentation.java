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
    public MediaDataManager getMediaAssetManager();

    /**
     * @param man
     * @stereotype initialize
     */
    public void setMediaAssetManager(MediaDataManager man);

    /**
     * @return
     */
    public MediaDataFactory getMediaAssetFactory();

    /**
     * @param man
     * @stereotype initialize
     */
    public void setMediaAssetFactory(MediaDataFactory man);
}
