package org.daisy.urakawa.properties;

import org.daisy.urakawa.exceptions.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.project.XUKAble;

import java.net.URI;

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
public class XMLAttributeImpl implements XMLAttribute, XUKAble {
    /**
     * @hidden
     */
    public XMLProperty getParent() {
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
    public XMLAttribute copy() {
        return null;
    }

    /**
     * @hidden
     */
    public String getValue() {
        return null;
    }

    /**
     * @hidden
     */
    public void setValue(String newValue) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException {
    }

    /**
     * @hidden
     */
    public boolean XUKIn(URI source) {
        return false;
    }

    /**
     * @hidden
     */
    public boolean XUKOut(URI destination) {
        return false;
    }
}
