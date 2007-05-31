using System;

namespace urakawa.exception
{
	/// <summary>
	/// Summary description for CheckedException.
	/// Exceptions of this type must be caught.
	/// </summary>
	public class CheckedException : Exception
	{
    /// <summary>
    /// Constructor setting the message of the exception
    /// </summary>
    /// <param name="msg">The message</param>
		public CheckedException(string msg) : base(msg)
		{
		}

    /// <summary>
    /// Constructor setting the message and inner <see cref="Exception"/> of the exception
    /// </summary>
    /// <param name="msg">The message</param>
    /// <param name="inner">The inner exception</param>
    public CheckedException(string msg, Exception inner) : base(msg, inner)
		{
		}
	}

	/// <summary>
	/// Exception thrown when a node does not exists in a child collection
	/// </summary>
	public class NodeDoesNotExistException : CheckedException
	{
		/// <summary>
		/// Constructor setting the message of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		public NodeDoesNotExistException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Constructor setting the message and inner <see cref="Exception"/> of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="inner">The inner exception</param>
		public NodeDoesNotExistException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}

	/// <summary>
	/// Exception thrown when trying to insert a node that is not detached
	/// </summary>
	public class NodeNotDetachedException : CheckedException
	{
		/// <summary>
		/// Constructor setting the message of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		public NodeNotDetachedException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Constructor setting the message and inner <see cref="Exception"/> of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="inner">The inner exception</param>
		public NodeNotDetachedException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}

	/// <summary>
	/// Exception thrown when a previous matching node exists in a child collection, where nodes are supposed to be exclusive
	/// </summary>
	public class NodeAlreadyExistException : CheckedException
	{
		/// <summary>
		/// Constructor setting the message of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		public NodeAlreadyExistException(string msg) : base(msg)
		{
		}

		/// <summary>
		/// Constructor setting the message and inner <see cref="Exception"/> of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="inner">The inner exception</param>
		public NodeAlreadyExistException(string msg, Exception inner) : base(msg, inner)
		{
		}
	}

	/// <summary>
	/// Abstract class to encapsulate errors related to wrong values for method parameters.
	/// This class cannot be instanciated and should be sub-classed.
	/// The aim is to avoid situations where values that are potentially
	/// detrimental to software integrity are silently ignored, or "swallowed".
	/// </summary>
	public abstract class MethodParameterIsInvalidException : CheckedException
	{
    /// <summary>
    /// Constructor setting the message of the exception
    /// </summary>
    /// <param name="msg">The message</param>
    protected MethodParameterIsInvalidException(string msg) : base(msg)
		{
		}

    /// <summary>
    /// Constructor setting the message and inner <see cref="Exception"/> of the exception
    /// </summary>
    /// <param name="msg">The message</param>
    /// <param name="inner">The inner exception</param>
    protected MethodParameterIsInvalidException(string msg, Exception inner) : base(msg, inner)
		{
		}
	}

  /// <summary>
  /// Some methods have stricter type rules than specified by the method signature.
  /// This exception should be raised when such type rules are broken.
  /// </summary>
  public class MethodParameterIsWrongTypeException : MethodParameterIsInvalidException
  {
    /// <summary>
    /// Constructor setting the message of the exception
    /// </summary>
    /// <param name="msg">The message</param>
    public MethodParameterIsWrongTypeException(string msg) : base(msg)
		{
		}

    /// <summary>
    /// Constructor setting the message and inner <see cref="Exception"/> of the exception
    /// </summary>
    /// <param name="msg">The message</param>
    /// <param name="inner">The inner exception</param>
    public MethodParameterIsWrongTypeException(string msg, Exception inner) : base(msg, inner)
		{
		}
  }

	/// <summary>
	/// Some methods forbid passing NULL values.
	/// This exception should be raised when NULL values are passed.
	/// </summary>
	public class MethodParameterIsNullException : MethodParameterIsInvalidException
	{
    /// <summary>
    /// Constructor setting the message of the exception
    /// </summary>
    /// <param name="msg">The message</param>
    public MethodParameterIsNullException(string msg) : base(msg)
		{
		}

    /// <summary>
    /// Constructor setting the message and inner <see cref="Exception"/> of the exception
    /// </summary>
    /// <param name="msg">The message</param>
    /// <param name="inner">The inner exception</param>
    public MethodParameterIsNullException(string msg, Exception inner) : base(msg, inner)
		{
		}
	}

