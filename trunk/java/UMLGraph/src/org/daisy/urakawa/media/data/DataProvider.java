package org.daisy.urakawa.media.data;

import java.io.InputStream;
import java.io.OutputStream;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.xuk.XukAble;

/**
 * 
 * @depend - Clone - org.daisy.urakawa.media.data.DataProvider
 * @depend - Aggregation 1 org.daisy.urakawa.media.data.DataProviderManager
 * @stereotype XukAble
 */
public interface DataProvider extends WithDataProviderManager, WithPresentation, XukAble,
		ValueEquatable<DataProvider> {
	public String getUid();

	public InputStream getInputStream();

	public OutputStream getOutputStream();

	public void delete();
	/**
	 * <p>
	 * Cloning method
	 * </p>
	 * 
	 * @return a copy.
	 */
	public DataProvider copy();

	public String getMimeType();
}
