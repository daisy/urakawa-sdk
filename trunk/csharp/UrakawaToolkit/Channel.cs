using System;
using urakawa.media;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class Channel : IChannel
	{
		private string mName;

		public Channel()
		{
		}
		#region IChannel Members

		public string getName()
		{
			return mName;
		}

		public void setName(string newName)
		{
			mName = newName;
		}

		public bool isMediaTypeSupported(MediaType mediaType)
		{
			// TODO:  Add Channel.isMediaTypeSupported implementation
			return false;
		}

		#endregion
	}
}
