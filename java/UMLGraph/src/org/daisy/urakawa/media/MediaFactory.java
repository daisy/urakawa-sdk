package org.daisy.urakawa.media;

/**
 * Abstract factory pattern: from the API user perspective:
 * do not use constructors, use a factory instead (which will delegate to the real constructor of its choice).
 */
public interface MediaFactory {
    /**
     * @param type the type of Media to create
     * @return a new Media object corresponding to the given type.
     */
    public Media createMedia(MediaType type);
}
