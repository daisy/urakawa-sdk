package org.daisy.urakawa.media.data;

import java.io.InputStream;
import java.io.OutputStream;

import org.daisy.urakawa.ValueEquatable;

/**
 *
 */
public interface DataProvider extends ValueEquatable<DataProvider>  {
    public InputStream getInputStream();

    public OutputStream getOutputStream();
}
