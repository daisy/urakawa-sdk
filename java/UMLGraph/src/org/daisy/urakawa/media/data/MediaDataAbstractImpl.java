package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;

/**
 * Partial reference implementation of the interface.
 * 
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 * @stereotype abstract
 */
public abstract class MediaDataAbstractImpl implements MediaData {
	/**
	 * @hidden
	 */
	public MediaDataManager getMediaDataManager() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setMediaDataManager(MediaDataManager mngr) {
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
	public String getName() {
		return "";
	}

	/**
	 * @hidden
	 */
	public void setName(String newName) {
	}

	/**
	 * @stereotype abstract
	 */
	public abstract List<DataProvider> getUsedDataProviders();

	/**
	 * @hidden
	 */
	public void delete() {
	}

	/**
	 * @stereotype abstract
	 */
	protected abstract MediaDataAbstractImpl mediaDataCopy();

	public MediaData copy() {
		return null;
	}

	/**
	 * @stereotype abstract
	 */
	public abstract boolean XukIn(XmlDataReader source);

	/**
	 * @stereotype abstract
	 */
	public abstract boolean XukOut(XmlDataWriter destination);

	/**
	 * @stereotype abstract
	 */
	public abstract String getXukLocalName();

	/**
	 * @hidden
	 */
	public String getXukNamespaceURI() {
		return "";
	}

	/**
	 * @stereotype abstract
	 */
	public abstract boolean ValueEquals(MediaData other);
}
