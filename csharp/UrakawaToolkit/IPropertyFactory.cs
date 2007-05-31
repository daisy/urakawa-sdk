using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core.property;
using urakawa.properties.channel;
using urakawa.properties.xml;

namespace urakawa
{
	/// <summary>
	/// Interface for a <see cref="Property"/> factory that supports creation
	/// of <see cref="ChannelsProperty"/>s and <see cref="XmlProperty"/>s
	/// </summary>
	public interface IPropertyFactory : IChannelsPropertyFactory, IXmlPropertyFactory
	{
		/// <summary>
		/// Gets the <see cref="IPresentation"/> of <c>this</c>
		/// </summary>
		/// <returns></returns>
		new IPresentation getPresentation();

		/// <summary>
		/// Sets the <see cref="IPresentation"/> of <c>this</c>
		/// </summary>
		/// <param name="newPres">The new <see cref="IPresentation"/></param>
		void setPresentation(IPresentation newPres);
	}
}
