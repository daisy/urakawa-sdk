package org.daisy.urakawa;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * This interface defines an "equals" method that compares the
 * <strong>values<strong> for 2 object instances. Classes that realize this
 * interface must implement the logic required to compare the object "values"
 * (i.e. may involve recursive comparison of sub-objects).
 * </p>
 * <p>
 * This should be used with care, because by mathematical definition, an "equal"
 * operator is that it must be reflexive, symmetric, and transitive. It is
 * mainly used for unit-testing the SDK.
 * </p>
 * 
 * @param <T> The object type on which to apply the equality operator.
 */
public interface IValueEquatable<T>
{
    /**
     * <p>
     * Compares the values of this and the given parameter.
     * </p>
     * 
     * @param other Object instance to compare value equality with. Cannot be
     *        null.
     * @return True if this has the same value as the given parameter. Otherwise
     *         false.
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * 
     */
    public boolean ValueEquals(T other) throws MethodParameterIsNullException;
}
