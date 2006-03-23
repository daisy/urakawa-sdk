package org.daisy.urakawa.exceptions;

/**
 * Some methods have parameters of numeric type (float, int, uint, etc.).
 * This exception should be thrown when values are out of bounds.
 */
class MethodParameterIsValueOutOfBounds extends MethodParameterIsInvalid {}