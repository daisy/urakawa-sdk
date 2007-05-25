package org.daisy.urakawa.media;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.media.data.MediaDataLocation;
import org.daisy.urakawa.xuk.XukAble;

/**
 * An abstract media location
 * 
 * @see Located
 * @see SrcMediaLocation
 * @see MediaDataLocation
 * @depend - Clone 1 MediaLocation
 */
public interface MediaLocation extends XukAble, ValueEquatable<MediaLocation> {
	/**
	 * Makes a copy
	 * 
	 * @return a copy
	 */
	MediaLocation copy();
}
