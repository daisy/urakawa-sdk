namespace urakawa.xuk
{
    public class XukStrings
    {
        private static bool m_IsPrettyFormat;
        public static bool IsPrettyFormat
        {
            get { return m_IsPrettyFormat; }
            set { m_IsPrettyFormat = value; }
        }


        #region low occurence


        #region the root

        public static string XukPretty
        {
            get { return "Xuk"; }
        }

        public static string XukCompressed
        {
            get { return "xuk"; }
        }

        public static string Xuk
        {
            get { return (IsPrettyFormat ? XukPretty : XukCompressed); }
        }

        #endregion

        public static string Presentations
        {
            get { return (IsPrettyFormat ? "Presentations" : "prez"); }
        }

        public static string RootNode
        {
            get { return (IsPrettyFormat ? "RootNode" : "root"); }
        }


        public static string RootUri
        {
            get { return (IsPrettyFormat ? "RootUri" : "rootUri"); }
        }

        public static string HeadNode
        {
            get { return (IsPrettyFormat ? "HeadNode" : "head"); }
        }

        public static string Metadatas
        {
            get { return (IsPrettyFormat ? "Metadatas" : "mtdts"); }
        }

        public static string MediaDatas
        {
            get { return (IsPrettyFormat ? "MediaDatas" : "medDts"); }
        }


        public static string Height
        {
            get { return (IsPrettyFormat ? "Height" : "h"); }
        }
        public static string Width
        {
            get { return (IsPrettyFormat ? "Width" : "w"); }
        }

        public static string DataProviders
        {
            get { return (IsPrettyFormat ? "DataProviders" : "dataProvs"); }
        }


        #region pcm

        public static string PCMFormatInfo
        {
            get { return (IsPrettyFormat ? "PCMFormatInfo" : "PCMInf"); }
        }

        public static string DefaultPCMFormat
        {
            get { return (IsPrettyFormat ? "DefaultPCMFormat" : "dfltPCM"); }
        }

        public static string NumberOfChannels
        {
            get { return (IsPrettyFormat ? "NumberOfChannels" : "ch"); }
        }
        public static string SampleRate
        {
            get { return (IsPrettyFormat ? "SampleRate" : "rate"); }
        }
        public static string BitDepth
        {
            get { return (IsPrettyFormat ? "BitDepth" : "depth"); }
        }

        public static string enforceSinglePCMFormat
        {
            get { return (IsPrettyFormat ? "enforceSinglePCMFormat" : "frceSglePCM"); }
        }

        public static string IsMarked
        {
            get { return (IsPrettyFormat ? "IsMarked" : "mrkd"); }
        }

        public static string PCMFormat
        {
            get { return (IsPrettyFormat ? "PCMFormat" : "PCM"); }
        }

        #endregion


        public static string DataFileDirectoryPath
        {
            get { return (IsPrettyFormat ? "DataFileDirectoryPath" : "dtDir"); }
        }

        public static string DataFileRelativePath
        {
            get { return (IsPrettyFormat ? "DataFileRelativePath" : "dtPath"); }
        }


        #region channels

        public static string ChannelItem
        {
            get { return (IsPrettyFormat ? "ChannelItem" : "chItm"); }
        }

        public static string Channels
        {
            get { return (IsPrettyFormat ? "Channels" : "chs"); }
        }

        #endregion

        #region undo-redo

        public static string UndoStack
        {
            get { return (IsPrettyFormat ? "UndoStack" : "udo"); }
        }

        public static string RedoStack
        {
            get { return (IsPrettyFormat ? "RedoStack" : "rdo"); }
        }

        public static string ActiveTransactions
        {
            get { return (IsPrettyFormat ? "ActiveTransactions" : "trns"); }
        }

        #endregion




        #region factories


        public static string TreeNodeFactory
        {
            get { return (IsPrettyFormat ? "TreeNodeFactory" : "nodFct"); }
        }

        public static string PropertyFactory
        {
            get { return (IsPrettyFormat ? "PropertyFactory" : "prpFct"); }
        }

        public static string ChannelFactory
        {
            get { return (IsPrettyFormat ? "ChannelFactory" : "chFct"); }
        }

        public static string MediaFactory
        {
            get { return (IsPrettyFormat ? "MediaFactory" : "medFct"); }
        }

        public static string MediaDataFactory
        {
            get { return (IsPrettyFormat ? "MediaDataFactory" : "medDtFct"); }
        }

        public static string DataProviderFactory
        {
            get { return (IsPrettyFormat ? "DataProviderFactory" : "dtPrvFct"); }
        }
        public static string CommandFactory
        {
            get { return (IsPrettyFormat ? "CommandFactory" : "cmdFct"); }
        }

        public static string MetadataFactory
        {
            get { return (IsPrettyFormat ? "MetadataFactory" : "metadtFct"); }
        }

        public static string PresentationFactory
        {
            get { return (IsPrettyFormat ? "PresentationFactory" : "presFct"); }
        }

        public static string ExternalFileDataFactory
        {
            get { return (IsPrettyFormat ? "ExternalFileDataFactory" : "ExFlDtFct"); }
        }

        public static string AlternateContentFactory
        {
            get { return (IsPrettyFormat ? "AlternateContentFactory" : "acFct"); }
        }


        #endregion


        #region commands


        public static string TreeNodeChangeTextCommand
        {
            get { return (IsPrettyFormat ? "TreeNodeChangeTextCommand" : "nodChTxtCmd"); }
        }

        public static string TreeNodeSetIsMarkedCommand
        {
            get { return (IsPrettyFormat ? "TreeNodeSetIsMarkedCommand" : "nodSetMrkCmd"); }
        }

        public static string TreeNodeSetManagedAudioMediaCommand
        {
            get { return (IsPrettyFormat ? "TreeNodeSetManagedAudioMediaCommand" : "nodSetManAudMedCmd"); }
        }

        public static string ManagedAudioMediaInsertDataCommand
        {
            get { return (IsPrettyFormat ? "ManagedAudioMediaInsertDataCommand" : "manAudMedInsertCmd"); }
        }

        public static string TreeNodeAudioStreamDeleteCommand
        {
            get { return (IsPrettyFormat ? "TreeNodeAudioStreamDeleteCommand" : "nodAudDelCmd"); }
        }

        public static string MetadataAddCommand
        {
            get { return (IsPrettyFormat ? "MetadataAddCommand" : "metaAddCmd"); }
        }
        public static string MetadataRemoveCommand
        {
            get { return (IsPrettyFormat ? "MetadataRemoveCommand" : "metaRemoveCmd"); }
        }
        public static string MetadataSetNameCommand
        {
            get { return (IsPrettyFormat ? "MetadataSetNameCommand" : "metaSetNameCmd"); }
        }
        public static string MetadataSetContentCommand
        {
            get { return (IsPrettyFormat ? "MetadataSetContentCommand" : "metaSetContentCmd"); }
        }
        public static string MetadataSetIdCommand
        {
            get { return (IsPrettyFormat ? "MetadataSetIdCommand" : "metaSetIdCmd"); }
        }

        //public static string AlternateContentMetadataAttributeAddCommand
        //{
        //    get { return (IsPrettyFormat ? "AlternateContentMetadataAttributeAddCommand" : "acMetaAttrAddCmd"); }
        //}

        //public static string AlternateContentMetadataAttributeRemoveCommand
        //{
        //    get { return (IsPrettyFormat ? "AlternateContentMetadataAttributeRemoveCommand" : "acMetaAttrRemoveCmd"); }
        //}

        //public static string AlternateContentMetadataAttributeSetNameCommand
        //{
        //    get { return (IsPrettyFormat ? "AlternateContentMetadataAttributeSetNameCommand" : "acMetaAttrNameSetCmd"); }
        //}
        //public static string AlternateContentMetadataAttributeSetContentCommand
        //{
        //    get { return (IsPrettyFormat ? "AlternateContentMetadataAttributeSetContentCommand" : "acMetaAttrContentSetCmd"); }
        //}

        public static string AlternateContentMetadataAddCommand
        {
            get { return (IsPrettyFormat ? "AlternateContentMetadataAddCommand" : "acMetaAddCmd"); }
        }
        public static string AlternateContentMetadataRemoveCommand
        {
            get { return (IsPrettyFormat ? "AlternateContentMetadataRemoveCommand" : "acMetaRemoveCmd"); }
        }
        public static string AlternateContentMetadataSetNameCommand
        {
            get { return (IsPrettyFormat ? "AlternateContentMetadataSetNameCommand" : "acMetaSetNameCmd"); }
        }
        public static string AlternateContentMetadataSetContentCommand
        {
            get { return (IsPrettyFormat ? "AlternateContentMetadataSetContentCommand" : "acMetaSetContentCmd"); }
        }
        public static string AlternateContentMetadataSetIdCommand
        {
            get { return (IsPrettyFormat ? "AlternateContentMetadataSetIdCommand" : "acMetaSetIdCmd"); }
        }

        public static string AlternateContentSetManagedMediaCommand
        {
            get { return (IsPrettyFormat ? "AlternateContentSetManagedMediaCommand" : "acSetManMedCmd"); }
        }
        public static string AlternateContentRemoveManagedMediaCommand
        {
            get { return (IsPrettyFormat ? "AlternateContentRemoveManagedMediaCommand" : "acRemManMedCmd"); }
        }

        public static string AlternateContentAddCommand
        {
            get { return (IsPrettyFormat ? "AlternateContentAddCommand" : "acAddCmd"); }
        }
        public static string AlternateContentRemoveCommand
        {
            get { return (IsPrettyFormat ? "AlternateContentRemoveCommand" : "acRemCmd"); }
        }

        #endregion

        #region managers


        public static string ChannelsManager
        {
            get { return (IsPrettyFormat ? "ChannelsManager" : "chsMan"); }
        }


        public static string MediaDataManager
        {
            get { return (IsPrettyFormat ? "MediaDataManager" : "medDtMan"); }
        }


        public static string DataProviderManager
        {
            get { return (IsPrettyFormat ? "DataProviderManager" : "dtProvMan"); }
        }


        public static string UndoRedoManager
        {
            get { return (IsPrettyFormat ? "UndoRedoManager" : "udoRdoMan"); }
        }

        public static string ExternalFileDataManager
        {
            get { return (IsPrettyFormat ? "ExternalFileDataManager" : "ExFlDtMan"); }
        }



        #endregion

        #endregion

        #region medium occurence

        public static string Sequence
        {
            get { return (IsPrettyFormat ? "Sequence" : "sq"); }
        }

        public static string AllowMultipleMediaTypes
        {
            get { return (IsPrettyFormat ? "AllowMultipleMediaTypes" : "alwMulTypes"); }
        }



        #region undo-redo

        public static string Identifier
        {
            get { return (IsPrettyFormat ? "Identifier" : "idntf"); }
        }

        public static string ShortDescription
        {
            get { return (IsPrettyFormat ? "ShortDescription" : "shrtDsc"); }
        }

        public static string LongDescription
        {
            get { return (IsPrettyFormat ? "LongDescription" : "lngDsc"); }
        }

        public static string Commands
        {
            get { return (IsPrettyFormat ? "Commands" : "cmds"); }
        }

        #endregion


        #region factories

        public static string RegisteredTypes
        {
            get { return (IsPrettyFormat ? "RegisteredTypes" : "types"); }
        }

        public static string Type
        {
            get { return (IsPrettyFormat ? "Type" : "type"); }
        }

        public static string XukLocalName
        {
            get { return (IsPrettyFormat ? "XukLocalName" : "name"); }
        }

        public static string BaseXukLocalName
        {
            get { return (IsPrettyFormat ? "BaseXukLocalName" : "baseName"); }
        }

        public static string XukNamespaceUri
        {
            get { return (IsPrettyFormat ? "XukNamespaceUri" : "ns"); }
        }

        public static string BaseXukNamespaceUri
        {
            get { return (IsPrettyFormat ? "BaseXukNamespaceUri" : "baseNs"); }
        }

        public static string AssemblyName
        {
            get { return (IsPrettyFormat ? "AssemblyName" : "assbly"); }
        }

        public static string AssemblyVersion
        {
            get { return (IsPrettyFormat ? "AssemblyVersion" : "assblyVer"); }
        }

        public static string FullName
        {
            get { return (IsPrettyFormat ? "FullName" : "fName"); }
        }
        #endregion


        public static string Metadata
        {
            get { return (IsPrettyFormat ? "Metadata" : "metadt"); }
        }
        public static string MetadataAttribute
        {
            get { return (IsPrettyFormat ? "MetadataAttribute" : "metadtattr"); }
        }
        public static string MetadataOtherAttributes
        {
            get { return (IsPrettyFormat ? "MetadataOtherAttributes" : "metadtattrs"); }
        }

        public static string Name
        {
            get { return (IsPrettyFormat ? "Name" : "n"); }
        }

        public static string Value
        {
            get { return (IsPrettyFormat ? "Value" : "v"); }
        }

        public static string LocalName
        {
            get { return (IsPrettyFormat ? "LocalName" : "n"); }
        }

        public static string NamespaceUri
        {
            get { return (IsPrettyFormat ? "NamespaceUri" : "ns"); }
        }


        #endregion

        #region high occurence


        public static string Language
        {
            get { return (IsPrettyFormat ? "Language" : "lang"); }
        }

        public static string Uid
        {
            get { return (IsPrettyFormat ? "Uid" : "uid"); }
        }

        public static string Src
        {
            get { return (IsPrettyFormat ? "Src" : "s"); }
        }



        #region tree

        public static string Children
        {
            get { return (IsPrettyFormat ? "Children" : "childs"); }
        }

        public static string Properties
        {
            get { return (IsPrettyFormat ? "Properties" : "ps"); }
        }

        #endregion

        #region xml

        public static string XmlAttributes
        {
            get { return (IsPrettyFormat ? "XmlAttributes" : "xmlAtts"); }
        }

        public static string XmlAttribute
        {
            get { return (IsPrettyFormat ? "XmlAttribute" : "xAt"); }
        }
        #endregion



        #region media

        public static string MediaDataUid
        {
            get { return (IsPrettyFormat ? "MediaDataUid" : "medDtUid"); }
        }

        public static string DataProviderItem
        {
            get { return (IsPrettyFormat ? "DataProviderItem" : "dtPrvItm"); }
        }

        public static string DataLength
        {
            get { return (IsPrettyFormat ? "DataLength" : "dtLen"); }
        }

        public static string MimeType
        {
            get { return (IsPrettyFormat ? "MimeType" : "mime"); }
        }

        public static string DataProvider
        {
            get { return (IsPrettyFormat ? "DataProvider" : "dtPrv"); }
        }

        public static string WavClips
        {
            get { return (IsPrettyFormat ? "WavClips" : "wvCls"); }
        }

        public static string WavClip
        {
            get { return (IsPrettyFormat ? "WavClip" : "wvCl"); }
        }

        public static string ClipBegin
        {
            get { return (IsPrettyFormat ? "ClipBegin" : "cB"); }
        }
        public static string ClipEnd
        {
            get { return (IsPrettyFormat ? "ClipEnd" : "cE"); }
        }

        public static string Text
        {
            get { return (IsPrettyFormat ? "Text" : "txt"); }
        }

        public static string MediaDataItem
        {
            get { return (IsPrettyFormat ? "MediaDataItem" : "medDtItm"); }
        }

        public static string Medias
        {
            get { return (IsPrettyFormat ? "Medias" : "meds"); }
        }

        #endregion


        #region channels

        public static string Channel
        {
            get { return (IsPrettyFormat ? "Channel" : "c"); }
        }

        public static string TextChannel
        {
            get { return (IsPrettyFormat ? "TextChannel" : "txCh"); }
        }

        public static string ImageChannel
        {
            get { return (IsPrettyFormat ? "ImageChannel" : "imgCh"); }
        }

        public static string VideoChannel
        {
            get { return (IsPrettyFormat ? "VideoChannel" : "vidCh"); }
        }

        public static string AudioChannel
        {
            get { return (IsPrettyFormat ? "AudioChannel" : "auCh"); }
        }

        public static string AudioXChannel
        {
            get { return (IsPrettyFormat ? "AudioXChannel" : "auXCh"); }
        }

        public static string ChannelMappings
        {
            get { return (IsPrettyFormat ? "ChannelMappings" : "cMps"); }
        }

        public static string ChannelMapping
        {
            get { return (IsPrettyFormat ? "ChannelMapping" : "cM"); }
        }

        public static string TextMedia
        {
            get { return (IsPrettyFormat ? "TextMedia" : "tx"); }
        }

        public static string ExternalImageMedia
        {
            get { return (IsPrettyFormat ? "ExternalImageMedia" : "exImgMed"); }
        }

        public static string ExternalVideoMedia
        {
            get { return (IsPrettyFormat ? "ExternalVideoMedia" : "exVidMed"); }
        }

        public static string ExternalAudioMedia
        {
            get { return (IsPrettyFormat ? "ExternalAudioMedia" : "exAuMed"); }
        }
        public static string ExternalTextMedia
        {
            get { return (IsPrettyFormat ? "ExternalTextMedia" : "exTxtMed"); }
        }

        public static string Property
        {
            get { return (IsPrettyFormat ? "Property" : "prp"); }
        }
        public static string XmlProperty
        {
            get { return (IsPrettyFormat ? "XmlProperty" : "xP"); }
        }
        public static string ChannelsProperty
        {
            get { return (IsPrettyFormat ? "ChannelsProperty" : "cP"); }
        }


        public static string AlternateContentProperty
        {
            get { return (IsPrettyFormat ? "AlternateContentProperty" : "acP"); }
        }
        public static string AlternateContents
        {
            get { return (IsPrettyFormat ? "AlternateContents" : "ACs"); }
        }
        public static string AlternateContent
        {
            get { return (IsPrettyFormat ? "AlternateContent" : "AC"); }
        }

        public static string Description
        {
            get { return (IsPrettyFormat ? "Description" : "Desc"); }
        }

        public static string ManagedAudioMedia
        {
            get { return (IsPrettyFormat ? "ManagedAudioMedia" : "mAu"); }
        }
        public static string TreeNode
        {
            get { return (IsPrettyFormat ? "TreeNode" : "n"); }
        }

        public static string FileDataProvider
        {
            get { return (IsPrettyFormat ? "FileDataProvider" : "fdp"); }
        }
        public static string Presentation
        {
            get { return (IsPrettyFormat ? "Presentation" : "prs"); }
        }
        public static string SequenceMedia
        {
            get { return (IsPrettyFormat ? "SequenceMedia" : "sqMed"); }
        }
        public static string WavAudioMediaData
        {
            get { return (IsPrettyFormat ? "WavAudioMediaData" : "wvAu"); }
        }
        public static string Project
        {
            get { return (IsPrettyFormat ? "Project" : "proj"); }
        }
        public static string CompositeCommand
        {
            get { return (IsPrettyFormat ? "CompositeCommand" : "cmpCmd"); }
        }

        public static string DefaultXmlNamespaceUri
        {
            get { return (IsPrettyFormat ? "DefaultXmlNamespaceUri" : "xmlNS"); }
        }

        public static string ManagedImageMedia
        {
            get { return (IsPrettyFormat ? "ManagedImageMedia" : "mIm"); }
        }
        public static string ManagedVideoMedia
        {
            get { return (IsPrettyFormat ? "ManagedVideoMedia" : "mVd"); }
        }

        public static string MovVideoMediaData
        {
            get { return (IsPrettyFormat ? "MovVideoMediaData" : "movVd"); }
        }
        public static string Mp3AudioMediaData
        {
            get { return (IsPrettyFormat ? "Mp3AudioMediaData" : "mp3Au"); }
        }
        public static string OggAudioMediaData
        {
            get { return (IsPrettyFormat ? "OggAudioMediaData" : "oggAu"); }
        }
        public static string Mp4AudioMediaData
        {
            get { return (IsPrettyFormat ? "Mp4AudioMediaData" : "mp4Au"); }
        }
        public static string MpgVideoMediaData
        {
            get { return (IsPrettyFormat ? "MpgVideoMediaData" : "mpgVd"); }
        }
        public static string Mp4VideoMediaData
        {
            get { return (IsPrettyFormat ? "Mp4VideoMediaData" : "mp4Vd"); }
        }
        public static string WebmVideoMediaData
        {
            get { return (IsPrettyFormat ? "WebmVideoMediaData" : "webmVd"); }
        }
        public static string OggVideoMediaData
        {
            get { return (IsPrettyFormat ? "OggVideoMediaData" : "oggVd"); }
        }
        public static string AviVideoMediaData
        {
            get { return (IsPrettyFormat ? "AviVideoMediaData" : "aviVd"); }
        }
        public static string WmvVideoMediaData
        {
            get { return (IsPrettyFormat ? "WmvVideoMediaData" : "wmvVd"); }
        }

        public static string JpgImageMediaData
        {
            get { return (IsPrettyFormat ? "JpgImageMediaData" : "jpgIm"); }
        }

        public static string PngImageMediaData
        {
            get { return (IsPrettyFormat ? "PngImageMediaData" : "pngIm"); }
        }

        public static string SvgImageMediaData
        {
            get { return (IsPrettyFormat ? "SvgImageMediaData" : "svgIm"); }
        }

        public static string GifImageMediaData
        {
            get { return (IsPrettyFormat ? "GifImageMediaData" : "gifIm"); }
        }

        public static string BmpImageMediaData
        {
            get { return (IsPrettyFormat ? "BmpImageMediaData" : "bmpIm"); }
        }

        public static string OriginalRelativePath
        {
            get { return (IsPrettyFormat ? "OriginalRelativePath" : "orgPth"); }
        }

        public static string OptionalInfo
        {
            get { return (IsPrettyFormat ? "OptionalInfo" : "opInf"); }
        }

        public static string CSSExternalFileData
        {
            get { return (IsPrettyFormat ? "CssExternalFileData" : "cssExFl"); }
        }

        public static string XSLTExternalFileData
        {
            get { return (IsPrettyFormat ? "XsltExternalFileData" : "XsltExFl"); }
        }


        public static string DTDExternalFileData
        {
            get { return (IsPrettyFormat ? "DTDExternalFileData" : "dtdExFl"); }
        }

        //public static string PLSExternalFileData
        //{
        //    get { return (IsPrettyFormat ? "PlsExternalFileData" : "plsExFl"); }
        //}

        public static string NCXExternalFileData
        {
            get { return (IsPrettyFormat ? "NCXExternalFileData" : "NCXExFl"); }
        }

        public static string CoverImageExternalFileData
        {
            get { return (IsPrettyFormat ? "CoverImageExternalFileData" : "CovrImgExFl"); }
        }

        public static string NavDocExternalFileData
        {
            get { return (IsPrettyFormat ? "NavDocExternalFileData" : "NavExFl"); }
        }

        //public static string JSExternalFileData
        //{
        //    get { return (IsPrettyFormat ? "JSExternalFileData" : "JSExFl"); }
        //}
        public static string GenericExternalFileData
        {
            get { return (IsPrettyFormat ? "GenericExternalFileData" : "GenExFl"); }
        }

        public static string ExternalFileDatas
        {
            get { return (IsPrettyFormat ? "ExternalFileDatas" : "exFlDts"); }
        }

        public static string ExternalFileDataItem
        {
            get { return (IsPrettyFormat ? "ExternalFileDataItem" : "exFlDtItm"); }
        }


        public static string IsPreservedForOutputFile
        {
            get { return (IsPrettyFormat ? "IsPreservedForOutputFile" : "preOutFl"); }
        }



        #endregion

        #endregion

    }
}
