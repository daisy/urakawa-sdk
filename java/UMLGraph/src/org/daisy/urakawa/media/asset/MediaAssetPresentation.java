package org.daisy.urakawa.media.asset;

import org.daisy.urakawa.media.MediaPresentation;

/**
 *
 */
public interface MediaAssetPresentation extends MediaPresentation {
    /**
     * @return
     */
    public MediaAssetManager getMediaAssetManager();

    /**
     * @param man
     * @stereotype initialize
     */
    public void setMediaAssetManager(MediaAssetManager man);

    /**
     * @return
     */
    public MediaAssetFactory getMediaAssetFactory();

    /**
     * @param man
     * @stereotype initialize
     */
    public void setMediaAssetFactory(MediaAssetFactory man);
}
