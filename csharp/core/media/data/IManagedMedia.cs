using System;
using System.Collections.Generic;
using System.Text;
using urakawa.events.media.data;

namespace urakawa.media.data
{
    /// <summary>
    /// Interface for <see cref="Media"/> that use <see cref="MediaData"/> to store their content
    /// </summary>
    public interface IManaged
    {
        /// <summary>
        /// Event fired after the <see cref="MediaData"/> of the <see cref="IManaged"/> has changed
        /// </summary>
        event EventHandler<MediaDataChangedEventArgs> MediaDataChanged;

        /// <summary>
        /// Gets the <see cref="MediaData"/> storing the content
        /// </summary>
        /// <returns>The media data</returns>
        MediaData MediaData { get; set; }

        /// <summary>
        /// Gets the <see cref="MediaDataFactory"/> creating the <see cref="MediaData"/>
        /// used by the <see cref="IManaged"/> <see cref="Media"/>.
        /// Convenience for <c>GetMediaData().getMediaDataManager().GetMediaDataFactory()</c>
        /// </summary>
        /// <returns>The media data factory</returns>
        MediaDataFactory MediaDataFactory { get; }
    }
}