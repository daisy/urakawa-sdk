using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using AudioLib;
using urakawa.ExternalFiles;
using urakawa.core;
using urakawa.daisy.export.visitor;
using urakawa.daisy.import;
using urakawa.data;
using urakawa.media;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.media.data.image;
using urakawa.media.data.video;
using urakawa.media.timing;
using urakawa.metadata;
using urakawa.metadata.daisy;
using urakawa.property.channel;
using urakawa.property.xml;
using urakawa.xuk;
using XmlAttribute = urakawa.property.xml.XmlAttribute;

#if ENABLE_SHARPZIP
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
#else
using Jaime.Olivares;
#endif

namespace urakawa.daisy.export
{
    public partial class Epub3_Export : DualCancellableProgressReporter
    {
        protected readonly bool m_includeImageDescriptions;
        protected readonly bool m_encodeToMp3;
        protected readonly SampleRate m_sampleRate;
        protected readonly bool m_audioStereo;
        protected readonly bool m_SkipACM;
        protected readonly ushort m_BitRate_Mp3 = 64;
        protected Presentation m_Presentation;
        protected string m_UnzippedOutputDirectory;

        protected string m_exportSpineItemProjectPath;
        protected string m_exportSpineItemPath;

        private string m_XukPath;

        private bool isXukSpine
        {
            get
            {
                return @"spine".Equals(m_Presentation.RootNode.GetXmlElementLocalName(),
                                       StringComparison.OrdinalIgnoreCase);
            }
        }

        public Epub3_Export(string xukPath,
            Presentation presentation,
            string exportDirectory,
            bool encodeToMp3, ushort bitRate_Mp3,
            SampleRate sampleRate, bool stereo,
            bool skipACM,
            bool includeImageDescriptions,
            string exportSpineItemProjectPath)
        {
            //m_Daisy3_Export = new Daisy3_Export(presentation, exportDirectory, null, encodeToMp3, bitRate_Mp3, sampleRate, stereo, skipACM, includeImageDescriptions);
            //AddSubCancellable(m_Daisy3_Export);

            RequestCancellation = false;

            m_XukPath = xukPath;

            m_includeImageDescriptions = includeImageDescriptions;
            m_encodeToMp3 = encodeToMp3;
            m_sampleRate = sampleRate;
            m_audioStereo = stereo;
            m_SkipACM = skipACM;
            m_BitRate_Mp3 = bitRate_Mp3;
            m_Presentation = presentation;

            m_UnzippedOutputDirectory = Path.Combine(exportDirectory, @"_UNZIPPED");
            if (!Directory.Exists(m_UnzippedOutputDirectory))
            {
                FileDataProvider.CreateDirectory(m_UnzippedOutputDirectory);
            }

            if (!string.IsNullOrEmpty(exportSpineItemProjectPath))
            {
                m_exportSpineItemProjectPath = FileDataProvider.NormaliseFullFilePath(exportSpineItemProjectPath).Replace('/', '\\');
            }
        }

        protected void processSingleSpineItem_1(XmlDocument opfXmlDoc, XmlNode opfXmlNode_spine, XmlNode opfXmlNode_manifest, string path, out XmlNode opfXmlNode_itemRef, out XmlNode opfXmlNode_item, out string uid_OPF_SpineItem)
        {
            opfXmlNode_itemRef = opfXmlDoc.CreateElement(null,
                "itemref",
                DiagramContentModelHelper.NS_URL_EPUB_PACKAGE);
            opfXmlNode_spine.AppendChild(opfXmlNode_itemRef);


            opfXmlNode_item = opfXmlDoc.CreateElement(null,
                "item",
                DiagramContentModelHelper.NS_URL_EPUB_PACKAGE);
            opfXmlNode_manifest.AppendChild(opfXmlNode_item);


            uid_OPF_SpineItem = GetNextID(ID_SpinePrefix);
            XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_itemRef, @"idref", uid_OPF_SpineItem);
            XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_item, @"id", uid_OPF_SpineItem);

            string ext = Path.GetExtension(path);

            string type = DataProviderFactory.GetMimeTypeFromExtension(ext);
            XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_item, @"media-type", type);

            XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_item, @"href", path);
        }


        protected bool processSingleSpineItem_2(XmlDocument opfXmlDoc, XmlNode opfXmlNode_spine, XmlNode opfXmlNode_manifest, string path, XmlNode opfXmlNode_item, XmlNode opfXmlNode_metadata, string uid_OPF_SpineItem, Presentation spineItemPresentation, string opsDirectoryPath, string fullSpineItemPath, Time timeTotal)
        {
            Time time = spineItemPresentation.RootNode.GetDurationOfManagedAudioMediaFlattened();
            if (time != null)
            {
                timeTotal.Add(time);

                string uid_OPF_SpineItemMO = uid_OPF_SpineItem + @"_mo";
                XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_item, @"media-overlay", uid_OPF_SpineItemMO);


                opfXmlNode_item = opfXmlDoc.CreateElement(null,
                    "item",
                    DiagramContentModelHelper.NS_URL_EPUB_PACKAGE);
                opfXmlNode_manifest.AppendChild(opfXmlNode_item);

                XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_item, @"id", uid_OPF_SpineItemMO);

                string type = DataProviderFactory.SMIL_MIME_TYPE; // GetMimeTypeFromExtension(Path.GetExtension(@".smil"));
                XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_item, @"media-type", type);

                string ext = Path.GetExtension(path);
                string smilPath = path.Replace(ext, ".smil");
                XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_item, @"href", smilPath);

                string fullSmilPath = Path.Combine(opsDirectoryPath, smilPath);
                fullSmilPath = FileDataProvider.NormaliseFullFilePath(fullSmilPath).Replace('/', '\\');


                XmlNode opfXmlNode_metaItemDur = opfXmlDoc.CreateElement(null,
                    @"meta",
                    DiagramContentModelHelper.NS_URL_EPUB_PACKAGE);
                opfXmlNode_metadata.AppendChild(opfXmlNode_metaItemDur);

                XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_metaItemDur, @"property", "media:duration");
                XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_metaItemDur, @"refines", "#" + uid_OPF_SpineItemMO);

                XmlNode timeNodeItemDur = opfXmlDoc.CreateTextNode(time.ToString());
                opfXmlNode_metaItemDur.AppendChild(timeNodeItemDur);
