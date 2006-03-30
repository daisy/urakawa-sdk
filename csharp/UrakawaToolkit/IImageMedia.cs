using System;

namespace urakawa.media
{
	/// <summary>
	/// Represents images which are external media and have a height and width
	/// </summary>
	public interface IImageMedia : IExternalMedia, IImageSize
	{
	}
}
