package org.daisy.urakawa.properties.channels;

import org.daisy.urakawa.exceptions.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

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
 * @depend - Create 1 Channel
 */
public interface ChannelFactory {
    /**
     * Creates a new Channel with a given name, which is not linked to the channels list yet.
     *
     * @param name cannot be null, cannot be empty String
     * @return cannot return null
     * @tagvalue Exceptions "MethodParameterIsNull, MethodParameterIsEmptyString"
     */
    public Channel createChannel(String name) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;
}
