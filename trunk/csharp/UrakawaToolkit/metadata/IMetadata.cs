using System;
using System.Collections.Generic;
using urakawa.xuk;

namespace urakawa.metadata
{
	/// <summary>
	/// Metadata item interface
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

		/// <summary>
		/// Gets the textual content of the <see cref="IMetadata"/> item.
		/// Convenience for <c><see cref="getOptionalAttributeValue"/>("Content")</c>
		/// </summary>
		/// <returns>The textual content</returns>
		string getContent();

		/// <summary>
		/// Sets the textual content of the <see cref="IMetadata"/> item.
		/// Convenience for <c><see cref="setOptionalAttributeValue"/>("Content", <paramref name="value"/>)</c>
		/// </summary>
		/// <param name="value">The new content value</param>
		void setContent(string value);

		/// <summary>
		/// Gets an attribute value of the <see cref="IMetadata"/> item by name
		/// </summary>
		/// <param name="name">The name of the attribute to get</param>
		/// <returns>The attribute value - <see cref="String.Empty"/> if no attribute exists with the given name</returns>
		string getOptionalAttributeValue(string name);

		/// <summary>
		/// Sets the value of a named attribute
		/// </summary>
		/// <param name="name">The name of the attribute</param>
		/// <param name="value">The new value</param>
		void setOptionalAttributeValue(string name, string value);

		/// <summary>
		/// Gets the names of all set attributes
		/// </summary>
		/// <returns>The <see cref="List{String}"/> of names</returns>
		List<string> getOptionalAttributeNames();
	}
}
