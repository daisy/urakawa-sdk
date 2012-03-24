using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

namespace urakawa.xuk
{
    //todo: add all the XukIn / XukOut strings here !!!!
    public class XukStrings
    {
        private static Project mProject;

        public XukStrings(Project proj)
        {
            mProject = proj;
        }

        public static bool IsPrettyFormat
        {
            get { return mProject.IsPrettyFormat(); }
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
            get { return ((mProject == null || IsPrettyFormat) ? XukPretty : XukCompressed); }
        }

        #endregion

        public static string Presentations
        {
            get { return ((mProject == null || IsPrettyFormat) ? "Presentations" : "prez"); }
        }

        public static string RootNode
        {
            get { return ((mProject == null || IsPrettyFormat) ? "RootNode" : "root"); }
        }


        public static string RootUri
        {
            get { return ((mProject == null || IsPrettyFormat) ? "RootUri" : "rootUri"); }
        }

        public static string Metadatas
        {
            get { return ((mProject == null || IsPrettyFormat) ? "Metadatas" : "mtdts"); }
        }

        public static string MediaDatas
        {
            get { return ((mProject == null || IsPrettyFormat) ? "MediaDatas" : "medDts"); }
        }


        public static string Height
        {
            get { return ((mProject == null || IsPrettyFormat) ? "Height" : "h"); }
        }
        public static string Width
        {
            get { return ((mProject == null || IsPrettyFormat) ? "Width" : "w"); }
        }

        public static string DataProviders
        {
            get { return ((mProject == null || IsPrettyFormat) ? "DataProviders" : "dataProvs"); }
        }


        #region pcm

        public static string PCMFormatInfo
        {
            get { return ((mProject == null || IsPrettyFormat) ? "PCMFormatInfo" : "PCMInf"); }
        }

        public static string DefaultPCMFormat
        {
            get { return ((mProject == null || IsPrettyFormat) ? "DefaultPCMFormat" : "dfltPCM"); }
        }

        public static string NumberOfChannels
        {
            get { return ((mProject == null || IsPrettyFormat) ? "NumberOfChannels" : "ch"); }
        }
        public static string SampleRate
        {
            get { return ((mProject == null || IsPrettyFormat) ? "SampleRate" : "rate"); }
        }
        public static string BitDepth
        {
            get { return ((mProject == null || IsPrettyFormat) ? "BitDepth" : "depth"); }
        }

        public static string enforceSinglePCMFormat
        {
            get { return ((mProject == null || IsPrettyFormat) ? "enforceSinglePCMFormat" : "frceSglePCM"); }
        }

        public static string IsMarked
        {
            get { return ((mProject == null || IsPrettyFormat) ? "IsMarked" : "mrkd"); }
        }

        public static string PCMFormat
        {
            get { return ((mProject == null || IsPrettyFormat) ? "PCMFormat" : "PCM"); }
        }

        #endregion


        public static string DataFileDirectoryPath
        {
            get { return ((mProject == null || IsPrettyFormat) ? "DataFileDirectoryPath" : "dtDir"); }
        }

        public static string DataFileRelativePath
        {
            get { return ((mProject == null || IsPrettyFormat) ? "DataFileRelativePath" : "dtPath"); }
        }


        #region channels

        public static string ChannelItem
        {
            get { return ((mProject == null || IsPrettyFormat) ? "ChannelItem" : "chItm"); }
        }

        public static string Channels
        {
            get { return ((mProject == null || IsPrettyFormat) ? "Channels" : "chs"); }
        }

        #endregion

        #region undo-redo

        public static string UndoStack
        {
            get { return ((mProject == null || IsPrettyFormat) ? "UndoStack" : "udo"); }
        }

        public static string RedoStack
        {
            get { return ((mProject == null || IsPrettyFormat) ? "RedoStack" : "rdo"); }
        }

        public static string ActiveTransactions
        {
            get { return ((mProject == null || IsPrettyFormat) ? "ActiveTransactions" : "trns"); }
        }

        #endregion




        #region factories


        public static string TreeNodeFactory
        {
            get { return ((mProject == null || IsPrettyFormat) ? "TreeNodeFactory" : "nodFct"); }
        }

