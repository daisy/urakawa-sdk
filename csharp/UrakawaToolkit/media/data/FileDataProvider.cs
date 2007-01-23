using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.data
{
	public class FileDataProvider : IDataProvider
	{
		#region IDataProvider Members

		public System.IO.Stream getInputStream()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public System.IO.Stream getOutputStream()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		#endregion
	}
}
