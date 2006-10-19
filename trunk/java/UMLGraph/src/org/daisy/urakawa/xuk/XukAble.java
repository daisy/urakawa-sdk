package org.daisy.urakawa.xuk;

import java.net.URI;

/**
 *
 */
public interface XukAble {
    /**
     * The implementation of XUKIn is expected to read and remove all tags up to and including the closing tag matching the element the reader was at when passed to it.
     * The call is expected to be forwarded to any owned element, in effect making it a recursive read of the XUK file
     *
     * @param source specified as URI but could be any other input type (e.g. XML stream)
     * @return true if de-serialization went well.
     */
    public boolean XUKIn(URI source);

    /**
     * The implementation of XUKOut is expected to write a tag for the object it is called on.
     * The call should be forwarded to any owned object, making it in effect be a recursive write of the CoreTree.
     *
     * @param destination specified as URI but could be any other input type (e.g. XML stream)
     * @return true if serialization went well.
     */
    public boolean XUKOut(URI destination);
}
