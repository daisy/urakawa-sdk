using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.data
{
	public abstract class AudioMediaAsset : IAudioMediaAsset
	{
		#region IMediaAsset Members

		public MediaType getMediaType()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IMediaAssetManager getAssetManager()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public string getUid()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public string getName()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void setName(string newName)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IDataProvider getDataProvider()
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
	}
}
