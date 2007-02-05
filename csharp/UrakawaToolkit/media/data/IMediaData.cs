using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;

namespace urakawa.media.data
{
	/// <summary>
	/// Interface for a <see cref="IMediaData"/>
	/// </summary>
	public interface IMediaData : xuk.IXukAble
	{
		IMediaDataManager getDataManager();
		string getUid();
		string getName();
		void setName(string newName);
		void delete();
		IMediaData copy();
	}
}
