using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using urakawa.xuk;
using urakawa.data;
using urakawa.progress;

#if USE_ISOLATED_STORAGE
using System.IO.IsolatedStorage;
#endif //USE_ISOLATED_STORAGE

namespace urakawa.ExternalFiles
{
    [XukNameUglyPrettyAttribute("ExFlDtMan", "ExternalFileDataManager")]
    public sealed class ExternalFilesDataManager : XukAbleManager<ExternalFileData>
    {
        public static readonly string STORAGE_FOLDER_NAME = "DAISY-Storage";
        public static readonly string STORAGE_FOLDER_PATH;

        static ExternalFilesDataManager()
        {
#if USE_ISOLATED_STORAGE
                    using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        store.CreateDirectory(dirName);

                        FieldInfo fi = store.GetType().GetField("m_RootDir", BindingFlags.NonPublic | BindingFlags.Instance);

                        STORAGE_FOLDER_PATH = (string)fi.GetValue(store)
                            + Path.DirectorySeparatorChar
                            + STORAGE_FOLDER_NAME;
                    }
#else
            STORAGE_FOLDER_PATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + STORAGE_FOLDER_NAME;

            if (!Directory.Exists(STORAGE_FOLDER_PATH))
            {
                FileDataProvider.CreateDirectory(STORAGE_FOLDER_PATH);
            }
#endif //USE_ISOLATED_STORAGE
        }


        public ExternalFilesDataManager(Presentation pres)
            : base(pres, "EF")
        {
        }
        
        public override bool CanAddManagedObject(ExternalFileData fileDataObject)
        {
            return true;
        }




        /// <summary>
        /// Creates a copy of a given ExternalFileData
        /// </summary>
        /// <param name="data">The ExternalFileData to copy</param>
        /// <returns>The copy</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="data"/> is <c>null</c>
        /// </exception>
        /// <exception cref="exception.IsNotManagerOfException">
        /// Thrown when <paramref name="data"/> is not managed by <c>this</c>
        /// </exception>
        public ExternalFileData CopyExternalFileData(ExternalFileData data)
        {
            if (data == null)
            {
                throw new exception.MethodParameterIsNullException("Can not copy a null ExternalFileData");
            }
            if (data.Presentation.ExternalFilesDataManager != this)
            {
                throw new exception.IsNotManagerOfException(
                    "Can not copy ExternalFileData that is not managed by this");
            }
            return data.Copy();
        }



        /// <summary>
        /// Creates a copy of the ExternalFileData with a given UID
        /// </summary>
        /// <param name="uid">The given UID</param>
        /// <returns>The copy</returns>
        /// <exception cref="exception.IsNotManagerOfException">
        /// Thrown when <c>this</c> does not manage a ExternalFileData data with the given UID
        /// </exception>
        public ExternalFileData CopyExternalFileData(string uid)
        {
            //if (!IsManagerOf(uid))
            //{
            //    throw new exception.IsNotManagerOfException(String.Format(
            //                                                    "The ExternalFileData manager does not manage a ExternalFileData with UID {0}",
            //                                                    uid));
            //}
            ExternalFileData data = GetManagedObject(uid);
            return CopyExternalFileData(data);
        }



        /// <summary>
        /// Clears the <see cref="ExternalFileDataDataManager"/> disassociating any linked <see cref="ExternalFileData"/>
        /// </summary>
        protected override void Clear()
        {
            foreach (ExternalFileData EFd in ManagedObjects.ContentsAs_ListCopy)
            {
                ManagedObjects.Remove(EFd);
            }
            base.Clear();
        }

