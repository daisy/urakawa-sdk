package org.daisy.urakawa.media.data;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.media.MediaFactory;
import org.daisy.urakawa.media.MediaLocation;

/**
 *
 */
public class MediaDataLocationImpl implements MediaDataLocation {
	/**
	 * @hidden
	 */
	public MediaLocation copy() {
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

	/**
	 * @hidden
	 */
	public MediaData getMediaAsset() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setMediaAsset(MediaData ass) {
	}

	/**
	 * @hidden
	 */
	public MediaData getMediaData() {
		return null;
	}

	/**
	 * @hidden
	 */
	public MediaDataFactory getMediaDataFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setMediaData(MediaData newData) {
	}

	/**
	 * @hidden
	 */
	public MediaDataFactory setMediaDataFactory(MediaDataFactory fact) {
		return null;
	}

	/**
	 * @hidden
	 */
	public MediaFactory getMediaFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public boolean ValueEquals(MediaLocation other)
			throws MethodParameterIsNullException {
		return false;
	}
}
