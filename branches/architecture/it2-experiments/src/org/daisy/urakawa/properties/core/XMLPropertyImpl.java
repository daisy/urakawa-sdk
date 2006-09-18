package org.daisy.urakawa.properties.core;

import org.daisy.urakawa.coreDataModel.CoreNode;
import org.daisy.urakawa.exceptions.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

import java.io.Reader;
import java.io.Writer;
import java.util.List;

/**
 * The actual implementation to be implemented by the implementation team ;)
 * All method bodies must be completed for realizing the required business logic.
 * -
 * Generally speaking, an end-user would not need to use this class directly.
 * They would just manipulate the corresponding abstract interface and use the provided
 * default factory implementation to create this class instances transparently.
 * -
 * However, this is the DEFAULT implementation for the API/Toolkit:
 * end-users should feel free to use this class as such (it's public after all),
 * or they can sub-class it in order to specialize their application.
 */
public class XMLPropertyImpl implements XMLProperty {
    /**
     * @hidden
     */
    public boolean XUKIn(Reader source) {
        return false;
    }

    /**
     * @hidden
     */
    public boolean XULOut(Writer destination) {
        return false;
    }

    /**
     * @hidden
     */
    public XMLType getXMLType() {
        return null;
    }

    /**
     * @hidden
     */
    public void setXMLType(XMLType newType) {
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
    public String getNamespace() {
        return null;
    }

    /**
     * @hidden
     */
    public void setName(String newName) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException {
    }

    /**
     * @hidden
     */
    public void setNamespace(String newNS) throws MethodParameterIsNullException {
    }

    /**
     * @hidden
     */
    public List getListOfAttributes() {
        return null;
    }

    /**
     * @hidden
     */
    public XMLPropertyImpl copy() {
        return null;
    }

    /**
     * @hidden
     */
    public boolean canBeAddedTo(CoreNode node) throws MethodParameterIsNullException {
        return false;
    }
}
