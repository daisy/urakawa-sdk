using System;

namespace urakawa.media
{
	/// <summary>
	/// Used internally to simplify implementation of media objects.
	/// </summary>
	public abstract class ExternalMedia : IExternalMedia
	{
		private MediaLocation mMediaLocation = new MediaLocation();
		
		internal ExternalMedia()
		{
		
		}

		/// <summary>
		/// Returns the location of the physical media being referenced 
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
		/// Set the media location for this object
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

		/// <summary>
		/// This function will be implemented by each IMedia implementor.
		/// It has no meaning for ExternalMedia objects.
		/// </summary>
		/// <returns></returns>
		public abstract bool isContinuous();

		/// <summary>
		/// This function will be implemented by each IMedia implementor.
		/// It has no meaning for ExternalMedia objects.
		/// </summary>
		/// <returns></returns>
		public abstract bool isDiscrete();

		/// <summary>
		/// This function will be implemented by each IMedia implementor.
		/// It has no meaning for ExternalMedia objects.
		/// </summary>
		/// <returns></returns>
		public abstract bool isSequence();

		/// <summary>
		/// This function will be implemented by each IMedia implementor.
		/// It has no meaning for ExternalMedia objects.
		/// </summary>
		/// <returns></returns>
		public abstract urakawa.media.MediaType getType();

		//this throws an exception because you can't copy an abstract class
		//this function exists because we didn't want to make copy() abstract
		//it needs to have its return type overridden in the more useful media classes, 
		//such as AudioMedia and VideoMedia
		IMedia IMedia.copy()
		{
			throw new exception.OperationNotValidException("Can not copy abstract class ExternalMedia");
		}

		#endregion

		#region IXUKable Members
		
		/// <summary>
		/// This function will be implemented by each IMedia implementor.
		/// It has no meaning for ExternalMedia objects.
		/// </summary>
		/// <returns></returns>
		public abstract bool XUKin(System.Xml.XmlReader source);

		/// <summary>
		/// This function will be implemented by each IMedia implementor.
		/// It has no meaning for ExternalMedia objects.
		/// </summary>
		/// <returns></returns>
		public abstract bool XUKout(System.Xml.XmlWriter destination);

		#endregion
	}
}