        public static string PropertyFactory
        {
            get { return ((mProject == null || IsPrettyFormat) ? "PropertyFactory" : "prpFct"); }
        }

        public static string ChannelFactory
        {
            get { return ((mProject == null || IsPrettyFormat) ? "ChannelFactory" : "chFct"); }
        }

        public static string MediaFactory
        {
            get { return ((mProject == null || IsPrettyFormat) ? "MediaFactory" : "medFct"); }
        }

        public static string MediaDataFactory
        {
            get { return ((mProject == null || IsPrettyFormat) ? "MediaDataFactory" : "medDtFct"); }
        }

        public static string DataProviderFactory
        {
            get { return ((mProject == null || IsPrettyFormat) ? "DataProviderFactory" : "dtPrvFct"); }
        }
        public static string CommandFactory
        {
            get { return ((mProject == null || IsPrettyFormat) ? "CommandFactory" : "cmdFct"); }
        }

        public static string MetadataFactory
        {
            get { return ((mProject == null || IsPrettyFormat) ? "MetadataFactory" : "metadtFct"); }
        }

        public static string PresentationFactory
        {
            get { return ((mProject == null || IsPrettyFormat) ? "PresentationFactory" : "presFct"); }
        }

        public static string ExternalFileDataFactory
            {
            get { return ((mProject == null || mProject.IsPrettyFormat ()) ? "ExternalFileDataFactory" : "ExFlDtFct"); }
            }

        public static string AlternateContentFactory
        {
            get { return ((mProject == null || IsPrettyFormat) ? "AlternateContentFactory" : "acFct"); }
        }


        #endregion


        #region commands


        public static string TreeNodeChangeTextCommand
        {
            get { return ((mProject == null || IsPrettyFormat) ? "TreeNodeChangeTextCommand" : "nodChTxtCmd"); }
        }

        public static string TreeNodeSetIsMarkedCommand
        {
            get { return ((mProject == null || IsPrettyFormat) ? "TreeNodeSetIsMarkedCommand" : "nodSetMrkCmd"); }
        }

        public static string TreeNodeSetManagedAudioMediaCommand
        {
            get { return ((mProject == null || IsPrettyFormat) ? "TreeNodeSetManagedAudioMediaCommand" : "nodSetManAudMedCmd"); }
        }

        public static string ManagedAudioMediaInsertDataCommand
        {
            get { return ((mProject == null || IsPrettyFormat) ? "ManagedAudioMediaInsertDataCommand" : "manAudMedInsertCmd"); }
        }

        public static string TreeNodeAudioStreamDeleteCommand
        {
            get { return ((mProject == null || IsPrettyFormat) ? "TreeNodeAudioStreamDeleteCommand" : "nodAudDelCmd"); }
        }

        public static string MetadataAddCommand
        {
            get { return ((mProject == null || IsPrettyFormat) ? "MetadataAddCommand" : "metaAddCmd"); }
        }
        public static string MetadataRemoveCommand
        {
            get { return ((mProject == null || IsPrettyFormat) ? "MetadataRemoveCommand" : "metaRemoveCmd"); }
        }
        public static string MetadataSetNameCommand
        {
            get { return ((mProject == null || IsPrettyFormat) ? "MetadataSetNameCommand" : "metaSetNameCmd"); }
        }
        public static string MetadataSetContentCommand
        {
            get { return ((mProject == null || IsPrettyFormat) ? "MetadataSetContentCommand" : "metaSetContentCmd"); }
        }
        public static string MetadataSetIdCommand
        {
            get { return ((mProject == null || IsPrettyFormat) ? "MetadataSetIdCommand" : "metaSetIdCmd"); }
        }

        public static string AlternateContentMetadataAttributeAddCommand
        {
            get { return ((mProject == null || IsPrettyFormat) ? "MetadataAttributeAddCommand" : "metaAttrAddCmd"); }
        }

        public static string AlternateContentMetadataAttributeRemoveCommand
        {
            get { return ((mProject == null || IsPrettyFormat) ? "MetadataAttributeRemoveCommand" : "metaAttrRemoveCmd"); }
        }

