using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.data
{
	public class MediaDataManager : IMediaDataManager
	{
		#region IMediaDataManager Members

		public IMediaDataPresentation getPresentation()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void setPresentation(IMediaDataPresentation pres)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IMediaDataFactory getMediaDataFactory()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IDataProviderFactory getDataProviderFactory()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IMediaData getMediaData(string uid)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public string getUidOfMediaData(IMediaData asset)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void addMediaData(IMediaData asset)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void removeMediaData(IMediaData asset)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IMediaData removeMediaData(string uid)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void deleteMediaData(IMediaData asset)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IMediaData copyMediaData(IMediaData asset)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IMediaData copyMediaData(string uid)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IXukAble Members

		public bool XukIn(System.Xml.XmlReader source)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool XukOut(System.Xml.XmlWriter destination)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public string getXukLocalName()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public string getXukNamespaceUri()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IMediaDataManager Members


		public void deleteMediaData(string uid)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
