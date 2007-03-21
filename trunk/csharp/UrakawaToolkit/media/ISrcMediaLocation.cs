using System;
namespace urakawa.media
{
	/// <summary>
	/// Interface for a <see cref="IMediaLocation"/> relying of a <see cref="String"/> src attribute
	/// </summary>
	public interface ISrcMediaLocation : IMediaLocation
	{
		/// <summary>
		/// Gets the src attribute
		/// </summary>
		/// <returns>The src attribute</returns>
		string getSrc();

		/// <summary>
		/// Sets the src sttribute 
		/// </summary>
		/// <param name="newSrc">The new src attribute</param>
		void setSrc(string newSrc);
	}
}
