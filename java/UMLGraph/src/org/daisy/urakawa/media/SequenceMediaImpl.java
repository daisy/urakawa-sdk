package org.daisy.urakawa.media;

import org.daisy.urakawa.InterfaceID;
import org.daisy.urakawa.exceptions.MediaTypeIsIllegalException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.MethodParameterIsValueOutOfBoundsException;

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
 * @see MediaFactoryImpl
 */
public class SequenceMediaImpl implements SequenceMedia {
    /**
     * @hidden
     */
    public Media getItem(int index) throws MethodParameterIsValueOutOfBoundsException {
        return null;
    }

    /**
     * @hidden
     */
    public Media setItem(int index, Media newItem) throws MethodParameterIsNullException, MethodParameterIsValueOutOfBoundsException, MediaTypeIsIllegalException {
        return null;
    }

    /**
     * @hidden
     */
    public void appendItem(Media newItem) throws MethodParameterIsNullException, MediaTypeIsIllegalException {
    }

    /**
     * @hidden
     */
    public Media removeItem(int index) throws MethodParameterIsValueOutOfBoundsException {
        return null;
    }

    /**
     * @hidden
     */
    public int getCount() {
        return 0;
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
    public MediaType getType() {
        return null;
    }

    /**
     * @hidden
     */
    public InterfaceID getInterfaceID() {
        return null;
    }

    /**
     * @hidden
     */
    public boolean conformsTo(InterfaceID iid) {
        return false;
    }
}