#if DEBUG
                string parentdirSmil = Path.GetDirectoryName(fullSmilPath);
                if (!Directory.Exists(parentdirSmil))
                {
                    FileDataProvider.CreateDirectory(parentdirSmil);
                }

                StreamWriter spineItemWriterSmil = File.CreateText(fullSmilPath);
                try
                {
                    spineItemWriterSmil.WriteLine(time.ToString());
                }
                finally
                {
                    spineItemWriterSmil.Close();
                }
#endif //DEBUG

                opfXmlNode_item = opfXmlDoc.CreateElement(null,
                    "item",
                    DiagramContentModelHelper.NS_URL_EPUB_PACKAGE);
                opfXmlNode_manifest.AppendChild(opfXmlNode_item);

                type = DataProviderFactory.AUDIO_WAV_MIME_TYPE;
                if (m_encodeToMp3)
                {
                    type = DataProviderFactory.AUDIO_MP3_MIME_TYPE;
                }
                XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_item, @"media-type", type);

                string audioPath = path.Replace(ext, m_encodeToMp3 ? @".mp3" : @".wav");
                XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_item, @"href", audioPath);

                string fullAudioPath = Path.Combine(opsDirectoryPath, audioPath);
                fullAudioPath = FileDataProvider.NormaliseFullFilePath(fullAudioPath).Replace('/', '\\');

                //spineItemPresentation.RootNode.OpenPcmInputStreamOfManagedAudioMediaFlattened()

                string fullAudioPath_ = fullAudioPath.Replace(@".mp3", @".wav").Replace(@".wav",
                                                                @"_"
                    //+ spineItemPresentation.MediaDataManager.DefaultPCMFormat.ToString()
                                                                + spineItemPresentation.MediaDataManager.DefaultPCMFormat.Data.SampleRate
                                                                + "-"
                                                                + (spineItemPresentation.MediaDataManager.DefaultPCMFormat.Data.NumberOfChannels == 1 ? @"mono" : @"stereo")
                                                                    + @".wav");

                string audioFileName = Path.GetFileName(fullAudioPath_);

                decimal bytes = spineItemPresentation.MediaDataManager.DefaultPCMFormat.Data.ConvertTimeToBytes(time.AsLocalUnits) / (decimal)(1024.0 * 1024.0);
                decimal bytes_ = Math.Round((decimal)bytes, 5, MidpointRounding.ToEven);
                string sizeStr = bytes_ + @"MB";
                reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.CreatingAudioFile, audioFileName, sizeStr));

                if (RequestCancellation)
                {
                    return true;
                }

                FileStream audioStream = new FileStream(fullAudioPath_, FileMode.Create, FileAccess.Write, FileShare.None);
                ulong audioStreamRiffOffset = spineItemPresentation.MediaDataManager.DefaultPCMFormat.Data.RiffHeaderWrite(audioStream, 0);

                DebugFix.Assert((long)audioStreamRiffOffset == audioStream.Position);

                uint totalBytesWritten = 0;

                TreeNode node = spineItemPresentation.RootNode;
                Stack<TreeNode> nodeStack = new Stack<TreeNode>();
                nodeStack.Push(node);

                while (nodeStack.Count > 0) //nodeStack.Peek() != null
                {
                    if (RequestCancellation)
                    {
                        audioStream.Close();
                        return true;
                    }

                    node = nodeStack.Pop();

                    ManagedAudioMedia manAudioMedia = node.GetManagedAudioMedia();
                    if (manAudioMedia != null && manAudioMedia.HasActualAudioMediaData)
                    {
                        Stream manAudioStream = manAudioMedia.AudioMediaData.OpenPcmInputStream();
                        try
                        {
                            const uint BUFFER_SIZE = 1024 * 1024 * 3; // 3 MB MAX BUFFER
                            uint bytesWritten = StreamUtils.Copy(manAudioStream, 0, audioStream, BUFFER_SIZE);

                            totalBytesWritten += bytesWritten;
                        }
                        catch
                        {
#if DEBUG
                            Debugger.Break();
#endif
                        }
                        finally
                        {
                            manAudioStream.Close();
                        }
                    }
                    else
                    {
                        foreach (TreeNode child in node.Children.ContentsAs_YieldEnumerableReversed)
                        {
                            nodeStack.Push(child);
                        }
                    }
                }

                DebugFix.Assert((long)totalBytesWritten == audioStream.Position - (long)audioStreamRiffOffset);

                audioStream.Position = 0;
                spineItemPresentation.MediaDataManager.DefaultPCMFormat.Data.RiffHeaderWrite(audioStream, totalBytesWritten);

                audioStream.Close();

                if (RequestCancellation)
                {
                    return true;
                }

                ushort nChannels = (ushort)(m_audioStereo ? 2 : 1);

                AudioLibPCMFormat pcmFormat = new AudioLibPCMFormat();
                pcmFormat.CopyFrom(node.Presentation.MediaDataManager.DefaultPCMFormat.Data);
                pcmFormat.SampleRate = (ushort)m_sampleRate;
                pcmFormat.NumberOfChannels = nChannels;

                if (m_encodeToMp3)
                {
                    WavFormatConverter formatConverter = new WavFormatConverter(true, m_SkipACM);

                    AddSubCancellable(formatConverter);

                    bool result = false;
                    try
                    {
                        result = formatConverter.CompressWavToMp3(fullAudioPath_, fullAudioPath, pcmFormat, m_BitRate_Mp3);
                    }
                    finally
                    {
                        RemoveSubCancellable(formatConverter);
                    }

                    if (result)
                    {
                        //double compressionRatio = (new FileInfo(fullAudioPath_).Length) / (new FileInfo(fullAudioPath).Length);

                        File.Delete(fullAudioPath_);
                    }
#if DEBUG
                    else
                    {
                        Debugger.Break();
                    }
#endif
                }
                else
                {
                    if ((ushort)m_sampleRate != node.Presentation.MediaDataManager.DefaultPCMFormat.Data.SampleRate
                        ||
                        nChannels != node.Presentation.MediaDataManager.DefaultPCMFormat.Data.NumberOfChannels)
                    {
                        WavFormatConverter formatConverter = new WavFormatConverter(true, m_SkipACM);

                        AddSubCancellable(formatConverter);

                        string destinationFilePath = null;
                        try
                        {
                            AudioLibPCMFormat originalPcmFormat;
                            destinationFilePath = formatConverter.ConvertSampleRate(fullAudioPath_, Path.GetDirectoryName(fullAudioPath_), pcmFormat, out originalPcmFormat);
                            if (originalPcmFormat != null)
                            {
                                DebugFix.Assert(node.Presentation.MediaDataManager.DefaultPCMFormat.Data.Equals(originalPcmFormat));
                            }
                        }
                        finally
                        {
                            RemoveSubCancellable(formatConverter);
                        }

                        if (!string.IsNullOrEmpty(destinationFilePath))
                        {
                            File.Delete(fullAudioPath_);
                            File.Move(destinationFilePath, fullAudioPath);
                        }
#if DEBUG
                        else
                        {
                            Debugger.Break();
                        }
#endif
                    }
                    else
                    {
                        File.Move(fullAudioPath_, fullAudioPath);
                    }
                }
            }

