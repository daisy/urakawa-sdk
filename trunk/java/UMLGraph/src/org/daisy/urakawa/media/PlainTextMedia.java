package org.daisy.urakawa.media;

import org.daisy.urakawa.exceptions.FactoryIsMissingException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.media.asset.PlainTextMediaAsset;
import org.daisy.urakawa.media.asset.MediaAssetLocation;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.Reader;

/**
 *
 */
public class PlainTextMedia implements TextMedia, Located {

    /**
     * Convenience method
     *
     * return (PlainTextMediaAsset)((MediaAssetLocation)getLocation()).getMediaAsset();
     */
    public PlainTextMediaAsset getPlainTextMediaAsset() {
        return (PlainTextMediaAsset) ((MediaAssetLocation) getLocation()).getMediaAsset();
    }

    /**
     * @hidden
     */
    public String getText() {
        PlainTextMediaAsset ass = getPlainTextMediaAsset();
        InputStream is = ass.getDataProvider().getInputStream();
        InputStreamReader reader = new InputStreamReader(is, ass.getEncoding());
        Reader in = new BufferedReader(reader);
        StringBuffer buffer = new StringBuffer();
        int ch;
        try {
            while ((ch = in.read()) > -1) {
                buffer.append((char) ch);
            }
            in.close();
        } catch (IOException e) {
            return null;
        }
        return buffer.toString();
    }

    /**
     * @hidden
     */
    public void setText(String text) throws MethodParameterIsNullException {
    }

    /**
     * @hidden
     */
    public MediaFactory getMediaFactory() {
        return null;
    }

    /**
     * @hidden
     */
    public void setMediaFactory(MediaFactory fact) {
    }

    /**
     * @hidden
     */
    public boolean isContinuous() {
        return false;
    }

    /**
     * @hidden
     */
    public boolean isDiscrete() {
        return false;
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
    public Media copy() throws FactoryIsMissingException {
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
    public void setLocation(MediaLocation location) throws MethodParameterIsNullException {
    }

    /**
     * @hidden
     */
    public MediaLocation getLocation() {
        return null;
    }
}
