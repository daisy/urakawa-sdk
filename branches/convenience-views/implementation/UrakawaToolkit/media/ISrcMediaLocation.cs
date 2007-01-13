using System;
namespace urakawa.media
{
	public interface ISrcMediaLocation : IMediaLocation
	{
		string getSrc();
		void setSrc(string newSrc);
	}
}
