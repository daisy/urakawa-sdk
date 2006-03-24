package org.daisy.urakawa.exceptions;

/**
 * Abstract class to encapsulate errors related to wrong values for method parameters.
 * This class cannot be instanciated and should be sub-classed.
 * 
 * The aim is to avoid situations where values that are potentially
 * nefast to software integrity are silently ignored, or "swallowed".
 * 
 * @stereotype abstract
 */
public abstract class MethodParameterIsInvalid extends CheckedException {
}