using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using urakawa.progress;
using urakawa.xuk;

namespace urakawa.data
{
    /// <summary>
    /// Manager for <see cref="DataProvider"/>s
    /// </summary>
    public sealed class DataProviderManager : XukAbleManager<DataProvider>
    {
        public override string GetTypeNameFormatted()
        {
            return XukStrings.DataProviderManager;
        }

        public DataProviderManager(Presentation pres)
            : base(pres, "DP")
        {
            mDataFileDirectory = null;
            m_CompareByteStreamsDuringValueEqual = true;
        }

        public void AllowCopyDataOnUriChanged(bool enable)
        {
            if (enable)
            {
                Presentation.RootUriChanged += Presentation_rootUriChanged;
            }
            else
            {
                Presentation.RootUriChanged -= Presentation_rootUriChanged;
            }
        }

        private List<string> mXukedInFilDataProviderPaths = new List<string>();
        private string mDataFileDirectory;

        /// <summary>
        /// Compares the data content of two data providers to check for value equality
        /// </summary>
        /// <param name="dp1">Data provider 1</param>
        /// <param name="dp2">Data provider 2</param>
        /// <returns>A <see cref="bool"/> indicating if the data content is identical</returns>
        public static bool CompareDataProviderContent(DataProvider dp1, DataProvider dp2)
        {
            Stream s1 = null;
            Stream s2 = null;
            bool allEq;
            try
            {
                s1 = dp1.OpenInputStream();
                s2 = dp2.OpenInputStream();
                allEq = ((s1.Length - s1.Position) == (s2.Length - s2.Position));
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
        /// If the DataFileDirectory has not been explicitly initialized using the <see cref="DataFileDirectory"/> setter,
        /// retrieving <see cref="DataFileDirectory"/> will assing it the default value "Data"</remarks>

        //{
        //    get
        //    {
        //        if (mDataFileDirectory == null) mDataFileDirectory = "Data";
        //        return mDataFileDirectory;
        //    }
        //set
        //{
        //    if (value == null)
        //    {
        //        throw new exception.MethodParameterIsNullException(
        //            "The DataFileDirectory can not be null");
        //    }
        //    if (mDataFileDirectory != null)
        //    {
        //        throw new exception.IsAlreadyInitializedException(
        //            "The DataProviderManager has already been initialized with a DataFileDirectory");
        //    }
        //    Uri tmp;
        //    if (!Uri.TryCreate(value, UriKind.Relative, out tmp))
        //    {
        //        throw new exception.InvalidUriException(String.Format(
        //                                                    "DataFileDirectory must be a relative Uri, '{0}' is not",
        //                                                    value));
        //    }

        //    if (!Directory.Exists(value))
        //    {
        //        Directory.CreateDirectory(value);
        //    }
        //    mDataFileDirectory = value;
        //}
        //}

        /// <summary>
        /// Moves the data file directory of the manager
        /// </summary>
        /// <param name="newDataFileDir">The new data file direcotry</param>
        /// <param name="deleteSource">A <see cref="bool"/> indicating if the source/old data files shlould be deleted</param>
        /// <param name="overwriteDestDir">A <see cref="bool"/> indicating if the new data directory should be overwritten</param>
        public void MoveDataFiles(string newDataFileDir, bool deleteSource, bool overwriteDestDir)
        {
            if (Path.IsPathRooted(newDataFileDir))
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The data file directory path must be relative");
            }
            string oldPath = DataFileDirectoryFullPath;
            mDataFileDirectory = newDataFileDir;
            string newPath = DataFileDirectoryFullPath;
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

        private void CopyDataFiles(string source, string dest)
        {
            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
            }

            foreach (FileDataProvider fdp in ManagedFileDataProviders)
            {
                string pathSource = Path.Combine(source, fdp.DataFileRelativePath);
                if (!File.Exists(pathSource))
                {
                    throw new exception.DataMissingException(String.Format(
                                                                 "Error while copying data files from {0} to {1}: Data file {2} does not exist in the source",
                                                                 source, dest, fdp.DataFileRelativePath));
                }
                string pathDest = Path.Combine(dest, fdp.DataFileRelativePath);
                if (!File.Exists(pathDest))
                {
                    File.Copy(pathSource, pathDest);
                }
            }
        }

        private string getDataFileDirectoryFullPath(Uri baseUri)
        {
            if (!baseUri.IsFile)
            {
                throw new exception.InvalidUriException(
                    "The base Uri of the presentation to which the DataProviderManager belongs must be a file Uri");
            }

            string localPathPresentation = baseUri.LocalPath;

            if (File.Exists(localPathPresentation))
            {
                localPathPresentation = Path.GetDirectoryName(localPathPresentation);
            }

            string localPath = Path.Combine(localPathPresentation, DataFileDirectory);

            //Uri dataFileDirUri = new Uri(baseUri, DataFileDirectory);
            //string localPath = dataFileDirUri.LocalPath;

            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }

            return localPath;
        }

