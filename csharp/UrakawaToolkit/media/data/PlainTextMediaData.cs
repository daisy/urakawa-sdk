using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.data
{
	public class PlainTextMediaData : IMediaData
	{
		public Encoding getEncoding()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public void setEncoding(Encoding enc)
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		#region IMediaData Members

		public IMediaDataManager getDataManager()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public string getUid()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public string getName()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public void setName(string newName)
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public IDataProvider getDataProvider()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		IMediaData IMediaData.copy()
		{
			return copy();
		}

		PlainTextMediaData copy()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}


		#endregion

		#region IXukAble Members

		public bool XukIn(System.Xml.XmlReader source)
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public bool XukOut(System.Xml.XmlWriter destination)
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public string getXukLocalName()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public string getXukNamespaceUri()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		#endregion
	}
}
