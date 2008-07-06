using System;
using System.Xml;
using urakawa.core;
using urakawa.xuk;

namespace urakawa.property.channel
{
    /// <summary>
    /// The actual implementation to be implemented by the implementation team ;)
    /// All method bodies must be completed for realizing the required business logic.
    /// -
    /// This is the DEFAULT implementation for the API/Toolkit:
    /// end-users should feel free to use this class as such,
    /// or they can sub-class it in order to specialize the instance creation process.
    /// -
    /// In addition, an end-user has the possibility to implement the
    /// singleton factory pattern, so that only one instance of the factory
    /// is used throughout the application life
    /// (by adding a method like "static Factory getFactory()").
    /// <seealso cref="Channel"/> 
    /// <seealso cref="Channel"/>
    /// <seealso cref="channel.ChannelsManager"/>
    /// </summary>
    public class ChannelFactory : WithPresentation, IXukAble
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        protected internal ChannelFactory()
        {
        }

        #region ChannelFactory Members

        /// <summary>
        /// Gets the <see cref="ChannelsManager"/> assigned the <see cref="Channel"/>s created
        /// by the <see cref="ChannelFactory"/>
        /// </summary>
        /// <returns>The <see cref="ChannelsManager"/></returns>
        public ChannelsManager ChannelsManager
        {
            get { return Presentation.ChannelsManager; }
        }

        /// <summary>
        /// Creates a new <see cref="Channel"/> matching a given QName.
        /// </summary>
        /// <param name="localName">The local part of the QName</param>
        /// <param name="namespaceUri">The namespace uri part of the QName</param>
        /// <returns>The created <see cref="Channel"/> or <c>null</c> is the given QName is not supported</returns>
        /// <remarks>
        /// The only supported QName is <c><see cref="urakawa.ToolkitSettings.XUK_NS"/>:Channel</c> which matches <see cref="Channel"/>
        /// </remarks>
        public virtual Channel CreateChannel(string localName, string namespaceUri)
        {
            if (namespaceUri == urakawa.ToolkitSettings.XUK_NS)
            {
                if (localName == typeof (Channel).Name)
                {
                    return new Channel(ChannelsManager);
                }
                else if (localName == typeof (AudioChannel).Name)
                {
                    return new AudioChannel(ChannelsManager);
                }
                else if (localName == typeof (TextChannel).Name)
                {
                    return new TextChannel(ChannelsManager);
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a <see cref="Channel"/> instance
        /// </summary>
        /// <returns>The instance</returns>
        public virtual Channel CreateChannel()
        {
            return CreateChannel("Channel", urakawa.ToolkitSettings.XUK_NS);
        }

        #endregion
    }
}