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
			bool allEq = false;
			try
			{
				s1 = dp1.getInputStream();
				s2 = dp2.getInputStream();
				allEq = ((s1.Length-s1.Position) == (s2.Length-s2.Position));
				while (allEq && (s1.Position < s1.Length))
				{
					if (s1.ReadByte() == s2.ReadByte()) allEq = false;
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
		/// managed by <c>this</c>
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
			string oldPAth = getDataFileDirectoryFullPath();
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
				if (deleteSource)
				{
					Directory.Move(getDataFileDirectoryFullPath(), newPath);
				}
				else
				{
					CopyDirectory(new DirectoryInfo(getDataFileDirectoryFullPath()), newPath);
				}
			}
			catch (Exception e)
			{
				throw new exception.OperationNotValidException(
					String.Format("Could not move data files to {0}: {1}", newPath, e.Message),
					e);
			}
		}

		private void CopyDirectory(DirectoryInfo di, string dest)
		{
			foreach (FileInfo fi in di.GetFiles())
			{
				fi.CopyTo(Path.Combine(dest, fi.Name));
			}
			foreach (DirectoryInfo sdi in di.GetDirectories())
			{
				CopyDirectory(sdi, Path.Combine(dest, sdi.Name));
			}
		}


		/// <summary>
		/// Gets the full path of the data file directory. 
		/// Convenience for <c>Path.Combine(getBasePath(), getDataFileDirectory())</c>
		/// </summary>
		/// <returns>The full path</returns>
		public string getDataFileDirectoryFullPath()
		{
			Uri baseUri = getMediaDataPresentation().getBaseUri();
			if (!baseUri.IsFile)
			{
				throw new exception.InvalidUriException(
					"The base Uri of the presentation to which the FileDataProviderManager belongs must be a file Uri");
			}
			return Path.Combine(baseUri.LocalPath, getDataFileDirectory());
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
		public void detachDataProvider(IDataProvider provider)
		{
			if (provider == null)
			{
				throw new exception.MethodParameterIsNullException("Can not detach a null DataProvider from the manager");
			}
			string uid = getUidOfDataProvider(provider);
		}


		/// <summary>
		/// Detaches the <see cref="IDataProvider"/> with a given UID from the manager
		/// </summary>
		/// <param name="uid">The given UID</param>
		public void detachDataProvider(string uid)
		{
			IDataProvider provider = getDataProvider(uid);
			detachDataProvider(uid, provider);
		}

		private void detachDataProvider(string uid, IDataProvider provider)
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

		protected void addDataProvider(IDataProvider provider, string uid)
		{
			if (provider == null)
			{
				throw new exception.MethodParameterIsNullException("Can not manage a null DataProvider");
			}
			if (mReverseLookupDataProvidersDictionary.ContainsKey(provider))
			{
				throw new exception.IsAlreadyManagerOfException("The given DataProvider is already managed by the manager");
			}
			if (mDataProvidersDictionary.ContainsKey(uid))
			{
				throw new exception.IsAlreadyInitializedException(String.Format(
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
		public IList<IDataProvider> getListOfManagedDataProviders()
		{
			return new List<IDataProvider>(mDataProvidersDictionary.Values);
		}

		#endregion

		#region IXukAble Members

		
		/// <summary>
		/// Reads the <see cref="FileDataProviderManager"/> from a FileDataProviderManager xuk element
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
			mDataProvidersDictionary.Clear();
			mReverseLookupDataProvidersDictionary.Clear();
			mXukedInFilDataProviderPaths = new List<string>();
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
			mXukedInFilDataProviderPaths = null;
			return true;
		}

		private IList<string> mXukedInFilDataProviderPaths;

		/// <summary>
		/// Reads the attributes of a FileDataProviderManager xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the attributes was succefully read</returns>
		protected virtual bool XukInAttributes(XmlReader source)
		{
			if (source.BaseURI == String.Empty)
			{
				mDataFileDirectory = source.GetAttribute("DataFileDirectoryPath");
			}
			else
			{
				Uri dfDir = new Uri(new Uri(source.BaseURI), source.GetAttribute("DataFileDirectoryPath"));
				mDataFileDirectory = dfDir.LocalPath;
			}
			return true;
		}

		/// <summary>
		/// Reads a child of a FileDataProviderManager xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the child was succefully read</returns>
		protected virtual bool XukInChild(XmlReader source)
		{
			bool readItem = false;
			if (source.NamespaceURI==ToolkitSettings.XUK_NS)
			{
				readItem=true;
				switch (source.LocalName)
				{
					case "mDataProviders":
						if (!XukInDataProviders(source)) return false;
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
			return true;
		}

		private bool XukInDataProviders(XmlReader source)
		{
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						if (source.LocalName == "mDataProviderItem" && source.NamespaceURI == ToolkitSettings.XUK_NS)
						{
							if (!XukInDataProviderItem(source)) return false;
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
			return true;
		}

		private bool XukInDataProviderItem(XmlReader source)
		{
			string uid = source.GetAttribute("uid");
			if (getDataProvider(uid) != null) return false;
			IDataProvider prov = getDataProviderFactory().createDataProvider("", source.LocalName, source.NamespaceURI);
			if (prov != null)
			{
				if (!prov.XukIn(source)) return false;
				if (prov is FileDataProvider)
				{
					FileDataProvider fdProv = (FileDataProvider)prov;
					if (mXukedInFilDataProviderPaths.Contains(fdProv.getDataFileRealtivePath().ToLower())) return false;
					mXukedInFilDataProviderPaths.Add(fdProv.getDataFileRealtivePath().ToLower());
				}
				addDataProvider(prov, uid);
			}
			else if (!source.IsEmptyElement)
			{
				source.ReadSubtree().Close();
			}
			return true;
		}

		/// <summary>
		/// Write a FileDataProviderManager element to a XUK file representing the <see cref="FileDataProviderManager"/> instance
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
		/// Writes the attributes of a FileDataProviderManager element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutAttributes(XmlWriter destination)
		{
			Uri baseUri = getMediaDataPresentation().getBaseUri();
			Uri dfdUri = new Uri(baseUri, getDataFileDirectory());
			destination.WriteAttributeString("DataFileDirectoryPath", baseUri.MakeRelativeUri(dfdUri).ToString());
			return true;
		}

		/// <summary>
		/// Write the child elements of a FileDataProviderManager element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
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
		public bool ValueEquals(IDataProviderManager other)
		{
			if (other is FileDataProviderManager)
			{
				FileDataProviderManager o = (FileDataProviderManager)other;
				if (o.getDataFileDirectory() != getDataFileDirectory()) return false;
				IList<IDataProvider> oDP = getListOfManagedDataProviders();
				if (o.getListOfManagedDataProviders().Count != oDP.Count) return false;
				foreach (IDataProvider dp in oDP)
				{
					string uid = dp.getUid();
					try
					{
						if (!o.getDataProvider(uid).ValueEquals(dp)) return false;
					}
					catch (exception.IsNotManagerOfException)
					{
						return false;
					}
				}
			}
			return false;
		}

		#endregion
	}
}
