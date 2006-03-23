package org.daisy.urakawa.exceptions;

/**
 * Exception that requires:
 * - to be declared in the method signature ("throws"" statement in Java)
 * - to be caught with mandatory try/catch/finally structures.
 * Otherwise, compile-time errors will be generated.
 * (in the case of Java, and presumaby C# as well. Not sure about C++ exception handling though)
 * In other words, the exception declaration itself is part of the contract defined by the model.
 * Such exceptions can actually be used to assert assumptions during unit-testing. When raised: the test fails.
 */
class CheckedException {

/**
 * The name of the Exception sub-class should be self-explicit.
 * It should be plain english that describes the error (e.g.: "SomethingWasNotFound", "StuffCouldNotConnect").
 * The mMessage string is a complementary message that can be NULL or empty,
 * but when it's not, the Exception framework will display it in the console to inform the developer.
 * This message can also be used to display in a proper GUI at the application level,
 * in order to have a user-friendly error handling framework.
 */
private string mMessage;

/**
 * @return mMessage. Can be NULL or empty.
 */
public string getMessage() {return mMessage;} 

/**
 * Constructor with mandatory member initializer, given by the constructor parameter which can be NULL or empty.
 * 
 * @param message 
 */
public CheckedException(string message) {mMessage = message;} 
}