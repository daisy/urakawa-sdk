package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;

/**
 * 
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 *
 */
public abstract class AbstractMediaData implements MediaData {
	public MediaDataManager getMediaDataManager() {
		return null;
	}

	public void setMediaDataManager(MediaDataManager mngr) {
	}

	public String getUid() {
		return "";
	}

	public String getName() {
		return "";
	}

	public void setName(String newName) {
	}

	public abstract List<DataProvider> getUsedDataProviders();

	public void delete() {
	}

	protected abstract AbstractMediaData mediaDataCopy();

	public MediaData copy() {
		return null;
	}

	public abstract boolean XukIn(XmlDataReader source);

	public abstract boolean XukOut(XmlDataWriter destination);

	public String getXukLocalName() {
		return "";
	}

	public String getXukNamespaceUri() {
		return "";
	}

	public abstract boolean ValueEquals(MediaData other);
}
