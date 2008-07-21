package org.daisy.urakawa;

/**
 * <p>
 * This interface is a place-holder for a comment, it is *not* part of the API,
 * please do not implement !!
 * </p>
 * <p>
 * An interface marked with "@designConvenienceInterface" is a special type of
 * interface, that does not need to exist in the reference implementation (in
 * other words, it's optional). It is present in the data model as a convenience
 * to help maintaining the design:
 * </p>
 * <ul>
 * <li>the methods in such interface are usually inherited by several other
 * object types, so having a single interface to centralize the methods avoids
 * redundancy, makes it less prone to errors: one single change in the design
 * contract defined in this interface automatically impacts all types that use
 * the interface.</li>
 * <li>if it's not for the first reason above (re-usability), having a separate
 * interface for a whole set of methods helps organizing the design code in
 * distinct, semantically-related modules which are easier to understand in the
 * context of the global architecture.</li>
 * </ul>
 */
public interface DesignConvenienceInterface
{
    /**
     * Does nothing.
     */
}