#if DEBUG
            string parentdir = Path.GetDirectoryName(fullSpineItemPath);
            if (!Directory.Exists(parentdir))
            {
                FileDataProvider.CreateDirectory(parentdir);
            }

            string body = spineItemPresentation.RootNode.GetXmlFragment(false);
            StreamWriter spineItemWriter = File.CreateText(fullSpineItemPath);
            try
            {
                spineItemWriter.Write(body);
            }
            finally
            {
                spineItemWriter.Close();
            }
#endif //DEBUG

            string fullSpineItemDirectory = Path.GetDirectoryName(fullSpineItemPath);

            foreach (ExternalFiles.ExternalFileData extFileData in spineItemPresentation.ExternalFilesDataManager.ManagedObjects.ContentsAs_Enumerable)
            {
                if (!extFileData.IsPreservedForOutputFile)
                {
                    continue;
                }

                string relativePath = extFileData.OriginalRelativePath;
                if (string.IsNullOrEmpty(relativePath))
                {
#if DEBUG
                    Debugger.Break();
#endif
                }

                string fullPath = Path.Combine(fullSpineItemDirectory, relativePath);
                fullPath = FileDataProvider.NormaliseFullFilePath(fullPath).Replace('/', '\\');

                if (!File.Exists(fullPath))
                {
                    extFileData.DataProvider.ExportDataStreamToFile(fullPath, false);

                    XmlNode opfXmlNode_spineItemExt = opfXmlDoc.CreateElement(null,
                                                                         "item",
                                                                         DiagramContentModelHelper.
                                                                             NS_URL_EPUB_PACKAGE);
                    opfXmlNode_manifest.AppendChild(opfXmlNode_spineItemExt);

                    string spineItemExt = Path.GetExtension(relativePath);

                    string spineItemType = DataProviderFactory.GetMimeTypeFromExtension(spineItemExt);
                    XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_spineItemExt, @"media-type",
                                                               spineItemType);

                    XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_spineItemExt, @"href",
                                                               relativePath);
                }
#if DEBUG
                else
                {
                    bool breakpoint = true;
                }
#endif
            }

            foreach (MediaData mediaData in spineItemPresentation.MediaDataManager.ManagedObjects.ContentsAs_Enumerable)
            {
                ImageMediaData imgMediaData = mediaData as ImageMediaData;
                VideoMediaData vidMediaData = mediaData as VideoMediaData;

                string relativePath = null;
                if (imgMediaData != null)
                {
                    relativePath = imgMediaData.OriginalRelativePath;
                }
                else if (vidMediaData != null)
                {
                    relativePath = vidMediaData.OriginalRelativePath;
                }
                else
                {
                    continue;
                }

                if (string.IsNullOrEmpty(relativePath))
                {
#if DEBUG
                    Debugger.Break();
#endif
                }

                string fullPath = Path.Combine(fullSpineItemDirectory, relativePath);
                fullPath = FileDataProvider.NormaliseFullFilePath(fullPath).Replace('/', '\\');

                if (!File.Exists(fullPath))
                {
                    if (imgMediaData != null)
                    {
                        imgMediaData.DataProvider.ExportDataStreamToFile(fullPath, false);
                    }
                    else if (vidMediaData != null)
                    {
                        vidMediaData.DataProvider.ExportDataStreamToFile(fullPath, false);
                    }
                    else
                    {
#if DEBUG
                        Debugger.Break();
#endif
                    }


                    XmlNode opfXmlNode_spineItemMedia = opfXmlDoc.CreateElement(null,
                                                                     "item",
                                                                     DiagramContentModelHelper.
                                                                         NS_URL_EPUB_PACKAGE);
                    opfXmlNode_manifest.AppendChild(opfXmlNode_spineItemMedia);

                    string spineItemExt = Path.GetExtension(relativePath);

                    string spineItemType = DataProviderFactory.GetMimeTypeFromExtension(spineItemExt);
                    XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_spineItemMedia, @"media-type",
                                                               spineItemType);

                    XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_spineItemMedia, @"href",
                                                               relativePath);
                }
#if DEBUG
                else
                {
                    bool breakpoint = true;
                }
