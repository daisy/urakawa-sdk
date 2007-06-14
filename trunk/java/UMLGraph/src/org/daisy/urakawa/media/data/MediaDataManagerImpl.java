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
	public void addMediaData(MediaData data) {
	}

	public MediaData copyMediaData(MediaData data)
			throws MethodParameterIsNullException {
		return null;
	}

	public MediaData copyMediaData(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	public void deleteMediaData(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	public void detachMediaData(MediaData data)
			throws MethodParameterIsNullException {
	}

	public List<MediaData> getListOfMediaData() {
		return null;
	}

	public List<String> getListOfUids() {
		return null;
	}

	public MediaData getMediaData(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	public String getUidOfMediaData(MediaData data)
			throws MethodParameterIsNullException {
		return null;
	}

	public Presentation getPresentation() {
		return null;
	}

	public void setPresentation(Presentation presentation)
			throws MethodParameterIsNullException {
	}

	public boolean XukIn(XmlDataReader source)
			throws MethodParameterIsNullException {
		return false;
	}

	public boolean XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException {
		return false;
	}

	public String getXukLocalName() {
		return null;
	}

	public String getXukNamespaceURI() {
		return null;
	}

	public boolean ValueEquals(MediaDataManager other)
			throws MethodParameterIsNullException {
		return false;
	}
}
