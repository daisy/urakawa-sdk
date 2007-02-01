using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using urakawa.xuk;

namespace urakawa.media.data

{
	/// <summary>
	/// Interface for a generic <see cref="IDataProvider"/> providing access to data storage 
	/// via input and output <see cref="Stream"/>s
	/// </summary>
	public interface IDataProvider : IXukAble
	{
		IMediaDataManager getManager();

		Stream getInputStream();

		Stream getOutputStream();
	}
}
