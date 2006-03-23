package org.daisy.urakawa.exceptions;


/**
 * Some methods forbid passing empty String values.
 * This exception should be thrown when empty String values are passed.
 */
class MethodParameterIsEmptyString extends MethodParameterIsInvalid {}