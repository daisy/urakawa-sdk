package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 * 
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public class FileDataProviderManagerImpl implements FileDataProviderManager {
	public void deleteUnusedDataProviders() {
	}

	public String getDataFileDirectory() {
		return "";
	}

	public void moveDataFiles(String newDataFileDir, boolean deleteSource,
			boolean overwriteDestDir) {
	}

	public String getDataFileDirectoryFullPath() {
		return "";
	}

	public String getNewDataFileRelPath(String extension) {
		return "";
	}

	public List<FileDataProvider> getListOfManagedFileDataProviders() {
		return null;
	}

	public void setDataFileDirectoryPath(String newPath) {
	}

	public void addDataProvider(DataProvider provider) {
	}

	public void detachDataProvider(DataProvider provider) {
	}

	public void detachDataProvider(String uid) {
	}

	public DataProvider getDataProvider(String uid) {
		return null;
	}

	public DataProviderFactory getDataProviderFactory() {
		return null;
	}

	public List<DataProvider> getListOfManagedDataProviders() {
		return null;
	}

	public String getUidOfDataProvider(DataProvider provider) {
		return null;
	}

	public void setPresentation(MediaDataPresentation ownerPres) {
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

	public boolean ValueEquals(DataProviderManager other)
			throws MethodParameterIsNullException {
		return false;
	}

	public MediaDataPresentation getPresentation() {
		// TODO Auto-generated method stub
		return null;
	}
}
