package org.daisy.urakawa.media;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 *
 */
public class SrcMediaLocationImpl implements SrcMediaLocation {
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
    public String getSrc() {
        return null;
    }

    /**
     * @hidden
     */
    public void setSrc(String src) {
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

	public boolean ValueEquals(MediaLocation other)
			throws MethodParameterIsNullException {

		return false;
	}
}
