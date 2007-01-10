using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.data
{
	public interface IMediaDataLocation : IMediaLocation
	{
		IMediaData getMediaData();

		void setMediaData(IMediaData newData);
	}
}
