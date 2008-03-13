using System;
namespace urakawa
{
	/// <summary>
	/// Interface for classes of objects associated with a <see cref="Presentation"/>
	/// </summary>
	public interface IWithPresentation
	{
		/// <summary>
		/// Gets the associated presentation
		/// </summary>
		/// <returns>The presentation</returns>
		Presentation getPresentation();

		/// <summary>
		/// Initializer - sets the associated presentation
		/// </summary>
		/// <param name="newPres">The presentation with which to associate</param>
		void setPresentation(Presentation newPres);
	}
}
