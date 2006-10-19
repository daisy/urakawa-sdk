package org.daisy.urakawa;

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
     */
    public Project(Presentation pres) {
    }

    /**
     * @return can be null;
     */
    public Presentation getPresentation() {
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
