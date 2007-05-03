package org.daisy.urakawa.media.data;

import java.nio.charset.Charset;

/**
 *
 */
public interface PlainTextMediaData extends MediaData {
    public Charset getEncoding();

    public void setEncoding(Charset cs);
}
