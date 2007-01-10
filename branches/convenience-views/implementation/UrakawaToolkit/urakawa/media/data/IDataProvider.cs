using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace urakawa.media.data
{
	public interface IDataProvider
	{
		Stream getInputStream();

		Stream getOutputStream();
	}
}
