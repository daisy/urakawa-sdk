package org.daisy.urakawa.properties;

import java.io.Reader;
import java.io.Writer;

/**
 * Defines the methods for the serialization of the core model in XUK.
 */
public interface XUKAble {
    /**
     * Reads and removes all tags up to and including the closing tag matching the element
     * the reader was at when passed to it.
     * The call is expected to be forwarded to any owned element, in effect making it a
     * recursive read of the XUK file.
     *
     * @param source The XML Reader to read from.
     * @return true if and only if all things were deserialized as expected.
     */
    boolean XUKIn(Reader source);

    /**
     * Writes a tag for the object it is called on.
     * The call should be forwarded to any owned object, making it in effect be a recursive
     * write of the core tree.
     *
     * @param destination The XML Writer to write to.
     * @return true if and only if all things were serialized as expected.
     */
    boolean XULOut(Writer destination);

}
