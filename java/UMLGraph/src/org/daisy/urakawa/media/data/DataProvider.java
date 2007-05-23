package org.daisy.urakawa.media.data;

import java.io.InputStream;
import java.io.OutputStream;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.xuk.XukAble;

/**
 *
 */
public interface DataProvider extends XukAble, ValueEquatable<DataProvider>  {
	DataProviderManager getDataProviderManager();

	String getUid();

	InputStream getInputStream();

	OutputStream getOutputStream();

	void delete();

	DataProvider copy();

	String getMimeType();
}