        public static string AlternateContentMetadataAttributeSetNameCommand
        {
            get { return ((mProject == null || IsPrettyFormat) ? "MetadataAttributeSetNameCommand" : "metaAttrNameSetCmd"); }
        }
        public static string AlternateContentMetadataAttributeSetContentCommand
        {
            get { return ((mProject == null || IsPrettyFormat) ? "MetadataAttributeSetContentCommand" : "metaAttrContentSetCmd"); }
        }

        public static string AlternateContentMetadataAddCommand
        {
            get { return ((mProject == null || IsPrettyFormat) ? "AlternateContentMetadataAddCommand" : "acMetaAddCmd"); }
        }
        public static string AlternateContentMetadataRemoveCommand
        {
            get { return ((mProject == null || IsPrettyFormat) ? "AlternateContentMetadataRemoveCommand" : "acMetaRemoveCmd"); }
        }
        public static string AlternateContentMetadataSetNameCommand
        {
            get { return ((mProject == null || IsPrettyFormat) ? "AlternateContentMetadataSetNameCommand" : "acMetaSetNameCmd"); }
        }
        public static string AlternateContentMetadataSetContentCommand
        {
            get { return ((mProject == null || IsPrettyFormat) ? "AlternateContentMetadataSetContentCommand" : "acMetaSetContentCmd"); }
        }
        public static string AlternateContentMetadataSetIdCommand
        {
            get { return ((mProject == null || IsPrettyFormat) ? "AlternateContentMetadataSetIdCommand" : "acMetaSetIdCmd"); }
        }

        public static string AlternateContentSetManagedMediaCommand
        {
            get { return ((mProject == null || IsPrettyFormat) ? "AlternateContentSetManagedMediaCommand" : "acSetManMedCmd"); }
        }
        public static string AlternateContentRemoveManagedMediaCommand
        {
            get { return ((mProject == null || IsPrettyFormat) ? "AlternateContentRemoveManagedMediaCommand" : "acRemManMedCmd"); }
        }

        public static string AlternateContentAddCommand
        {
            get { return ((mProject == null || IsPrettyFormat) ? "AlternateContentAddCommand" : "acAddCmd"); }
        }
        public static string AlternateContentRemoveCommand
        {
            get { return ((mProject == null || IsPrettyFormat) ? "AlternateContentRemoveCommand" : "acRemCmd"); }
        }

        #endregion

        #region managers


        public static string ChannelsManager
        {
            get { return ((mProject == null || IsPrettyFormat) ? "ChannelsManager" : "chsMan"); }
        }


        public static string MediaDataManager
        {
            get { return ((mProject == null || IsPrettyFormat) ? "MediaDataManager" : "medDtMan"); }
        }


        public static string DataProviderManager
        {
            get { return ((mProject == null || IsPrettyFormat) ? "DataProviderManager" : "dtProvMan"); }
        }


        public static string UndoRedoManager
        {
            get { return ((mProject == null || IsPrettyFormat) ? "UndoRedoManager" : "udoRdoMan"); }
        }

        public static string ExternalFileDataManager
            {
            get { return ((mProject == null || mProject.IsPrettyFormat ()) ? "ExternalFileDataManager" : "ExFlDtMan"); }
            }



        #endregion

        #endregion

        #region medium occurence

        public static string Sequence
        {
            get { return ((mProject == null || IsPrettyFormat) ? "Sequence" : "sq"); }
        }

        public static string AllowMultipleMediaTypes
        {
            get { return ((mProject == null || IsPrettyFormat) ? "AllowMultipleMediaTypes" : "alwMulTypes"); }
        }



        #region undo-redo

        public static string ShortDescription
        {
            get { return ((mProject == null || IsPrettyFormat) ? "ShortDescription" : "shrtDsc"); }
        }

        public static string LongDescription
        {
            get { return ((mProject == null || IsPrettyFormat) ? "LongDescription" : "lngDsc"); }
        }

        public static string Commands
        {
            get { return ((mProject == null || IsPrettyFormat) ? "Commands" : "cmds"); }
        }

        #endregion


        #region factories

        public static string RegisteredTypes
        {
            get { return ((mProject == null || IsPrettyFormat) ? "RegisteredTypes" : "types"); }
        }

        public static string Type
        {
            get { return ((mProject == null || IsPrettyFormat) ? "Type" : "type"); }
        }

