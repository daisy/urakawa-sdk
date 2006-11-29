package org.daisy.urakawa.media.asset;

import org.daisy.urakawa.media.MediaLocation;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;

/**
 *
 */
public class MediaAssetLocationImpl implements MediaAssetLocation {
    /**
     * @hidden
     */
    public MediaLocation copy() {
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

    /**
     * @hidden
     */
    public MediaAsset getMediaAsset() {
        return null;
    }

    /**
     * @hidden
     */
    public void setMediaAsset(MediaAsset ass) {
    }
}
