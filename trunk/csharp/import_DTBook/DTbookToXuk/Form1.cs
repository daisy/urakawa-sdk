using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using urakawa.media;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.media.data.audio.codec;
using urakawa.media.timing;
using urakawa.metadata;
using urakawa.xuk;
using core = urakawa.core;
using System.Xml;
using urakawa;
using urakawa.property.channel;
using urakawa.property.xml;
using TreeNode = urakawa.core.TreeNode;

namespace DTbookToXuk
{
    public partial class Form1 : Form
    {
        private string m_DTBook_FilePath;
        private XmlDocument m_DTBookXmlDoc;
        private Project m_Project;
        private TextChannel m_textChannel;
        private AudioChannel m_audioChannel;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSBook_Click(object sender, EventArgs e)
        {

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            //open.InitialDirectory = @"C:\";
            open.Filter = "XML Files (*.xml)|*.xml|All files(*.*)|*.*";
            open.FilterIndex = 1;
            open.RestoreDirectory = true;
            if (open.ShowDialog(this) == DialogResult.OK)
            {
                m_DTBook_FilePath = open.FileName;
                txtBookName.Text = m_DTBook_FilePath;

                transformDTBook();
            }
        }

        private void transformDTBook()
        {
            m_DTBookXmlDoc = readXmlDocument(m_DTBook_FilePath);

            initializeDataModel();
        }

        private void initializeDataModel()
        {
            m_Project = new Project();

            //m_Project.PresentationFactory.Create();
            Presentation presentation = m_Project.AddNewPresentation();

            string dirPath = Path.GetDirectoryName(m_DTBook_FilePath);
            if (!dirPath.EndsWith("" + Path.DirectorySeparatorChar))
            {
                dirPath = dirPath + Path.DirectorySeparatorChar;
            }
            presentation.RootUri = new Uri(dirPath);

            m_Project.SetPrettyFormat(false);

            // BEGIN OF TEST
            // => creating all kinds of objects in order to initialize the factories
            // and cache the mapping between XUK names (pretty or compressed) and actual types.
            Channel ch = presentation.ChannelFactory.Create();
            presentation.ChannelsManager.RemoveChannel(ch);
            ch = presentation.ChannelFactory.CreateAudioChannel();
            presentation.ChannelsManager.RemoveChannel(ch);
            ch = presentation.ChannelFactory.CreateTextChannel();
            presentation.ChannelsManager.RemoveChannel(ch);
            //
            DataProvider dp = presentation.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);
            presentation.DataProviderManager.RemoveDataProvider(dp, true);
            //
            MediaData md = presentation.MediaDataFactory.CreateAudioMediaData();
            presentation.MediaDataManager.RemoveMediaData(md);
            //
            presentation.CommandFactory.CreateCompositeCommand();
            //
            presentation.MediaFactory.CreateExternalImageMedia();
            presentation.MediaFactory.CreateExternalVideoMedia();
            presentation.MediaFactory.CreateExternalTextMedia();
            presentation.MediaFactory.CreateExternalAudioMedia();
            presentation.MediaFactory.CreateManagedAudioMedia();
            presentation.MediaFactory.CreateSequenceMedia();
            presentation.MediaFactory.CreateTextMedia();
            //
            presentation.MetadataFactory.CreateMetadata();
            //
            presentation.PropertyFactory.CreateChannelsProperty();
            presentation.PropertyFactory.CreateXmlProperty();
            //
            presentation.TreeNodeFactory.Create();
            //
            // END OF TEST

            /*
            Metadata mdAuthor = presentation.MetadataFactory.CreateMetadata();
            mdAuthor.Name = "dc:author";
            mdAuthor.Content = "Daniel + Chhavi + Rachana";

            presentation.AddMetadata(mdAuthor);

            Metadata mdDate = presentation.MetadataFactory.CreateMetadata();
            mdDate.Name = "dc:date";
            mdDate.Content = System.DateTime.Now.ToString();

            presentation.AddMetadata(mdDate);
            */

            m_textChannel = presentation.ChannelFactory.CreateTextChannel();
            m_textChannel.Name = "Our Text Channel";


            // Recursive parsing
            parseXmlDocAndPopulateDataModel(m_DTBookXmlDoc, null);
            //
            // Experimentation with audio stuff, added metadata from OPF, etc.
            parseOtherDaisyFilesAndPopulateDataModel();
            //



            Uri uriComp = new Uri(m_DTBook_FilePath + ".COMPRESSED.xuk");

