using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using urakawa.progress;
using urakawa.xuk;

namespace urakawa.data
{
    [XukNameUglyPrettyAttribute("dtProvMan", "DataProviderManager")]
    public sealed class DataProviderManager : XukAbleManager<DataProvider>
    {
        public DataProviderManager(Presentation pres)
            : base(pres, "DP")
        {
            mDataFileDirectory = null;
            m_CompareByteStreamsDuringValueEqual = true;
        }

        //public void AllowCopyDataOnUriChanged(bool enable)
        //{
        //    if (enable)
        //    {
        //        Presentation.RootUriChanged += Presentation_rootUriChanged;
        //    }
        //    else
        //    {
        //        Presentation.RootUriChanged -= Presentation_rootUriChanged;
        //    }
        //}

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

        ////{
        ////    get
        ////    {
        ////        if (mDataFileDirectory == null) mDataFileDirectory = "Data";
        ////        return mDataFileDirectory;
        ////    }
        ////set
        ////{
        ////    if (value == null)
        ////    {
        ////        throw new exception.MethodParameterIsNullException(
        ////            "The DataFileDirectory can not be null");
        ////    }
        ////    if (mDataFileDirectory != null)
        ////    {
        ////        throw new exception.IsAlreadyInitializedException(
        ////            "The DataProviderManager has already been initialized with a DataFileDirectory");
        ////    }
        ////    Uri tmp;
        ////    if (!Uri.TryCreate(value, UriKind.Relative, out tmp))
        ////    {
        ////        throw new exception.InvalidUriException(String.Format(
        ////                                                    "DataFileDirectory must be a relative Uri, '{0}' is not",
        ////                                                    value));
        ////    }

        ////    if (!Directory.Exists(value))
        ////    {
        ////        Directory.CreateDirectory(value);
        ////    }
        ////    mDataFileDirectory = value;
        ////}
        ////}

        ///// <summary>
        ///// Moves the data file directory of the manager
        ///// </summary>
        ///// <param name="newDataFileDir">The new data file direcotry</param>
        ///// <param name="deleteSource">A <see cref="bool"/> indicating if the source/old data files shlould be deleted</param>
        ///// <param name="overwriteDestDir">A <see cref="bool"/> indicating if the new data directory should be overwritten</param>
        //public void MoveDataFiles(string newDataFileDir, bool deleteSource, bool overwriteDestDir)
        //{
        //    if (Path.IsPathRooted(newDataFileDir))
        //    {
        //        throw new exception.MethodParameterIsOutOfBoundsException(
        //            "The data file directory path must be relative");
        //    }
        //    string oldPath = DataFileDirectoryFullPath;
        //    mDataFileDirectory = newDataFileDir;
        //    string newPath = DataFileDirectoryFullPath;
        //    try
        //    {
        //        if (Directory.Exists(newPath))
        //        {
        //            if (overwriteDestDir)
        //            {
        //                Directory.Delete(newPath);
        //            }
        //            else
        //            {
        //                throw new exception.OperationNotValidException(
        //                    String.Format("Directory {0} already exists", newPath));
        //            }
        //        }
        //        CopyDataFiles(oldPath, newPath);
        //        if (deleteSource && Directory.Exists(oldPath))
        //        {
        //            Directory.Delete(oldPath);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new exception.OperationNotValidException(
        //            String.Format("Could not move data files to {0}: {1}", newPath, e.Message),
        //            e);
        //    }
        //}

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
                FileDataProvider.CreateDirectory(localPath);
            }

