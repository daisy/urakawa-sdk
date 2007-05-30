package org.daisy.urakawa.core;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * Abstract factory pattern: from the API user perspective: do not use
 * constructors, use a factory instead (which will delegate to the real
 * constructor of its choice). - A Factory offers much more flexibility than
 * standard constructors. For example, optimized constructors can be used for
 * instanciating many objects at once (e.g. parallel processing). - Another
 * example is to have a memory-efficient object allocator for when instanciating
 * many objects of the same type throught the course of the execution of the
 * program, by always returning the same "Flyweight" instance of the object
 * (e.g. Text media object is likely to created thousands of times in a Daisy
 * book, for each small fragment of text). Implementation of the "Flyweight"
 * pattern are quite common: Dom4J (Namespace object), Swing (TreeRenderer),
 * etc. More info:
 * http://exciton.cs.rice.edu/javaresources/DesignPatterns/FlyweightPattern.htm -
 * This factory may be implemented as a singleton, but this is not a
 * requirement. The implementation can decide what pattern suits it best. - This
 * factory should take care of building certain types of Nodes, characterized by
 * their Property attribute. For example, Daisy nodes should include a
 * ChannelsProperty with audio and text channel, which imply that the channel
 * must be registered by the ChannelsManager beforeHand. This also has
 * implication on what a Validator does for the type of Node created.
 * 
 * @depend - Create 1 CoreNode
 */
public interface CoreNodeFactory extends WithCorePresentation {
	/**
	 * Creates a new node, which is not linked to the core data tree yet.
	 * 
	 * @return cannot return null.
	 */
	public CoreNode createNode();

	/**
	 * The namespace+name combination defines the key to a map that provides
	 * specific node implementation. This is used for allowing CoreNode to be
	 * deserialized in XUK format.
	 * 
	 * @param xukLocalName
	 * @param xukNamespaceURI
	 * @return can return null (in case the NS:name specification does not match
	 *         any supported node type).
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 */
	public CoreNode createNode(String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException;
}