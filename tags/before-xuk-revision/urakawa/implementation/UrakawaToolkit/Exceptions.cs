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
	/// whose name does not exist in the list of current channels.
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
	/// whose name is already used in the list of current channels.
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

	//this used to be in urakawa.core.property
	//when the property namespace was removed (to comply with the design), the best place
	//for this class seemed here with the other exceptions
//
//	public class NonAllowedQNameException : UncheckedException
//	{
//		public NonAllowedQNameException()
//		{
//		}
//		override public string Message
//		{
//			get
//			{
//				return "The supplied string did not match the RegEx '\\A[_a-zA-Z]+[_a-zA-Z0-9]*\\Z'"; 
//			}
//		}
//	}

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

}
