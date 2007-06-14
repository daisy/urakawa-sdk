package org.daisy.urakawa.xuk;

import org.daisy.urakawa.exception.CheckedException;

/**
 * <p>
 * This exception is raised when an attempt to serialize the object model into
 * the XUK format fails.
 * </p>
 * <p>
 * This should really be considered as a base class, to be sub-classed by
 * implementors into more concrete exception types. It can also be used as a
 * wrapper for a implementation-dependent exception type. For example,
 * implementations may use different XML processors and therefore may
 * potentially use totally different XML error types.
 * </p>
 */
public class XukSerializationFailedException extends CheckedException {
	/**
	 * 
	 */
	private static final long serialVersionUID = 1482461742466983879L;
}
