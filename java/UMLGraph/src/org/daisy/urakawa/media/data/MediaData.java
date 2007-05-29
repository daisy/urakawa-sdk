package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public interface MediaData extends XukAble, ValueEquatable<MediaData> {
	public MediaDataManager getMediaDataManager();

	public void setMediaDataManager(MediaDataManager mngr);

	public String getUid();

	public String getName();

	public void setName(String newName);

	public void delete();

	public MediaData copy();

	public List<DataProvider> getListOfUsedDataProviders();
}
