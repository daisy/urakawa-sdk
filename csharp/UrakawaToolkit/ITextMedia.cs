using System;

namespace urakawa.media
{
	/// <summary>
	/// Text media conforms to this interface.
	/// </summary>
	public interface ITextMedia : IMedia
	{
		string getText();

		//throws MethodParameterIsNullException, MethodParameterIsEmptyStringException
		void setText(string text);
	}
}
