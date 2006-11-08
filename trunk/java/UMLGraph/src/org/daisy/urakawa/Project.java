package org.daisy.urakawa;

import org.daisy.urakawa.metadata.MetadataFactory;

import java.net.URI;

/**
 * @depend 1 Composition 1 Presentation
 * @zzTODO_depend - Composition 1 MetadataFactory
 */
public class Project {
    /**
     * Default constructor: empty xuk.
     */
    public Project() {
    }

    /**
     * @param pres initialize the xuk with this Presentation instance.
     * @param metadataFactory initialize the xuk with this MetadataFactory instance.
     */
    public Project(Presentation pres, MetadataFactory metadataFactory) {
    }

    /**
     * @return can be null;
     */
    public Presentation getPresentation() {
        return null;
    }

    /**
     * @return can be null;
     */
    public MetadataFactory getMetadataFactory() {
        return null;
    }

    /**
     * @param dataLocation although this is specified as a URI, it could be another type (e.g. XML stream).
     * @return true if successful.
     */
    public boolean openXUK(URI dataLocation) {
        return false;
    }

    /**
     * @param dataLocation although this is specified as a URI, it could be another type (e.g. XML stream).
     * @return true if successful.
     */
    public boolean saveXUK(URI dataLocation) {
        return false;
    }
}
