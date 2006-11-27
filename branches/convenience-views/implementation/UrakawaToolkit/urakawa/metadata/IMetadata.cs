using System;
using urakawa.xuk;

namespace urakawa.metadata
{
	/// <summary>
	/// Generic interface that supports XUK in/out
	/// </summary>
	public interface IMetadata : IXukAble, IValueEquatable<IMetadata>
	{
    /// <summary>
    /// Gets the name of the <see cref="IMetadata"/>
    /// </summary>
    /// <returns>The name</returns>
    string getName();

    /// <summary>
    /// Sets the name of the <see cref="IMetadata"/>
    /// </summary>
    /// <param name="newName">The new name</param>
    void setName(string newName);
	}
}
