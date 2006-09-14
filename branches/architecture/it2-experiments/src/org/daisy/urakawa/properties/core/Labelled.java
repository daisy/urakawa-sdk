package org.daisy.urakawa.properties.core;

/**
 * Allows the labelling of components of the model.
 * A Labelled object is described by one or several localized MediaLabel.
 *
 * @depend - Aggregation 0..n MediaLabel
 */
public interface Labelled {
    /**
     * Returns the MediaLabel describing this labelled object in the given locale, or
     * null if such a label does not exist.
     *
     * @param lang The locale of the returned Label.
     * @return The MediaLabel describing this labelled object in the given locale.
     */
    MediaLabel getLabel(String lang);

    /**
     * Sets the label describing this labelled object in the given locale.
     *
     * @param label The new label in the appropriate locale.
     * @param lang  The locale of the new label.
     */
    void setLabel(MediaLabel label, String lang);
}