            {
                SaveXukAction actionSave = new SaveXukAction(m_Project, uriComp);
                bool saveWasCancelled;
                Progress.ExecuteProgressAction(actionSave, out saveWasCancelled);
                if (saveWasCancelled)
                {
                    return;
                }
            }

            Uri uriPretty = new Uri(m_DTBook_FilePath + ".PRETTY.xuk");

            m_Project.SetPrettyFormat(true);

            {
                SaveXukAction actionSave = new SaveXukAction(m_Project, uriPretty);
                bool saveWasCancelled;
                Progress.ExecuteProgressAction(actionSave, out saveWasCancelled);
                if (saveWasCancelled)
                {
                    return;
                }
            }

            Project projectComp = new Project();

            //not needed, automatically detected
            //project.PrettyFormat = false;

            {
                OpenXukAction actionOpen = new OpenXukAction(projectComp, uriComp);
                bool openWasCancelled;
                Progress.ExecuteProgressAction(actionOpen, out openWasCancelled);
                if (openWasCancelled)
                {
                    return;
                }
            }

            System.Diagnostics.Debug.Assert(m_Project.ValueEquals(projectComp));

            Project projectPretty = new Project();

            {
                OpenXukAction actionOpen = new OpenXukAction(projectPretty, uriPretty);
                bool openWasCancelled;
                Progress.ExecuteProgressAction(actionOpen, out openWasCancelled);
                if (openWasCancelled)
                {
                    return;
                }
            }
            System.Diagnostics.Debug.Assert(projectComp.ValueEquals(projectPretty));
            System.Diagnostics.Debug.Assert(m_Project.ValueEquals(projectPretty));
        }

        private void parseOtherDaisyFilesAndPopulateDataModel()
        {
            string dirPath = Path.GetDirectoryName(m_DTBook_FilePath);
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            FileInfo[] fileInfos = dirInfo.GetFiles("*.opf");
            string opfFilePath = null;
            foreach (FileInfo fi in fileInfos)
            {
                opfFilePath = fi.FullName;
                break;
            }
            if (opfFilePath == null)
            {
                return;
            }


            XmlDocument opfXmlDoc = readXmlDocument(opfFilePath);

            parseOpfDcMetaData(opfXmlDoc);
            parseOpfMetaData(opfXmlDoc);

            List<string> spineListOfSmilFiles;
            string ncxPath;
            parseOpfManifestAndSpine(opfXmlDoc, out spineListOfSmilFiles, out ncxPath);

            if (ncxPath != null)
            {
                string fullNcxPath = Path.Combine(dirPath, ncxPath);
                parseNcx(fullNcxPath);
            }

            if (spineListOfSmilFiles != null)
            {
                Presentation presentation = m_Project.GetPresentation(0);

                m_audioChannel = presentation.ChannelFactory.CreateAudioChannel();
                m_audioChannel.Name = "Our Audio Channel";

                presentation.MediaDataManager.EnforceSinglePCMFormat = true;

                foreach (string smilPath in spineListOfSmilFiles)
                {
                    string fullSmilPath = Path.Combine(dirPath, smilPath);
                    parseSmil(fullSmilPath);
                }
            }
        }

