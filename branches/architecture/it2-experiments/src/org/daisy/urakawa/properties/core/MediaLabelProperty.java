package org.daisy.urakawa.properties.core;

import org.daisy.urakawa.properties.Property;

/**
 * @depend - Composition 1..n MediaLabel
 */
public interface MediaLabelProperty extends Property {
    MediaLabel getMediaLabel(String lang);
}