        // it's only public because XukIn needs it !
        // TODO: several Presentations with the same Data folder will conflict within a single Project !!
        public string DataFileDirectory = "Data";

        /// <summary>
        /// Gets the full path of the data file directory and creates it. 
        /// Convenience for <c>Path.Combine(getBasePath(), getDataFileDirectory())</c>
        /// </summary>
        /// <returns>The full path</returns>
        public string DataFileDirectoryFullPath
        {
            get
            {
                return getDataFileDirectoryFullPath(Presentation.RootUri);
            }
        }

        /*
        /// <summary>
        /// Initializer that sets the path of the data file directory
        /// used by <see cref="FileDataProvider"/>s managed by <c>this</c>
        /// </summary>
        public string DataFileDirectoryPath
        {
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException(
                        "The path of the data file directory can not be null");
                }
                if (mDataFileDirectory != null)
                {
                    throw new exception.IsAlreadyInitializedException(
                        "The data provider manager already has a data file directory");
                }
                if (!Directory.Exists(value))
                {
                    Directory.CreateDirectory(value);
                }
                mDataFileDirectory = value;
            }
        }
         */

        /// <summary>
        /// Gets a new data file path relative to the path of the data file directory of the manager
        /// </summary>
        /// <param name="extension">The entension of the new data file path</param>
        /// <returns>The relative path</returns>
        public string GetNewDataFileRelPath(string extension)
        {
            string res;
            while (true)
            {
                res = Path.ChangeExtension(Path.GetRandomFileName(), extension);
                foreach (FileDataProvider prov in ManagedFileDataProviders)
                {
                    if (!prov.IsDataFileInitialized) continue;
                    if (res.ToLower() == prov.DataFileRelativePath.ToLower()) continue;
                }
                break;
            }

            return res;
        }

        /// <summary>
        /// Gets a list of the <see cref="FileDataProvider"/>s managed by the manager
        /// </summary>
        /// <returns>The list of file data providers</returns>
        public IEnumerable<FileDataProvider> ManagedFileDataProviders
        {
            get
            {
                //List<FileDataProvider> res = new List<FileDataProvider>();
                foreach (DataProvider prov in ManagedObjects.ContentsAs_YieldEnumerable)
                {
                    if (prov is FileDataProvider)
                    {
                        //res.Add((FileDataProvider)prov);
                        yield return (FileDataProvider)prov;
                    }
                }
                yield break;
                //return res;
            }
        }

        #region IDataProviderManager Members

        private void Presentation_rootUriChanged(Object o, events.presentation.RootUriChangedEventArgs e)
        {
            if (e.PreviousUri != null)
            {
                string prevDataDirFullPath = getDataFileDirectoryFullPath(e.PreviousUri);
                if (Directory.Exists(prevDataDirFullPath))
                {
                    CopyDataFiles(prevDataDirFullPath, DataFileDirectoryFullPath);
                }
            }
        }

        /// <summary>
        /// Detaches one of the <see cref="DataProvider"/>s managed by the manager
        /// </summary>
        /// <param name="provider">The <see cref="DataProvider"/> to delete</param>
        /// <param name="delete">A <see cref="bool"/> indicating if the removed data provider should be deleted</param>
        public void RemoveDataProvider(DataProvider provider, bool delete)
        {
            if (provider == null)
            {
                throw new exception.MethodParameterIsNullException("Can not detach a null DataProvider from the manager");
            }
            if (delete)
            {
                provider.Delete();
            }
            else
            {
                //string uid = GetUidOfManagedObject(provider);
                //RemoveManagedObject(uid);

                RemoveManagedObject(provider);
            }
        }

