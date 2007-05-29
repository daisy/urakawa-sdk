package org.daisy.urakawa.media.data;

import java.io.InputStream;
import java.io.OutputStream;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public class FileDataProvider implements DataProvider {
	public String getDataFileRelativePath() {
		return "";
	};

	public String getDataFileFullPath() {
		return "";
	};

	public FileDataProviderManager getFileDataProviderManager() {
		return null;
	}

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
}