#endif
            }

            return false;
        }


        public override void DoWork()
        {
            RequestCancellation = false;

            if (RequestCancellation) return;

            reportProgress(-1, @"Creating EPUB directory structure..."); //UrakawaSDK_daisy_Lang.BLAbla

            string hasNavDoc = null;
            string hasNCX = null;

            bool hasMetaInfContainerXML = false;

            string metainfDirectoryPath = Path.Combine(m_UnzippedOutputDirectory, "META-INF");
            if (!Directory.Exists(metainfDirectoryPath))
            {
                FileDataProvider.CreateDirectory(metainfDirectoryPath);
            }


            string opsRelativeDirectoryPath = @"OPS"; //OEBPS
            string opfRelativeFilePath = opsRelativeDirectoryPath + @"/content.opf";

            if (isXukSpine)
            {
                XmlAttribute xmlAttr = m_Presentation.RootNode.GetXmlProperty().GetAttribute(Daisy3_Import.OPF_ContainerRelativePath);
                if (xmlAttr != null)
                {
                    opfRelativeFilePath = xmlAttr.Value.Replace('\\', '/');
                    if (opfRelativeFilePath.StartsWith(@"./"))
                    {
                        opfRelativeFilePath = opfRelativeFilePath.Substring(2);
                    }

                    opsRelativeDirectoryPath = opfRelativeFilePath;

                    int index = opsRelativeDirectoryPath.LastIndexOf('/');
                    if (index < 0)
                    {
#if DEBUG
                        Debugger.Break();
#endif
                        opsRelativeDirectoryPath = null;
                    }
                    else
                    {
                        DebugFix.Assert(index > 1);

                        opsRelativeDirectoryPath = opsRelativeDirectoryPath.Substring(0, index);
                    }
                }
            }
            else
            {
#if DEBUG
                Debugger.Break();
#endif
            }

            string opsDirectoryPath = m_UnzippedOutputDirectory;
            if (!string.IsNullOrEmpty(opsRelativeDirectoryPath))
            {
                opsDirectoryPath = Path.Combine(m_UnzippedOutputDirectory, opsRelativeDirectoryPath);
                if (!Directory.Exists(opsDirectoryPath))
                {
                    FileDataProvider.CreateDirectory(opsDirectoryPath);
                }
            }

            string opfFilePath = Path.Combine(m_UnzippedOutputDirectory, opfRelativeFilePath);
            XmlDocument opfXmlDoc = createXmlDocument_OPF();

            XmlNode opfXmlNode_package = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(opfXmlDoc, false, "package", DiagramContentModelHelper.NS_URL_EPUB_PACKAGE);
            XmlNode opfXmlNode_spine = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(opfXmlNode_package, false, "spine", DiagramContentModelHelper.NS_URL_EPUB_PACKAGE);
            XmlNode opfXmlNode_manifest = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(opfXmlNode_package, false, "manifest", DiagramContentModelHelper.NS_URL_EPUB_PACKAGE);
            XmlNode opfXmlNode_metadata = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(opfXmlNode_package, false, "metadata", DiagramContentModelHelper.NS_URL_EPUB_PACKAGE);

            Time timeTotal = new Time();

            if (isXukSpine)
            {
                foreach (XmlAttribute xmlAttribute in m_Presentation.RootNode.GetXmlProperty().Attributes.ContentsAs_Enumerable)
                {
                    string localName = xmlAttribute.LocalName;

                    if (localName == Daisy3_Import.OPF_ContainerRelativePath)
                    {
                        continue;
                    }

                    string value = xmlAttribute.Value;

                    if (localName == @"prefix")
                    {
                        XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_package, @"prefix", value);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(xmlAttribute.Prefix))
                    {
                        string nsUri = xmlAttribute.GetNamespaceUri();

                        DebugFix.Assert(!string.IsNullOrEmpty(nsUri));

                        XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_spine, xmlAttribute.PrefixedLocalName, value, nsUri);
                    }
                    else
                    {
                        XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_spine, localName, value);
                    }
                }

                foreach (Metadata metadata in m_Presentation.Metadatas.ContentsAs_Enumerable)
                {
                    string name = metadata.NameContentAttribute.Name;
                    string value = metadata.NameContentAttribute.Value;
                    string nsUri = metadata.NameContentAttribute.NamespaceUri;

                    if (metadata.IsMarkedAsPrimaryIdentifier)
                    {
                        DebugFix.Assert(SupportedMetadata_Z39862005.DC_Identifier.Equals(name, StringComparison.OrdinalIgnoreCase));

                        if (metadata.OtherAttributes != null)
                        {
                            foreach (MetadataAttribute metadataAttribute in metadata.OtherAttributes.ContentsAs_Enumerable)
                            {
                                if (metadataAttribute.Name.Equals(@"id", StringComparison.OrdinalIgnoreCase))
                                {
                                    //DebugFix.Assert(nsUri == XmlReaderWriterHelper.NS_URL_XML);
                                    //DebugFix.Assert(nsUri == DiagramContentModelHelper.NS_URL_DC);

                                    XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_package, "unique-identifier", metadataAttribute.Value);
                                    break;
                                }
                            }
                        }
                    }

                    XmlNode opfXmlNode_meta = null;

                    if (name == @"link" && value == @"link")
                    {
                        DebugFix.Assert(metadata.OtherAttributes != null);

                        opfXmlNode_meta = opfXmlDoc.CreateElement(null,
                            "link",
                            DiagramContentModelHelper.NS_URL_EPUB_PACKAGE);
                        opfXmlNode_metadata.AppendChild(opfXmlNode_meta);
                    }
                    else if (name.IndexOf(':') > 0)
                    {
                        string prefix;
                        string localName;
                        XmlProperty.SplitLocalName(name, out prefix, out localName);

                        if (prefix == @"dc")
                        {
                            DebugFix.Assert(nsUri == DiagramContentModelHelper.NS_URL_DC);

                            string nsUri_ = opfXmlNode_package.GetNamespaceOfPrefix(@"dc");
                            if (string.IsNullOrEmpty(nsUri_))
                            {
                                XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_package,
                                    XmlReaderWriterHelper.NS_PREFIX_XMLNS + ":dc",
                                    DiagramContentModelHelper.NS_URL_DC, XmlReaderWriterHelper.NS_URL_XMLNS);
                            }

                            opfXmlNode_meta = opfXmlDoc.CreateElement(prefix, localName, nsUri);
                            opfXmlNode_metadata.AppendChild(opfXmlNode_meta);
                        }
                        else
                        {
                            DebugFix.Assert(string.IsNullOrEmpty(nsUri));

                            opfXmlNode_meta = opfXmlDoc.CreateElement(null,
                                @"meta",
                                DiagramContentModelHelper.NS_URL_EPUB_PACKAGE);
                            opfXmlNode_metadata.AppendChild(opfXmlNode_meta);

                            XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_meta, "property", name);
                        }

                        XmlNode contentNode = opfXmlDoc.CreateTextNode(value);
                        opfXmlNode_meta.AppendChild(contentNode);
                    }
                    else
                    {
                        DebugFix.Assert(string.IsNullOrEmpty(nsUri));

                        opfXmlNode_meta = opfXmlDoc.CreateElement(null,
                            "meta",
                            DiagramContentModelHelper.NS_URL_EPUB_PACKAGE);
                        opfXmlNode_metadata.AppendChild(opfXmlNode_meta);

                        XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_meta, "name", name);
                        XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_meta, "content", value);
                    }

                    if (metadata.OtherAttributes != null)
                    {
                        foreach (MetadataAttribute metadataAttribute in metadata.OtherAttributes.ContentsAs_Enumerable)
                        {
                            XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_meta, metadataAttribute.Name, metadataAttribute.Value);
                        }
                    }
                }



                foreach (ExternalFiles.ExternalFileData extFileData in m_Presentation.ExternalFilesDataManager.ManagedObjects.ContentsAs_Enumerable)
                {
                    if (!extFileData.IsPreservedForOutputFile)
                    {
#if DEBUG
                        Debugger.Break();
#endif
                        continue;
                    }

                    string relativePath = extFileData.OriginalRelativePath;
                    if (string.IsNullOrEmpty(relativePath))
                    {
#if DEBUG
                        Debugger.Break();
#endif
                    }

                    bool isCover = extFileData is CoverImageExternalFileData;
                    bool isNCX = extFileData is NCXExternalFileData;
                    bool isNav = extFileData is NavDocExternalFileData;

                    bool isMetaInf = false;

                    string fullPath = null;
                    if (relativePath.StartsWith(Daisy3_Import.META_INF_prefix))
                    {
                        isMetaInf = true;

                        relativePath = relativePath.Substring(Daisy3_Import.META_INF_prefix.Length);

                        fullPath = Path.Combine(m_UnzippedOutputDirectory, "META-INF/" + relativePath);
                    }
                    else
                    {
                        fullPath = Path.Combine(opsDirectoryPath, relativePath);
                    }

                    fullPath = FileDataProvider.NormaliseFullFilePath(fullPath).Replace('/', '\\');
                    if (!File.Exists(fullPath))
                    {
                        extFileData.DataProvider.ExportDataStreamToFile(fullPath, false);

                        if (isMetaInf)
                        {
                            hasMetaInfContainerXML = true;
                        }

                        if (!isMetaInf)
                        {
                            XmlNode opfXmlNode_itemExt = opfXmlDoc.CreateElement(null,
                                                                                 "item",
                                                                                 DiagramContentModelHelper.
                                                                                     NS_URL_EPUB_PACKAGE);
                            opfXmlNode_manifest.AppendChild(opfXmlNode_itemExt);

                            string ext = Path.GetExtension(relativePath);

                            string type = DataProviderFactory.GetMimeTypeFromExtension(ext);
                            XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_itemExt, @"media-type",
                                                                       type);

                            XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_itemExt, @"href",
                                                                       relativePath);

                            if (isCover)
                            {
                                XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_itemExt, @"id",
                                                                           @"cover-image");

                                XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_itemExt, @"properties",
                                                                           @"cover-image");

                                XmlNode opfXmlNode_metaCover = opfXmlDoc.CreateElement(null,
                                                                                       @"meta",
                                                                                       DiagramContentModelHelper.
                                                                                           NS_URL_EPUB_PACKAGE);
                                opfXmlNode_metadata.AppendChild(opfXmlNode_metaCover);

                                XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_metaCover, @"name",
                                                                           "cover");
                                XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_metaCover, @"content",
                                                                           "cover-image");
                            }
                            else if (isNCX)
                            {
                                XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_itemExt, @"id", "ncx");
                                XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_spine, @"toc", "ncx");

                                hasNCX = fullPath;
                            }
                            else if (isNav) // Note: when NavDoc is external data, it is not in spine (and vice versa)
                            {
                                XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_itemExt, @"properties",
                                                                           "nav");

                                hasNavDoc = fullPath;
                            }
                        }
                    }
                }

                string rootDir = Path.GetDirectoryName(m_XukPath);

                foreach (TreeNode treeNode in m_Presentation.RootNode.Children.ContentsAs_Enumerable)
                {
                    TextMedia txtMedia = treeNode.GetTextMedia() as TextMedia;
                    if (txtMedia == null)
                    {
#if DEBUG
                        Debugger.Break();
#endif
                        continue;
                    }
                    string path = txtMedia.Text;

                    XmlProperty xmlProp = treeNode.GetXmlProperty();
                    if (xmlProp == null)
                    {
#if DEBUG
                        Debugger.Break();
#endif
                        continue;
                    }

                    string name = treeNode.GetXmlElementLocalName();
                    if (name != "metadata")
                    {
#if DEBUG
                        Debugger.Break();
#endif
                        continue;
                    }

                    XmlNode opfXmlNode_itemRef;
                    XmlNode opfXmlNode_item;
                    string uid_OPF_SpineItem;
                    processSingleSpineItem_1(opfXmlDoc, opfXmlNode_spine, opfXmlNode_manifest, path, out opfXmlNode_itemRef, out opfXmlNode_item, out uid_OPF_SpineItem);


                    string title = null;
                    bool hasXuk = false;
                    bool isNavDocItem = false;
                    foreach (XmlAttribute xmlAttr in xmlProp.Attributes.ContentsAs_Enumerable)
                    {
                        if (xmlAttr.LocalName == @"xuk" && xmlAttr.Value == @"true")
                        {
                            hasXuk = true;
                        }
                        else if (xmlAttr.LocalName == @"title")
                        {
                            title = xmlAttr.Value;
                        }
                        else if (xmlAttr.LocalName == @"media-overlay")
                        {
                            // NOP
                        }
                        else if (xmlAttr.LocalName == @"nav")
                        {
                            isNavDocItem = true;
                        }
                        else if (xmlAttr.LocalName == @"properties_spine")
                        {
                            XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_itemRef, @"properties", xmlAttr.Value);
                        }
                        else if (xmlAttr.LocalName == @"properties_manifest")
                        {
                            XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_item, @"properties", xmlAttr.Value);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(xmlAttr.Prefix))
                            {
                                string nsUri = xmlAttr.GetNamespaceUri();

                                DebugFix.Assert(!string.IsNullOrEmpty(nsUri));

                                XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_itemRef, xmlAttr.PrefixedLocalName, xmlAttr.Value, nsUri);
                            }
                            else
                            {
                                XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_itemRef, xmlAttr.LocalName, xmlAttr.Value);
                            }
                        }
                    }

                    if (!hasXuk)
                    {
                        continue;
                    }


                    string fullXukPath = Daisy3_Import.GetXukFilePath_SpineItem(rootDir, path, title);
                    if (!File.Exists(fullXukPath))
                    {
#if DEBUG
                        Debugger.Break();
#endif //DEBUG
                        continue;
                    }

                    string fullSpineItemPath = Path.Combine(opsDirectoryPath, path);
                    fullSpineItemPath = FileDataProvider.NormaliseFullFilePath(fullSpineItemPath).Replace('/', '\\');

                    if (isNavDocItem)
                    {
                        hasNavDoc = fullSpineItemPath;
                    }

                    if (!string.IsNullOrEmpty(m_exportSpineItemProjectPath))
                    {
                        string fullXukPathNormalised = FileDataProvider.NormaliseFullFilePath(fullXukPath).
                            Replace('/', '\\');

                        if (m_exportSpineItemProjectPath.Equals(fullXukPathNormalised, StringComparison.OrdinalIgnoreCase))
                        {
                            m_exportSpineItemPath = fullSpineItemPath;
                        }
                        else if (!isNavDocItem)
                        {
                            opfXmlNode_spine.RemoveChild(opfXmlNode_itemRef);
                            opfXmlNode_manifest.RemoveChild(opfXmlNode_item);

                            continue;
                        }
                    }

                    Uri uri = new Uri(fullXukPath, UriKind.Absolute);

                    Project project = new Project();

                    OpenXukAction action = new OpenXukAction(project, uri);
                    action.ShortDescription = "...";
                    action.LongDescription = "...";
                    action.Execute();

                    if (project.Presentations.Count <= 0)
                    {
#if DEBUG
                        Debugger.Break();
#endif //DEBUG
                        continue;
                    }

                    Presentation spineItemPresentation = project.Presentations.Get(0);

                    bool cancel = processSingleSpineItem_2(opfXmlDoc, opfXmlNode_spine, opfXmlNode_manifest, path, opfXmlNode_item, opfXmlNode_metadata, uid_OPF_SpineItem, spineItemPresentation, opsDirectoryPath, fullSpineItemPath, timeTotal);
                    if (cancel) return;
                }
            }
            else // NOT XukSpine
            {
#if DEBUG
                Debugger.Break();
#endif

                string path = @"book.xhtml";

                XmlNode opfXmlNode_itemRef;
                XmlNode opfXmlNode_item;
                string uid_OPF_SpineItem;
                processSingleSpineItem_1(opfXmlDoc, opfXmlNode_spine, opfXmlNode_manifest, path, out opfXmlNode_itemRef, out opfXmlNode_item, out uid_OPF_SpineItem);

                string fullSpineItemPath = Path.Combine(opsDirectoryPath, path);
                fullSpineItemPath = FileDataProvider.NormaliseFullFilePath(fullSpineItemPath).Replace('/', '\\');

                bool cancel = processSingleSpineItem_2(opfXmlDoc, opfXmlNode_spine, opfXmlNode_manifest, path, opfXmlNode_item, opfXmlNode_metadata, uid_OPF_SpineItem, m_Presentation, opsDirectoryPath, fullSpineItemPath, timeTotal);
                if (cancel) return;
            }



            if (!string.IsNullOrEmpty(m_exportSpineItemProjectPath))
            {
                if (!string.IsNullOrEmpty(hasNCX)
                    && File.Exists(hasNCX))
                {
                    fixNavReferencesSingleChapterExport(hasNCX, "content", "src", hasNavDoc);
                }

                if (!string.IsNullOrEmpty(hasNavDoc)
                    && File.Exists(hasNavDoc))
                {
                    fixNavReferencesSingleChapterExport(hasNavDoc, "a", "href", hasNavDoc);
                }
            }

            if (!hasMetaInfContainerXML)
            {
                string containerXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<container version=\"1.0\" xmlns=\"urn:oasis:names:tc:opendocument:xmlns:container\">\n<rootfiles>\n<rootfile full-path=\"" + opfRelativeFilePath + "\" media-type=\"application/oebps-package+xml\" />\n</rootfiles>\n</container>";

                string containerPath = Path.Combine(metainfDirectoryPath, "container.xml");
                StreamWriter containerWriter = File.CreateText(containerPath);
                try
                {
                    containerWriter.Write(containerXML);
                }
                finally
                {
                    containerWriter.Close();
                }
            }

            if (string.IsNullOrEmpty(hasNavDoc))
            {
                string navDocRelativePath = @"nav.xhtml";

                XmlNode opfXmlNode_navDoc = opfXmlDoc.CreateElement(null,
                                                                     "item",
                                                                     DiagramContentModelHelper.
                                                                         NS_URL_EPUB_PACKAGE);
                opfXmlNode_manifest.AppendChild(opfXmlNode_navDoc);

                XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_navDoc, @"media-type", DataProviderFactory.XHTML_MIME_TYPE);

                XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_navDoc, @"href", navDocRelativePath);

                XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_navDoc, @"properties", "nav");

                // TODO: auto-generate NavDoc (HTML5 outline algorithm)
                string navDocPath = Path.Combine(opsDirectoryPath, navDocRelativePath);

