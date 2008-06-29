using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using urakawa.xuk;

namespace urakawa.media.data

{
	/// <summary>
	/// Interface for a generic <see cref="IDataProvider"/> providing access to data storage 
	/// via input and output <see cref="Stream"/>s
	/// </summary>
	public interface IDataProvider : IXukAble, IValueEquatable<IDataProvider>
	{
	    /// <summary>
	    /// Gets the <see cref="IDataProviderManager"/> associated with <c>this</c>
	    /// </summary>
	    /// <returns>The associated <see cref="IDataProviderManager"/></returns>
	    IDataProviderManager DataProviderManager { get; set; }

	    /// <summary>
	    /// Gets the UID of the data provider in the context of the manager. 
	    /// Convenience for <c>getDataProviderManager().GetUidOfDataProvider(this)</c>
	    /// </summary>
	    /// <returns>The UID</returns>
	    string Uid { get; }

	    /// <summary>
		/// Gets a <see cref="Stream"/> providing read access to the data
		/// </summary>
		/// <returns>The input <see cref="Stream"/></returns>
		/// <exception cref="exception.DataMissingException">
		/// Thrown if the data stored in the <see cref="IDataProvider"/> is missing from the underlying storage mechanism
		/// </exception>
		/// <remarks>
		/// Make sure to close any <see cref="Stream"/> returned by this method when it is no longer needed. 
		/// If there are any open input <see cref="Stream"/>s, subsequent calls to methods
		/// <see cref="GetOutputStream"/> and <see cref="Delete"/> will cause <see cref="exception.InputStreamsOpenException"/>s
		/// </remarks>
		Stream GetInputStream();

		/// <summary>
		/// Gets a <see cref="Stream"/> providing write access to the data
		/// </summary>
		/// <returns>The output <see cref="Stream"/></returns>
		/// <exception cref="exception.DataMissingException">
		/// Thrown if the data stored in the <see cref="IDataProvider"/> is missing from the underlying storage mechanism
		/// </exception>
		/// <exception cref="exception.OutputStreamOpenException">
		/// Thrown if another output <see cref="Stream"/> from the data provider is already/still open
		/// </exception>
		/// <remarks>
		/// Make sure to close any <see cref="Stream"/> returned by this method when it is no longer needed. 
		/// If there are any open input <see cref="Stream"/>s, subsequent calls to methods
		/// <see cref="GetOutputStream"/> and <see cref="GetInputStream"/> and <see cref="Delete"/> 
		/// will cause <see cref="exception.OutputStreamOpenException"/>s
		/// </remarks>
		Stream GetOutputStream();

		/// <summary>
		/// Deletes any resources associated with <c>this</c> permanently. Additionally removes the <see cref="IDataProvider"/>
		/// from it's <see cref="IDataProviderManager"/>
		/// </summary>
		/// <exception cref="exception.OutputStreamOpenException">
		/// Thrown if a output <see cref="Stream"/> from the <see cref="IDataProvider"/> is currently open
		/// </exception>
		/// <exception cref="exception.InputStreamsOpenException">
		/// Thrown if one or more input <see cref="Stream"/>s from the <see cref="IDataProvider"/> are currently open
		/// </exception>
		void Delete();

		/// <summary>
		/// Creates a copy of <c>this</c> including a copy of the data
		/// </summary>
		/// <returns>The copy</returns>
		IDataProvider Copy();

		/// <summary>
		/// Exports <c>this</c> to a given destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The destination <see cref="Presentation"/></param>
		/// <returns>The exported <see cref="IDataProvider"/></returns>
		IDataProvider Export(Presentation destPres);

	    /// <summary>
	    /// Gets the MIME type of the media stored in the data provider
	    /// </summary>
	    /// <returns>The MIME type</returns>
	    string MimeType { get; }
	}
}
