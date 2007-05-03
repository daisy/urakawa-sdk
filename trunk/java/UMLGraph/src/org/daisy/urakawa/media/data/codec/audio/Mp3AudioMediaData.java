package org.daisy.urakawa.media.data.codec.audio;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.media.data.AudioMediaDataImpl;
import org.daisy.urakawa.media.data.MediaDataManager;
import org.daisy.urakawa.media.data.DataProvider;
import org.daisy.urakawa.media.MediaType;

/**
 *
 */
public class Mp3AudioMediaData extends AudioMediaDataImpl {
    /**
     * EXAMPLE
     */
    public boolean isVariableBitRate() {return false;}

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
    public void setAssetManager(MediaDataManager man) {
    }

    /**
     * @hidden
     */
    public MediaDataManager getAssetManager() {
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