	/// <summary>
	/// Some methods have parameters of numeric type (float, int, uint, etc.).
	/// This exception should be thrown when values are out of allowed bounds.
	/// </summary>
	public class MethodParameterIsOutOfBoundsException : MethodParameterIsInvalidException
	{
    /// <summary>
    /// Constructor setting the message of the exception
    /// </summary>
    /// <param name="msg">The message</param>
    public MethodParameterIsOutOfBoundsException(string msg) : base(msg)
		{
		}

    /// <summary>
    /// Constructor setting the message and inner <see cref="Exception"/> of the exception
    /// </summary>
    /// <param name="msg">The message</param>
    /// <param name="inner">The inner exception</param>
    public MethodParameterIsOutOfBoundsException(string msg, Exception inner) : base(msg, inner)
		{
		}
	}

	/// <summary>
	/// Some methods forbid passing empty String values.
	///  This exception should be thrown when empty String values are passed.
	/// </summary>
	public class MethodParameterIsEmptyStringException : MethodParameterIsInvalidException
	{
    /// <summary>
    /// Constructor setting the message of the exception
    /// </summary>
    /// <param name="msg">The message</param>
    public MethodParameterIsEmptyStringException(string msg) : base(msg)
		{
		}

    /// <summary>
    /// Constructor setting the message and inner <see cref="Exception"/> of the exception
    /// </summary>
    /// <param name="msg">The message</param>
    /// <param name="inner">The inner exception</param>
    public MethodParameterIsEmptyStringException(string msg, Exception inner) : base(msg, inner)
		{
		}
	}

	/// <summary>
	/// This exception should be thrown when trying to remove a Channel
	/// whose localName does not exist in the list of current channels.
	/// </summary>
	public class ChannelDoesNotExistException : CheckedException
	{
    /// <summary>
    /// Constructor setting the message of the exception
    /// </summary>
    /// <param name="msg">The message</param>
    public ChannelDoesNotExistException(string msg) : base(msg)
		{
		}

    /// <summary>
    /// Constructor setting the message and inner <see cref="Exception"/> of the exception
    /// </summary>
    /// <param name="msg">The message</param>
    /// <param name="inner">The inner exception</param>
    public ChannelDoesNotExistException(string msg, Exception inner) : base(msg, inner)
		{
		}
	}

	/// <summary>
	/// This exception should be thrown when trying to add a Channel
	/// whose localName is already used in the list of current channels.
	/// </summary>
	public class ChannelAlreadyExistsException : CheckedException
	{
    /// <summary>
    /// Constructor setting the message of the exception
    /// </summary>
    /// <param name="msg">The message</param>
    public ChannelAlreadyExistsException(string msg) : base(msg)
		{
		}

    /// <summary>
    /// Constructor setting the message and inner <see cref="Exception"/> of the exception
    /// </summary>
    /// <param name="msg">The message</param>
    /// <param name="inner">The inner exception</param>
    public ChannelAlreadyExistsException(string msg, Exception inner) : base(msg, inner)
		{
		}
	}

	/// <summary>
	/// This exception should be raised when trying to use a MediaType that is not legal in the current context.
	/// </summary>
	public class MediaTypeIsIllegalException : CheckedException
	{
    /// <summary>
    /// Constructor setting the message of the exception
    /// </summary>
    /// <param name="msg">The message</param>
    public MediaTypeIsIllegalException(string msg) : base(msg)
		{
		}

    /// <summary>
    /// Constructor setting the message and inner <see cref="Exception"/> of the exception
    /// </summary>
    /// <param name="msg">The message</param>
    /// <param name="inner">The inner exception</param>
    public MediaTypeIsIllegalException(string msg, Exception inner) : base(msg, inner)
		{
		}
	}

	/// <summary>
	/// This exception should be raised when trying to use a time offset that is not allowed to be negative.
	/// </summary>
	public class TimeOffsetIsNegativeException : CheckedException
	{
		/// <summary>
		/// Constructor setting the message of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		public TimeOffsetIsNegativeException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Constructor setting the message and inner <see cref="Exception"/> of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="inner">The inner exception</param>
		public TimeOffsetIsNegativeException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}

	/// <summary>
	/// This exception should be raised when trying to parse an invalid time string representation
	/// </summary>
	public class TimeStringRepresentationIsInvalidException : CheckedException
	{
		/// <summary>
		/// Constructor setting the message of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		public TimeStringRepresentationIsInvalidException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Constructor setting the message and inner <see cref="Exception"/> of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="inner">The inner exception</param>
		public TimeStringRepresentationIsInvalidException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}

