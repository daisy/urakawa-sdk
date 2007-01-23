using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.data
{
	public class MediaDataFactory : IMediaDataFactory
	{
		#region IMediaDataFactory Members

		public IMediaDataPresentation getPresentation()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public void setPresentation(IMediaDataPresentation pres)
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public IMediaDataManager getMediaDataManager()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public IMediaData createMediaData(string xukLocalName, string xukNamespaceUri)
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public IMediaData createMediaData(Type mediaType)
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		#endregion
	}
}
