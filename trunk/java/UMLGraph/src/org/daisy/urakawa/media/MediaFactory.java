package org.daisy.urakawa.media;

import org.daisy.urakawa.exceptions.IsAlreadyInitializedException;
import org.daisy.urakawa.exceptions.IsNotInitializedException;

/**
 * Abstract factory pattern: from the API user perspective:
 * do not use constructors, use a factory instead
 * (which will delegate to the real constructor of its choice).
 * -
 * A Factory offers much more flexibility than standard constructors.
 * For example, optimized constructors can be used for instanciating many
 * objects at once (e.g. parallel processing).
 * -
 * Another example is to have a memory-efficient object allocator for
 * when instanciating many objects of the same type throught the course
 * of the execution of the program, by always returning the same "Flyweight"
 * instance of the object (e.g. Text media object is likely to created thousands of times in a Daisy book,
 * for each small fragment of text).
 * Implementation of the "Flyweight" pattern are quite common:
 * Dom4J (Namespace object), Swing (TreeRenderer), etc.
 * More info:
 * http://exciton.cs.rice.edu/javaresources/DesignPatterns/FlyweightPattern.htm
 * -
 * This factory may be implemented as a singleton, but this is not a requirement.
 * The implementation can decide what pattern suits it best.
 *
 * @zdepend - Create 1 AudioMedia
 * @zdepend - Create 1 VideoMedia
 * @zdepend - Create 1 ImageMedia
 * @zdepend - Create 1 TextMedia
 * @depend - Create 1 Media
 * @depend - - - MediaType
 * @depend - Aggregation 1 MediaPresentation
 */
public interface MediaFactory {
    /**
     * @param type the type of Media to create
     * @return a new Media object corresponding to the given type.
     */
    public Media createMedia(MediaType type) throws IsNotInitializedException;

    public Media createMedia(String xukLocalName, String xukNamespaceURI) throws IsNotInitializedException;

    public MediaLocation createMediaLocation() throws IsNotInitializedException;

    public MediaLocation createMediaLocation(String xukLocalName, String xukNamespaceURI) throws IsNotInitializedException;

    public MediaPresentation getPresentation() throws IsNotInitializedException;

    /**
     * @stereotype initialize
     * @param pres
     */
    public void setPresentation(MediaPresentation pres) throws IsAlreadyInitializedException;
}
