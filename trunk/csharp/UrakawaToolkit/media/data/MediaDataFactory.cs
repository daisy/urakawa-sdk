using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media.data.codec.audio;

namespace urakawa.media.data
{
	/// <summary>
	/// <para>Default implmentation of <see cref="IMediaDataFactory"/>.</para>
	/// <para>Supports creation of the following <see cref="IMediaData"/> types:
	/// <list type="ul">
	/// <item><see cref="codec.audio.WavAudioMediaData"/></item>
	/// <item><see cref="PlainTextMediaData"/></item>
	/// </list>
	/// </para>
	/// </summary>
	public class MediaDataFactory : IMediaDataFactory
	{
		private MediaDataManager mMediaDataManager;

		#region IMediaDataFactory Members

		/// <summary>
		/// Gets the <see cref="IMediaDataPresentation"/> associated with <c>this</c>.
		/// Convenience for <c>this.getMediaDataManager().getPresentation()</c>.
		/// </summary>
		/// <returns>The associated <see cref="IMediaDataPresentation"/></returns>
		public IMediaDataPresentation getPresentation()
		{
			return getMediaDataManager().getPresentation();
		}

		/// <summary>
		/// Gets the <see cref="IMediaDataManager"/> associated with <c>this</c>
		/// (via the <see cref="IMediaDataPresentation"/> associated with <c>this</c>.
		/// Convenience for <c><see cref="getPresentation"/>().<see cref="IMediaDataPresentation.getMediaDataManager"/>()</c>
		/// </summary>
		/// <returns>The <see cref="IMediaDataManager"/></returns>
		public IMediaDataManager getMediaDataManager()
		{
			return getPresentation().getMediaDataManager();
		}

		/// <summary>
		/// Initializer associating <c>this</c> with a owning media data manager
		/// </summary>
		/// <param name="mngr">The owning manager</param>
		public void setMediaDataManager(IMediaDataManager mngr)
		{
			if (mngr == null)
			{
				throw new exception.MethodParameterIsNullException("The media data manager owning this can not be null");
			}
			if (mMediaDataManager != null)
			{
				throw new exception.IsAlreadyInitializedException("The media data factory has already been initialized with an owning mamager");
			}
			if (mngr is MediaDataManager)
			{
				mMediaDataManager = (MediaDataManager)mngr;
			}
			throw new exception.MethodParameterIsWrongTypeException(
				"The IMediaDataManager of a MediaDataFactory must a MediaDataManager");
		}

		/// <summary>
		/// Creates an instance of a <see cref="IMediaData"/> of type matching a given XUK QName
		/// </summary>
		/// <param name="xukLocalName">The local name part of the QName</param>
		/// <param name="xukNamespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="IMediaData"/> instance or <c>null</c> if the given QName is supported</returns>
		public virtual IMediaData createMediaData(string xukLocalName, string xukNamespaceUri)
		{
			if (xukLocalName == null || xukNamespaceUri == null)
			{
				throw new exception.MethodParameterIsNullException(
					"No part of the QName can be null");
			}
			if (xukNamespaceUri == ToolkitSettings.XUK_NS)
			{
				switch (xukLocalName)
				{
					case "WavAudioMediaData":
						return new WavAudioMediaData(getMediaDataManager());
					case "PlainTextMediaData":
						return new PlainTextMediaData(getMediaDataManager());
					default:
						break;
				}
			}
			return null;
		}

		/// <summary>
		/// Creates a <see cref="IMediaData"/> instance of a given <see cref="Type"/>
		/// </summary>
		/// <param name="mt">The given <see cref="Type"/></param>
		/// <returns>
		/// The created <see cref="IMediaData"/> instance 
		/// or <c>null</c> if the given media <see cref="Type"/> is not supported
		/// </returns>
		public virtual IMediaData createMediaData(Type mt)
		{
			IMediaData res = createMediaData(ToolkitSettings.XUK_NS, mt.Name);
			if (typeof(IAudioMediaData).IsAssignableFrom(mt))
			{
				return new WavAudioMediaData(getMediaDataManager());
			}
			else if (typeof(PlainTextMediaData).IsAssignableFrom(mt))
			{
				return new PlainTextMediaData(getMediaDataManager());
			}
			if (res.GetType()==mt) return res;
			return null;
		}

		#endregion
	}
}
