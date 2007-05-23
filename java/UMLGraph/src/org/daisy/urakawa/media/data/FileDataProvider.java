package org.daisy.urakawa.media.data;

import java.io.InputStream;
import java.io.OutputStream;
import java.net.URI;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 *
 */
public class FileDataProvider implements DataProvider {
	
	String getDataFileRelativePath(){return "";};
	public String getDataFileFullPath(){return "";};
	
    public URI getFileUri() {
        return null;
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
	public DataProvider copy() {

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
	public String getUid() {

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
