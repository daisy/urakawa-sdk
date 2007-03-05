using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace urakawa.media.data
{
	/// <summary>
	/// Default implementation of <see cref="IDataProviderManager"/> and <see cref="IDataProviderFactory"/>
	/// </summary>
	public class FileDataProviderManager : IDataProviderManager
	{
		/// <summary>
		/// Constructor setting the <see cref="IDataProviderFactory"/> of the manager
		/// </summary>
		/// <param name="providerFact">The factory</param>
		public FileDataProviderManager(IDataProviderFactory providerFact)
		{
			if (providerFact == null)
			{
				throw new exception.MethodParameterIsNullException("A FileDataProviderManager can not have a null DataProviderFactory");
			}
			mFactory = providerFact;
			mFactory.setDataProviderManager(this);
		}

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

		/// <summary>
		/// Gets a new data file path relative to the path of the data file directory of the manager
		/// </summary>
		/// <param name="extension">The entension of the new data file path</param>
		/// <returns>The relative path</returns>
		public string getNewDataFileRelPath(string extension)
		{
			string res;
			while (true)
			{
				string dataFileDir = getDataFileDirectoryPath();
				res = Path.ChangeExtension(Path.GetRandomFileName(), extension);
				foreach (FileDataProvider prov in getListOfManagedFileDataProviders())
				{
					if (res.ToLower() == prov.getDataFileRealtivePath().ToLower())
					{
						continue;
					}
				}
				break;
			}

			return res;
		}

		/// <summary>
		/// Gets a list of the <see cref="FileDataProvider"/>s managed by the manager
		/// </summary>
		/// <returns>The list of file data providers</returns>
		public IList<FileDataProvider> getListOfManagedFileDataProviders()
		{
			List<FileDataProvider> res = new List<FileDataProvider>();
			foreach (IDataProvider prov in getListOfManagedDataProviders())
			{
				if (prov is FileDataProvider)
				{
					res.Add((FileDataProvider)prov);
				}
			}
			return res;
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
					"The FileDataProviderManager has not been a initialized with a owning presentation");
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
					"The FileDataProviderManager has already been associated with a owning presentation");
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

		List<IDataProvider> mManagedDataProviders = new List<IDataProvider>();

		/// <summary>
		/// Detaches one of the <see cref="IDataProvider"/>s managed by the manager
		/// </summary>
		/// <param name="provider">The <see cref="IDataProvider"/> to delete</param>
		public void detachDataProvider(IDataProvider provider)
		{
			if (provider == null)
			{
				throw new exception.MethodParameterIsNullException("Can not detach a null DataProvider from the manager");
			}
			if (!mManagedDataProviders.Remove(provider))
			{
				throw new exception.IsNotManagerOfException("The given data DataProvider is not managed by the manager");
			}
		}

		/// <summary>
		/// Adds a <see cref="IDataProvider"/> to be managed by the manager
		/// </summary>
		/// <param name="provider">The data provider</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="provider"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsAlreadyManagerOfException">
		/// Thrown when <paramref name="provider"/> is already managed by <c>this</c>
		/// </exception>
		/// <exception cref="exception.IsNotManagerOfException">
		/// Thrown when <paramref name="provider"/> does not return <c>this</c> as owning manager
		/// </exception>
		/// <seealso cref="IDataProvider.getDataProviderManager"/>
		public void addDataProvider(IDataProvider provider)
		{
			if (provider == null)
			{
				throw new exception.MethodParameterIsNullException("Can not manage a null DataProvider");
			}
			if (mManagedDataProviders.Contains(provider))
			{
				throw new exception.IsAlreadyManagerOfException("The given DataProvider is already managed by the manager");
			}
			if (provider.getDataProviderManager() != this)
			{
				throw new exception.IsNotManagerOfException("The given DataProvider does not return this as FileDataProviderManager");
			}
			mManagedDataProviders.Add(provider);
		}

		/// <summary>
		/// Gets a list of the <see cref="IDataProvider"/>s managed by the manager
		/// </summary>
		/// <returns>The list</returns>
		public IList<IDataProvider> getListOfManagedDataProviders()
		{
			return new List<IDataProvider>(mManagedDataProviders);
		}

		#endregion

		#region IXukAble Members

		
		/// <summary>
		/// Reads the <see cref="DataproviderFactory"/> from a DataproviderFactory xuk element
		/// </summary>
		/// <param localName="source">The source <see cref="System.Xml.XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the read was succesful</returns>
		public bool XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element) return false;
			if (!XukInAttributes(source)) return false;
			mManagedDataProviders.Clear();
			mXukedInFileDataProviders = new Dictionary<string, FileDataProvider>();
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
			mXukedInFileDataProviders = null;
			return true;
		}

		private Dictionary<string, FileDataProvider> mXukedInFileDataProviders;

		/// <summary>
		/// Reads the attributes of a DataproviderFactory xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the attributes was succefully read</returns>
		protected virtual bool XukInAttributes(XmlReader source)
		{
			//TODO: Figure out how to handle relative paths
			mDataFileDirectoryPath = source.GetAttribute("DataFileDirectoryPath");

			return true;
		}

		/// <summary>
		/// Reads a child of a DataproviderFactory xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the child was succefully read</returns>
		protected virtual bool XukInChild(XmlReader source)
		{
			bool readItem = false;
			if (source.NamespaceURI == ToolkitSettings.XUK_NS && source.LocalName == "mDataProviders")
			{
				if (!source.IsEmptyElement)
				{
					while (source.Read())
					{
						if (source.NodeType==XmlNodeType.Element)
						{
							IDataProvider prov = getDataProviderFactory().createDataProvider("", source.LocalName, source.NamespaceURI);
							if (prov != null)
							{
								if (!prov.XukIn(source)) return false;
								if (prov is FileDataProvider)
								{
									//check if relative file path is unique
									FileDataProvider fdProv = (FileDataProvider)prov;
									if (mXukedInFileDataProviders.ContainsKey(fdProv.getDataFileRealtivePath().ToLower())) return false;
									mXukedInFileDataProviders.Add(fdProv.getDataFileRealtivePath().ToLower(), fdProv);
								}
							}
							else if (!source.IsEmptyElement)
							{
								source.ReadSubtree().Close();
							}
						}
						else if (source.NodeType == XmlNodeType.EndElement)
						{
							break;
						}
						if (source.EOF) break;
					}
				}
			}
			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past invalid MediaDataItem element
			}
			return true;
		}

		/// <summary>
		/// Write a DataProviderFactory element to a XUK file representing the <see cref="DataProviderFactory"/> instance
		/// </summary>
		/// <param localName="destination">The destination <see cref="System.Xml.XmlWriter"/></param>
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
		/// Writes the attributes of a DataProviderFactory element
		/// </summary>
		/// <param localName="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutAttributes(XmlWriter destination)
		{
			destination.WriteAttributeString("DataFileDirectoryPath", mDataFileDirectoryPath);
			return true;
		}

		/// <summary>
		/// Write the child elements of a DataProviderFactory element.
		/// </summary>
		/// <param localName="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutChildren(XmlWriter destination)
		{
			destination.WriteStartElement("mDataProviders", ToolkitSettings.XUK_NS);
			foreach (IDataProvider prov in getListOfManagedDataProviders())
			{
				if (!prov.XukOut(destination)) return false;
			}
			destination.WriteEndElement();
			return true;
		}

		
		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="DataProviderFactory"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="DataProviderFactory"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

		#region IValueEquatable<IDataProviderManager> Members

		/// <summary>
		/// Determines of <c>this</c> has the same value as a given other instance
		/// </summary>
		/// <param name="other">The other instance</param>
		/// <returns>A <see cref="bool"/> indicating the result</returns>
		public bool ValueEquals(IDataProviderManager other)
		{
			if (other is FileDataProviderManager)
			{
				FileDataProviderManager o = (FileDataProviderManager)other;
				if (o.getDataFileDirectoryPath() != getDataFileDirectoryPath()) return false;
				
			}
			return false;
		}

		#endregion
	}
}
