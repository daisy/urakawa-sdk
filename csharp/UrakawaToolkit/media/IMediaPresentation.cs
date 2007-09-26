using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.media
{
	/// <summary>
	/// Interface for a presentation that supports <see cref="IMedia"/>
	/// </summary>
	public interface IMediaPresentation : ITreePresentation
	{
		/// <summary>
		/// Gets the <see cref="IMediaFactory"/> creating <see cref="IMedia"/>
		/// for the <see cref="IMediaPresentation"/>
		/// </summary>
		/// <returns>The <see cref="IMediaPresentation"/></returns>
		IMediaFactory getMediaFactory();

		/// <summary>
		/// Gets the base uri for relative uri's of <see cref="IMedia"/> objects in the presentaion
		/// </summary>
		/// <returns>The base uri</returns>
		Uri getRootUri();

		/// <summary>
		/// Sets the base uri for relative uri's of <see cref="IMedia"/> objects in the presentaion
		/// </summary>
		/// <param name="newBase">The new base uri</param>
		void setRootUri(Uri newBase);

		/// <summary>
		/// Fired when the base <see cref="Uri"/> has changed
		/// </summary>
		event BaseUriChangedEventHandler BaseUriChanged;

		/// <summary>
		/// Gets the list of <see cref="IMedia"/> used by the <see cref="TreeNode"/> tree of the presentation
		/// </summary>
		/// <returns>The list</returns>
		List<IMedia> getListOfUsedMedia();
	}


	/// <summary>
	/// Arguments for the <see cref="IMediaPresentation.BaseUriChanged"/> event
	/// </summary>
	public class BaseUriChangedEventArgs : EventArgs
	{
		private Uri mPreviousUri;

		/// <summary>
		/// Gets the previous <see cref="Uri"/>
		/// </summary>
		/// <returns>The previous Uri</returns>
		public Uri getPreviousUri()
		{
			return mPreviousUri;
		}

		/// <summary>
		/// Constructor seting the previous <see cref="Uri"/>
		/// </summary>
		/// <param name="prevUri">The previous Uri</param>
		public BaseUriChangedEventArgs(Uri prevUri)
		{
			mPreviousUri = prevUri;
		}
	}

	/// <summary>
	/// Event handler for the <see cref="IMediaPresentation.BaseUriChanged"/> event
	/// </summary>
	/// <param name="pres">The sending <see cref="IMediaPresentation"/></param>
	/// <param name="e">The arguments of the event</param>
	public delegate void BaseUriChangedEventHandler(IMediaPresentation pres, BaseUriChangedEventArgs e);
}