        private void parseSmil(string fullSmilPath)
        {
            Presentation presentation = m_Project.GetPresentation(0);

            string dataPath = presentation.DataProviderManager.DataFileDirectoryFullPath;
            if (Directory.Exists(dataPath))
            {
                Directory.Delete(dataPath, true);
            }

            string dirPath = Path.GetDirectoryName(m_DTBook_FilePath);
            //presentation.DataProviderManager.DataFileDirectoryFullPath

            XmlDocument smilXmlDoc = readXmlDocument(fullSmilPath);

            XmlNodeList listOfAudioNodes = smilXmlDoc.GetElementsByTagName("audio");
            if (listOfAudioNodes != null)
            {
                foreach (XmlNode audioNode in listOfAudioNodes)
                {
                    XmlAttributeCollection attributeCol = audioNode.Attributes;

                    if (attributeCol != null)
                    {
                        XmlNode attrAudioSrc = attributeCol.GetNamedItem("src");
                        if (attrAudioSrc != null && !String.IsNullOrEmpty(attrAudioSrc.Value))
                        {
                            XmlNode parent = audioNode.ParentNode;
                            if (parent != null && parent.Name == "a")
                            {
                                parent = parent.ParentNode;
                            }

                            if (parent != null)
                            {
                                XmlNodeList listOfAudioPeers = parent.ChildNodes;
                                foreach (XmlNode peerNode in listOfAudioPeers)
                                {
                                    if (peerNode.NodeType == XmlNodeType.Element && peerNode.Name == "text")
                                    {
                                        XmlAttributeCollection peerAttrs = peerNode.Attributes;

                                        if (peerAttrs != null)
                                        {
                                            XmlNode attrTextSrc = peerAttrs.GetNamedItem("src");
                                            if (attrTextSrc != null && !String.IsNullOrEmpty(attrTextSrc.Value))
                                            {
                                                int index = attrTextSrc.Value.LastIndexOf('#');
                                                if (index < (attrTextSrc.Value.Length - 1))
                                                {
                                                    string dtbookFragmentId = attrTextSrc.Value.Substring(index + 1);

                                                    TreeNode tNode = getTreeNodeWithXmlElementId(dtbookFragmentId);
                                                    if (tNode != null)
                                                    {
                                                        AbstractAudioMedia existingAudioMedia = tNode.GetAudioMedia();
                                                        if (existingAudioMedia != null)
                                                        {
                                                            //Ignore.
                                                            //System.Diagnostics.Debug.Fail("TreeNode already has media ??");
                                                        }

                                                        XmlNode attrClipBegin = attributeCol.GetNamedItem("clipBegin");
                                                        XmlNode attrClipEnd = attributeCol.GetNamedItem("clipEnd");

                                                        Media media = null;
                                                        if (attrAudioSrc.Value.EndsWith("wav"))
                                                        {
                                                            presentation.MediaDataFactory.DefaultAudioMediaDataType =
                                                                typeof (WavAudioMediaData);
                                                            WavAudioMediaData mediaData =
                                                                (WavAudioMediaData)
                                                                presentation.MediaDataFactory.CreateAudioMediaData();

                                                            media = presentation.MediaFactory.CreateManagedAudioMedia();
                                                            ((ManagedAudioMedia) media).AudioMediaData = mediaData;

                                                            string fullWavPath = Path.Combine(dirPath,
                                                                                              attrAudioSrc.Value);

                                                            PCMDataInfo pcmInfo = null;
                                                            Stream wavStream = null;
                                                            try
                                                            {
                                                                wavStream = File.Open(fullWavPath, FileMode.Open,
                                                                                      FileAccess.Read, FileShare.Read);
                                                                pcmInfo = PCMDataInfo.ParseRiffWaveHeader(wavStream);
                                                                presentation.MediaDataManager.DefaultPCMFormat = pcmInfo;
                                                                TimeDelta duration = new TimeDelta(pcmInfo.Duration);

                                                                Time clipB = Time.Zero;
                                                                Time clipE = Time.MaxValue;

                                                                if (attrClipBegin != null &&
                                                                    !string.IsNullOrEmpty(attrClipBegin.Value))
                                                                {
                                                                    clipB = new Time(TimeSpan.Parse(attrClipBegin.Value));
                                                                }
                                                                if (attrClipEnd != null &&
                                                                    !string.IsNullOrEmpty(attrClipEnd.Value))
                                                                {
                                                                    clipE = new Time(TimeSpan.Parse(attrClipEnd.Value));
                                                                }
                                                                if (clipB != Time.Zero || clipE != Time.MaxValue)
                                                                {
                                                                    duration = clipE.GetTimeDelta(clipB);
                                                                }
                                                                long byteOffset = 0;
                                                                if (clipB != Time.Zero)
                                                                {
                                                                    byteOffset = pcmInfo.GetByteForTime(clipB);
                                                                }
                                                                if (byteOffset > 0)
                                                                {
                                                                    wavStream.Seek(byteOffset, SeekOrigin.Current);
                                                                }
                                                                mediaData.InsertAudioData(wavStream, Time.Zero, duration);
                                                                /*
                                                                wavStream.Position = 0;
                                                                wavStream.Seek(0, SeekOrigin.Begin);
                                                                mediaData.AppendAudioDataFromRiffWave(wavStream);
                                                                */
                                                            }
                                                            finally
                                                            {
                                                                if (wavStream != null) wavStream.Close();
                                                            }
                                                        }
                                                        else
                                                        {
                                                            media = presentation.MediaFactory.CreateExternalAudioMedia();
                                                            ((ExternalAudioMedia) media).Src = attrAudioSrc.Value;
                                                            if (attrClipBegin != null &&
                                                                !string.IsNullOrEmpty(attrClipBegin.Value))
                                                            {
                                                                ((ExternalAudioMedia) media).ClipBegin =
                                                                    new Time(TimeSpan.Parse(attrClipBegin.Value));
                                                            }
                                                            if (attrClipEnd != null &&
                                                                !string.IsNullOrEmpty(attrClipEnd.Value))
                                                            {
                                                                ((ExternalAudioMedia) media).ClipEnd =
                                                                    new Time(TimeSpan.Parse(attrClipEnd.Value));
                                                            }
                                                        }

                                                        ChannelsProperty chProp = tNode.GetProperty<ChannelsProperty>();
                                                        if (chProp == null)
                                                        {
                                                            chProp =
                                                                presentation.PropertyFactory.CreateChannelsProperty();
                                                            tNode.AddProperty(chProp);
                                                        }
                                                        chProp.SetMedia(m_audioChannel, media);
                                                        break; // scan peers to audio node
                                                    }
                                                    else
                                                    {
                                                        System.Diagnostics.Debug.Fail("XmlProperty with ID not found ??");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private XmlDocument readXmlDocument(string path)
        {
            XmlReaderSettings settings = new XmlReaderSettings();

            settings.IgnoreWhitespace = false;
            settings.ProhibitDtd = false;
            settings.XmlResolver = null;

            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;

            XmlReader xmlReader = XmlReader.Create(path, settings);

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.XmlResolver = null;
            try
            {
                xmldoc.Load(xmlReader);
            }
            finally
            {
                xmlReader.Close();
            }

            return xmldoc;
        }

        private TreeNode getTreeNodeWithXmlElementId(string id)
        {
            Presentation pres = m_Project.GetPresentation(0);
            return getTreeNodeWithXmlElementId(pres.RootNode, id);
        }

        private TreeNode getTreeNodeWithXmlElementId(TreeNode node, string id)
        {
            if (node.GetXmlElementId() == id) return node;

            for (int i = 0; i < node.ChildCount; i++)
            {
                TreeNode child = getTreeNodeWithXmlElementId(node.GetChild(i), id);
                if (child != null)
                {
                    return child;
                }
            }
            return null;
        }

        private void parseNcx(string ncxPath)
        {
            Presentation presentation = m_Project.GetPresentation(0);

            XmlDocument ncxXmlDoc = readXmlDocument(ncxPath);

            XmlNodeList listOfHeadRootNodes = ncxXmlDoc.GetElementsByTagName("head");
            if (listOfHeadRootNodes != null)
            {
                foreach (XmlNode headNodeRoot in listOfHeadRootNodes)
                {
                    XmlNodeList listOfMetaNodes = headNodeRoot.ChildNodes;
                    if (listOfMetaNodes != null)
                    {
                        foreach (XmlNode metaNode in listOfMetaNodes)
                        {
                            if (metaNode.NodeType == XmlNodeType.Element
                                && metaNode.Name == "meta")
                            {
                                XmlAttributeCollection attributeCol = metaNode.Attributes;

                                if (attributeCol != null)
                                {
                                    XmlNode attrName = attributeCol.GetNamedItem("name");
                                    XmlNode attrContent = attributeCol.GetNamedItem("content");
                                    if (attrName != null && attrContent != null && !String.IsNullOrEmpty(attrName.Value)
                                            && !String.IsNullOrEmpty(attrContent.Value))
                                    {
                                        Metadata md = presentation.MetadataFactory.CreateMetadata();
                                        md.Name = attrName.Value;
                                        md.Content = attrContent.Value;
                                        presentation.AddMetadata(md);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void parseOpfManifestAndSpine(XmlDocument opfXmlDoc, out List<string> spineListOfSmilFiles, out string ncxPath)
        {
            spineListOfSmilFiles = new List<string>();

            ncxPath = null;

            XmlNodeList listOfSpineRootNodes = opfXmlDoc.GetElementsByTagName("spine");
            if (listOfSpineRootNodes != null)
            {
                foreach (XmlNode spineNodeRoot in listOfSpineRootNodes)
                {
                    XmlNodeList listOfSpineItemNodes = spineNodeRoot.ChildNodes;
                    if (listOfSpineItemNodes != null)
                    {
                        foreach (XmlNode spineItemNode in listOfSpineItemNodes)
                        {
                            if (spineItemNode.NodeType == XmlNodeType.Element
                                && spineItemNode.Name == "itemref")
                            {
                                XmlAttributeCollection spineItemAttributes = spineItemNode.Attributes;

                                if (spineItemAttributes != null)
                                {
                                    XmlNode attrIdRef = spineItemAttributes.GetNamedItem("idref");
                                    if (attrIdRef != null)
                                    {
                                        spineListOfSmilFiles.Add(attrIdRef.Value);
                                    }
                                }
                            }
                        }
                    }
                }
            }










            XmlNodeList listOfManifestRootNodes = opfXmlDoc.GetElementsByTagName("manifest");
            if (listOfManifestRootNodes != null)
            {
                foreach (XmlNode manifNodeRoot in listOfManifestRootNodes)
                {
                    XmlNodeList listOfManifestItemNodes = manifNodeRoot.ChildNodes;
                    if (listOfManifestItemNodes != null)
                    {
                        foreach (XmlNode manifItemNode in listOfManifestItemNodes)
                        {
                            if (manifItemNode.NodeType == XmlNodeType.Element
                                && manifItemNode.Name == "item")
                            {
                                XmlAttributeCollection manifItemAttributes = manifItemNode.Attributes;

                                if (manifItemAttributes != null)
                                {
                                    XmlNode attrHref = manifItemAttributes.GetNamedItem("href");
                                    XmlNode attrMediaType = manifItemAttributes.GetNamedItem("media-type");
                                    if (attrHref != null && attrMediaType != null)
                                    {
                                        if (attrMediaType.Value == "application/smil")
                                        {
                                            XmlNode attrID = manifItemAttributes.GetNamedItem("id");
                                            if (attrID != null)
                                            {
                                                int i = spineListOfSmilFiles.IndexOf(attrID.Value);
                                                if (i >= 0)
                                                {
                                                    spineListOfSmilFiles[i] = attrHref.Value;
                                                }
                                            }
                                        }
                                        else if (attrMediaType.Value == "application/x-dtbncx+xml")
                                        {
                                            ncxPath = attrHref.Value;
                                        }
                                        else if (attrMediaType.Value == "application/x-dtbook+xml")
                                        {
                                            // Ignore (DTBook)
                                        }
                                        else if (attrMediaType.Value == "text/xml")
                                        {
                                            // Ignore (OPF)
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void parseOpfDcMetaData(XmlDocument opfXmlDoc)
        {
            Presentation presentation = m_Project.GetPresentation(0);

            XmlNodeList listOfMetaDataRootNodes = opfXmlDoc.GetElementsByTagName("dc-metadata");
            if (listOfMetaDataRootNodes != null)
            {
                foreach (XmlNode mdNodeRoot in listOfMetaDataRootNodes)
                {
                    XmlNodeList listOfMetaDataNodes = mdNodeRoot.ChildNodes;
                    if (listOfMetaDataNodes != null)
                    {
                        foreach (XmlNode mdNode in listOfMetaDataNodes)
                        {
                            if (mdNode.NodeType == XmlNodeType.Element)
                            {
                                Metadata md = presentation.MetadataFactory.CreateMetadata();
                                md.Name = mdNode.Name;
                                md.Content = mdNode.InnerText;
                                presentation.AddMetadata(md);
                            }
                        }
                    }
                }
            }
        }

        private void parseOpfMetaData(XmlDocument opfXmlDoc)
        {
            Presentation presentation = m_Project.GetPresentation(0);

            XmlNodeList listOfMetaDataRootNodes = opfXmlDoc.GetElementsByTagName("x-metadata");
            if (listOfMetaDataRootNodes != null)
            {
                foreach (XmlNode mdNodeRoot in listOfMetaDataRootNodes)
                {
                    XmlNodeList listOfMetaDataNodes = mdNodeRoot.ChildNodes;
                    if (listOfMetaDataNodes != null)
                    {
                        foreach (XmlNode mdNode in listOfMetaDataNodes)
                        {
                            if (mdNode.NodeType == XmlNodeType.Element && mdNode.Name == "meta")
                            {
                                XmlAttributeCollection mdAttributes = mdNode.Attributes;

                                if (mdAttributes != null)
                                {
                                    XmlNode attrName = mdAttributes.GetNamedItem("name");
                                    XmlNode attrContent = mdAttributes.GetNamedItem("content");

                                    if (attrName != null && attrContent != null && !String.IsNullOrEmpty(attrName.Value)
                                        && !String.IsNullOrEmpty(attrContent.Value))
                                    {
                                        Metadata md = presentation.MetadataFactory.CreateMetadata();
                                        md.Name = attrName.Value;
                                        md.Content = attrContent.Value;
                                        presentation.AddMetadata(md);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void parseXmlDocAndPopulateDataModel(XmlNode xmlNode, core.TreeNode parentTreeNode)
        {

            XmlNodeType xmlType = xmlNode.NodeType;
            switch (xmlType)
            {
                case XmlNodeType.Attribute:
                    {
                        System.Diagnostics.Debug.Fail("Calling this method with an XmlAttribute should never happen !!");
                        break;
                    }
                case XmlNodeType.Document:
                    {
                        parseXmlDocAndPopulateDataModel(((XmlDocument)xmlNode).DocumentElement, parentTreeNode);
                        break;
                    }
                case XmlNodeType.Element:
                    {
                        Presentation presentation = m_Project.GetPresentation(0);

                        core.TreeNode treeNode = presentation.TreeNodeFactory.Create();

                        if (parentTreeNode == null)
                        {
                            presentation.RootNode = treeNode;
                            parentTreeNode = presentation.RootNode;
                        }
                        else
                        {
                            parentTreeNode.AppendChild(treeNode);
                        }

                        XmlProperty xmlProp = presentation.PropertyFactory.CreateXmlProperty();
                        treeNode.AddProperty(xmlProp);
                        xmlProp.LocalName = xmlNode.Name;
                        if (xmlNode.ParentNode != null && xmlNode.ParentNode.NodeType == XmlNodeType.Document)
                        {
                            presentation.PropertyFactory.DefaultXmlNamespaceUri = xmlNode.NamespaceURI;
                        }

                        if (xmlNode.NamespaceURI != presentation.PropertyFactory.DefaultXmlNamespaceUri)
                        {
                            xmlProp.NamespaceUri = xmlNode.NamespaceURI;
                        }

                        XmlAttributeCollection attributeCol = xmlNode.Attributes;

                        if (attributeCol != null)
                        {
                            for (int i = 0; i < attributeCol.Count; i++)
                            {
                                XmlNode attr = attributeCol.Item(i);
                                xmlProp.SetAttribute(attr.Name, "", attr.Value);
                            }


                            if (xmlNode.Name == "meta")
                            {
                                XmlNode attrName = attributeCol.GetNamedItem("name");
                                XmlNode attrContent = attributeCol.GetNamedItem("content");
                                if (attrName != null && attrContent != null && !String.IsNullOrEmpty(attrName.Value)
                                        && !String.IsNullOrEmpty(attrContent.Value))
                                {
                                    Metadata md = presentation.MetadataFactory.CreateMetadata();
                                    md.Name = attrName.Value;
                                    md.Content = attrContent.Value;
                                    presentation.AddMetadata(md);
                                }
                            }
                        }


                        foreach (XmlNode childXmlNode in xmlNode.ChildNodes)
                        {
                            parseXmlDocAndPopulateDataModel(childXmlNode, treeNode);
                        }
                        break;
                    }
                case XmlNodeType.Text:
                    {
                        Presentation presentation = m_Project.GetPresentation(0);

                        string text = xmlNode.Value;
                        TextMedia textMedia = presentation.MediaFactory.CreateTextMedia();
                        textMedia.Text = text;

                        ChannelsProperty cProp = presentation.PropertyFactory.CreateChannelsProperty();
                        cProp.SetMedia(m_textChannel, textMedia);

                        int counter = 0;
                        foreach (XmlNode childXmlNode in xmlNode.ParentNode.ChildNodes)
                        {
                            XmlNodeType childXmlType = childXmlNode.NodeType;
                            if (childXmlType == XmlNodeType.Text || childXmlType == XmlNodeType.Element)
                            {
                                counter++;
                            }
                        }
                        if (counter == 1)
                        {
                            parentTreeNode.AddProperty(cProp);
                        }
                        else
                        {
                            core.TreeNode txtWrapperNode = presentation.TreeNodeFactory.Create();
                            txtWrapperNode.AddProperty(cProp);
                            parentTreeNode.AppendChild(txtWrapperNode);
                        }

                        break;
                    }
                default:
                    {
                        return;
                    }
            }
        }


        private void btnClear_Click(object sender, EventArgs e)
        {

            txtBookName.Clear();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {

        }
    }
}
