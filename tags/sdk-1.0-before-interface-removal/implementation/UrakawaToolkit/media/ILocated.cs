using System;

namespace urakawa.media
{
	/// <summary>
	/// This interface associates a media object with its source location
	/// </summary>
	public interface ILocated
	{
		/// <summary>
		/// Get the src location of the external media
		/// </summary>
		/// <returns>The src location</returns>
		string getSrc();

		/// <summary>
		/// Set the external media's src location.
		/// </summary>
		/// <param name="newSrc">The new src location</param>
		void setSrc(string newSrc);
	}
}
