using System;

namespace urakawa.media
{
	/// <summary>
	/// Text media conforms to this interface.
	/// </summary>
	public interface ITextMedia : IMedia
	{
		/// <summary>
		/// Get the text string for the TextMedia.
		/// </summary>
		/// <returns></returns>
		string getText();

		/// <summary>
		/// Set the text string for the TextMedia.
		/// Throws <see cref="urakawa.exception.MethodParameterIsNullException"/>, 
		/// <see cref="urakawa.exception.MethodParameterIsEmptyStringException"/>
		/// </summary>
		/// <param name="text">The new text string.</param>
		void setText(string text);
	}
}
