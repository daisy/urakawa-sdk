package org.daisy.urakawa.media.data;

import java.net.URI;
import java.util.List;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

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

	public void XukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void XukOut(XmlDataWriter destination, URI baseURI)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}

	public void removeMediaData(MediaData data)
			throws MethodParameterIsNullException {
	}

	public void removeMediaData(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	public boolean isManagerOf(String uid) {
		return false;
	}

	public void xukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void xukOut(XmlDataWriter destination, URI baseURI)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}
}
