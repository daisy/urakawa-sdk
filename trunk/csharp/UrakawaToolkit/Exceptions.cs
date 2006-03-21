using System;

namespace urakawa.core.exception
{
	/// <summary>
	/// Summary description for CheckedException.
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
  public class NodeDoesNotExistException : urakawa.core.exception.CheckedException
  {
    public NodeDoesNotExistException(string msg) : base(msg)
    {
    }

    public NodeDoesNotExistException(string msg, Exception inner) : base(msg, inner)
    {
    }


  }
  /// <summary>
  /// Summary description for MethodParameterIsInvalidException.
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

  public class MethodParameterIsNullException : MethodParameterIsInvalidException
  {
    public MethodParameterIsNullException(string msg) : base(msg)
    {
    }
  }

  public class MethodParameterIsValueOutOfBoundsException : MethodParameterIsInvalidException
  {
    public MethodParameterIsValueOutOfBoundsException(string msg) : base(msg)
    {
    }

    public MethodParameterIsValueOutOfBoundsException(string msg, Exception inner) : base(msg, inner)
    {
    }
  }
}
