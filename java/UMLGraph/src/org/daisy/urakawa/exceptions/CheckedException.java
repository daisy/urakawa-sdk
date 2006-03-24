package org.daisy.urakawa.exceptions;

/**
 * Exception that requires:
 * - to be declared in the method signature ("throws"" statement in Java)
 * - to be caught with mandatory try/catch/finally structures.
 * Otherwise, compile-time errors will be generated.
 * (in the case of Java, and presumaby C# as well. Not sure about C++ exception handling though)
 * In other words, the exception declaration itself is part of the contract defined by the model.
 * Such exceptions can actually be used to assert assumptions during unit-testing. When raised: the test fails.
 * 
 * Note: a common mistake is when developers insert the mandatory "try"+"catch"+"finally" cascaded structures
 * but write no specific action and "swallow" the Exception raised at runtime.
 * This is very bad practice and an application should always handle raised exceptions to avoid
 * errors to silently be ignored...and later create an un-detectable bug.
 * 
 * When a GUI is present than it should take care of notifying the user and prompting for action.
 * 
 * Otherwise another way for not ignoring exceptions is by forwarding them in the form of an
 * {@link UncheckedException}. This can help as a temporary mesure, but real execution flow control
 * strategies should ultimately be implemented by the client. 
 * 
 * It is important to realize that the although model presented here is based on real Exceptions (Java in this case),
 * it is only for the sole purpose of explicitely describing the execution flow under certain conditions.
 * Whether or not the implementation of this model is realized with Exceptions or with return HRESULT values,
 * or even with out-parameters, is another matter. It is the responsibility of the implementation
 * to determine the best technical solution for achieving the specifications given here.
 * It is obviously very much language-dependent, but one can expect that most high-level
 * (object-oriented) programming languages will provide some sort of Exceptions framework or native handling.
 */
public class CheckedException extends java.lang.Exception {
}