	/// <summary>
	/// Unchecked exceptions do not require catching and handling
	/// </summary>
	public class UncheckedException : Exception
	{
    /// <summary>
    /// Constructor setting the message of the exception
    /// </summary>
    /// <param name="msg">The message</param>
    public UncheckedException(string msg) : base(msg)
		{
		}

    /// <summary>
    /// Constructor setting the message and inner <see cref="Exception"/> of the exception
    /// </summary>
    /// <param name="msg">The message</param>
    /// <param name="inner">The inner exception</param>
    public UncheckedException(string msg, Exception inner) : base(msg, inner)
		{
		}
	}

	/// <summary>
	/// (from the design docs)
	/// This exception should be thrown/raised when trying to
	/// call an operation (aka class method) on an object that does not
	/// allow a specific modification of the state in the current context.
	/// ...
	/// Wherever a "canDoXXX()" method can be found, the corresponding operation "doXXX()"
	/// should use this exception/error to let the user-agent of the API/Toolkit
	/// know about the non-permitted operation for which there was an attempt to execute.
	/// </summary>
	public class OperationNotValidException : UncheckedException
	{
    /// <summary>
    /// Constructor setting the message of the exception
    /// </summary>
    /// <param name="msg">The message</param>
    public OperationNotValidException(string msg) : base(msg)
		{
		}

    /// <summary>
    /// Constructor setting the message and inner <see cref="Exception"/> of the exception
    /// </summary>
    /// <param name="msg">The message</param>
    /// <param name="inner">The inner exception</param>
    public OperationNotValidException(string msg, Exception inner) : base(msg, inner)
		{
		}
	}

	/// <summary>
	/// This exception should be raised when trying to use a PropertyType that is not legal in the current context.
	/// </summary>
	public class PropertyTypeIsIllegalException : CheckedException
	{
		/// <summary>
		/// Constructor setting the message of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		public PropertyTypeIsIllegalException(string msg) : base(msg)
		{
		}

		/// <summary>
		/// Constructor setting the message and inner <see cref="Exception"/> of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="inner">The inner exception</param>
		public PropertyTypeIsIllegalException(string msg, Exception inner) : base(msg, inner)
		{
		}
	}

	/// <summary>
	/// Exception thrown when a factory unexpectedly can not create an object of the desired type
	/// </summary>
	public class FactoryCanNotCreateTypeException : CheckedException
	{
		/// <summary>
		/// Constructor setting the message of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		public FactoryCanNotCreateTypeException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Constructor setting the message and inner <see cref="Exception"/> of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="inner">The inner exception</param>
		public FactoryCanNotCreateTypeException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}

	
	/// <summary>
	/// Exception thrown when a factory is missing
	/// </summary>
	public class FactoryIsMissingException : CheckedException
	{
		/// <summary>
		/// Constructor setting the message of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		public FactoryIsMissingException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Constructor setting the message and inner <see cref="Exception"/> of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="inner">The inner exception</param>
		public FactoryIsMissingException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}


	/// <summary>
	/// Exception thrown when a node belongs to a different presentation than expected
	/// </summary>
	public class NodeInDifferentPresentationException : CheckedException
	{
		/// <summary>
		/// Constructor setting the message of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		public NodeInDifferentPresentationException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Constructor setting the message and inner <see cref="Exception"/> of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="inner">The inner exception</param>
		public NodeInDifferentPresentationException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}

	/// <summary>
	/// Exception thrown when a node belongs to a different presentation than expected
	/// </summary>
	public class NodeIsAncestorException : CheckedException
	{
		/// <summary>
		/// Constructor setting the message of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		public NodeIsAncestorException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Constructor setting the message and inner <see cref="Exception"/> of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="inner">The inner exception</param>
		public NodeIsAncestorException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}

	/// <summary>
	/// Exception thrown when a node belongs to a different presentation than expected
	/// </summary>
	public class NodeIsDescendantException : CheckedException
	{
		/// <summary>
		/// Constructor setting the message of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		public NodeIsDescendantException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Constructor setting the message and inner <see cref="Exception"/> of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="inner">The inner exception</param>
		public NodeIsDescendantException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}

	/// <summary>
	/// Exception thrown when a node belongs to a different presentation than expected
	/// </summary>
	public class NodeIsSelfException : CheckedException
	{
		/// <summary>
		/// Constructor setting the message of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		public NodeIsSelfException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Constructor setting the message and inner <see cref="Exception"/> of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="inner">The inner exception</param>
		public NodeIsSelfException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}

	/// <summary>
	/// Exception thrown when a node belongs to a different presentation than expected
	/// </summary>
	public class NodeHasParentException : CheckedException
	{
		/// <summary>
		/// Constructor setting the message of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		public NodeHasParentException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Constructor setting the message and inner <see cref="Exception"/> of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="inner">The inner exception</param>
		public NodeHasParentException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}