        public static string XukLocalName
        {
            get { return ((mProject == null || IsPrettyFormat) ? "XukLocalName" : "name"); }
        }

        public static string BaseXukLocalName
        {
            get { return ((mProject == null || IsPrettyFormat) ? "BaseXukLocalName" : "baseName"); }
        }

        public static string XukNamespaceUri
        {
            get { return ((mProject == null || IsPrettyFormat) ? "XukNamespaceUri" : "ns"); }
        }

        public static string BaseXukNamespaceUri
        {
            get { return ((mProject == null || IsPrettyFormat) ? "BaseXukNamespaceUri" : "baseNs"); }
        }

        public static string AssemblyName
        {
            get { return ((mProject == null || IsPrettyFormat) ? "AssemblyName" : "assbly"); }
        }

        public static string AssemblyVersion
        {
            get { return ((mProject == null || IsPrettyFormat) ? "AssemblyVersion" : "assblyVer"); }
        }

        public static string FullName
        {
            get { return ((mProject == null || IsPrettyFormat) ? "FullName" : "fName"); }
        }
        #endregion


        public static string Metadata
        {
            get { return ((mProject == null || IsPrettyFormat) ? "Metadata" : "metadt"); }
        }
        public static string MetadataAttribute
        {
            get { return ((mProject == null || IsPrettyFormat) ? "MetadataAttribute" : "metadtattr"); }
        }
        public static string MetadataOtherAttributes
        {
            get { return ((mProject == null || IsPrettyFormat) ? "MetadataOtherAttributes" : "metadtattrs"); }
        }

        public static string Name
        {
            get { return ((mProject == null || IsPrettyFormat) ? "Name" : "n"); }
        }

        public static string Value
        {
            get { return ((mProject == null || IsPrettyFormat) ? "Value" : "v"); }
        }

        public static string LocalName
        {
            get { return ((mProject == null || IsPrettyFormat) ? "LocalName" : "n"); }
        }

        public static string NamespaceUri
        {
            get { return ((mProject == null || IsPrettyFormat) ? "NamespaceUri" : "ns"); }
        }


        #endregion

        #region high occurence


        public static string Language
        {
            get { return ((mProject == null || IsPrettyFormat) ? "Language" : "lang"); }
        }

        public static string Uid
        {
            get { return ((mProject == null || IsPrettyFormat) ? "Uid" : "uid"); }
        }

        public static string Src
        {
            get { return ((mProject == null || IsPrettyFormat) ? "Src" : "s"); }
        }



        #region tree

        public static string Children
        {
            get { return ((mProject == null || IsPrettyFormat) ? "Children" : "childs"); }
        }

        public static string Properties
        {
            get { return ((mProject == null || IsPrettyFormat) ? "Properties" : "ps"); }
        }

        #endregion

        #region xml

        public static string XmlAttributes
        {
            get { return ((mProject == null || IsPrettyFormat) ? "XmlAttributes" : "xmlAtts"); }
        }

        public static string XmlAttribute
        {
            get { return ((mProject == null || IsPrettyFormat) ? "XmlAttribute" : "xAt"); }
        }
        #endregion



        #region media

        public static string MediaDataUid
        {
            get { return ((mProject == null || IsPrettyFormat) ? "MediaDataUid" : "medDtUid"); }
        }

        public static string DataProviderItem
        {
            get { return ((mProject == null || IsPrettyFormat) ? "DataProviderItem" : "dtPrvItm"); }
        }

        public static string DataLength
        {
            get { return ((mProject == null || IsPrettyFormat) ? "DataLength" : "dtLen"); }
        }

        public static string MimeType
        {
            get { return ((mProject == null || IsPrettyFormat) ? "MimeType" : "mime"); }
        }

        public static string DataProvider
        {
            get { return ((mProject == null || IsPrettyFormat) ? "DataProvider" : "dtPrv"); }
        }

        public static string WavClips
        {
            get { return ((mProject == null || IsPrettyFormat) ? "WavClips" : "wvCls"); }
        }

        public static string WavClip
        {
            get { return ((mProject == null || IsPrettyFormat) ? "WavClip" : "wvCl"); }
        }

