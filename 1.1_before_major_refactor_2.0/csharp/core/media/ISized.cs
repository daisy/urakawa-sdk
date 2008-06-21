using System;

namespace urakawa.media
{
	/// <summary>
	/// Summary description for ISized.
	/// </summary>
	public interface ISized
	{
		/// <summary>
		/// Event fired after the size (height or width) of the <see cref="ISized"/> has changed
		/// </summary>
		event EventHandler<events.media.SizeChangedEventArgs> sizeChanged;

		/// <summary>
		/// Get the width of the <see cref="ISized"/> object.
		/// </summary>
		/// <returns>The width</returns>
		int getWidth();

		/// <summary>
		/// Get the height of the <see cref="ISized"/> object.
		/// </summary>
		/// <returns>The height</returns>
		int getHeight();

		/// <summary>
		/// Sets the width of the <see cref="ISized"/> object.
		/// </summary>
		/// <param name="newHeight">The new width</param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the new width is negative
		/// </exception>
		void setWidth(int newHeight);

		/// <summary>
		/// Sets the height of the <see cref="ISized"/> object.
		/// </summary>
		/// <param name="newHeight">The new height</param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the new height is negative
		/// </exception>
		void setHeight(int newHeight);

		/// <summary>
		/// Sets the size of the <see cref="ISized"/> object.
		/// </summary>
		/// <param name="newWidth">The new width</param>
		/// <param name="newHeight">The new height</param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the new width or height is negative
		/// </exception>
		void setSize(int newHeight, int newWidth);
	}
}
