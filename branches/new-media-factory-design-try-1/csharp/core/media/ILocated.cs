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
        string Src { get; set; }

        /// <summary>
        /// Gets the <see cref="Uri"/> of the <see cref="ExternalMedia"/> 
        /// - uses <c>getMediaFactory().getPresentation().getRootUri()</c> as base <see cref="Uri"/>
        /// </summary>
        /// <returns>The <see cref="Uri"/></returns>
        /// <exception cref="exception.InvalidUriException">
        /// Thrown when the value <see cref="Src"/> is not a well-formed <see cref="Uri"/>
        /// </exception>
        Uri Uri { get; }
    }
}