        public static string ClipBegin
        {
            get { return ((mProject == null || IsPrettyFormat) ? "ClipBegin" : "cB"); }
        }
        public static string ClipEnd
        {
            get { return ((mProject == null || IsPrettyFormat) ? "ClipEnd" : "cE"); }
        }

        public static string Text
        {
            get { return ((mProject == null || IsPrettyFormat) ? "Text" : "txt"); }
        }

        public static string MediaDataItem
        {
            get { return ((mProject == null || IsPrettyFormat) ? "MediaDataItem" : "medDtItm"); }
        }

        public static string Medias
        {
            get { return ((mProject == null || IsPrettyFormat) ? "Medias" : "meds"); }
        }

        #endregion


        #region channels

        public static string Channel
        {
            get { return ((mProject == null || IsPrettyFormat) ? "Channel" : "c"); }
        }

        public static string TextChannel
        {
            get { return ((mProject == null || IsPrettyFormat) ? "TextChannel" : "txCh"); }
        }

        public static string ImageChannel
        {
            get { return ((mProject == null || IsPrettyFormat) ? "ImageChannel" : "imgCh"); }
        }

        public static string VideoChannel
        {
            get { return ((mProject == null || IsPrettyFormat) ? "VideoChannel" : "vidCh"); }
        }

        public static string AudioChannel
        {
            get { return ((mProject == null || IsPrettyFormat) ? "AudioChannel" : "auCh"); }
        }

        public static string ChannelMappings
        {
            get { return ((mProject == null || IsPrettyFormat) ? "ChannelMappings" : "cMps"); }
        }

        public static string ChannelMapping
        {
            get { return ((mProject == null || IsPrettyFormat) ? "ChannelMapping" : "cM"); }
        }

        public static string TextMedia
        {
            get { return ((mProject == null || IsPrettyFormat) ? "TextMedia" : "tx"); }
        }

        public static string ExternalImageMedia
        {
            get { return ((mProject == null || IsPrettyFormat) ? "ExternalImageMedia" : "exImgMed"); }
        }

        public static string ExternalVideoMedia
        {
            get { return ((mProject == null || IsPrettyFormat) ? "ExternalVideoMedia" : "exVidMed"); }
        }

        public static string ExternalAudioMedia
        {
            get { return ((mProject == null || IsPrettyFormat) ? "ExternalAudioMedia" : "exAuMed"); }
        }
        public static string ExternalTextMedia
        {
            get { return ((mProject == null || IsPrettyFormat) ? "ExternalTextMedia" : "exTxtMed"); }
        }

        public static string Property
        {
            get { return ((mProject == null || IsPrettyFormat) ? "Property" : "prp"); }
        }
        public static string XmlProperty
        {
            get { return ((mProject == null || IsPrettyFormat) ? "XmlProperty" : "xP"); }
        }
        public static string ChannelsProperty
        {
            get { return ((mProject == null || IsPrettyFormat) ? "ChannelsProperty" : "cP"); }
        }


        public static string AlternateContentProperty
        {
            get { return ((mProject == null || IsPrettyFormat) ? "AlternateContentProperty" : "acP"); }
        }
        public static string AlternateContents
        {
            get { return ((mProject == null || IsPrettyFormat) ? "AlternateContents" : "ACs"); }
        }
        public static string AlternateContent
        {
            get { return ((mProject == null || IsPrettyFormat) ? "AlternateContent" : "AC"); }
        }

        public static string Description
        {
            get { return ((mProject == null || IsPrettyFormat) ? "Description" : "Desc"); }
        }

        public static string ManagedAudioMedia
        {
            get { return ((mProject == null || IsPrettyFormat) ? "ManagedAudioMedia" : "mAu"); }
        }
        public static string TreeNode
        {
            get { return ((mProject == null || IsPrettyFormat) ? "TreeNode" : "n"); }
        }

        public static string FileDataProvider
        {
            get { return ((mProject == null || IsPrettyFormat) ? "FileDataProvider" : "fdp"); }
        }
        public static string Presentation
        {
            get { return ((mProject == null || IsPrettyFormat) ? "Presentation" : "prs"); }
        }
        public static string SequenceMedia
        {
            get { return ((mProject == null || IsPrettyFormat) ? "SequenceMedia" : "sqMed"); }
        }
        public static string WavAudioMediaData
        {
            get { return ((mProject == null || IsPrettyFormat) ? "WavAudioMediaData" : "wvAu"); }
        }
        public static string Project
        {
            get { return ((mProject == null || IsPrettyFormat) ? "Project" : "proj"); }
        }
        public static string CompositeCommand
        {
            get { return ((mProject == null || IsPrettyFormat) ? "CompositeCommand" : "cmpCmd"); }
        }

