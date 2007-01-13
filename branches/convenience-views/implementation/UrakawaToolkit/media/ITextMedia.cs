using System;

namespace urakawa.media
{
	/// <summary>
	/// Interface for <see cref="IMedia"/> of textual type. 
	/// In implementations of this interface method <see cref="IMedia.getMediaType"/> 
	/// should return <see cref="MediaType.TEXT"/>
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
		/// <param localName="text">The new text string.</param>
		void setText(string text);
	}
}
