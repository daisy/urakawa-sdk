package org.daisy.urakawa.media.data;

import java.net.URI;
import java.util.List;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Partial reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype Abstract
 */
public abstract class MediaDataAbstractImpl implements MediaData {
	/**
	 * @stereotype Abstract
	 */
	public abstract List<DataProvider> getListOfUsedDataProviders();

	/**
	 * @stereotype Abstract
	 */
	public abstract void XukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException;

	/**
	 * @stereotype Abstract
	 */
	public abstract void XukOut(XmlDataWriter destination, URI baseURI)
			throws MethodParameterIsNullException,
			XukSerializationFailedException;

	/**
	 * @stereotype Abstract
	 */
	public abstract String getXukLocalName();

	/**
	 * @stereotype Abstract
	 */
	public abstract boolean ValueEquals(MediaData other);

	/**
	 * @hidden
	 */
	public MediaData copy() {
		return null;
	}

	/**
	 * @hidden
	 */
	public MediaDataManager getMediaDataManager() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setMediaDataManager(MediaDataManager mngr) {
	}

	/**
	 * @hidden
	 */
	public String getUid() {
		return "";
	}

	/**
	 * @hidden
	 */
	public String getName() {
		return "";
	}

	/**
	 * @hidden
	 */
	public void setName(String newName) {
	}

	/**
	 * @hidden
	 */
	public void delete() {
	}

	/**
	 * @hidden
	 */
	public String getXukNamespaceURI() {
		return "";
	}
}
