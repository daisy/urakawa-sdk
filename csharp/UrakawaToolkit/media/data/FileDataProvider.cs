using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace urakawa.media.data
{

	/// <summary>
	/// Implementation of interface <see cref="IDataProvider"/> using files as data storage
	/// </summary>
	public class FileDataProvider : IDataProvider
	{
		protected internal FileDataProvider(DataProviderManager mngr, string relPath)
		{
			mManager = mngr;
			mRelativeFilePath = relPath;
		}

		private DataProviderManager mManager;

		private string mRelativeFilePath;

		/// <summary>
		/// Gets the full path of the file storing the data the instance
		/// </summary>
		/// <returns>The full path</returns>
		public string getFilePath()
		{
			return Path.Combine(mManager.getDataFileDirectoryPath(), mRelativeFilePath);
		}

		#region IDataProvider Members

		/// <summary>
		/// Gets an input <see cref="Stream"/> providing read access to the <see cref="FileDataProvider"/>
		/// </summary>
		/// <returns>The input stream</returns>
		public Stream getInputStream()
		{
			FileStream inputStream;
			string fp = getFilePath();
			if (!File.Exists(fp))
			{
				throw new exception.DataFileDoesNotExistException(
					String.Format("Data file {0} does not exist", fp));
			}
			try
			{
				inputStream = new FileStream(fp, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			catch (Exception e)
			{
				throw new exception.OperationNotValidException(
					String.Format("Could not open file {0}", fp),
					e);
			}
			return inputStream;
		}

		/// <summary>
		/// Gets an output <see cref="Stream"/> providing write access to the <see cref="FileDataProvider"/>
		/// </summary>
		/// <returns>The ouput stream</returns>
		public Stream getOutputStream()
		{
			FileStream outputStream;
			string fp = getFilePath();
			if (!File.Exists(fp))
			{
				throw new exception.DataFileDoesNotExistException(
					String.Format("Data file {0} does not exist", fp));
			}
			try
			{
				outputStream = new FileStream(fp, FileMode.Open, FileAccess.Write, FileShare.Read);
			}
			catch (Exception e)
			{
				throw new exception.OperationNotValidException(
					String.Format("Could not open file {0}", fp),
					e);
			}
			return outputStream;
		}




		public void delete()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IDataProvider copy()
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


		
		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="FileDataProvider"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="FileDataProvider"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion


		#region IDataProvider Members

		IDataProviderManager IDataProvider.getDataProviderManager()
		{
			return getDataProviderManager();
		}

		/// <summary>
		/// Gets the <see cref="DataProviderManager"/> managing <c>this</c>
		/// </summary>
		/// <returns>The manager</returns>
		public DataProviderManager getDataProviderManager()
		{
			return mManager;
		}

		#endregion
	}
}
