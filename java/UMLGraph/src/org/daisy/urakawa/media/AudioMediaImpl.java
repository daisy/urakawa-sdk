package org.daisy.urakawa.media;

import org.daisy.urakawa.InterfaceID;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.TimeOffsetIsNegativeException;

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
public class AudioMediaImpl implements AudioMedia {
    /**
     * @hidden
     */
    public TimeDelta getDuration() {
        return null;
    }

    /**
     * @hidden
     */
    public Time getClipBegin() {
        return null;
    }

    /**
     * @hidden
     */
    public Time getClipEnd() {
        return null;
    }

    /**
     * @hidden
     */
    public void setClipBegin(Time newClipBegin) throws MethodParameterIsNullException, TimeOffsetIsNegativeException {
    }

    /**
     * @hidden
     */
    public void setClipEnd(Time newClipEnd) throws MethodParameterIsNullException, TimeOffsetIsNegativeException {
    }

    /**
     * @hidden
     */
    public ClippedMedia split(Time splitPoint) throws MethodParameterIsNullException, TimeOffsetIsNegativeException {
        return null;
    }

    /**
     * @hidden
     */
    public MediaLocation getLocation() {
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
