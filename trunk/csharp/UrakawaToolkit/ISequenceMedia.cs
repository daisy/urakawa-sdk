using System;

namespace urakawa.media
{
	/// <summary>
	/// A sequence is a collection of any media object.
	/// All objects in the collection must be of the same type.
	/// </summary>
	public interface ISequenceMedia : IMedia
	{
		IMedia getItem(int index); 

		IMedia setItem(int index, IMedia newItem);

		void appendItem(IMedia newItem);

		IMedia removeItem(int index); 

		int getCount();
	}
}
