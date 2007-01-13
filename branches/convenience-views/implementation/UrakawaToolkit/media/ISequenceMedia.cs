using System;

namespace urakawa.media
{
	/// <summary>
	/// A sequence is a collection of any media object.
	/// All objects in the collection must be of the same type.
	/// </summary>
	public interface ISequenceMedia : IMedia
	{
		/// <summary>
		/// Return the media object at a given index.
		/// </summary>
		/// <param localName="index">The given index</param>
		/// <returns>The <see cref="IMedia"/> item at the given index</returns>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the given index is out of bounds
		/// </exception>
		IMedia getItem(int index); 

		/// <summary>
		/// Inserts a given <see cref="IMedia"/> item at a given index
		/// </summary>
		/// <param localName="index">The given index</param>
		/// <param localName="newItem">The given <see cref="IMedia"/> item</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the given <see cref="IMedia"/> to insert is <c>null</c>
		/// </exception>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the given index is out of bounds
		/// </exception>
		/// <exception cref="exception.MethodParameterIsWrongTypeException">
		/// The <see cref="IMedia"/> item to insert has a <see cref="MediaType"/> that 
		/// is incompatible with the <see cref="ISequenceMedia"/>
		/// </exception>
		/// <remarks>
		/// The first <see cref="IMedia"/> inserted into an <see cref="ISequenceMedia"/> 
		/// determines it's <see cref="MediaType"/>. 
		/// Prior to the first insertion an <see cref="ISequenceMedia"/> has <see cref="MediaType"/>
		/// <see cref="MediaType.EMPTY_SEQUENCE"/>
		/// </remarks>
		void insertItem(int index, IMedia newItem);

		/// <summary>
		/// Remove the <see cref="IMedia"/> item at a given index.
		/// </summary>
		/// <param localName="index">The given index</param>
		/// <returns>The <see cref="IMedia"/> item that was removed</returns>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the given index is out of bounds
		/// </exception>
		IMedia removeItem(int index); 

		/// <summary>
		/// Return the number of <see cref="IMedia"/> items in the sequence
		/// </summary>
		/// <returns>The number of <see cref="IMedia"/> items</returns>
		int getCount();
	}
}
