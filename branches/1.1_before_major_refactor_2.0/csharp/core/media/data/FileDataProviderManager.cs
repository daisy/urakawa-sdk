using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using urakawa.progress;

namespace urakawa.media.data
{
	/// <summary>
	/// Default implementation of <see cref="IDataProviderManager"/> and <see cref="IDataProviderFactory"/>
	/// </summary>
	public class FileDataProviderManager : WithPresentation, IDataProviderManager
	{

        public void allowCopyDataOnUriChanged(bool enable)
        {
            if (enable)
            {
                getPresentation().rootUriChanged += Presentation_rootUriChanged;
            }
            else
            {
                getPresentation().rootUriChanged -= Presentation_rootUriChanged;
            }
        }

		private Dictionary<string, IDataProvider> mDataProvidersDictionary = new Dictionary<string, IDataProvider>();
		private Dictionary<IDataProvider, string> mReverseLookupDataProvidersDictionary = new Dictionary<IDataProvider, string>();
		private List<string> mXukedInFilDataProviderPaths = new List<string>();
		private string mDataFileDirectory;


		/// <summary>
		/// Initializes the manager with a <see cref="Presentation"/>, 
		/// </summary>
		/// <param name="newPres">The new presentation</param>
		public override void setPresentation(Presentation newPres)
		{
			base.setPresentation(newPres);


            //Removed because too "dangerous": a host application may have a different file management logic than the one implemented here.
            //(i.e. copying more than just the managed media data)
			//newPres.rootUriChanged += new EventHandler<urakawa.events.presentation.RootUriChangedEventArgs>(Presentation_rootUriChanged);
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		internal protected FileDataProviderManager()
		{
			mDataFileDirectory = null;
		}

		/// <summary>
		/// Appends data from a given input <see cref="Stream"/> to a given <see cref="IDataProvider"/>
		/// </summary>
		/// <param name="data">The given input stream</param>
		/// <param name="count">The number of bytes to append</param>
		/// <param name="provider">The given data provider</param>
		public static void appendDataToProvider(Stream data, int count, IDataProvider provider)
		{
            if (count <= 0)
            {
                return;
            }

            if (count > data.Length)
            {
                throw new exception.InputStreamIsTooShortException(
                            String.Format("The given data Stream is shorter than the requested {0:0} bytes",
                            count));
            }

		    Stream provOutputStream = provider.getOutputStream();
			try
			{
				provOutputStream.Seek(0, SeekOrigin.End);

                const int BUFFER_SIZE = 1024 * 300; // 300 KB MAX BUFFER 
                if (count <= BUFFER_SIZE) 
                {
                    byte[] buffer = new byte[count];
                    int bytesRead = data.Read(buffer, 0, count);
                    if (bytesRead > 0)
                    {
                        provOutputStream.Write(buffer, 0, bytesRead);
                    }
                    else
                    {
                        throw new exception.InputStreamIsTooShortException(
                            String.Format("Can not read {0:0} bytes from the given data Stream",
                            count));
                    }
                }
                else
                {
                    int bytesRead = 0;
                    byte[] buffer = new byte[BUFFER_SIZE];

                    while ((bytesRead = data.Read(buffer, 0, BUFFER_SIZE)) > 0)
                    {
                        provOutputStream.Write(buffer, 0, bytesRead);
                    }
                }

                /*
			    int bytesAppended = 0;
                byte[] buf = new byte[1024 * 10]; // 10 KB
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
                */
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
		/// managed by <c>this</c>, relative to the base uri of the <see cref="Presentation"/>
		/// owning the file data provider manager.
		/// </summary>
		/// <returns>The path</returns>
		/// <remarks>
		/// The DataFileDirectory is initialized lazily:
		/// If the DataFileDirectory has not been explicitly initialized using the <see cref="setDataFileDirectory"/> method,
		/// calling <see cref="getDataFileDirectory"/> will assing it the default value "Data"</remarks>
		public string getDataFileDirectory()
		{
			if (mDataFileDirectory == null) mDataFileDirectory = "Data";
			return mDataFileDirectory;
		}

		/// <summary>
		/// Initializes the <see cref="FileDataProvider"/> with a DataFileDirectory
		/// </summary>
		/// <param name="dataDir">The new DataFileDirectory - must be a relative uri</param>
		public void setDataFileDirectory(string dataDir)
		{
			if (dataDir == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The DataFileDirectory can not be null");
			}
			if (mDataFileDirectory != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The FileDataProviderManager has already been initialized with a DataFileDirectory");
			}
			Uri tmp;
			if (!Uri.TryCreate(dataDir, UriKind.Relative, out tmp))
			{
				throw new exception.InvalidUriException(String.Format(
					"DataFileDirectory must be a relative Uri, '{0}' is not", dataDir));
			}
			mDataFileDirectory = dataDir;
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
				string parentDir = Path.GetDirectoryName(path);
				if (!Directory.Exists(parentDir)) CreateDirectory(parentDir);
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
					throw new exception.DataMissingException(String.Format(
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
			return getDataFileDirectoryFullPath(getPresentation().getRootUri());
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

		void Presentation_rootUriChanged(Object o, urakawa.events.presentation.RootUriChangedEventArgs e)
		{
			if (e.PreviousUri != null)
			{
				string prevDataDirFullPath = getDataFileDirectoryFullPath(e.PreviousUri);
				if (Directory.Exists(prevDataDirFullPath))
				{
					CopyDataFiles(prevDataDirFullPath, getDataFileDirectoryFullPath());
				}
			}
		}

		/// <summary>
		/// Gets the <see cref="FileDataProviderFactory"/> of the <see cref="IDataProviderManager"/>
		/// </summary>
		/// <returns>The <see cref="IDataProviderFactory"/></returns>
		public FileDataProviderFactory getDataProviderFactory()
		{
			FileDataProviderFactory fact = getPresentation().getDataProviderFactory() as FileDataProviderFactory;
			if (fact == null)
			{
				throw new exception.IncompatibleManagerOrFactoryException(
					"The DataProviderFactory of the Presentation owning a FileDataProviderManager must be a FileDataProviderFactory");
			}
			return fact;
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
		protected void addDataProvider(IDataProvider provider, string uid)
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
		public void setDataProviderUid(IDataProvider provider, string uid)
		{
			removeDataProvider(provider, false);
			addDataProvider(provider, uid);
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
			foreach (MediaData md in getPresentation().getMediaDataManager().getListOfMediaData())
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

		IDataProviderFactory IDataProviderManager.getDataProviderFactory()
		{
			return getDataProviderFactory();
		}

		#endregion

		#region IXukAble Members

		/// <summary>
		/// Clears the <see cref="FileDataProviderManager"/>, clearing any links to <see cref="IDataProvider"/>s
		/// </summary>
		protected override void clear()
		{
			mDataProvidersDictionary.Clear();
			mDataFileDirectory = null;
			mReverseLookupDataProvidersDictionary.Clear();
			mXukedInFilDataProviderPaths.Clear();
			base.clear();
		}


		/// <summary>
		/// Reads the attributes of a FileDataProviderManager xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected override void xukInAttributes(XmlReader source)
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
        /// <param name="handler">The handler for progress</param>
        protected override void xukInChild(XmlReader source, ProgressHandler handler)
		{
			bool readItem = false;
			if (source.NamespaceURI==ToolkitSettings.XUK_NS)
			{
				readItem=true;
				switch (source.LocalName)
				{
					case "mDataProviders":
						xukInDataProviders(source, handler);
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

		private void xukInDataProviders(XmlReader source, ProgressHandler handler)
		{
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						if (source.LocalName == "mDataProviderItem" && source.NamespaceURI == ToolkitSettings.XUK_NS)
						{
							xukInDataProviderItem(source, handler);
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

		private void xukInDataProviderItem(XmlReader source, ProgressHandler handler)
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
							prov.xukIn(source, handler);
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
		/// Writes the attributes of a FileDataProviderManager element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected override void xukOutAttributes(XmlWriter destination, Uri baseUri)
		{
			Uri presBaseUri = getPresentation().getRootUri();
			Uri dfdUri = new Uri(presBaseUri, getDataFileDirectory());
			destination.WriteAttributeString("dataFileDirectoryPath", presBaseUri.MakeRelativeUri(dfdUri).ToString());
			base.xukOutAttributes(destination, baseUri);
		}

		/// <summary>
		/// Write the child elements of a FileDataProviderManager element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
        /// <param name="handler">The handler for progress</param>
        protected override void xukOutChildren(XmlWriter destination, Uri baseUri, ProgressHandler handler)
		{
			destination.WriteStartElement("mDataProviders", ToolkitSettings.XUK_NS);
			foreach (IDataProvider prov in getListOfDataProviders())
			{
				destination.WriteStartElement("mDataProviderItem", ToolkitSettings.XUK_NS);
				destination.WriteAttributeString("uid", prov.getUid());
				prov.xukOut(destination, baseUri, handler);
				destination.WriteEndElement();
			}
			destination.WriteEndElement();
			base.xukOutChildren(destination, baseUri, handler);
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
