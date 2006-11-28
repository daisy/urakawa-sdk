package org.daisy.urakawa.media;

import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;

/**
 *
 */
public class MediaLocationImpl implements MediaLocation {
    /**
     * @hidden
     */
    public MediaLocation copy() {
        return null;
    }

    public boolean XukIn(XmlDataReader source) {
        return false;
    }

    public boolean XukOut(XmlDataWriter destination) {
        return false;
    }

    public String getXukLocalName() {
        return null;
    }

    public String getXukNamespaceURI() {
        return null;
    }
}
