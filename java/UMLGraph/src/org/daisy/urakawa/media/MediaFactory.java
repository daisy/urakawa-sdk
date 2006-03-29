package org.daisy.urakawa.media;

/**
 * Abstract factory pattern: from the API user perspective:
 * do not use constructors, use a factory instead
 * (which will delegate to the real constructor of its choice).
 * 
 * This factory may be implemented as a singleton, but this is not a requirement.
 * The implementation can decide what pattern suits it best.
 * 
 * @depend - Create 1 AudioMedia
 * @depend - Create 1 VideoMedia
 * @depend - Create 1 ImageMedia
 * @depend - Create 1 TextMedia
 */
public interface MediaFactory {
    /**
     * @param type the type of Media to create
     * @return a new Media object corresponding to the given type.
     */
    public Media createMedia(MediaType type);
}
