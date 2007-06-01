package org.daisy.urakawa.media.data;

import java.io.InputStream;
import java.io.OutputStream;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @todo verify / add comments and exceptions
 */
public interface DataProvider extends WithDataProviderManager, XukAble,
		ValueEquatable<DataProvider> {
	public String getUid();

	public InputStream getInputStream();

	public OutputStream getOutputStream();

	public void delete();

	public DataProvider copy();

	public String getMimeType();
}
