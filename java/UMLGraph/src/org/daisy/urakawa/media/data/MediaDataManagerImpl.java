package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class MediaDataManagerImpl implements MediaDataManager {
	/**
	 * @hidden
	 */
	public void addMediaData(MediaData data) {
	}

	/**
	 * @hidden
	 */
	public MediaData copyMediaData(MediaData data)
			throws MethodParameterIsNullException {
		return null;
	}

	/**
	 * @hidden
	 */
	public MediaData copyMediaData(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	/**
	 * @hidden
	 */
	public void deleteMediaData(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	/**
	 * @hidden
	 */
	public void detachMediaData(MediaData data)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public List<MediaData> getListOfMediaData() {
		return null;
	}

	/**
	 * @hidden
	 */
	public List<String> getListOfUids() {
		return null;
	}

	/**
	 * @hidden
	 */
	public MediaData getMediaData(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	/**
	 * @hidden
	 */
	public String getUidOfMediaData(MediaData data)
			throws MethodParameterIsNullException {
		return null;
	}

	/**
	 * @hidden
	 */
	public DataProviderFactory getDataProviderFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setDataProviderFactory(DataProviderFactory factory)
			throws MethodParameterIsNullException {
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
	public void setMediaDataFactory(MediaDataFactory factory)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public Presentation getPresentation() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setPresentation(Presentation presentation)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public boolean XukIn(XmlDataReader source)
			throws MethodParameterIsNullException {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException {
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
	public boolean ValueEquals(MediaDataManager other)
			throws MethodParameterIsNullException {
		return false;
	}
}
