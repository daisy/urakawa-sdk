using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class Channel
	{
		private string mName;

		public Channel()
		{
		}

		public string getName()
		{
			return mName;
		}

		public void setName(string newName)
		{
			mName = newName;
		}
	}
}
