package org.daisy.urakawa.media;

import java.net.URI;

/**
 *
 */
public interface UriMediaLocation extends SrcMediaLocation {
    public URI getMediaUrl();
    public void setMediaUrl(URI url);
}
