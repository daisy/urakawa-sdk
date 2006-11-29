package org.daisy.urakawa.media.asset.codec.audio;

import org.daisy.urakawa.media.asset.AudioMediaAssetImpl;
import org.daisy.urakawa.media.asset.MediaAssetManager;
import org.daisy.urakawa.media.asset.DataProvider;
import org.daisy.urakawa.media.MediaType;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;

/**
 *
 */
public class Mp3AudioMediaAsset extends AudioMediaAssetImpl {
    /**
     * @hidden
     */
    public String getUid() {
        return null;
    }

    /**
     * @hidden
     */
    public String getName() {
        return null;
    }

    /**
     * @hidden
     */
    public void setName(String name) {
    }

    /**
     * @hidden
     */
    public MediaType getMediaType() {
        return null;
    }

    /**
     * @hidden
     */
    public void setAssetManager(MediaAssetManager man) {
    }

    /**
     * @hidden
     */
    public MediaAssetManager getAssetManager() {
        return null;
    }

    /**
     * @hidden
     */
    public DataProvider getDataProvider() {
        return null;
    }

    /**
     * @hidden
     */
    public boolean XukIn(XmlDataReader source) {
        return false;
    }

    /**
     * @hidden
     */
    public boolean XukOut(XmlDataWriter destination) {
        return false;
    }

    /**
     * @hidden
     */
    public String getXukLocalName() {
        return null;
    }

    /**
     * @hidden
     */
    public String getXukNamespaceURI() {
        return null;
    }
}
