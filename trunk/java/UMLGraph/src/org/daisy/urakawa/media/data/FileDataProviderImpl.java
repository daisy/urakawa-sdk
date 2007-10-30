package org.daisy.urakawa.media.data;

import java.io.InputStream;
import java.io.OutputStream;
import java.net.URI;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class FileDataProviderImpl implements FileDataProvider {
	public FileDataProvider copy() {
		return null;
	}

	public String getDataFileFullPath() {
		return null;
	}

	public String getDataFileRelativePath() {
		return null;
	}

	public void delete() {
	}

	public InputStream getInputStream() {
		return null;
	}

	public String getMimeType() {
		return null;
	}

	public OutputStream getOutputStream() {
		return null;
	}

	public String getUid() {
		return null;
	}

	public DataProviderManager getDataProviderManager() {
		return null;
	}

	public void setDataProviderManager(DataProviderManager manager)
			throws MethodParameterIsNullException {
	}

	public void XukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void XukOut(XmlDataWriter destination, URI baseURI)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}

	public String getXukLocalName() {
		return null;
	}

	public String getXukNamespaceURI() {
		return null;
	}

	public boolean ValueEquals(DataProvider other)
			throws MethodParameterIsNullException {
		return false;
	}

	public FileDataProviderManager getFileDataProviderManager() {
		return null;
	}

	public void setFileDataProviderManager(FileDataProviderManager manager)
			throws MethodParameterIsNullException {
	}
}
