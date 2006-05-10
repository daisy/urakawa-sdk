using System;
using urakawa.media;

namespace urakawa.core
{
	/// <summary>
	/// 
	/// </summary>
	public interface IChannel
	{
		void setName(String name);
		String getName();
		bool isMediaTypeSupported(MediaType mediaType);

	}
}
