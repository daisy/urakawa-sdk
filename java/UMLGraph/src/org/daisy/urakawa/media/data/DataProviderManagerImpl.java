package org.daisy.urakawa.media.data;

import java.net.URI;
import java.util.List;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

public class DataProviderManagerImpl implements DataProviderManager {
	public void addDataProvider(DataProvider provider)
			throws MethodParameterIsNullException {
	}

	public void deleteUnusedDataProviders() {
	}

	public void detachDataProvider(DataProvider provider)
			throws MethodParameterIsNullException {
	}

	public DataProvider getDataProvider(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	public List<DataProvider> getListOfDataProviders() {
		return null;
	}

	public String getUidOfDataProvider(DataProvider provider)
			throws MethodParameterIsNullException {
		return null;
	}

	public void removeDataProvider(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	public void removeDataProvider(DataProvider provider)
			throws MethodParameterIsNullException {
	}

	public void removeUnusedDataProviders() {
	}

	public Presentation getPresentation() {
		return null;
	}

	public void setPresentation(Presentation presentation)
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

	public boolean ValueEquals(DataProviderManager other)
			throws MethodParameterIsNullException {
		return false;
	}

	public boolean isManagerOf(String uid) {
		return false;
	}
}
