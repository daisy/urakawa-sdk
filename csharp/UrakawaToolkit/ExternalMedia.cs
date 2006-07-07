using System;

namespace urakawa.media
{
	/// <summary>
	/// for internal use
	/// </summary>
	public abstract class ExternalMedia : IExternalMedia
	{
		private MediaLocation mMediaLocation = new MediaLocation();
		
		internal ExternalMedia()
		{
		
		}


		/// <summary>
		/// returns the location of the physical media being referenced 
		/// (e.g., could be a file path)
		/// </summary>
		/// <returns>an <see cref="IMediaLocation"/> object which can be queried to find the media location</returns>
		public MediaLocation getLocation()
		{
			return mMediaLocation;
		}
		#region IExternalMedia Members

		IMediaLocation IExternalMedia.getLocation()
		{
			return getLocation();
		}

		/// <summary>
		/// set the media location for this object
		/// </summary>
		/// <param name="location"></param>
		public void setLocation(IMediaLocation location)
		{
			if (location == null)
			{
				throw new exception.MethodParameterIsNullException("media location parameter cannot be null");
			}

			mMediaLocation = (MediaLocation)location;
		}

		#endregion

		#region IMedia Members

		public abstract bool isContinuous();

		public abstract bool isDiscrete();

		public abstract bool isSequence();

		public abstract urakawa.media.MediaType getType();

    IMedia IMedia.copy()
    {
      throw new exception.OperationNotValidException("Can not copy abstract class ExternalMedia");
    }

		#endregion

		#region IXUKable Members
		
		public abstract bool XUKin(System.Xml.XmlReader source);

		public abstract bool XUKout(System.Xml.XmlWriter destination);

		#endregion
	}
}
