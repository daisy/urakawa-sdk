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
	/**
	 * @hidden
	 */
	public String getDataFileRelativePath() {
		return "";
	};

	/**
	 * @hidden
	 */
	public String getDataFileFullPath() {
		return "";
	};

	/**
	 * @hidden
	 */
	public FileDataProviderManager getFileDataProviderManager() {
		return null;
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
	public InputStream getInputStream() {
		return null;
	}

	/**
	 * @hidden
	 */
	public OutputStream getOutputStream() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void delete() {
	}

	/**
	 * @hidden
	 */
	public DataProvider copy() {
		return null;
	}

	/**
	 * @hidden
	 */
	public DataProviderManager getDataProviderManager() {
		return null;
	}

	/**
	 * @hidden
	 */
	public String getMimeType() {
		return null;
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
	public boolean ValueEquals(DataProvider other)
			throws MethodParameterIsNullException {
		return false;
	}

	/**
	 * @hidden
	 */
	public void setDataProviderManager(DataProviderManager manager)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public void setFileDataProviderManager(FileDataProviderManager manager)
			throws MethodParameterIsNullException {
	}
}
