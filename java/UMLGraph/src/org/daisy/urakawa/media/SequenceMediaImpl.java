package org.daisy.urakawa.media;

import org.daisy.urakawa.exceptions.MediaTypeIsIllegalException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;

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
 * -
 * In addition, an end-user would be able to implement its own factory
 * in order to create instances from its own implementations.
 *
 * @see MediaFactory
 */
public class SequenceMediaImpl implements SequenceMedia {
    /**
     * @hidden
     */
    public Media getItem(int index) throws MethodParameterIsOutOfBoundsException {
        return null;
    }

    /**
     * @hidden
     */
    public void insertItem(int index, Media newItem) throws MethodParameterIsNullException, MethodParameterIsOutOfBoundsException, MediaTypeIsIllegalException {
    }

    /**
     * @hidden
     */
    public Media removeItem(int index) throws MethodParameterIsOutOfBoundsException {
        return null;
    }

    /**
     * @hidden
     */
    public int getCount() {
        return 0;
    }

    public MediaFactory getMediaFactory() {
        return null;
    }

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

    public MediaType getMediaType() {
        return null;
    }

    /**
     * @hidden
     */
    public MediaType getType() {
        return null;
    }

    /**
     * @hidden
     */
    public boolean canInsertItem(int index, Media newItem) throws MethodParameterIsNullException, MethodParameterIsOutOfBoundsException, MediaTypeIsIllegalException {
        return false;
    }

    /**
     * @hidden
     */
    public boolean canRemoveItem(int index) throws MethodParameterIsOutOfBoundsException {
        return false;
    }

    /**
     * @hidden
     */
    public Media copy() {
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
