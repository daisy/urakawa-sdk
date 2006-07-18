using System;

namespace urakawa.core
{
	/// <summary>
	/// Interface for a <see cref="ICoreNode"/> factory
	/// </summary>
	public interface ICoreNodeFactory
	{
    /// <summary>
    /// Creates a new <see cref="ICoreNode"/>
    /// </summary>
    /// <returns>The new <see cref="ICoreNode"/></returns>
    ICoreNode createNode();
	}
}
