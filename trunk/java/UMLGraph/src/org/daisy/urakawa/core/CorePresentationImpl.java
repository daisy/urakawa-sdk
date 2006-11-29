package org.daisy.urakawa.core;

import org.daisy.urakawa.core.property.CorePropertyFactory;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;

/**
 * @depend - - - CoreNode
 */
public class CorePresentationImpl implements CorePresentation {
    /**
     * @hidden
     */
    public CoreNode getRootNode() {
        return null;
    }

    /**
     * @hidden
     */
    public CoreNodeFactory getCoreNodeFactory() {
        return null;
    }

    /**
     * @hidden
     */
    public CorePropertyFactory getPropertyFactory() {
        return null;
    }

    /**
     * @hidden
     */
    public void setRootNode(CoreNode node) {
    }

    /**
     * @hidden
     */
    public void setCoreNodeFactory(CoreNodeFactory fact) throws MethodParameterIsNullException {
    }

    /**
     * @hidden
     */
    public void setPropertyFactory(CorePropertyFactory fact) throws MethodParameterIsNullException {
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
