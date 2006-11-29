package org.daisy.urakawa.media.asset;

import java.io.InputStream;
import java.io.OutputStream;

/**
 *
 */
public interface DataProvider {
    public InputStream getInputStream();

    public OutputStream getOutputStream();
}
