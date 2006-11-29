package org.daisy.urakawa.media;

import org.daisy.urakawa.exceptions.FactoryIsMissingException;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;

/**
 *
 */
public class XmlMediaImpl implements XmlMedia {
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
    public String getFragmentIdentifier() {
        return null;
    }

    /**
     * @hidden
     */
    public void setFragmentIdentifier(String id) {
    }
}