        private bool m_CompareByteStreamsDuringValueEqual = true;

        public bool CompareByteStreamsDuringValueEqual
        {
            get { return m_CompareByteStreamsDuringValueEqual; }
            set { m_CompareByteStreamsDuringValueEqual = value; }
        }

        /// <summary>
        /// Remove all <see cref="DataProvider"/> that are managed by the manager
        /// </summary>
        /// <param name="delete">A <see cref="bool"/> indicating if the removed data providers should be deleted</param>
        public void RemoveUnusedDataProviders(List<DataProvider> usedDataProviders, bool delete)
        {
            foreach (DataProvider prov in ManagedObjects.ContentsAs_YieldEnumerable)
            {
                if (!usedDataProviders.Contains(prov))
                {
                    RemoveDataProvider(prov, delete);
                }
            }
        }

        #endregion

        #region IXukAble Members

        /// <summary>
        /// Clears the <see cref="DataProviderManager"/>, clearing any links to <see cref="DataProvider"/>s
        /// </summary>
        protected override void Clear()
        {
            foreach (DataProvider dp in ManagedObjects.ContentsAs_ListCopy)
            {
                ManagedObjects.Remove(dp);
            }
            mDataFileDirectory = null;
            mXukedInFilDataProviderPaths.Clear();
            base.Clear();
        }


        /// <summary>
        /// Reads the attributes of a DataProviderManager xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

            string dataFileDirectoryPath = source.GetAttribute(XukStrings.DataFileDirectoryPath);
            if (string.IsNullOrEmpty(dataFileDirectoryPath))
            {
                throw new exception.XukException(
                    "dataFileDirectoryPath attribute is missing from DataProviderManager element");
            }
            DataFileDirectory = dataFileDirectoryPath;
        }

