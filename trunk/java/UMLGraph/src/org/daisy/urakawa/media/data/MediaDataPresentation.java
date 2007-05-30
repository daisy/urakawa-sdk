package org.daisy.urakawa.media.data;

import org.daisy.urakawa.media.MediaPresentation;

/**
 * @depend - Aggregation 1 MediaDataManager
 * 
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public interface MediaDataPresentation extends MediaPresentation,
		WithMediaDataFactory {
	/**
	 * @return
	 */
	public MediaDataManager getMediaDataManager();

	/**
	 * @param man
	 * @stereotype initialize
	 */
	public void setMediaDataManager(MediaDataManager man);

	/**
	 * @return
	 */
	public DataProviderManager getDataProviderManager();

	/**
	 * @param man
	 * @stereotype initialize
	 */
	public void setDataProviderManager(DataProviderManager man);
}
