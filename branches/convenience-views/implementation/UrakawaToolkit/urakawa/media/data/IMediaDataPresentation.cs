using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.data
{
	public interface IMediaDataPresentation : IMediaPresentation
	{
		IMediaDataManager getMediaDataManager();

		IMediaDataFactory getMediaDataFactory();
	}
}
