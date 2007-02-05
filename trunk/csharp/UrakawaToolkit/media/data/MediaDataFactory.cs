using System;
using System.Collections.Generic;
using System.Text;

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
		private IMediaDataPresentation mPresentation;

		#region IMediaDataFactory Members

		/// <summary>
		/// Gets the <see cref="IMediaDataPresentation"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The associated <see cref="IMediaDataPresentation"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when no <see cref="IMediaDataPresentation"/> has been associated with <c>this</c>
		/// </exception>
		public IMediaDataPresentation getPresentation()
		{
			if (mPresentation == null)
			{
				throw new exception.IsNotInitializedException("The MediaDataFactory has not been initialized with a MediaDataPresentation");
			}
			return mPresentation;
		}

		/// <summary>
		/// Initializer - associates <c>this</c> with a given <see cref="IMediaDataPresentation"/>
		/// </summary>
		/// <param name="pres">The given <see cref="IMediaDataPresentation"/></param>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when <c>this</c> has previously been associated with a <see cref="IMediaDataPresentation"/>
		/// </exception>
		public void setPresentation(IMediaDataPresentation pres)
		{
			if (mPresentation != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The MediaDataFactory has already been initialized with a MediaDataPresentation");
			}
			mPresentation = pres;
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
						return new codec.audio.WavAudioMediaData(getMediaDataManager());
					case "":
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
			if (res.GetType()==mt) return res;
			return null;
		}

		/// <summary>
		/// Creates a <see cref="FileDataProvider"/>
		/// </summary>
		/// <returns>The <see cref="FileDataProvider"/></returns>
		public virtual IDataProvider createDataProvider()
		{
			return new FileDataProvider(getMediaDataManager());
		}

		#endregion
	}
}