        public static string DefaultXmlNamespaceUri
        {
            get { return ((mProject == null || IsPrettyFormat) ? "DefaultXmlNamespaceUri" : "xmlNS"); }
        }

        public static string ManagedImageMedia
        {
            get { return ((mProject == null || IsPrettyFormat) ? "ManagedImageMedia" : "mIm"); }
        }
        public static string ManagedVideoMedia
        {
            get { return ((mProject == null || IsPrettyFormat) ? "ManagedVideoMedia" : "mVd"); }
        }

        public static string MovVideoMediaData
        {
            get { return ((mProject == null || IsPrettyFormat) ? "MovVideoMediaData" : "movVd"); }
        }
        public static string MpgVideoMediaData
        {
            get { return ((mProject == null || IsPrettyFormat) ? "MpgVideoMediaData" : "mpgVd"); }
        }
        public static string Mp4VideoMediaData
        {
            get { return ((mProject == null || IsPrettyFormat) ? "Mp4VideoMediaData" : "mp4Vd"); }
        }
        public static string WebmVideoMediaData
        {
            get { return ((mProject == null || IsPrettyFormat) ? "WebmVideoMediaData" : "webmVd"); }
        }
        public static string OggVideoMediaData
        {
            get { return ((mProject == null || IsPrettyFormat) ? "OggVideoMediaData" : "oggVd"); }
        }
        public static string AviVideoMediaData
        {
            get { return ((mProject == null || IsPrettyFormat) ? "AviVideoMediaData" : "aviVd"); }
        }
        public static string WmvVideoMediaData
        {
            get { return ((mProject == null || IsPrettyFormat) ? "WmvVideoMediaData" : "wmvVd"); }
        }

        public static string JpgImageMediaData
        {
            get { return ((mProject == null || IsPrettyFormat) ? "JpgImageMediaData" : "jpgIm"); }
        }

        public static string PngImageMediaData
        {
            get { return ((mProject == null || IsPrettyFormat) ? "PngImageMediaData" : "pngIm"); }
        }

        public static string SvgImageMediaData
        {
            get { return ((mProject == null || IsPrettyFormat) ? "SvgImageMediaData" : "svgIm"); }
        }

        public static string GifImageMediaData
        {
            get { return ((mProject == null || IsPrettyFormat) ? "GifImageMediaData" : "gifIm"); }
        }

        public static string BmpImageMediaData
        {
            get { return ((mProject == null || IsPrettyFormat) ? "BmpImageMediaData" : "bmpIm"); }
        }

        public static string OriginalRelativePath
        {
            get { return ((mProject == null || IsPrettyFormat) ? "OriginalRelativePath" : "orgPth"); }
        }

        public static string CSSExternalFileData
        {
            get { return ((mProject == null || IsPrettyFormat) ? "CssExternalFileData" : "cssExFl"); }
                }

        public static string XSLTExternalFileData
        {
        get { return ((mProject == null || mProject.IsPrettyFormat ()) ? "XsltExternalFileData" : "XsltExFl"); }
                }
        

        public static string DTDExternalFileData
            {
            get { return ((mProject == null || mProject.IsPrettyFormat ()) ? "DTDExternalFileData" : "dtdExFl"); }
            }

        public static string ExternalFileDatas
            {
            get { return ((mProject == null || mProject.IsPrettyFormat ()) ? "ExternalFileDatas" : "exFlDts"); }
            }

        public static string ExternalFileDataItem
            {
            get { return ((mProject == null || mProject.IsPrettyFormat ()) ? "ExternalFileDataItem" : "exFlDtItm"); }
            }


        public static string IsPreservedForOutputFile
            {
            get { return ((mProject == null || mProject.IsPrettyFormat ()) ? "IsPreservedForOutputFile" : "preOutFl"); }
            }

        

        #endregion

        #endregion

    }
}
