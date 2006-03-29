using System;

namespace urakawa.core
{
	/// <summary>
	/// 
	/// </summary>
	public interface IChannelFactory
	{
		Channel createChannel(string name);
	}
}
