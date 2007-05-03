package org.daisy.urakawa.media;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.xuk.XukAble;

/**
 * Abstract location. e.g.: URI
 */
public interface MediaLocation extends XukAble, ValueEquatable<MediaLocation>  {
    /**
     * @return a distinct copy of the MediaLocation object.
     */
    MediaLocation copy();
}
