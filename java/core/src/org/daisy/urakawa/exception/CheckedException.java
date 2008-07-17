package org.daisy.urakawa.exception;

/**
 * <p>
 * This base type of exception needs to be declared in the method signature when
 * thrown (aka "raised") from the method body, or needs to be caught using the
 * standard try/catch/finally flow control structure. Most languages have native
 * support for exceptions and the compiler will probably generate errors when
 * not following the rules previously mentioned.
 * </p>
 * <p>
 * This is not actually a design requirement in resulting implementations,
 * because using this type of exceptions can have a significant runtime cost.
 * However, implementations which enforce the use of checked-exceptions will not
 * only see the benefits during the testing phase (for asserting values or
 * behaviors), but also while regular users enjoy the application, thanks to
 * accurate error reporting based with easy-to-track stack trace reports (which
 * can be generated in a convenient UI, for the user to submit bug-reports).
 * </p>
 */
public class CheckedException extends java.lang.Exception {
	/**
	 * 
	 */
	private static final long serialVersionUID = -2560128942581692365L;

	public CheckedException() {
		super();
	}

	public CheckedException(Exception e) {
		super(e);
	}
}