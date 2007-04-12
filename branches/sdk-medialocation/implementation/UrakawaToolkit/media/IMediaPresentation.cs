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
		/// Gets the <see cref="IMediaFactory"/> creating <see cref="IMedia"/>
		/// for the <see cref="IMediaPresentation"/>
		/// </summary>
		/// <returns>The <see cref="IMediaPresentation"/></returns>
		urakawa.media.IMediaFactory getMediaFactory();
	}
}