        /// <summary>
        /// Reads a child of a DataProviderManager xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukInChild(XmlReader source, ProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == XukNamespaceUri)
            {
                readItem = true;
                if (source.LocalName == XukStrings.DataProviders)
                {
                    XukInDataProviders(source, handler);
                }
                else if (true || !Presentation.Project.IsPrettyFormat()
                    //&& source.LocalName == XukStrings.DataProviderItem
                    )
                {
                    //XukInDataProviderItem(source, handler);
                    XukInDataProvider(source, handler);
                }
                else
                {
                    readItem = false;
                }
            }
            if (!(readItem || source.IsEmptyElement))
            {
                source.ReadSubtree().Close(); //Read past invalid MediaDataItem element
            }
        }

        private void XukInDataProviders(XmlReader source, ProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        if (source.LocalName == XukStrings.DataProviderItem && source.NamespaceURI == XukNamespaceUri)
                        {
                            XukInDataProviderItem(source, handler);
                        }
                        else
                        {
                            XukInDataProvider(source, handler);
                        }

                        //else if (!source.IsEmptyElement)
                        //{
                        //    source.ReadSubtree().Close();
                        //}
                    }
                    else if (source.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }
                    if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
                }
            }
        }

        private void XukInDataProvider(XmlReader source, ProgressHandler handler)
        {
            if (source.NodeType == XmlNodeType.Element)
            {
                DataProvider prov = Presentation.DataProviderFactory.Create("", source.LocalName, source.NamespaceURI);
                if (prov != null)
                {
                    prov.XukIn(source, handler);

                    //string uid = source.GetAttribute(XukStrings.Uid);
                    if (string.IsNullOrEmpty(prov.Uid))
                    {
                        throw new exception.XukException("uid attribute of mDataProviderItem element is missing");
                    }
                    if (prov is FileDataProvider)
                    {
                        FileDataProvider fdProv = (FileDataProvider)prov;
                        if (mXukedInFilDataProviderPaths.Contains(fdProv.DataFileRelativePath.ToLower()))
                        {
                            throw new exception.XukException(String.Format(
                                                                 "Another FileDataProvider using data file {0} has already been Xukked in",
                                                                 fdProv.DataFileRelativePath.ToLower()));
                        }
                        mXukedInFilDataProviderPaths.Add(fdProv.DataFileRelativePath.ToLower());
                    }

                    if (IsManagerOf(prov.Uid))
                    {
                        if (GetManagedObject(prov.Uid) != prov)
                        {
                            throw new exception.XukException(
                                String.Format("Another DataProvider exists in the manager with uid {0}", prov.Uid));
                        }
                    }
                    else
                    {
                        SetUidOfManagedObject(prov, prov.Uid);
                    }
                }
                else if (!source.IsEmptyElement)
                {
                    source.ReadSubtree().Close();
                }
            }
        }

        private void XukInDataProviderItem(XmlReader source, ProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                string uid = source.GetAttribute(XukStrings.Uid);

                bool addedProvider = false;
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        DataProvider prov = Presentation.DataProviderFactory.Create("", source.LocalName, source.NamespaceURI);
                        if (prov != null)
                        {
                            if (addedProvider)
                            {
                                throw new exception.XukException(
                                    "Multiple DataProviders within the same mDataProviderItem is not supported");
                            }

                            string uid_ = source.GetAttribute(XukStrings.Uid);

                            prov.XukIn(source, handler);
                            if (prov is FileDataProvider)
                            {
                                FileDataProvider fdProv = (FileDataProvider)prov;
                                if (mXukedInFilDataProviderPaths.Contains(fdProv.DataFileRelativePath.ToLower()))
                                {
                                    throw new exception.XukException(String.Format(
                                                                         "Another FileDataProvider using data file {0} has already been Xukked in",
                                                                         fdProv.DataFileRelativePath.ToLower()));
                                }
                                mXukedInFilDataProviderPaths.Add(fdProv.DataFileRelativePath.ToLower());
                            }

                            if (string.IsNullOrEmpty(uid_) && !string.IsNullOrEmpty(uid))
                            {
                                prov.Uid = uid;
                            }


                            if (IsManagerOf(prov.Uid))
                            {
                                if (GetManagedObject(prov.Uid) != prov)
                                {
                                    throw new exception.XukException(
                                        String.Format("Another DataProvider exists in the manager with uid {0}", prov.Uid));
                                }
                            }
                            else
                            {
                                SetUidOfManagedObject(prov, prov.Uid);
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
        /// Writes the attributes of a DataProviderManager element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

            Uri presBaseUri = Presentation.RootUri;
            Uri dfdUri = new Uri(presBaseUri, DataFileDirectory);
            destination.WriteAttributeString(XukStrings.DataFileDirectoryPath, presBaseUri.MakeRelativeUri(dfdUri).ToString());

        }

        /// <summary>
        /// Write the child elements of a DataProviderManager element.
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, ProgressHandler handler)
        {
            if (Presentation.Project.IsPrettyFormat())
            {
                destination.WriteStartElement(XukStrings.DataProviders, XukNamespaceUri);
            }
            foreach (DataProvider prov in ManagedObjects.ContentsAs_YieldEnumerable)
            {
                if (false && Presentation.Project.IsPrettyFormat())
                {
                    destination.WriteStartElement(XukStrings.DataProviderItem, XukNamespaceUri);
                    //destination.WriteAttributeString(XukStrings.Uid, prov.Uid);
                }

                prov.XukOut(destination, baseUri, handler);

                if (false && Presentation.Project.IsPrettyFormat())
                {
                    destination.WriteEndElement();
                }
            }
            if (Presentation.Project.IsPrettyFormat())
            {
                destination.WriteEndElement();
            }
            base.XukOutChildren(destination, baseUri, handler);
        }

        #endregion

        #region IValueEquatable<IDataProviderManager> Members

        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }
            DataProviderManager otherManager = other as DataProviderManager;

            if (otherManager == null)
            {
                return false;
            }

            if (otherManager.DataFileDirectory != DataFileDirectory) return false;

            return true;
        }

        #endregion

        public override bool CanAddManagedObject(DataProvider managedObject)
        {
            return true;
        }
    }
}