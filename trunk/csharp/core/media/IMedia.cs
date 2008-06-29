using System;
using urakawa.xuk;
using urakawa.events;

namespace urakawa.media
{
	/// <summary>
	/// This is the base interface for all media-related classes and interfaces.  
	/// Media is continuous (time-based) or discrete (static), and is of a specific type.
	/// </summary>
	public interface IMedia : IWithPresentation, IXukAble, IValueEquatable<IMedia>, IChangeNotifier
	{
		/// <summary>
		/// Event fired after the language of the <see cref="IMedia"/> has changed
		/// </summary>
		event EventHandler<urakawa.events.LanguageChangedEventArgs> LanguageChanged;

	    /// <summary>
	    /// Gets the <see cref="IMediaFactory"/> associated with the <see cref="IMedia"/>
	    /// </summary>
	    /// <returns>The <see cref="IMediaFactory"/></returns>
	    IMediaFactory MediaFactory { get; }

	    /// <summary>
	    /// Determines if the <see cref="IMedia"/> is continuous
	    /// </summary>
	    /// <returns><c>true</c> if the <see cref="IMedia"/> is continuous, <c>false</c> else</returns>
	    bool IsContinuous { get; }

	    /// <summary>
	    /// Convenience Equivalent to <c>!IsContinuous</c>
	    /// </summary>
	    /// <returns><c>!isContinuous</c></returns>
	    bool IsDiscrete { get; }

	    /// <summary>
	    /// tells you if the media object itself is a sequence
	    /// does not tell you if your individual media object is part of a sequence
	    /// </summary>
	    /// <returns></returns>
	    bool IsSequence { get; }

	    /// <summary>
		/// Gets a copy of the <see cref="IMedia"/>
		/// </summary>
		/// <returns></returns>
		IMedia Copy();

		/// <summary>
		/// Exports the media to a destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The destination presentation</param>
		/// <returns>The exported media</returns>
		IMedia Export(Presentation destPres);

	    /// <summary>
	    /// Gets or sets the language of the media
	    /// </summary>
	    /// <returns>The language</returns>
	    string Language { get; set; }
	}
}
