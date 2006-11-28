package org.daisy.urakawa.media;

import org.daisy.urakawa.xuk.XukAble;

/**
 * Abstract location. e.g.: URI
 */
public interface MediaLocation extends XukAble {
    /**
     * @return a distinct copy of the MediaLocation object.
     */
    MediaLocation copy();
}
