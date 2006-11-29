package org.daisy.urakawa.media.asset;

import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;

import java.nio.charset.Charset;

/**
 *
 */
public class PlainTextMediaAsset implements MediaAsset {
    public Charset getEncoding() {return null;}

    public void setEncoding(Charset cs) {}

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
