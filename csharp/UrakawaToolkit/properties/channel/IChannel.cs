using System;
using urakawa.core;
using urakawa.xuk;

namespace urakawa.properties.channel
{
	/// <summary>
	/// Interface for a channel used to storing <see cref="media.IMedia"/>s 
	/// on <see cref="ICoreNode"/>s via. the <see cref="IChannelsProperty"/>
	/// </summary>
	public interface IChannel : IXukAble
	{
    /// <summary>
    /// Sets the localName of the <see cref="IChannel"/>
    /// </summary>
		/// <param name="name">The new localName</param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref localName="localName"/> is null
    /// </exception>
    /// <exception cref="exception.MethodParameterIsEmptyStringException">
    /// Thrown when <paramref localName="localName"/> is an empty string
    /// </exception>
    void setName(string name);

    /// <summary>
    /// Gets the localName of the <see cref="IChannel"/>
    /// </summary>
    /// <returns>The localName</returns>
    string getName();

    /// <summary>
    /// Checks of a given <see cref="media.MediaType"/> is supported by the channel
    /// </summary>
    /// <param name="type">The <see cref="media.MediaType"/></param>
    /// <returns>A <see cref="bool"/> indicating if the <see cref="media.MediaType"/>
    /// is supported</returns>
    bool isMediaTypeSupported(urakawa.media.MediaType type);

		/// <summary>
		/// Adds a <see cref="urakawa.media.MediaType"/> to the list of <see cref="urakawa.media.MediaType"/>s 
		/// supported by the <see cref="IChannel"/>
		/// </summary>
		/// <param name="newType"></param>
		void addSupportedMediaType(urakawa.media.MediaType newType);

		/// <summary>
		/// Gets the <see cref="IChannelsManager"/> managing the <see cref="IChannel"/>
		/// </summary>
		/// <returns>The <see cref="IChannelsManager"/></returns>
		IChannelsManager getChannelsManager();

		/// <summary>
		/// Gets the Xuk id of the <see cref="IChannel"/>
		/// </summary>
		/// <returns>The Xuk Id as calculated by 
		/// <c>this.getChannelsManager().getUidOfChannel(this)</c></returns>
		string getUid();
	}
}