        public List<DataProvider> UsedDataProviders
        {
            get
            {
                List<DataProvider> usedDataProviders = new List<DataProvider>();
                foreach (ExternalFileData eFD in ManagedObjects.ContentsAs_Enumerable)
                {
                    foreach (DataProvider prov in eFD.UsedDataProviders)
                    {
                        if (!usedDataProviders.Contains(prov))
                        {
                            usedDataProviders.Add(prov);
                        }
                    }
                }
                return usedDataProviders;
            }
        }


        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }
            ExternalFilesDataManager otherManager = other as ExternalFilesDataManager;

            if (otherManager == null)
            {
                return false;
            }


            return true;
        }

        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

        }

        /// <summary>
        /// Write the child elements of a ExternalFileDataManager element.
        /// Mode specifically the <see cref="ExternalFileData"/> of <c>this</c> is written to a ExternalFileData element
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
                destination.WriteStartElement(XukStrings.ExternalFileDatas, XukAble.XUK_NS);
            }

            foreach (ExternalFileData exfd in ManagedObjects.ContentsAs_Enumerable)
            {
                if (false && Presentation.Project.PrettyFormat)
                {
                    destination.WriteStartElement(XukStrings.ExternalFileDataItem, XukAble.XUK_NS);
                }

                exfd.XukOut(destination, baseUri, handler);

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

        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

        }



        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == XukAble.XUK_NS)
            {
                readItem = true;

                if (source.LocalName == XukStrings.ExternalFileDatas)
                {
                    XukInExternalFileDatas(source, handler);
                }
                else if (true || !Presentation.Project.PrettyFormat)
                {
                    XukInExternalFileData(source, handler);
                }
                else
                {
                    readItem = false;
                }
            }
            if (!(readItem || source.IsEmptyElement))
            {
                source.ReadSubtree().Close();
            }
        }


        private void XukInExternalFileData(XmlReader source, IProgressHandler handler)
        {
            if (source.NodeType == XmlNodeType.Element)
            {
                ExternalFileData data = null;
                
                data = Presentation.ExternalFilesDataFactory.Create_SkipManagerInitialization(source.LocalName, source.NamespaceURI);
                if (data != null)
                {
                    data.XukIn(source, handler);

                    if (string.IsNullOrEmpty(data.Uid))
                    {
                        throw new exception.XukException(
                            "uid attribute is missing from mExternalFileDataItem attribute");
                    }

                    Presentation.ExternalFilesDataManager.AddManagedObject_NoSafetyChecks(data, data.Uid);
                            
                    //if (IsManagerOf(data.Uid))
                    //{
                    //    if (GetManagedObject(data.Uid) != data)
                    //    {
                    //        throw new exception.XukException(
                    //            String.Format("Another ExternalFileData exists in the manager with uid {0}", data.Uid));
                    //    }
                    //}
                    //else
                    //{
                    //    SetUidOfManagedObject(data, data.Uid);
                    //}
                }
                else if (!source.IsEmptyElement)
                {
                    source.ReadSubtree().Close();
                }
            }
        }



        private void XukInExternalFileDatas(XmlReader source, IProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        if (source.LocalName == XukStrings.ExternalFileDataItem && source.NamespaceURI == XukAble.XUK_NS)
                        {
                            XukInExternalFileDataItem(source, handler);
                        }
                        else
                        {
                            XukInExternalFileData(source, handler);
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


        private void XukInExternalFileDataItem(XmlReader source, IProgressHandler handler)
        {
            ExternalFileData data = null;
            if (!source.IsEmptyElement)
            {
                string uid = XukAble.ReadXukAttribute(source, XukAble.Uid_NAME);

                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        data = Presentation.ExternalFilesDataFactory.Create_SkipManagerInitialization(source.LocalName, source.NamespaceURI);
                        if (data != null)
                        {
                            string uid_ = XukAble.ReadXukAttribute(source, XukAble.Uid_NAME);

                            data.XukIn(source, handler);

                            if (string.IsNullOrEmpty(uid_) && !string.IsNullOrEmpty(uid))
                            {
                                data.Uid = uid;
                            }

                            if (string.IsNullOrEmpty(data.Uid))
                            {
                                throw new exception.XukException(
                                    "uid attribute is missing from mExternalFileDataItem attribute");
                            }

                            Presentation.ExternalFilesDataManager.AddManagedObject_NoSafetyChecks(data, data.Uid);
                            
                            //if (IsManagerOf(data.Uid))
                            //{
                            //    if (GetManagedObject(data.Uid) != data)
                            //    {
                            //        throw new exception.XukException(
                            //            String.Format("Another ExternalFileData exists in the manager with uid {0}", data.Uid));
                            //    }
                            //}
                            //else
                            //{
                            //    SetUidOfManagedObject(data, data.Uid);
                            //}
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




    }
}
