using System;
namespace urakawa
{
	/// <summary>
	/// Interface for classes of objects associated with a <see cref="Presentation"/>
	/// </summary>
	public interface IWithPresentation
	{
	    /// <summary>
	    /// Gets or sets the associated presentation
	    /// </summary>
	    /// <returns>The presentation</returns>
	    Presentation Presentation { get; set; }
	}
}
