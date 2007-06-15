package org.daisy.urakawa;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * This interface defines an "equals" method that compares the _values_ for 2
 * objects of a given type, not just the _pointer reference_.
 * </p>
 * <p>
 * Classes that implement this interface must specify the type "T" to compare
 * values for. This allows specific classes to define the concept of value and
 * implement the process required to verify equality (for example, this may
 * imply performing deep-recursion "inside" the object components).
 * </p>
 * <p>
 * This should be used with care, because by mathematical definition, an "equal"
 * operator should be reflexive, symmetric, and transitive.
 * </p>
 * 
 * @param <T>
 *            The object type on which to apply the equality operator.
 */
public interface ValueEquatable<T> {
	/**
	 * <p>
	 * Compares the values of this and the given parameter.
	 * </p>
	 * 
	 * @param other
	 *            Object to compare value equality with. Cannot be null.
	 * @return True if this has the same value as the given parameter. Otherwise
	 *         false.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean ValueEquals(T other) throws MethodParameterIsNullException;
}
