using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;

namespace urakawa.media.data
{
	/// <summary>
	/// Interface for an <see cref="IAudioMedia"/> storing it's audio in an <see cref="IAudioMediaData"/>
	/// </summary>
	public interface IManagedAudioMedia : IAudioMedia, IManagedMedia
	{
		/// <summary>
		/// Gets the <see cref="IAudioMediaData"/> storing the audio of the <see cref="IManagedAudioMedia"/>
		/// </summary>
		/// <returns>The audio media data</returns>
		new IAudioMediaData getMediaData();
	}
}