#if DEBUG
                StreamWriter navDocWriter = File.CreateText(navDocPath);
                try
                {
                    navDocWriter.Write(navDocPath);
                }
                finally
                {
                    navDocWriter.Close();
                }
#endif
            }

            if (!timeTotal.IsEqualTo(Time.Zero))
            {
                XmlNode opfXmlNode_metaTotalDur = opfXmlDoc.CreateElement(null,
                                                                          @"meta",
                                                                          DiagramContentModelHelper.NS_URL_EPUB_PACKAGE);
                opfXmlNode_metadata.AppendChild(opfXmlNode_metaTotalDur);

                XmlDocumentHelper.CreateAppendXmlAttribute(opfXmlDoc, opfXmlNode_metaTotalDur, @"property",
                                                           "media:duration");

                XmlNode timeNodeTotalDur = opfXmlDoc.CreateTextNode(timeTotal.ToString());
                opfXmlNode_metaTotalDur.AppendChild(timeNodeTotalDur);
            }

            XmlReaderWriterHelper.WriteXmlDocument(opfXmlDoc, opfFilePath);

#if false && DEBUG
                    // Empty directories will not be included in ZIP

                    string dir_empty = Path.Combine(m_OutputDirectory, "dir-empty");
                    FileDataProvider.CreateDirectory(dir_empty);

                    string dir_non_empty = Path.Combine(m_OutputDirectory, "dir-non-empty");
                    FileDataProvider.CreateDirectory(dir_non_empty);

                    string subdir_empty = Path.Combine(dir_non_empty, "subdir-empty");
                    FileDataProvider.CreateDirectory(subdir_empty);

                    string subdir_non_empty = Path.Combine(dir_non_empty, "subdir-non-empty");
                    FileDataProvider.CreateDirectory(subdir_non_empty);

                    string testFile = Path.Combine(subdir_non_empty, "testFile.txt");
                    StreamWriter writer = File.CreateText(testFile);
                    try
                    {
                        writer.Write("Hello world!");
                    }
                    finally
                    {
                        writer.Close();
                    }
