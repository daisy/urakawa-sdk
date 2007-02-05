using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.data
{
	public class PlainTextMediaData : MediaData
	{

		protected internal PlainTextMediaData(IMediaDataManager mngr)
		{
			setDataManager(mngr);
		}

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

		public IDataProvider getDataProvider()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public override IMediaData copy()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		#endregion

		#region IXukAble Members

		public override bool XukIn(System.Xml.XmlReader source)
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public override bool XukOut(System.Xml.XmlWriter destination)
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		#endregion

		public override void delete()
		{
			throw new Exception("The method or operation is not implemented.");
			//TODO: Implement method
		}

		protected override IList<IDataProvider> getUsedDataProviders()
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}
