package org.daisy.urakawa.exceptions;

/**
 * This exception should be thrown/raised when trying to
 * call an operation (aka class method) on an object that does not
 * allow a specific modification of the state in the current context.
 * This is an UncheckedException so that not every single low-level
 * operation that potentially modifies the state of the data model
 * has to declare the Exception in its method signature.
 * This allows flexibility in the way the business logic is described
 * in the UML diagram, in terms of the excecution flow (no need for a full state chart).
 * Wherever a "canDoXXX()" method can be found, the corresponding operation "doXXX()"
 * should use this exception/error to let the user-agent of the API/Toolkit
 * know about the non-permitted operation for which there was an attempt to execute.
 */
public class OperationNotValidException extends UncheckedException {
}