#endif

            //DirectoryInfo dirInfo = new DirectoryInfo(m_OutputDirectory);

            //FileInfo[] files = dirInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly);
            ////IEnumerable<FileInfo> opfFiles = dirInfo.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly);

            //            string[] allFiles = Directory.GetFileSystemEntries(m_OutputDirectory, "*.*"
            //#if NET40
            //, SearchOption.TopDirectoryOnly
            //#endif
            //);
            //            for (int i = 0; i < allFiles.Length; i++)
            //            {
            //                string fileName = Path.GetFileName(allFiles[i]);

            //                if (allFiles[i] != opsDirectoryPath
            //                    && fileName != ".DS_Store" && fileName != ".svn")
            //                {
            //                    string dest = allFiles[i].Replace(m_OutputDirectory, opsDirectoryPath);
            //                    if (Directory.Exists(allFiles[i]))
            //                    {
            //                        Directory.Move(allFiles[i], dest);
            //                    }
            //                    else
            //                    {
            //                        File.Move(allFiles[i], dest);
            //                    }
            //                }
            //            }


            string mimeTypePath = Path.Combine(m_UnzippedOutputDirectory, "mimetype");
            StreamWriter mimeTypeWriter = File.CreateText(mimeTypePath);
            try
            {
                mimeTypeWriter.Write("application/epub+zip");
            }
            finally
            {
                mimeTypeWriter.Close();
            }

            PackageToZip();
        }

        protected void fixNavReferencesSingleChapterExport(string path, string element, string attribute, string navdoc)
        {
            string xml = File.ReadAllText(path);
            xml = xml.Replace("\r\n", "\n");

            string targetFileName = Path.GetFileName(m_exportSpineItemPath);
            string regexp1 = element + "\\s*" + attribute + "\\s*=\\s*\"[^\"]*" + Regex.Escape(targetFileName) + "[^\"]*\"";

            xml = Regex.Replace(xml, regexp1, "TOBI1$&TOBI2", RegexOptions.Singleline | RegexOptions.IgnoreCase);

#if false && DEBUG
            StreamWriter containerWriter = File.CreateText(path);
            try
            {
                containerWriter.Write(xml);
            }
            finally
            {
                containerWriter.Close();
            }
#endif
            bool checkNavDoc = !string.IsNullOrEmpty(navdoc)
                               && targetFileName != Path.GetFileName(navdoc);
            string regexp1_ = null;
            if (checkNavDoc)
            {
                targetFileName = Path.GetFileName(navdoc);
                regexp1_ = element + "\\s*" + attribute + "\\s*=\\s*\"[^\"]*" + Regex.Escape(targetFileName) +
                                  "[^\"]*\"";

                xml = Regex.Replace(xml, regexp1_, "TOBI1$&TOBI2", RegexOptions.Singleline | RegexOptions.IgnoreCase);

#if false && DEBUG
            containerWriter = File.CreateText(path);
            try
            {
                containerWriter.Write(xml);
            }
            finally
            {
                containerWriter.Close();
            }
#endif
            }
            string regexp2 = "(<" + element + "\\s*" + attribute + "\\s*=\\s*\")([^\"]*)(\")";
            xml = Regex.Replace(xml, regexp2, "$1#$3", RegexOptions.Singleline | RegexOptions.IgnoreCase);

#if false && DEBUG
            containerWriter = File.CreateText(path);
            try
            {
                containerWriter.Write(xml);
            }
            finally
            {
                containerWriter.Close();
            }
#endif
            string regexp3 = "TOBI1(" + regexp1 + ")TOBI2";
            xml = Regex.Replace(xml, regexp3, "$1", RegexOptions.Singleline | RegexOptions.IgnoreCase);

#if false && DEBUG
            containerWriter = File.CreateText(path);
            try
            {
                containerWriter.Write(xml);
            }
            finally
            {
                containerWriter.Close();
            }
#endif

            if (checkNavDoc)
            {
                string regexp4 = "TOBI1(" + regexp1_ + ")TOBI2";
                xml = Regex.Replace(xml, regexp4, "$1", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }

#if true || !DEBUG
            StreamWriter
#endif
 containerWriter = File.CreateText(path);
            try
            {
                containerWriter.Write(xml);
            }
            finally
            {
                containerWriter.Close();
            }
        }

        public void PackageToZip()
        {
            string zipOutputDirectory = Path.GetDirectoryName(m_UnzippedOutputDirectory);
            //string zipOutputDirectory = Directory.GetParent(zipOutputDirectory).FullName;

            string epubFileName = Path.GetFileName(zipOutputDirectory);
            string title = Daisy3_Import.GetTitle(m_Presentation);
            if (!string.IsNullOrEmpty(title))
            {
                title = Daisy3_Import.CleanupTitle(title, 40);

                epubFileName = title;
            }

            string epubFilePath = Path.Combine(zipOutputDirectory, epubFileName + ".epub");
            if (File.Exists(epubFilePath))
            {
                File.Delete(epubFilePath);
            }

            reportProgress(-1, @"Creating EPUB file archive: " + epubFilePath); //UrakawaSDK_daisy_Lang.BLAbla


            string unzippedOutputDirectory_Normalised = FileDataProvider.NormaliseFullFilePath(m_UnzippedOutputDirectory).Replace('/', '\\');

#if ENABLE_SHARPZIP
            FastZipEvents zipEvents = new FastZipEvents();
            zipEvents.ProcessFile = ProcessEvents;

            FastZip zip = new FastZip(zipEvents);
            zip.CreateEmptyDirectories = true;
            //zip.UseZip64 = UseZip64.On;

            string emptyDirectoryPath = Path.Combine(zipOutputDirectory, Path.GetFileName(zipOutputDirectory));
            Directory.CreateDirectory(emptyDirectoryPath);
            //zip.CreateZip( filePath, zipOutputDirectory, true, null);
            zip.CreateZip(filePath, emptyDirectoryPath, false, null);
            Directory.Delete(emptyDirectoryPath);

            ZipFile zippeFile = new ZipFile(filePath);
            zippeFile.BeginUpdate();
            //zippeFile.Delete(emptyDirectoryPath);
            string initialPath = Directory.GetParent(zipOutputDirectory).FullName;
            //System.Windows.Forms.MessageBox.Show(initialPath);
            //zippeFile.AddDirectory(
            string mimeTypePath = Path.Combine(zipOutputDirectory, "mimetype");
            ICSharpCode.SharpZipLib.Zip.StaticDiskDataSource dataSource = new StaticDiskDataSource(mimeTypePath);
            //zippeFile.Add(dataSource , Path.Combine(Path.GetFileName (emptyDirectoryPath), "mimetype"), CompressionMethod.Stored);
            zippeFile.Add(dataSource, mimeTypePath.Replace(initialPath, ""), CompressionMethod.Stored);

            string[] listOfFiles = Directory.GetFiles(zipOutputDirectory, "*.*", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < listOfFiles.Length; i++)
            {
                if (listOfFiles[i] != mimeTypePath)
                {
                    zippeFile.Add(listOfFiles[i], listOfFiles[i].Replace(initialPath, ""));
                }
            }
            string[] listOfDirectories = Directory.GetDirectories(zipOutputDirectory, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < listOfDirectories.Length; i++)
            {

                string[] listOfInternalFiles = Directory.GetFiles(listOfDirectories[i], "*.*", SearchOption.TopDirectoryOnly);
                //if (listOfInternalFiles.Length == 0) zippeFile.AddDirectory(listOfDirectories[i]);
                for (int j = 0; j < listOfInternalFiles.Length; j++)
                {
                    zippeFile.Add(listOfInternalFiles[j], listOfInternalFiles[j].Replace(initialPath, ""));
                }
            }

            //zippeFile.Add(Path.Combine(zipOutputDirectory, "mimetype"), "mimetype");

            zippeFile.CommitUpdate();

            zippeFile.Close();
#else
            using (ZipStorer zip = ZipStorer.Create(epubFilePath, ""))
            {
                zip.EncodeUTF8 = true;

                string[] allFiles = Directory.GetFiles(m_UnzippedOutputDirectory, "*.*", SearchOption.AllDirectories);
                for (int i = 0; i < allFiles.Length; i++)
                {
                    string filePath = allFiles[i];
                    string filePath_Normalised = FileDataProvider.NormaliseFullFilePath(filePath).Replace('/', '\\');

                    string fileName = Path.GetFileName(filePath);

                    string relativePath = filePath_Normalised.Substring(unzippedOutputDirectory_Normalised.Length);
#if DEBUG
                    DebugFix.Assert(relativePath == filePath_Normalised.Replace(unzippedOutputDirectory_Normalised, ""));
#endif

                    if (fileName != ".DS_Store" && fileName != ".svn")
                    {
                        zip.AddFile(
                            relativePath == @"mimetype" ? ZipStorer.Compression.Store : ZipStorer.Compression.Deflate,
                            filePath,
                            relativePath,
                            "");
                    }
                }

                //string[] allDirectories = Directory.GetDirectories(zipOutputDirectory, "*.*", SearchOption.AllDirectories);
                //for (int i = 0; i < allDirectories.Length; i++)
                //{
                //    string fileName = Path.GetFileName(allDirectories[i]);

                //    if (fileName != ".svn")
                //    {
                //        // TODO: if DIR is empty...problem: ZIP API doesn't handle empty folders, only file storage methods.
                //        zip.AddFile(ZipStorer.Compression.Deflate, allDirectories[i], allDirectories[i].Replace(parentDirectory, ""), "");
                //    }
                //}
            }
#endif

#if DEBUG
            DisplayZipHeaderForVerification(epubFilePath);
#endif //DEBUG
        }


#if DEBUG
        private void DisplayZipHeaderForVerification(string zipFilePath)
        {
            BinaryReader reader = new BinaryReader(File.OpenRead(zipFilePath));
            Console.WriteLine("1-4. Signatures: " + reader.ReadInt32());

            Console.WriteLine("5-6. version: " + reader.ReadInt16());
            Console.WriteLine("7-8. Compression: " + reader.ReadInt16());
            Console.WriteLine("9-10. last modified: " + reader.ReadInt16());
            Console.WriteLine("11-12. last modified date: " + reader.ReadInt16());
            Console.WriteLine("13-16. CRC: " + reader.ReadInt32());
            reader.Close();
        }
#endif //DEBUG

#if ENABLE_SHARPZIP
        private void ProcessEvents(object sender, ScanEventArgs args)
        {
        }
#endif //ENABLE_SHARPZIP


        private XmlDocument createXmlDocument_OPF()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = null;

            xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);

            XmlNode package = xmlDoc.CreateElement(null,
                "package",
                DiagramContentModelHelper.NS_URL_EPUB_PACKAGE);

            xmlDoc.AppendChild(package);

            // get from metadata
            //XmlDocumentHelper.CreateAppendXmlAttribute(xmlDoc, package, "unique-identifier", "uid");

            XmlDocumentHelper.CreateAppendXmlAttribute(xmlDoc, package, "version", "3.0");


            if (isXukSpine)
            {
                XmlAttribute xmlAttr = m_Presentation.RootNode.GetXmlProperty().GetAttribute("prefix");
                if (xmlAttr != null)
                {
                    //"rendition: http://www.idpf.org/vocab/rendition/# cc: http://creativecommons.org/ns#"
                    XmlDocumentHelper.CreateAppendXmlAttribute(xmlDoc, package, "prefix", xmlAttr.Value);
                }
            }

            XmlNode metadata = xmlDoc.CreateElement(null, "metadata", package.NamespaceURI);
            package.AppendChild(metadata);

            XmlNode manifest = xmlDoc.CreateElement(null, "manifest", package.NamespaceURI);
            package.AppendChild(manifest);

            XmlNode spine = xmlDoc.CreateElement(null, "spine", package.NamespaceURI);
            package.AppendChild(spine);

            return xmlDoc;
        }


        protected const string ID_HtmlPrefix = "h_";
        protected long m_Counter_ID_HtmlPrefix = 0;

        protected const string ID_SpinePrefix = "spine_";
        protected long m_Counter_ID_OpfPrefix = 0;

        protected const string ID_MoPrefix = "mo_";
        protected long m_Counter_ID_MoPrefix = 0;

        protected long m_Counter_ID_Generic = 0;

        protected string GetNextID(string prefix)
        {
            long counter = 0;

            if (prefix == ID_HtmlPrefix)
            {
                m_Counter_ID_HtmlPrefix++;
                counter = m_Counter_ID_HtmlPrefix;
            }
            else if (prefix == ID_SpinePrefix)
            {
                m_Counter_ID_OpfPrefix++;
                counter = m_Counter_ID_OpfPrefix;
            }
            else
            {
                m_Counter_ID_Generic++;
                counter = m_Counter_ID_Generic;
            }

            return prefix + counter.ToString();
        }
    }
}
