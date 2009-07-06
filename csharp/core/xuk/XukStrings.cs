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
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? XukPretty : XukCompressed); }
        }

        #endregion

        public static string Presentations
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "Presentations" : "prez"); }
        }

        public static string RootNode
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "RootNode" : "root"); }
        }


        public static string RootUri
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "RootUri" : "rootUri"); }
        }

        public static string Metadatas
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "Metadatas" : "mtdts"); }
        }

        public static string MediaDatas
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "MediaDatas" : "medDts"); }
        }


        public static string Height
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "Height" : "h"); }
        }
        public static string Width
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "Width" : "w"); }
        }

        public static string DataProviders
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "DataProviders" : "dataProvs"); }
        }


        #region pcm

        public static string PCMFormatInfo
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "PCMFormatInfo" : "PCMInf"); }
        }

        public static string DefaultPCMFormat
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "DefaultPCMFormat" : "dfltPCM"); }
        }

        public static string NumberOfChannels
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "NumberOfChannels" : "ch"); }
        }
        public static string SampleRate
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "SampleRate" : "rate"); }
        }
        public static string BitDepth
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "BitDepth" : "depth"); }
        }

        public static string enforceSinglePCMFormat
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "enforceSinglePCMFormat" : "frceSglePCM"); }
        }


        public static string PCMFormat
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "PCMFormat" : "PCM"); }
        }

        #endregion


        public static string DataFileDirectoryPath
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "DataFileDirectoryPath" : "dtDir"); }
        }

        public static string DataFileRelativePath
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "DataFileRelativePath" : "dtPath"); }
        }


        #region channels

        public static string ChannelItem
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "ChannelItem" : "chItm"); }
        }

        public static string Channels
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "Channels" : "chs"); }
        }

        #endregion

        #region undo-redo

        public static string UndoStack
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "UndoStack" : "udo"); }
        }

        public static string RedoStack
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "RedoStack" : "rdo"); }
        }

        public static string ActiveTransactions
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "ActiveTransactions" : "trns"); }
        }

        #endregion




        #region factories


        public static string TreeNodeFactory
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "TreeNodeFactory" : "nodFct"); }
        }

        public static string PropertyFactory
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "PropertyFactory" : "prpFct"); }
        }

        public static string ChannelFactory
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "ChannelFactory" : "chFct"); }
        }

        public static string MediaFactory
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "MediaFactory" : "medFct"); }
        }

        public static string MediaDataFactory
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "MediaDataFactory" : "medDtFct"); }
        }

        public static string DataProviderFactory
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "DataProviderFactory" : "dtPrvFct"); }
        }
        public static string CommandFactory
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "CommandFactory" : "cmdFct"); }
        }

        public static string MetadataFactory
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "MetadataFactory" : "metadtFct"); }
        }

        public static string PresentationFactory
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "PresentationFactory" : "presFct"); }
        }

        

        #endregion


        #region managers


        public static string ChannelsManager
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "ChannelsManager" : "chsMan"); }
        }


        public static string MediaDataManager
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "MediaDataManager" : "medDtMan"); }
        }


        public static string DataProviderManager
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "DataProviderManager" : "dtProvMan"); }
        }


        public static string UndoRedoManager
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "UndoRedoManager" : "udoRdoMan"); }
        }



        #endregion

#endregion

#region medium occurence

        public static string Sequence
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "Sequence" : "sq"); }
        }

        public static string AllowMultipleMediaTypes
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "AllowMultipleMediaTypes" : "alwMulTypes"); }
        }


        public static string Metadata
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "Metadata" : "metadt"); }
        }


        #region undo-redo

        public static string ShortDescription
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "ShortDescription" : "shrtDsc"); }
        }

        public static string LongDescription
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "LongDescription" : "lngDsc"); }
        }

        public static string Commands
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "Commands" : "cmds"); }
        }

        #endregion


        #region factories

        public static string RegisteredTypes
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "RegisteredTypes" : "types"); }
        }

        public static string Type
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "Type" : "type"); }
        }

        public static string XukLocalName
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "XukLocalName" : "name"); }
        }

        public static string BaseXukLocalName
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "BaseXukLocalName" : "baseName"); }
        }

        public static string XukNamespaceUri
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "XukNamespaceUri" : "ns"); }
        }

        public static string BaseXukNamespaceUri
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "BaseXukNamespaceUri" : "baseNs"); }
        }

        public static string AssemblyName
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "AssemblyName" : "assbly"); }
        }

        public static string AssemblyVersion
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "AssemblyVersion" : "assblyVer"); }
        }

        public static string FullName
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "FullName" : "fName"); }
        }
        #endregion



        public static string MetaDataName
        {
            //TODO: Special need: same strings because of Dictionary keys in Metadata.cs
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "Name" : "Name"); }
        }
        public static string MetaDataContent
        {
            //TODO: Special need: same strings because of Dictionary keys in Metadata.cs
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "Content" : "Content"); }
        }

        public static string Name
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "Name" : "n"); }
        }

        public static string Value
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "Value" : "v"); }
        }

        public static string LocalName
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "LocalName" : "n"); }
        }

        public static string NamespaceUri
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "NamespaceUri" : "ns"); }
        }


