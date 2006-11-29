package org.daisy.urakawa.core.property;

import org.daisy.urakawa.core.CorePresentation;
import org.daisy.urakawa.exceptions.IsNotInitializedException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.IsAlreadyInitializedException;

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
 * @zdepend - Create 1 XmlProperty
 * @zdepend - Create 1 ChannelsProperty
 * @depend - Create 1 Property
 * @depend - - - PropertyType
 */
public interface CorePropertyFactory {

    /**
     * @param xukLocalName
     * @param xukNamespaceUri
     * @return a new Property object corresponding to the given type.
     */
    public Property createProperty(String xukLocalName, String xukNamespaceUri);

    /**
     * @return the Presentation to which the CoreNodeFactory belongs. Cannot return null.
     */
    public CorePresentation getPresentation() throws IsNotInitializedException;

    /**
     * @param presentation cannot be null;
     * @stereotype Initialize
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public void setPresentation(CorePresentation presentation) throws MethodParameterIsNullException, IsAlreadyInitializedException;
}