	/// <summary>
	/// Exception thrown when a node belongs to a different presentation than expected
	/// </summary>
	public class NodeHasNoParentException : CheckedException
	{
		/// <summary>
		/// Constructor setting the message of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		public NodeHasNoParentException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Constructor setting the message and inner <see cref="Exception"/> of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="inner">The inner exception</param>
		public NodeHasNoParentException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}

	/// <summary>
	/// Exception thrown when an object is being used before it has been initialized
	/// </summary>
	public class IsNotInitializedException : CheckedException
	{
		/// <summary>
		/// Constructor setting the message of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		public IsNotInitializedException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Constructor setting the message and inner <see cref="Exception"/> of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="inner">The inner exception</param>
		public IsNotInitializedException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}

	/// <summary>
	/// Exception thrown when trying to initialize an object that has already been initialized
	/// </summary>
	public class IsAlreadyInitializedException : CheckedException
	{
		/// <summary>
		/// Constructor setting the message of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		public IsAlreadyInitializedException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Constructor setting the message and inner <see cref="Exception"/> of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="inner">The inner exception</param>
		public IsAlreadyInitializedException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}

	
	/// <summary>
	/// Thrown when a given <see cref="Object"/> is not managed by the manager
	/// </summary>
	public class IsNotManagerOfException : CheckedException
	{
		/// <summary>
		/// Constructor setting the message of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		public IsNotManagerOfException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Constructor setting the message and inner <see cref="Exception"/> of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="inner">The inner exception</param>
		public IsNotManagerOfException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}

	
	/// <summary>
	/// Thrown when a given <see cref="Object"/> is already managed by the manager
	/// </summary>
	public class IsAlreadyManagerOfException : CheckedException
	{
		/// <summary>
		/// Constructor setting the message of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		public IsAlreadyManagerOfException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Constructor setting the message and inner <see cref="Exception"/> of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="inner">The inner exception</param>
		public IsAlreadyManagerOfException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}


	
	/// <summary>
	/// Thrown when data does not conform to an expected data format,
	/// eg. when encountering an invalid WAVE header
	/// </summary>
	public class InvalidDataFormatException : CheckedException
	{
		/// <summary>
		/// Constructor setting the message of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		public InvalidDataFormatException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Constructor setting the message and inner <see cref="Exception"/> of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="inner">The inner exception</param>
		public InvalidDataFormatException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}

	
	/// <summary>
	/// Thrown when an input <see cref="System.IO.Stream"/> is shorter than excepted,
	/// that is there are too few <see cref="byte"/>s between the current <see cref="System.IO.Stream.Position"/> 
	/// and the end of the <see cref="System.IO.Stream"/> (<see cref="System.IO.Stream.Length"/>)
	/// </summary>
	public class InputStreamIsTooShortException : CheckedException
	{
		/// <summary>
		/// Constructor setting the message of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		public InputStreamIsTooShortException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Constructor setting the message and inner <see cref="Exception"/> of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="inner">The inner exception</param>
		public InputStreamIsTooShortException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}

	
	/// <summary>
	/// Thrown when a data file used by an object unexpectedly does not exist
	/// </summary>
	public class DataFileDoesNotExistException : CheckedException
	{
		/// <summary>
		/// Constructor setting the message of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		public DataFileDoesNotExistException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Constructor setting the message and inner <see cref="Exception"/> of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="inner">The inner exception</param>
		public DataFileDoesNotExistException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}


	
	/// <summary>
	/// Thrown when a collection/enumeration of items passed as a parameter unexpectedly contains no items
	/// </summary>
	public class MethodParameterHasNoItemsException : MethodParameterIsInvalidException
	{
		/// <summary>
		/// Constructor setting the message of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		public MethodParameterHasNoItemsException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Constructor setting the message and inner <see cref="Exception"/> of the exception
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="inner">The inner exception</param>
		public MethodParameterHasNoItemsException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}

	
	/// <summary>
	/// Thrown when a given <see cref="Uri"/> is invalid
	/// </summary>
	public class InvalidUriException : CheckedException
	{
		/// <summary>
		/// Constructor setting the message of the exception
		/// </summary>
		/// <param localName="msg">The message</param>
		public InvalidUriException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Constructor setting the message and inner <see cref="Exception"/> of the exception
		/// </summary>
		/// <param localName="msg">The message</param>
		/// <param localName="inner">The inner exception</param>
		public InvalidUriException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}



}