#endregion

#region high occurence


        public static string Language
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "Language" : "lang"); }
        }

        public static string Uid
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "Uid" : "uid"); }
        }

        public static string Src
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "Src" : "s"); }
        }



        #region tree

        public static string Children
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "Children" : "childs"); }
        }

        public static string Properties
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "Properties" : "ps"); }
        }

        #endregion

        #region xml

        public static string XmlAttributes
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "XmlAttributes" : "xmlAtts"); }
        }

        public static string XmlAttribute
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "XmlAttribute" : "xAt"); }
        }
        #endregion



        #region media

        public static string AudioMediaDataUid
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "AudioMediaDataUid" : "medDtUid"); }
        }
        public static string DataProviderItem
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "DataProviderItem" : "dtPrvItm"); }
        }

        public static string DataLength
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "DataLength" : "dtLen"); }
        }

        public static string MimeType
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "MimeType" : "mime"); }
        }

        public static string DataProvider
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "DataProvider" : "dtPrv"); }
        }

        public static string WavClips
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "WavClips" : "wvCls"); }
        }

        public static string WavClip
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "WavClip" : "wvCl"); }
        }

        public static string ClipBegin
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "ClipBegin" : "cB"); }
        }
        public static string ClipEnd
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "ClipEnd" : "cE"); }
        }

        public static string Text
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "Text" : "txt"); }
        }

        public static string MediaDataItem
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "MediaDataItem" : "medDtItm"); }
        }

        #endregion


        #region channels

        public static string Channel
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "Channel" : "c"); }
        }

        public static string TextChannel
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "TextChannel" : "txCh"); }
        }

        public static string ImageChannel
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "ImageChannel" : "imgCh"); }
        }

        public static string AudioChannel
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "AudioChannel" : "auCh"); }
        }

        public static string ChannelMappings
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "ChannelMappings" : "cMps"); }
        }

        public static string ChannelMapping
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "ChannelMapping" : "cM"); }
        }
        
        public static string TextMedia
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "TextMedia" : "tx"); }
        }

        public static string ExternalImageMedia
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "ExternalImageMedia" : "exImgMed"); }
        }

        public static string ExternalVideoMedia
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "ExternalVideoMedia" : "exVidMed"); }
        }

        public static string ExternalAudioMedia
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "ExternalAudioMedia" : "exAuMed"); }
        }
        public static string ExternalTextMedia
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "ExternalTextMedia" : "exTxtMed"); }
        }

        public static string Property
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "Property" : "prp"); }
        }
        public static string XmlProperty
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "XmlProperty" : "xP"); }
        }
        public static string ChannelsProperty
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "ChannelsProperty" : "cP"); }
        }
        public static string ManagedAudioMedia
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "ManagedAudioMedia" : "mAu"); }
        }
        public static string TreeNode
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "TreeNode" : "n"); }
        }

        public static string FileDataProvider
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "FileDataProvider" : "fdp"); }
        }
        public static string Presentation
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "Presentation" : "prs"); }
        }
        public static string SequenceMedia
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "SequenceMedia" : "sqMed"); }
        }
        public static string WavAudioMediaData
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "WavAudioMediaData" : "wvAu"); }
        }
        public static string Project
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "Project" : "proj"); }
        }
        public static string CompositeCommand
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "CompositeCommand" : "cmpCmd"); }
        }

        public static string DefaultXmlNamespaceUri
        {
            get { return ((mProject == null || mProject.IsPrettyFormat()) ? "DefaultXmlNamespaceUri" : "xmlNS"); }
        }

        #endregion

#endregion

    }
}
