using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace urakawa.media.data
{

	/// <summary>
	/// Implementation of interface <see cref="IDataProvider"/> using files as data storage
	/// </summary>
	public class FileDataProvider : IDataProvider
	{
		/// <summary>
		/// Constructs a new file data provider with a given manager and relative path
		/// </summary>
		/// <param name="mngr">The manager of the constructed instance</param>
		/// <param name="relPath">The relative path of the data file of the constructed instance</param>
		/// <param name="mimeType">The MIME type of the data to store in the constructed instance</param>
		protected internal FileDataProvider(FileDataProviderManager mngr, string relPath, string mimeType)
		{
			mManager = mngr;
			mDataFileRelativePath = relPath;
			mMimeType = mimeType;
			mManager.addDataProvider(this);
		}

		private FileDataProviderManager mManager;

		private string mDataFileRelativePath;

		/// <summary>
		/// Gets the path of the file storing the data of the instance, realtive to the path of data file directory
		/// of the owning <see cref="FileDataProviderManager"/>
		/// </summary>
		/// <returns></returns>
		public string getDataFileRealtivePath()
		{
			return mDataFileRelativePath;
		}

		/// <summary>
		/// Gets the full path of the file storing the data the instance
		/// </summary>
		/// <returns>The full path</returns>
		public string getDataFileFullPath()
		{
			return Path.Combine(mManager.getDataFileDirectoryFullPath(), mDataFileRelativePath);
		}

		#region IDataProvider Members

		private bool hasBeenInitialized = false;

		/// <summary>
		/// Gets the UID of the data provider in the context of the manager. 
		/// Convenience for <c>getDataProviderManager().getUidOfDataProvider(this)</c>
		/// </summary>
		/// <returns>The UID</returns>
		public string getUid()
		{
			return getDataProviderManager().getUidOfDataProvider(this);
		}

		private void checkDataFile()
		{
			if (File.Exists(getDataFileFullPath()))
			{
				if (!hasBeenInitialized)
				{
					File.Delete(getDataFileFullPath());
				}
				else
				{
					return;
				}
			}
			if (hasBeenInitialized)
			{
				throw new exception.DataFileDoesNotExistException(
					String.Format("The data file {0} does not exist", getDataFileFullPath()));
			}
			try
			{
				File.Create(getDataFileFullPath()).Close();
			}
			catch (Exception e)
			{
				throw new exception.OperationNotValidException(
					String.Format("Could not create data file {0}: {1}", getDataFileFullPath(), e.Message),
					e);
			}
			hasBeenInitialized = true;
		}

		/// <summary>
		/// Gets an input <see cref="Stream"/> providing read access to the <see cref="FileDataProvider"/>
		/// </summary>
		/// <returns>The input stream</returns>
		public Stream getInputStream()
		{
			FileStream inputStream;
			string fp = getDataFileFullPath();
			checkDataFile();
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
			checkDataFile();
			string fp = getDataFileFullPath();
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

		/// <summary>
		/// Deletes the file data provider, including any existing data files. Also detaches it self 
		/// the owning data provider manager
		/// </summary>
		/// <exception cref="exception.OperationNotValidException">
		/// Thrown if an exception occurs while deleting the data file of <c>this</c>. 
		/// The occuring exception can be accessed as the inner exception of the thrown exception.
		/// </exception>
		public void delete()
		{
			if (File.Exists(getDataFileFullPath()))
			{
				try
				{
					File.Delete(getDataFileFullPath());
				}
				catch (Exception e)
				{
					throw new exception.OperationNotValidException(String.Format(
						"Could not delete data file {0}: {1}",
						getDataFileFullPath(), e.Message), e);
				}
			}
			getDataProviderManager().detachDataProvider(this);
		}

		/// <summary>
		/// Copies the file data provider including the data 
		/// </summary>
		/// <returns>The copy</returns>
		public IDataProvider copy()
		{
			IDataProvider c = getDataProviderManager().getDataProviderFactory().createDataProvider(
				getMimeType(), getXukLocalName(), getXukNamespaceUri());
			if (c == null)
			{
				throw new exception.FactoryCanNotCreateTypeException(String.Format(
					"The data provider factory can not create a data provider matching QName {0}:{1}",
					getXukNamespaceUri(), getXukLocalName()));
			}
			Stream thisData = getInputStream();
			FileDataProviderManager.appendDataToProvider(thisData, (int)(thisData.Length - thisData.Position), c);
			return c;
		}


		IDataProviderManager IDataProvider.getDataProviderManager()
		{
			return getDataProviderManager();
		}

		/// <summary>
		/// Gets the <see cref="FileDataProviderManager"/> managing <c>this</c>
		/// </summary>
		/// <returns>The manager</returns>
		public FileDataProviderManager getDataProviderManager()
		{
			return mManager;
		}

		private string mMimeType;

		/// <summary>
		/// Gets the MIME type of the media stored in the data provider
		/// </summary>
		/// <returns>The MIME type</returns>
		public string getMimeType()
		{
			return mMimeType;
		}
		#endregion

		#region IXukAble Members

		/// <summary>
		/// Reads the <see cref="FileDataProvider"/> from a FileDataProvider xuk element
		/// </summary>
		/// <param name="source">The source <see cref="System.Xml.XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the read was succesful</returns>
		public bool XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element) return false;
			if (!XukInAttributes(source)) return false;
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						if (!XukInChild(source)) return false;
					}
					else if (source.NodeType == XmlNodeType.EndElement)
					{
						break;
					}
					if (source.EOF) break;
				}
			}
			return true;
		}

		/// <summary>
		/// Reads the attributes of a FileDataProvider xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the attributes was succefully read</returns>
		protected virtual bool XukInAttributes(XmlReader source)
		{
			if (hasBeenInitialized)
			{
				if (File.Exists(getDataFileFullPath()))
				{
					try
					{
						File.Delete(getDataFileFullPath());
					}
					catch (Exception)
					{
						return false;
					}
				}
			}
			mDataFileRelativePath = source.GetAttribute("DataFileRelativePath");
			if (mDataFileRelativePath == null || mDataFileRelativePath == "") return false;
			hasBeenInitialized = true;//Assume that the data file exists
			mMimeType = source.GetAttribute("MimeType");
			return true;
		}

		/// <summary>
		/// Reads a child of a FileDataProvider xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the child was succefully read</returns>
		protected virtual bool XukInChild(XmlReader source)
		{
			bool readItem = false;
			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past invalid MediaDataItem element (which is all since thare are no valid ones)
			}
			return true;
		}

		/// <summary>
		/// Write a FileDataProvider element to a XUK file representing the <see cref="FileDataProvider"/> instance
		/// </summary>
		/// <param name="destination">The destination <see cref="System.Xml.XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		public bool XukOut(System.Xml.XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not XukOut to a null XmlWriter");
			}
			destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
			if (!XukOutAttributes(destination)) return false;
			if (!XukOutChildren(destination)) return false;
			destination.WriteEndElement();
			return true;
		}

		/// <summary>
		/// Writes the attributes of a FileDataProvider element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutAttributes(XmlWriter destination)
		{
			checkDataFile();//Ensure that data file exist even if no data has yet been written to it.
			destination.WriteAttributeString("DataFileRelativePath", getDataFileRealtivePath());
			destination.WriteAttributeString("MimeType", getMimeType());
			return true;
		}

		/// <summary>
		/// Write the child elements of a FileDataProvider element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutChildren(XmlWriter destination)
		{
			// No children, nothing to do.
			return true;
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

		#region IValueEquatable<IDataProvider> Members

		/// <summary>
		/// Determines if the 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool ValueEquals(IDataProvider other)
		{
			if (other is FileDataProvider)
			{
				FileDataProvider o = (FileDataProvider)other;
				if (o.getDataFileRealtivePath() != getDataFileRealtivePath()) return false;
				if (o.getMimeType() != getMimeType()) return false;
				if (!FileDataProviderManager.compareDataProviderContent(this, o)) return false;
			}
			return false;
		}

		#endregion
	}
}
