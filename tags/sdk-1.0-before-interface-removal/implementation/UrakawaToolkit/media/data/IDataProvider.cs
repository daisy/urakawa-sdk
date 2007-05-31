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
		IDataProviderManager getDataProviderManager();

		/// <summary>
		/// Gets the UID of the data provider in the context of the manager. 
		/// Convenience for <c>getDataProviderManager().getUidOfDataProvider(this)</c>
		/// </summary>
		/// <returns>The UID</returns>
		string getUid();

		/// <summary>
		/// Gets a <see cref="Stream"/> providing read access to the data
		/// </summary>
		/// <returns>The input <see cref="Stream"/></returns>
		Stream getInputStream();

		/// <summary>
		/// Gets a <see cref="Stream"/> providing write access to the data
		/// </summary>
		/// <returns>The output <see cref="Stream"/></returns>
		Stream getOutputStream();

		/// <summary>
		/// Deletes any resources associated with <c>this</c> permanently
		/// </summary>
		void delete();

		/// <summary>
		/// Creates a copy of <c>this</c>including a copy of the data
		/// </summary>
		/// <returns>The copy</returns>
		IDataProvider copy();

		/// <summary>
		/// Gets the MIME type of the media stored in the data provider
		/// </summary>
		/// <returns>The MIME type</returns>
		string getMimeType();
	}
}
