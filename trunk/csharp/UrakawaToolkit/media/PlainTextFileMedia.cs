using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace urakawa.media.data
{
	/// <summary>
	/// An implementation of <see cref="ITextMedia"/> based on text storage in <see cref="PlainTextMediaData"/>
	/// </summary>
	public class PlainTextMedia : ITextMedia
	{
		protected internal PlainTextMedia(IMediaFactory fact)
		{
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The MediaFactory of the PlainTextMedia can not be null");
			}
			mFactory = fact;
		}


		private IMediaFactory mFactory;
		private PlainTextMediaData mMediaData;

		/// <summary>
		/// Gets the <see cref="PlainTextMediaData"/> used to store the text of <c>this</c>
		/// </summary>
		/// <returns>The plain text media data</returns>
		public PlainTextMediaData getPlainTextMediaData()
		{
			if (mMediaData == null)
			{
				throw new exception.IsNotInitializedException(
					"The PlainTextMedia has not been initialized with a PlainTextMediaData");
			}
			return mMediaData;
		}

		/// <summary>
		/// Sets the <see cref="PlainTextMediaData"/> used to store the text of <c>this</c>
		/// </summary>
		/// <param name="newMediaData">The new plain text media data</param>
		public void setPlainTextMediaData(PlainTextMediaData newMediaData)
		{
			if (newMediaData == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The PlainTextMediaData associated with the PlainTextMedia can not be null");
			}
			mMediaData = newMediaData;
		}

		#region ITextMedia Members

		/// <summary>
		/// Gets the text of <c>this</c>
		/// </summary>
		/// <returns>The text</returns>
		public string getText()
		{
			return getPlainTextMediaData().getText();
		}

		/// <summary>
		/// Sets the text of <c>this</c>
		/// </summary>
		/// <param name="text">The new text</param>
		public void setText(string text)
		{
			getPlainTextMediaData().setText(text);
		}

		#endregion

		#region IMedia Members


		/// <summary>
		/// Gets the <see cref="IMediaFactory"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The factory</returns>
		public IMediaFactory getMediaFactory()
		{
			return mFactory;
		}

		/// <summary>
		/// Determines if <c>this</c> is a continuous media (wich it is not)
		/// </summary>
		/// <returns><c>false</c></returns>
		public bool isContinuous()
		{
			return false;
		}

		/// <summary>
		/// Determines if <c>this</c> is a descrete media (which it is)
		/// </summary>
		/// <returns><c>true</c></returns>
		public bool isDiscrete()
		{
			return true;
		}

		/// <summary>
		/// Determines if <see cref="this"/> is a sequence media (which it is not)
		/// </summary>
		/// <returns><c>false</c></returns>
		public bool isSequence()
		{
			return false;
		}

		/// <summary>
		/// Gets the <see cref="MediaType"/> of <c>this</c> (which is <see cref="MediaType.TEXT"/>)
		/// </summary>
		/// <returns><see cref="MediaType.TEXT"/></returns>
		public MediaType getMediaType()
		{
			return MediaType.TEXT;
		}

		IMedia IMedia.copy()
		{
			return copy();
		}

		/// <summary>
		/// Creates a copy of <c>this</c>
		/// </summary>
		/// <returns>The copy</returns>
		public PlainTextMedia copy()
		{
			IMedia oCopy = getMediaFactory().createMedia(getXukLocalName(), getXukNamespaceUri());
			if (!(oCopy is PlainTextMedia))
			{
				throw new exception.FactoryCanNotCreateTypeException(
					"The Mediafactory of the PlainTextMedia can not create a PlainTextMedia");
			}
			PlainTextMedia theCopy = (PlainTextMedia)oCopy;
			theCopy.setPlainTextMediaData(getPlainTextMediaData().copy());
			return theCopy;
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

		/// <summary>
		/// Determines if <c>this</c> has the same value as a given other <see cref="IMedia"/>
		/// </summary>
		/// <param name="other">The other media</param>
		/// <returns>A <see cref="bool"/> indicating if the values are equal</returns>
		public bool ValueEquals(IMedia other)
		{
			if (!(other is PlainTextMedia)) return false;
			return getPlainTextMediaData().ValueEquals(((PlainTextMedia)other).getPlainTextMediaData());
		}

		#endregion

		//#region ILocated Members

		//IMediaLocation ILocated.getSrc()
		//{
		//  return getLocation();
		//}

		///// <summary>
		///// Gets a <see cref="MediaDataLocation"/> pointing to the <see cref="PlainTextMediaData"/> of <c>this</c>
		///// </summary>
		///// <returns>The location</returns>
		//public IMediaDataLocation getLocation()
		//{
		//  MediaDataLocation loc = new MediaDataLocation(getMediaFactory(), getPlainTextMediaData().getMediaDataManager().getMediaDataFactory());
		//  loc.setMediaData(getPlainTextMediaData());
		//  return loc;
		//}

		///// <summary>
		///// Sets the <see cref="PlainTextMediaData"/> of <c>this</c> via. a <see cref="MediaDataLocation"/> pointing the the new <see cref="PlainTextMediaData"/>
		///// </summary>
		///// <param name="location">The location pointing to the new <see cref="PlainTextMediaData"/></param>
		//public void setLocation(IMediaLocation location)
		//{
		//  if (!(location is data.IMediaDataLocation))
		//  {
		//    throw new exception.MethodParameterIsWrongTypeException(
		//      "The location of a PlainTextMedia must be a IMediaDataLocation");
		//  }
		//  IMediaDataLocation mdLoc = (IMediaDataLocation)location;
		//  if (!(mdLoc.getMediaData() is PlainTextMediaData))
		//  {
		//    throw new exception.OperationNotValidException(
		//      "The MediaData pointed to by the new IMediaDataLocation must be a PlainTextMediaData");
		//  }
		//  setPlainTextMediaData((PlainTextMediaData)mdLoc.getMediaData());
		//}

		//#endregion
	}
}
