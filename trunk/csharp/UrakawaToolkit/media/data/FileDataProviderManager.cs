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
		private Dictionary<string, IDataProvider> mDataProvidersDictionary = new Dictionary<string, IDataProvider>();
		private Dictionary<IDataProvider, string> mReverseLookupDataProvidersDictionary = new Dictionary<IDataProvider, string>();
		private IMediaDataPresentation mPresentation;
		private IDataProviderFactory mFactory;
		private string mDataFileDirectory;

		/// <summary>
		/// Constructor setting the base path and the data directory
		/// of the file data provider manager
		/// </summary>
		/// <param name="dataDir">
		/// The data file directory of the manager - relative to <paramref name="basePath"/>. 
		/// If <c>null</c>, "Data" is used
		/// </param>
		public FileDataProviderManager(string dataDir)
			: this(null, dataDir)
		{
		}

		/// <summary>
		/// Constructor setting the <see cref="IDataProviderFactory"/> of the manager, the base path and the data directory
		/// of the file data provider manager
		/// </summary>
		/// <param name="providerFact">The factory - if <c>null</c> a <see cref="FileDataProviderFactory"/> is used</param>
		/// <param name="dataDir">
		/// The data file directory of the manager - relative to <paramref name="basePath"/>. 
		/// If <c>null</c>, "Data" is used
		/// </param>
		public FileDataProviderManager(IDataProviderFactory providerFact, string dataDir)
		{
			if (providerFact == null) providerFact = new FileDataProviderFactory();
			mFactory = providerFact;
			mFactory.setDataProviderManager(this);
			if (dataDir == null) dataDir = "Data";
			if (Path.IsPathRooted(dataDir))
			{
				throw new exception.MethodParameterIsOutOfBoundsException("The data file directory path must be relative");
			}
			mDataFileDirectory = dataDir;
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
			try
			{
				provOutputStream.Seek(0, SeekOrigin.End);
				int bytesAppended = 0;
				byte[] buf = new byte[1024];
				while (bytesAppended < count)
				{
					if (bytesAppended + buf.Length >= count)
					{
						buf = new byte[count - bytesAppended];
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
			finally
			{
				provOutputStream.Close();
			}
		}

		/// <summary>
		/// Compares the data content of two data providers to check for value equality
		/// </summary>
		/// <param name="dp1">Data provider 1</param>
		/// <param name="dp2">Data provider 2</param>
		/// <returns>A <see cref="bool"/> indicating if the data content is identical</returns>
		public static bool compareDataProviderContent(IDataProvider dp1, IDataProvider dp2)
		{
			Stream s1 = null;
			Stream s2 = null;
			bool allEq = true;
			try
			{
				s1 = dp1.getInputStream();
				s2 = dp2.getInputStream();
				allEq = ((s1.Length-s1.Position) == (s2.Length-s2.Position));
				while (allEq && (s1.Position < s1.Length))
				{
					if (s1.ReadByte() != s2.ReadByte())
					{
						allEq = false;
						break;
					}
				}
			}
			finally
			{
				if (s1 != null) s1.Close();
				if (s2 != null) s2.Close();
			}
			return allEq;
		}


		/// <summary>
		/// Gets the path of the data file directory used by <see cref="FileDataProvider"/>s
		/// managed by <c>this</c>, relative to the base uri of the <see cref="IMediaDataPresentation"/>
		/// owning the file data provider manager.
		/// </summary>
		/// <returns>The path</returns>
		public string getDataFileDirectory()
		{
			return mDataFileDirectory;
		}

		/// <summary>
		/// Moves the data file directory of the manager
		/// </summary>
		/// <param name="newDataFileDir">The new data file direcotry</param>
		/// <param name="deleteSource">A <see cref="bool"/> indicating if the source/old data files shlould be deleted</param>
		/// <param name="overwriteDestDir">A <see cref="bool"/> indicating if the new data directory should be overwritten</param>
		public void moveDataFiles(string newDataFileDir, bool deleteSource, bool overwriteDestDir)
		{
			if (Path.IsPathRooted(newDataFileDir))
			{
				throw new exception.MethodParameterIsOutOfBoundsException("The data file directory path must be relative");
			}
			string oldPath = getDataFileDirectoryFullPath();
			mDataFileDirectory = newDataFileDir;
			string newPath = getDataFileDirectoryFullPath();
			try
			{
				if (Directory.Exists(newPath))
				{
					if (overwriteDestDir)
					{
						Directory.Delete(newPath);
					}
					else
					{
						throw new exception.OperationNotValidException(
							String.Format("Directory {0} already exists", newPath));
					}
				}
				CopyDataFiles(oldPath, newPath);
				if (deleteSource && Directory.Exists(oldPath))
				{
					Directory.Delete(oldPath);
				}
			}
			catch (Exception e)
			{
				throw new exception.OperationNotValidException(
					String.Format("Could not move data files to {0}: {1}", newPath, e.Message),
					e);
			}
		}

		private void CreateDirectory(string path)
		{
			if (!Directory.Exists(path))
			{
				if (!Directory.Exists(Path.GetDirectoryName(path))) CreateDirectory(Path.GetDirectoryName(path));
				Directory.CreateDirectory(path);
			}
		}

		private void CopyDataFiles(string source, string dest)
		{
			CreateDirectory(dest);
			foreach (FileDataProvider fdp in getListOfManagedFileDataProviders())
			{
				if (!File.Exists(Path.Combine(source, fdp.getDataFileRelativePath())))
				{
					throw new exception.DataFileDoesNotExistException(String.Format(
						"Error while copying data files from {0} to {1}: Data file {2} does not exist in the source",
						source, dest, fdp.getDataFileRelativePath()));
				}
				File.Copy(Path.Combine(source, fdp.getDataFileRelativePath()), Path.Combine(dest, fdp.getDataFileRelativePath()));
			}
		}

		private string getDataFileDirectoryFullPath(Uri baseUri)
		{
			if (!baseUri.IsFile)
			{
				throw new exception.InvalidUriException(
					"The base Uri of the presentation to which the FileDataProviderManager belongs must be a file Uri");
			}
			Uri dataFileDirUri = new Uri(baseUri, getDataFileDirectory());
			return dataFileDirUri.LocalPath;
		}


		/// <summary>
		/// Gets the full path of the data file directory. 
		/// Convenience for <c>Path.Combine(getBasePath(), getDataFileDirectory())</c>
		/// </summary>
		/// <returns>The full path</returns>
		public string getDataFileDirectoryFullPath()
		{
			return getDataFileDirectoryFullPath(getMediaDataPresentation().getBaseUri());
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
			if (mDataFileDirectory != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The data provider manager already has a data file directory");
			}
			if (!Directory.Exists(newPath))
			{
				Directory.CreateDirectory(newPath);
			}
			mDataFileDirectory = newPath;
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
				res = Path.ChangeExtension(Path.GetRandomFileName(), extension);
				foreach (FileDataProvider prov in getListOfManagedFileDataProviders())
				{
					if (res.ToLower() == prov.getDataFileRelativePath().ToLower())
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
		public List<FileDataProvider> getListOfManagedFileDataProviders()
		{
			List<FileDataProvider> res = new List<FileDataProvider>();
			foreach (IDataProvider prov in getListOfDataProviders())
			{
				if (prov is FileDataProvider)
				{
					res.Add((FileDataProvider)prov);
				}
			}
			return res;
		}


		#region IDataProviderManager Members



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
			if (ownerPres == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The owning Presentation of the FileDataProviderManager can not be null");
			}
			if (mPresentation != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The FileDataProviderManager has already been associated with a owning Presentation");
			}
			mPresentation = ownerPres;
			mPresentation.BaseUriChanged += new BaseUriChangedEventHandler(Presentation_BaseUriChanged);
		}

		void Presentation_BaseUriChanged(IMediaPresentation pres, BaseUriChangedEventArgs e)
		{
			if (e.getPreviousUri() != null)
			{
				string prevDataDirFullPath = getDataFileDirectoryFullPath(e.getPreviousUri());
				if (Directory.Exists(prevDataDirFullPath))
				{
					CopyDataFiles(prevDataDirFullPath, getDataFileDirectoryFullPath());
				}
			}
		}

		/// <summary>
		/// Gets the <see cref="IDataProviderFactory"/> of the <see cref="IDataProviderManager"/>
		/// </summary>
		/// <returns>The <see cref="IDataProviderFactory"/></returns>
		public IDataProviderFactory getDataProviderFactory()
		{
			return mFactory;
		}

		/// <summary>
		/// Detaches one of the <see cref="IDataProvider"/>s managed by the manager
		/// </summary>
		/// <param name="provider">The <see cref="IDataProvider"/> to delete</param>
		/// <param name="delete">A <see cref="bool"/> indicating if the removed data provider should be deleted</param>
		public void removeDataProvider(IDataProvider provider, bool delete)
		{
			if (provider == null)
			{
				throw new exception.MethodParameterIsNullException("Can not detach a null DataProvider from the manager");
			}
			if (delete)
			{
				provider.delete();
			}
			else
			{
				string uid = getUidOfDataProvider(provider);
				removeDataProvider(uid, provider);
			}
		}


		/// <summary>
		/// Detaches the <see cref="IDataProvider"/> with a given UID from the manager
		/// </summary>
		/// <param name="uid">The given UID</param>
		/// <param name="delete">A <see cref="bool"/> indicating if the removed data provider should be deleted</param>
		public void removeDataProvider(string uid, bool delete)
		{
			IDataProvider provider = getDataProvider(uid);
			if (delete)
			{
				provider.delete();
			}
			else
			{
				removeDataProvider(uid, provider);
			}
		}

		private void removeDataProvider(string uid, IDataProvider provider)
		{
			mDataProvidersDictionary.Remove(uid);
			mReverseLookupDataProvidersDictionary.Remove(provider);
		}

		/// <summary>
		/// Gets the UID of a given <see cref="IDataProvider"/>
		/// </summary>
		/// <param name="provider">The given data provider</param>
		/// <returns>The UID of <paramref name="provider"/></returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="provider"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsNotManagerOfException">
		/// Thrown when data provider <paramref name="provider"/> is not managed by <c>this</c>
		/// </exception>
		public string getUidOfDataProvider(IDataProvider provider)
		{
			if (provider == null)
			{
				throw new exception.MethodParameterIsNullException("Can not get the uid of a null DataProvider");
			}
			if (!mReverseLookupDataProvidersDictionary.ContainsKey(provider))
			{
				throw new exception.IsNotManagerOfException("The given DataProvider is not managed by this");
			}
			return mReverseLookupDataProvidersDictionary[provider];
		}

		/// <summary>
		/// Gets the <see cref="IDataProvider"/> with a given UID
		/// </summary>
		/// <param name="uid">The given UID</param>
		/// <returns>The data provider with the given UID</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="uid"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsNotManagerOfException">
		/// When no data providers managed by <c>this</c> has the given UID
		/// </exception>
		public IDataProvider getDataProvider(string uid)
		{
			if (uid == null)
			{
				throw new exception.MethodParameterIsNullException("Can not get the data provider with UID null");
			}
			if (!mDataProvidersDictionary.ContainsKey(uid))
			{
				throw new exception.IsNotManagerOfException(
					String.Format("The manager does not manage a DataProvider with UID {0}", uid));
			}
			return mDataProvidersDictionary[uid];
		}

		/// <summary>
		/// Adds a <see cref="IDataProvider"/> to the manager with a given uid
		/// </summary>
		/// <param name="provider">The data provider to add</param>
		/// <param name="uid">The uid to assign to the added data provider</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="provider"/> or <paramref name="uid"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsAlreadyManagerOfException">
		/// Thrown when the data provider is already added tothe manager 
		/// or if the manager already manages another data provider with the given uid
		/// </exception>
		/// <exception cref="exception.IsNotManagerOfException">Thrown if the data provides does not have <c>this</c> as manager</exception>
		public void addDataProvider(IDataProvider provider, string uid)
		{
			if (provider == null)
			{
				throw new exception.MethodParameterIsNullException("Can not manage a null DataProvider");
			}
			if (uid == null)
			{
				throw new exception.MethodParameterIsNullException("A managed DataProvider can not have uid null");
			}
			if (mReverseLookupDataProvidersDictionary.ContainsKey(provider))
			{
				throw new exception.IsAlreadyManagerOfException("The given DataProvider is already managed by the manager");
			}
			if (mDataProvidersDictionary.ContainsKey(uid))
			{
				throw new exception.IsAlreadyManagerOfException(String.Format(
					"Another DataProvider with uid {0} is already manager by the manager", uid));
			}
			if (provider.getDataProviderManager() != this)
			{
				throw new exception.IsNotManagerOfException("The given DataProvider does not return this as FileDataProviderManager");
			}

			mDataProvidersDictionary.Add(uid, provider);
			mReverseLookupDataProvidersDictionary.Add(provider, uid);
		}

		/// <summary>
		/// Determines if the manager manages a <see cref="IDataProvider"/> with a given uid
		/// </summary>
		/// <param name="uid">The given uid</param>
		/// <returns>
		/// A <see cref="bool"/> indicating if the manager manages a <see cref="IDataProvider"/> with the given uid
		/// </returns>
		public bool isManagerOf(string uid)
		{
			return mDataProvidersDictionary.ContainsKey(uid);
		}

		/// <summary>
		/// Sets the uid of a given <see cref="IDataProvider"/> to a given value
		/// </summary>
		/// <param name="provider">The given data provider</param>
		/// <param name="uid">The given uid</param>
		protected void setDataProviderUid(IDataProvider provider, string uid)
		{
			removeDataProvider(provider, false);
			addDataProvider(provider, uid);
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
			addDataProvider(provider, getNextUid());
		}

		private string getNextUid()
		{
			ulong i = 0;
			while (i < UInt64.MaxValue)
			{
				string newId = String.Format(
					"DPID{0:0000}", i);
				if (!mDataProvidersDictionary.ContainsKey(newId)) return newId;
				i++;
			}
			throw new OverflowException("YOU HAVE WAY TOO MANY DATAPROVIDERS!!!");
		}

		/// <summary>
		/// Gets a list of the <see cref="IDataProvider"/>s managed by the manager
		/// </summary>
		/// <returns>The list</returns>
		public List<IDataProvider> getListOfDataProviders()
		{
			return new List<IDataProvider>(mDataProvidersDictionary.Values);
		}

		/// <summary>
		/// Remove all <see cref="IDataProvider"/> that are managed by the manager, 
		/// but are not used by any <see cref="MediaData"/>
		/// </summary>
		/// <param name="delete">A <see cref="bool"/> indicating if the removed data providers should be deleted</param>
		public void removeUnusedDataProviders(bool delete)
		{
			List<IDataProvider> usedDataProviders = new List<IDataProvider>();
			foreach (MediaData md in getMediaDataPresentation().getMediaDataManager().getListOfMediaData())
			{
				foreach (IDataProvider prov in md.getListOfUsedDataProviders())
				{
					if (!usedDataProviders.Contains(prov)) usedDataProviders.Add(prov);
				}
			}
			foreach (IDataProvider prov in getListOfDataProviders())
			{
				if (!usedDataProviders.Contains(prov))
				{
					removeDataProvider(prov, delete);
				}
			}
		}

		#endregion

		#region IXukAble Members

		
		/// <summary>
		/// Reads the <see cref="FileDataProviderManager"/> from a FileDataProviderManager xuk element
		/// </summary>
		/// <param name="source">The source <see cref="System.Xml.XmlReader"/></param>
		public void XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element)
			{
				throw new exception.XukException("Can not read FileDataProviderManager from a non-element node");
			}
			try
			{
				mDataProvidersDictionary.Clear();
				mDataFileDirectory = null;
				XukInAttributes(source);
				mReverseLookupDataProvidersDictionary.Clear();
				mXukedInFilDataProviderPaths = new List<string>();
				if (!source.IsEmptyElement)
				{
					while (source.Read())
					{
						if (source.NodeType == XmlNodeType.Element)
						{
							XukInChild(source);
						}
						else if (source.NodeType == XmlNodeType.EndElement)
						{
							break;
						}
						if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
					}
				}
				mXukedInFilDataProviderPaths = null;

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukIn of FileDataProviderManager: {0}", e.Message),
					e);
			}
		}

		private List<string> mXukedInFilDataProviderPaths;

		/// <summary>
		/// Reads the attributes of a FileDataProviderManager xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInAttributes(XmlReader source)
		{
			string dataFileDirectoryPath = source.GetAttribute("dataFileDirectoryPath");
			if (dataFileDirectoryPath == null || dataFileDirectoryPath == "")
			{
				throw new exception.XukException(
					"dataFileDirectoryPath attribute is missing from FileDataProviderManager element");
			}
			setDataFileDirectoryPath(dataFileDirectoryPath);
		}

		/// <summary>
		/// Reads a child of a FileDataProviderManager xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInChild(XmlReader source)
		{
			bool readItem = false;
			if (source.NamespaceURI==ToolkitSettings.XUK_NS)
			{
				readItem=true;
				switch (source.LocalName)
				{
					case "mDataProviders":
						XukInDataProviders(source);
						break;
					default:
						readItem = false;
						break;
				}
			}
			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past invalid MediaDataItem element
			}
		}

		private void XukInDataProviders(XmlReader source)
		{
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						if (source.LocalName == "mDataProviderItem" && source.NamespaceURI == ToolkitSettings.XUK_NS)
						{
							XukInDataProviderItem(source);
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
					if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
				}
			}
		}

		private void XukInDataProviderItem(XmlReader source)
		{
			string uid = source.GetAttribute("uid");
			if (!source.IsEmptyElement)
			{
				bool addedProvider = false;
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						IDataProvider prov = getDataProviderFactory().createDataProvider("", source.LocalName, source.NamespaceURI);
						if (prov != null)
						{

							if (addedProvider)
							{
								throw new exception.XukException("Multiple DataProviders within the same mDataProviderItem is not supported");
							}
							prov.XukIn(source);
							if (prov is FileDataProvider)
							{
								FileDataProvider fdProv = (FileDataProvider)prov;
								if (mXukedInFilDataProviderPaths.Contains(fdProv.getDataFileRelativePath().ToLower()))
								{
									throw new exception.XukException(String.Format(
										"Another FileDataProvider using data file {0} has already been Xukked in",
										fdProv.getDataFileRelativePath().ToLower()));
								}
								mXukedInFilDataProviderPaths.Add(fdProv.getDataFileRelativePath().ToLower());
							}
							if (uid == null || uid == "")
							{
								throw new exception.XukException("uid attribute of mDataProviderItem element is missing");
							}
							if (isManagerOf(uid))
							{
								if (getDataProvider(uid) != prov)
								{
									throw new exception.XukException(
										String.Format("Another DataProvider exists in the manager with uid {0}", uid));
								}
							}
							else
							{
								setDataProviderUid(prov, uid);
							}
							addedProvider = true;
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
					if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
				}
			}
		}

		/// <summary>
		/// Write a FileDataProviderManager element to a XUK file representing the <see cref="FileDataProviderManager"/> instance
		/// </summary>
		/// <param name="destination">The destination <see cref="System.Xml.XmlWriter"/></param>
		public void XukOut(XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not XukOut to a null XmlWriter");
			}

			try
			{
				destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
				XukOutAttributes(destination);
				XukOutChildren(destination);
				destination.WriteEndElement();

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukOut of FileDataProviderManager: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a FileDataProviderManager element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutAttributes(XmlWriter destination)
		{
			Uri baseUri = getMediaDataPresentation().getBaseUri();
			Uri dfdUri = new Uri(baseUri, getDataFileDirectory());
			destination.WriteAttributeString("dataFileDirectoryPath", baseUri.MakeRelativeUri(dfdUri).ToString());
		}

		/// <summary>
		/// Write the child elements of a FileDataProviderManager element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutChildren(XmlWriter destination)
		{
			destination.WriteStartElement("mDataProviders", ToolkitSettings.XUK_NS);
			foreach (IDataProvider prov in getListOfDataProviders())
			{
				destination.WriteStartElement("mDataProviderItem", ToolkitSettings.XUK_NS);
				destination.WriteAttributeString("uid", prov.getUid());
				prov.XukOut(destination);
				destination.WriteEndElement();
			}
			destination.WriteEndElement();
		}

		
		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="FileDataProviderFactory"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="FileDataProviderFactory"/> in Xuk
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
		/// <remarks>The base path of the <see cref="FileDataProviderManager"/>s are not compared</remarks>
		public bool valueEquals(IDataProviderManager other)
		{
			if (other is FileDataProviderManager)
			{
				FileDataProviderManager o = (FileDataProviderManager)other;
				if (o.getDataFileDirectory() != getDataFileDirectory()) return false;
				List<IDataProvider> oDP = getListOfDataProviders();
				if (o.getListOfDataProviders().Count != oDP.Count) return false;
				foreach (IDataProvider dp in oDP)
				{
					string uid = dp.getUid();
					if (!o.isManagerOf(uid)) return false;
					if (!o.getDataProvider(uid).valueEquals(dp)) return false;
				}
			}
			return true;
		}

		#endregion
	}
}
