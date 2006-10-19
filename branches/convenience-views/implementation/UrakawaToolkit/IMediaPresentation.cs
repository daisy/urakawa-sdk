using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.media
{
	/// <summary>
	/// Interface for a presentation that supports <see cref="IMedia"/>
	/// </summary>
	public interface IMediaPresentation : ICorePresentation
	{
		/// <summary>
		/// Gets the <see cref="urakawa.media.IMediaFactory"/> creating <see cref="urakawa.media.IMedia"/>
		/// for the <see cref="IPresentation"/>
		/// </summary>
		/// <returns></returns>
		urakawa.media.IMediaFactory getMediaFactory();
	}
}
