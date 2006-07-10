package org.daisy.urakawa.properties.daisy;

import org.daisy.urakawa.properties.core.MediaLabel;

/**
 * @depend - Composition 1..n MediaLabel
 */
public interface NavList {
    MediaLabel getMediaLabel(String lang);
    void setMediaLabel(String lang, MediaLabel label);
}
