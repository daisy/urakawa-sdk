using System;

namespace urakawa.media
{
	/// <summary>
	/// Video media is both time-based and has a visual presence
	/// </summary>
	public interface IVideoMedia : IClippedMedia, IImageSize
	{
	}
}
