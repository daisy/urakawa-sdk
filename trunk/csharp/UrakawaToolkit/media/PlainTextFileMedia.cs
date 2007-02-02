using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace urakawa.media.data
{
	public class PlainTextMedia : ITextMedia, ILocated
	{
		protected internal PlainTextMedia(IMediaDataManager mngr)
		{
		}

		private IMediaDataLocation mLocation;


		public PlainTextMediaData getPlainTextMediaData()
		{
			if (mLocation==null)
			{
				throw new exception.IsNotInitializedException(
					"The PlainTextMedia has not yet been initialized with a location");
			}
			IMediaData temp = mLocation.getMediaData();
			if (!(temp is data.PlainTextMediaData))
			{
				//TODO: Invent exception for this case
				throw new Exception(
					"The location of a PlainTextMedia point to a PlainTextMediaData");
			}
			return (PlainTextMediaData)temp;

		}

		#region ITextMedia Members

		public string getText()
		{
			Stream s = getPlainTextMediaData().getDataProvider().getInputStream();
			StreamReader rd = new StreamReader(s, getPlainTextMediaData().getEncoding());
			return rd.ReadToEnd();
		}

		public void setText(string text)
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		#endregion

		#region IMedia Members

		public IMediaFactory getMediaFactory()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public bool isContinuous()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public bool isDiscrete()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public bool isSequence()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public MediaType getMediaType()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public IMedia copy()
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

		#region IValueEquatable<IMedia> Members

		public bool ValueEquals(IMedia other)
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		#endregion

		#region ILocated Members

		IMediaLocation ILocated.getLocation()
		{
			return getLocation();
		}



		public data.IMediaDataLocation getLocation()
		{
			return mLocation;
		}

		public void setLocation(IMediaLocation location)
		{
			if (!(location is data.IMediaDataLocation))
			{
				throw new exception.MethodParameterIsWrongTypeException(
					"The location of a PlainTextMedia must be a IMediaDataLocation");
			}
		}

		#endregion
	}
}
