package org.daisy.urakawa;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * This interface defines an "equals" method
 * to compare that the _values_ for a given class type are equals.
 * Implementing classes of type "T" define the concept of value
 * and implement the process required to verify equality.
 * (this might, for example, imply deep-recursion inside the object's components)
 * In java, the native "equals" method on the root Object class type provides similar functionality, when implemented correctly.
 * The C# framework provides similar features for testing equality of types. 
 * To avoid ambiguity (for example, equality tested on pointer values vs actual object value based on type-specific semantics),
 * this interface is used instead of the language built-in "equal" functionality.  
 *
 * @param <T> The class type for which to compare equality.
 */
public interface ValueEquatable<T> {

	/**
	 * @param other Object to compare value equality for. Cannot be null.
	 * @return True if this has the same value as the given parameter. Otherwise false (this and "other" have different values).
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean ValueEquals(T other) throws MethodParameterIsNullException;
}