            return localPath;
        }

        // legacy for Obi?
        public void SetDataFileDirectoryWithPrefix(string dataFolderPrefix)
        {
            if (string.IsNullOrEmpty(dataFolderPrefix))
            {
                DataFileDirectory = DefaultDataFileDirectory;
            }
            else
            {
                DataFileDirectory = dataFolderPrefix + DefaultDataFileDirectorySeparator + DefaultDataFileDirectory;
            }
        }

        // Tobi doesn't use "_Data" anymore
        public void SetCustomDataFileDirectory(string dataFolderCustomName)
        {
            if (string.IsNullOrEmpty(dataFolderCustomName))
            {
                DataFileDirectory = DefaultDataFileDirectory;
            }
            else
            {
                DataFileDirectory = dataFolderCustomName; // + DefaultDataFileDirectorySeparator + DefaultDataFileDirectory;
            }
        }

        public const string DefaultDataFileDirectory = "Data";
        public const string DefaultDataFileDirectorySeparator = "_";

        // public read/write visibility => HACKY :(
        public string DataFileDirectory = DefaultDataFileDirectory;

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
        public string GetNewDataFileRelPath(string extension, string namePrefix)
        {
            string res;
            while (true)
            {
                res = Path.ChangeExtension((String.IsNullOrEmpty(namePrefix) ? "" : namePrefix) + Path.GetRandomFileName(), extension);

                string fullPath = Path.Combine(DataFileDirectoryFullPath, res);
                if (File.Exists(fullPath)) continue;

#if DEBUG
                foreach (FileDataProvider prov in ManagedFileDataProviders)
                {
                    if (!prov.IsDataFileInitialized) continue;
                    if (prov.DataFileRelativePath.Equals(res, StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.Fail("This situation should have been caught by the File.EXist() test above !");
                        continue;
                    }
                }
#endif

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
                foreach (DataProvider prov in ManagedObjects.ContentsAs_Enumerable)
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

        //private void Presentation_rootUriChanged(Object o, events.presentation.RootUriChangedEventArgs e)
        //{
        //    if (e.PreviousUri != null)
        //    {
        //        string prevDataDirFullPath = getDataFileDirectoryFullPath(e.PreviousUri);
        //        if (Directory.Exists(prevDataDirFullPath))
        //        {
        //            CopyDataFiles(prevDataDirFullPath, DataFileDirectoryFullPath);
        //        }
        //    }
        //}

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

        ///// <summary>
        ///// Remove all <see cref="DataProvider"/> that are managed by the manager
        ///// </summary>
        ///// <param name="delete">A <see cref="bool"/> indicating if the removed data providers should be deleted</param>
        //public void RemoveUnusedDataProviders(List<DataProvider> usedDataProviders, bool delete)
        //{
        //    foreach (DataProvider prov in ManagedObjects.ContentsAs_YieldEnumerable)
        //    {
        //        if (!usedDataProviders.Contains(prov))
        //        {
        //            RemoveDataProvider(prov, delete);
        //        }
        //    }
        //}

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

            DataFileDirectory = FileDataProvider.UriDecode(dataFileDirectoryPath);
        }

        /// <summary>
        /// Reads a child of a DataProviderManager xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == XukAble.XUK_NS)
            {
                readItem = true;
                if (source.LocalName == XukStrings.DataProviders)
                {
                    XukInDataProviders(source, handler);
                }
                else if (true || !Presentation.Project.PrettyFormat
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

        private void XukInDataProviders(XmlReader source, IProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        if (source.LocalName == XukStrings.DataProviderItem && source.NamespaceURI == XukAble.XUK_NS)
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

        private void XukInDataProvider(XmlReader source, IProgressHandler handler)
        {
            if (source.NodeType == XmlNodeType.Element)
            {
                DataProvider prov = Presentation.DataProviderFactory.Create_SkipManagerInitialization("", source.LocalName, source.NamespaceURI);
                if (prov != null)
                {
                    prov.XukIn(source, handler);

                    //if (prov is FileDataProvider
                    //    && !File.Exists(((FileDataProvider)prov).DataFileFullPath))
                    //{
                    // SEE BELOW
                    //}

                    //string uid = source.GetAttribute(XukStrings.Uid);
                    if (string.IsNullOrEmpty(prov.Uid))
                    {
                        throw new exception.XukException("uid attribute of mDataProviderItem element is missing");
                    }

                    Presentation.DataProviderManager.AddManagedObject_NoSafetyChecks(prov, prov.Uid);
                    //if (IsManagerOf(prov.Uid))
                    //{
                    //    if (GetManagedObject(prov.Uid) != prov)
                    //    {
                    //        throw new exception.XukException(
                    //            String.Format("Another DataProvider exists in the manager with uid {0}", prov.Uid));
                    //    }
                    //}
                    //else
                    //{
                    //    SetUidOfManagedObject(prov, prov.Uid);
                    //}

                    if (prov is FileDataProvider)
                    {
                        FileDataProvider fdProv = (FileDataProvider)prov;

                        foreach (string path in mXukedInFilDataProviderPaths)
                        {
                            if (path.Equals(fdProv.DataFileRelativePath, StringComparison.OrdinalIgnoreCase))
                            {
                                throw new exception.XukException(String.Format(
                                                                     "Another FileDataProvider using data file {0} has already been Xukked in",
                                                                     fdProv.DataFileRelativePath));
                            }
                        }


                        if (!File.Exists(fdProv.DataFileFullPath))
                        {
                            Presentation.DataProviderManager.RemoveManagedObject(prov);
                            return;
                        }
                        mXukedInFilDataProviderPaths.Add(fdProv.DataFileRelativePath);
                    }
                }
                else if (!source.IsEmptyElement)
                {
                    source.ReadSubtree().Close();
                }
            }
        }

        private void XukInDataProviderItem(XmlReader source, IProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                string uid = XukAble.ReadXukAttribute(source, XukAble.Uid_NAME);

                bool addedProvider = false;
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        DataProvider prov = Presentation.DataProviderFactory.Create_SkipManagerInitialization("", source.LocalName, source.NamespaceURI);
                        if (prov != null)
                        {
                            if (addedProvider)
                            {
                                throw new exception.XukException(
                                    "Multiple DataProviders within the same mDataProviderItem is not supported");
                            }

                            prov.XukIn(source, handler);

                            //string uid_ = source.GetAttribute(XukStrings.Uid);
                            if (string.IsNullOrEmpty(prov.Uid) && !string.IsNullOrEmpty(uid))
                            {
                                prov.Uid = uid;
                            }

                            Presentation.DataProviderManager.AddManagedObject_NoSafetyChecks(prov, prov.Uid);

                            //if (IsManagerOf(prov.Uid))
                            //{
                            //    if (GetManagedObject(prov.Uid) != prov)
                            //    {
                            //        throw new exception.XukException(
                            //            String.Format("Another DataProvider exists in the manager with uid {0}", prov.Uid));
                            //    }
                            //}
                            //else
                            //{
                            //    SetUidOfManagedObject(prov, prov.Uid);
                            //}
                            if (prov is FileDataProvider)
                            {
                                FileDataProvider fdProv = (FileDataProvider)prov;

                                foreach (string path in mXukedInFilDataProviderPaths)
                                {
                                    if (path.Equals(fdProv.DataFileRelativePath, StringComparison.OrdinalIgnoreCase))
                                    {
                                        throw new exception.XukException(String.Format(
                                                                         "Another FileDataProvider using data file {0} has already been Xukked in",
                                                                         fdProv.DataFileRelativePath));
                                    }
                                }

                                if (!File.Exists(fdProv.DataFileFullPath))
                                {
                                    Presentation.DataProviderManager.RemoveManagedObject(fdProv);
                                    return;
                                }
                                mXukedInFilDataProviderPaths.Add(fdProv.DataFileRelativePath);
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
            string str = presBaseUri.MakeRelativeUri(dfdUri).ToString();
            destination.WriteAttributeString(XukStrings.DataFileDirectoryPath, str);
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
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            if (Presentation.Project.PrettyFormat)
            {
                destination.WriteStartElement(XukStrings.DataProviders, XukAble.XUK_NS);
            }
            foreach (DataProvider prov in ManagedObjects.ContentsAs_Enumerable)
            {
                if (false && Presentation.Project.PrettyFormat)
                {
                    destination.WriteStartElement(XukStrings.DataProviderItem, XukAble.XUK_NS);
                    //destination.WriteAttributeString(XukStrings.Uid, prov.Uid);
                }

                prov.XukOut(destination, baseUri, handler);

                if (false && Presentation.Project.PrettyFormat)
                {
                    destination.WriteEndElement();
                }
            }
            if (Presentation.Project.PrettyFormat)
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

            //if (otherManager.DataFileDirectory != DataFileDirectory) return false;

            return true;
        }

        #endregion

        public override bool CanAddManagedObject(DataProvider managedObject)
        {
            return true;
        }
    }
}