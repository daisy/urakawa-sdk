package exceptions;

import java.util.List;
import java.util.Map;
import java.util.ArrayList;
import java.util.Arrays;

/**
 * @view
 * @opt nodefillcolor LightGray
 *
 * @match class *
 * @opt hide
 *
 * @match class exceptions.*
 * @opt !hide
 *
 */
class ViewExceptions extends ViewBase {}


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


/**
 * Exception that doesn't require:
 * - to be declared in the method signature ("throws" statement in Java)
 * - to be caught with mandatory try/catch/finally structures.
 * In other words, it can be raised from anywhere in the code, with no compiler error if not declared.
 * (in the case of Java, and presumaby C# as well. Not sure about C++ exception handling)
 * Exceptions of that type usually correspond to unexpected errors not handled by the contract defined by the model.
 * (like NullPointerException in Java, which does not exist in C++ afaik)
 */
class UncheckedException {}

/**
 * Some methods forbid passing NULL values.
 * This exception should be raised when NULL values are passed.
 */
class MethodParameterIsNull extends MethodParameterIsInvalid {}

/**
 * Some methods have parameters of numeric type (float, int, uint, etc.).
 * This exception should be thrown when values are out of bounds.
 */
class MethodParameterIsValueOutOfBounds extends MethodParameterIsInvalid {}

/**
 * Abstract class to encapsulate errors related to wrong values for method parameters.
 * This class cannot be instanciated and should be sub-classed.
 */
abstract class MethodParameterIsInvalid extends CheckedException {}

/**
 * 
 */
class NodeDoesNotExist extends CheckedException {}

/**
 * Some methods forbid passing empty String values.
 * This exception should be thrown when empty String values are passed.
 */
class MethodParameterIsEmptyString extends MethodParameterIsInvalid {}

/**
 * This exception should be thrown when trying to remove a Channel
 * which name does not exist in the list of current channels.
 */
class ChannelNameDoesNotExist extends CheckedException {}

/**
 * This exception should be thrown when trying to add a Channel
 * which name is already used in the list of current channels.
 */
class ChannelNameAlreadyExist extends CheckedException {}

