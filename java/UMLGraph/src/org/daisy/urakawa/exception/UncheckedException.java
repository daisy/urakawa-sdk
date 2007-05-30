package org.daisy.urakawa.exception;

/**
 * Exception that doesn't require:
 * - to be declared in the method signature ("throws" statement in Java)
 * - to be caught with mandatory try/catch/finally structures.
 * In other words, it can be raised from anywhere in the code, with no compiler error if not declared.
 * (in the case of Java, and presumaby C# as well. Not sure about C++ exception handling)
 * Exceptions of that type usually correspond to unexpected errors not handled by the contract defined by the model.
 * (like NullPointerException in Java, which does not exist in C++ afaik)
 * -
 * This class will probably never be used in the implementation of the model,
 * as it is raised in context not defined by the design contract.
 * It is here just to contrast explicitely with the {@link CheckedException} class.
 */
public class UncheckedException extends java.lang.RuntimeException {

	/**
	 * 
	 */
	private static final long serialVersionUID = 6771810869719751079L;
}