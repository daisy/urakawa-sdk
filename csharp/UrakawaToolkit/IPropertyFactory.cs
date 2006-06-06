using System;

namespace urakawa.core
{
	/// <summary>
	/// Interface for factories creating <see cref="IProperty"/>s
	/// </summary>
	public interface IPropertyFactory
	{
    /// <summary>
    /// Creates a <see cref="IChannelsProperty"/>
    /// </summary>
    /// <returns>The created <see cref="IChannelsProperty"/></returns>
		IChannelsProperty createChannelsProperty();
    /// <summary>
    /// Creates a <see cref="IXmlProperty"/>
    /// </summary>
    /// <param name="name">The local name of the <see cref="IXmlProperty"/></param>
    /// <param name="ns">The namespace of the <see cref="IXmlProperty"/></param>
    /// <returns>The created <see cref="IChannelsProperty"/></returns>
    IXmlProperty createXmlProperty(string name, string ns);
	}
}
