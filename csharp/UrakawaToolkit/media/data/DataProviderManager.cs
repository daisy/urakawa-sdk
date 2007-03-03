using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace urakawa.media.data
{
	/// <summary>
	/// Default implementation of <see cref="IDataProviderManager"/> and <see cref="IDataProviderFactory"/>
	/// </summary>
	public class DataProviderManager : IDataProviderManager
	{
		/// <summary>
		/// Appends data from a given input <see cref="Stream"/> to a given <see cref="IDataProvider"/>
		/// </summary>
		/// <param name="data">The given input stream</param>
		/// <param name="count">The number of bytes to append</param>
		/// <param name="provider">The given data provider</param>
		public static void appendDataToProvider(Stream data, int count, IDataProvider provider)
		{
			Stream provOutputStream = provider.getOutputStream();
			provOutputStream.Seek(0, SeekOrigin.End);
			int bytesAppended = 0;
			byte[] buf = new byte[1024];
			while (bytesAppended < count)
			{
				if (bytesAppended+buf.Length>=count)
				{
					buf = new byte[count-bytesAppended];
				}
				if (data.Read(buf, 0, buf.Length) != buf.Length)
				{
					throw new exception.InputStreamIsTooShortException(
						String.Format("Can not add {0:0} bytes from the given data Stream", count));
				}
				provOutputStream.Write(buf, 0, buf.Length);
				bytesAppended += buf.Length;
			}
		}

		private string mDataFileDirectoryPath;

		/// <summary>
		/// Gets the path of the data file directory used by <see cref="FileDataProvider"/>s
		/// managed by <c>this</c>
		/// </summary>
		/// <returns>The path</returns>
		public string getDataFileDirectoryPath()
		{
			return mDataFileDirectoryPath;
		}

		/// <summary>
		/// Initializer that sets the path of the data file directory
		/// used by <see cref="FileDataProvider"/>s managed by <c>this</c>
		/// </summary>
		/// <param name="newPath"></param>
		public void setDataFileDirectoryPath(string newPath)
		{
			if (newPath == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The path of the data file directory can not be null");
			}
			if (mDataFileDirectoryPath != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The data provider manager already has a data file directory");
			}
			if (!Directory.Exists(newPath))
			{
				Directory.CreateDirectory(newPath);
			}
			mDataFileDirectoryPath = newPath;
		}


		#region IDataProviderManager Members

		private IMediaDataPresentation mPresentation;


		/// <summary>
		/// Gets the <see cref="IMediaDataPresentation"/> that owns the <see cref="IDataProviderManager"/>
		/// </summary>
		/// <returns>The <see cref="IMediaDataPresentation"/> that owns <c>this</c></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when no owning <see cref="IMediaDataPresentation"/> has been associated with <c>this</c>
		/// </exception>
		public IMediaDataPresentation getMediaDataPresentation()
		{
			if (mPresentation == null)
			{
				throw new exception.IsNotInitializedException(
					"The DataProviderManager has not been a initialized with a owning presentation");
			}
			return mPresentation;
		}

		/// <summary>
		/// Initializer associating an owning <see cref="IMediaDataPresentation"/> with <c>this</c>
		/// </summary>
		/// <param name="ownerPres">The owning presentation</param>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when a owning <see cref="IMediaDataPresentation"/> has already been associated with <c>this</c>
		/// </exception>
		public void setPresentation(IMediaDataPresentation ownerPres)
		{
			if (mPresentation != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The DataProviderManager has already been associated with a owning presentation");
			}
			mPresentation = ownerPres;
		}

		private IDataProviderFactory mFactory;

		/// <summary>
		/// Gets the <see cref="IDataProviderFactory"/> of the <see cref="IDataProviderManager"/>
		/// </summary>
		/// <returns>The <see cref="IDataProviderFactory"/></returns>
		public IDataProviderFactory getDataProviderFactory()
		{
			return mFactory;
		}

		public void detachDataProvider(IDataProvider provider)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void addDataProvider(IDataProvider provider)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IList<IDataProvider> getListOfManagedDataProvider()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IXukAble Members

		public bool XukIn(System.Xml.XmlReader source)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool XukOut(System.Xml.XmlWriter destination)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public string getXukLocalName()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public string getXukNamespaceUri()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IValueEquatable<IDataProviderManager> Members

		public bool ValueEquals(IDataProviderManager other)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
