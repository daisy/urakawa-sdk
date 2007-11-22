using System;

namespace urakawa.media
{
	/// <summary>
	/// Interface for <see cref="IMedia"/> of textual type. 
	/// </summary>
	public interface ITextMedia : IMedia
	{
		/// <summary>
		/// Event fired after the text of the <see cref="ITextMedia"/> has changed
		/// </summary>
		event EventHandler<urakawa.events.TextChangedEventArgs> textChanged;

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
