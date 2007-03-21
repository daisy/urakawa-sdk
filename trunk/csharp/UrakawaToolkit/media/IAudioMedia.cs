using System;

namespace urakawa.media
{
	/// <summary>
	/// Audio media - in any implementation, <see cref="IMedia.getMediaType"/>
	/// should return <see cref="MediaType.AUDIO"/>
	/// </summary>
	public interface IAudioMedia : IMedia, ILocated
	{
	}
}
