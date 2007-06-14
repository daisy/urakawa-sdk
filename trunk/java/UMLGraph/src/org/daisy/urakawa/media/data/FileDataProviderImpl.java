package org.daisy.urakawa.media.data;

import java.io.InputStream;
import java.io.OutputStream;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

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
