using System;
using urakawa.core;
using urakawa.core.property;
using urakawa.media;
using urakawa.media.data;
using urakawa.properties.channel;
using urakawa.properties.xml;
using urakawa.xuk;

namespace urakawa
{
	/// <summary>
	/// The presentation
	/// </summary>
	public interface IPresentation : ICorePresentation, IMediaDataPresentation, IChannelPresentation, IXmlPresentation, IValueEquatable<IPresentation>
	{
		/// <summary>
		/// Gets the <see cref="IPropertyFactory"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The factory</returns>
		new IPropertyFactory getPropertyFactory();
	}
}
