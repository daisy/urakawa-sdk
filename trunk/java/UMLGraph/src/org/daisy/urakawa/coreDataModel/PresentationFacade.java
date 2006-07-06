package org.daisy.urakawa.coreDataModel;

import java.net.URI;

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
 * @depend - Create 1 Presentation
 */
public interface PresentationFacade {
    /**
     * @return an empty Daisy presentation (with the right ChannelsProperty etc.), ready to be authored.
     */
    Presentation createEmptyDaisyPresentation();

    /**
     * @param uri the location of the XUK file
     * @return a new presentation with all the content described by the XUK XML file, ready to be authored.
     */
    Presentation createDaisyPresentationFromXUK(URI uri);

    /**
     * @param presentation the Daisy presentation to be serialized in XML XUK format.
     * @return the location of the generated XUK file.
     */
    URI saveDaisyPresentationToXUK(Presentation presentation);
}
