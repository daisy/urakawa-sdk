using System;

namespace urakawa.media
{
	/// <summary>
	/// Summary description for ISized.
	/// </summary>
	public interface ISized
	{
		/// <summary>
		/// Get the image width.
		/// </summary>
		/// <returns>The width</returns>
		int getWidth();

		/// <summary>
		/// Get the image height.
		/// </summary>
		/// <returns>The height</returns>
		int getHeight();

		/// <summary>
		/// Sets the image width
		/// </summary>
		/// <param localName="newHeight">The new width</param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the new width is negative
		/// </exception>
		void setWidth(int newHeight);

		/// <summary>
		/// Sets the image height
		/// </summary>
		/// <param localName="newHeight">The new height</param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the new height is negative
		/// </exception>
		void setHeight(int newHeight);
	}
}
