package org.daisy.urakawa.media.data;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.media.MediaLocation;

/**
 *
 */
public class MediaDataLocationImpl implements MediaDataLocation {
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
    public MediaData getMediaAsset() {
        return null;
    }

    /**
     * @hidden
     */
    public void setMediaAsset(MediaData ass) {
    }
}
