package org.daisy.urakawa;

/**
 * The abstract concept of an ID for an interface.
 * Just a way of uniquely identifying an interface type,
 * so that there is no technical dependency on some "instanceof"-like operator.
 *
 * The actual implementation of this paradigm is implementation-specific
 * and this design should therefore not be taken literaly.
 * For example, in Java this could be *implemented* using "instanceof",
 * whereas in SmallTalk there would be a specific "conformTo" operator for interfaces.
 *
 * @see InterfaceID
 * @depend 1 Aggregation 1 InterfaceID
 */
public interface IdentifiableInterface {
    /**
     * @return a unique "id" for specifying the type of an interface, not its implementation.
     */
    public InterfaceID getInterfaceID();

    /**
     * @param iid the unique "id" of an interface type, not its implementation.
     * @return true if this interface is a subtype of the given interface (known uniquely via its "id").
     */
    public boolean conformsTo(InterfaceID iid);
}
