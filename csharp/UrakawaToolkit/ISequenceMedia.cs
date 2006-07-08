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
		/// Return the media object at the index given.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		IMedia getItem(int index); 

		/// <summary>
		/// Set the media item at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="newItem"></param>
		/// <returns></returns>
		IMedia setItem(int index, IMedia newItem);

		/// <summary>
		/// Append a media object to the sequence.
		/// </summary>
		/// <param name="newItem"></param>
		void appendItem(IMedia newItem);

		/// <summary>
		/// Remove the item at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		IMedia removeItem(int index); 

		/// <summary>
		/// Return the number of media objects in the sequence
		/// </summary>
		/// <returns></returns>
		int getCount();
	}
}
