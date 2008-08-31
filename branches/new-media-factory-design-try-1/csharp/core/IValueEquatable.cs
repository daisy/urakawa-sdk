using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa
{
    /// <summary>
    /// Defines a generalized method that classes must implement to create type-specific method for 
    /// determining value equality of instances
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    public interface IValueEquatable<T>
    {
        /// <summary>
        /// Determines of <c>this</c> has the same value as a given other instance
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>A <see cref="bool"/> indicating the result</returns>
        bool ValueEquals(T other);
    }
}