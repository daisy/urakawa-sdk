package org.daisy.urakawa;

/**
 * @tagvalue Notes "{Test,
 *           <br/> TEST <br/>
 *           <br/> tEst }Notes"
 * This interface is a place-holder for a comment, it is *not* part of the API,
 * please do not implement !!
 * <p>
 * An interface marked with "@leafInterface" is a special type of interface:
 * Such interface (e.g. "MyType") has a direct matching implementation (e.g.
 * "MyTypeImpl"), and only *one*. It is present in the data model in order for
 * the reader to easily identify the set of methods that defines its qualities
 * (otherwise it gets all mixed-up in the resulting concrete class). However,
 * implementors may choose not to expose this interface in the facade API of
 * their reference implementation (in other words, it's optional), and instead,
 * use the matching concrete class directly as a type. However, it is judicious
 * to keep such interface in the API of an SDK reference implementation, if for
 * example, multiple implementations are provided. This is typically applicable
 * if the factory design pattern is used to generate instances of a type,
 * possibly based on several different actual implementations of the type.
 * </p>
 */
public interface LeafInterface {
}
