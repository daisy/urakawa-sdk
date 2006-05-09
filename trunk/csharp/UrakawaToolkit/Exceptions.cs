using System;

namespace urakawa.exception
{
	/// <summary>
	/// Summary description for CheckedException.
	/// Exceptions of this type must be caught.
	/// </summary>
	public class CheckedException : Exception
	{
		public CheckedException(string msg) : base(msg)
		{
		}

		public CheckedException(string msg, Exception inner) : base(msg, inner)
		{
		}
	}

	/// <summary>
	/// Exception thrown when a node does not exists in a child collection
	/// </summary>
	public class NodeDoesNotExistException : CheckedException
	{
		public NodeDoesNotExistException(string msg) : base(msg)
		{
		}

		public NodeDoesNotExistException(string msg, Exception inner) : base(msg, inner)
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
		protected MethodParameterIsInvalidException(string msg) : base(msg)
		{
		}

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
		public MethodParameterIsWrongTypeException(string msg) : base(msg)
		{
		}

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
		public MethodParameterIsNullException(string msg) : base(msg)
		{
		}

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
		public MethodParameterIsOutOfBoundsException(string msg) : base(msg)
		{
		}

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
		public MethodParameterIsEmptyStringException(string msg) : base(msg)
		{
		}

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
		public ChannelDoesNotExistException(string msg) : base(msg)
		{
		}

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
		public ChannelAlreadyExistsException(string msg) : base(msg)
		{
		}

		public ChannelAlreadyExistsException(string msg, Exception inner) : base(msg, inner)
		{
		}
	}

	/// <summary>
	/// This exception should be raised when trying to use a MediaType that is not legal in the current context.
	/// </summary>
	public class MediaTypeIsIllegalException : CheckedException
	{
		public MediaTypeIsIllegalException(string msg) : base(msg)
		{
		}

		public MediaTypeIsIllegalException(string msg, Exception inner) : base(msg, inner)
		{
		}
	}

	/// <summary>
	/// This exception should be raised when trying to use a time offset that is not allowed to be negative.
	/// </summary>
	public class TimeOffsetIsNegativeException : CheckedException
	{
		public TimeOffsetIsNegativeException(string msg) : base(msg)
		{
		}

		public TimeOffsetIsNegativeException(string msg, Exception inner) : base(msg, inner)
		{
		}
	}

	/// <summary>
	/// Unchecked exceptions do not require catching & handling
	/// </summary>
	public class UncheckedException : Exception
	{
		public UncheckedException(string msg) : base(msg)
		{
		}

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
		public OperationNotValidException(string msg) : base(msg)
		{
		}

		public OperationNotValidException(string msg, Exception inner) : base(msg, inner)
		{
		}
	}

}
