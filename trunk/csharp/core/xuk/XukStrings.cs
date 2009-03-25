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
            get { return (mProject.IsPrettyFormat() ? XukPretty : XukCompressed); }
        }

        #endregion

        public static string Presentations
        {
            get { return (mProject.IsPrettyFormat() ? "Presentations" : "prez"); }
        }

        public static string RootNode
        {
            get { return (mProject.IsPrettyFormat() ? "RootNode" : "root"); }
        }


        public static string RootUri
        {
            get { return (mProject.IsPrettyFormat() ? "RootUri" : "rootUri"); }
        }

        public static string Metadatas
        {
            get { return (mProject.IsPrettyFormat() ? "Metadatas" : "mtdts"); }
        }

        public static string MediaDatas
        {
            get { return (mProject.IsPrettyFormat() ? "MediaDatas" : "medDts"); }
        }


        public static string Height
        {
            get { return (mProject.IsPrettyFormat() ? "Height" : "h"); }
        }
        public static string Width
        {
            get { return (mProject.IsPrettyFormat() ? "Width" : "w"); }
        }

        public static string DataProviders
        {
            get { return (mProject.IsPrettyFormat() ? "DataProviders" : "dataProvs"); }
        }


        #region pcm

        public static string PCMFormatInfo
        {
            get { return (mProject.IsPrettyFormat() ? "PCMFormatInfo" : "PCMInf"); }
        }

        public static string DefaultPCMFormat
        {
            get { return (mProject.IsPrettyFormat() ? "DefaultPCMFormat" : "dfltPCM"); }
        }

        public static string NumberOfChannels
        {
            get { return (mProject.IsPrettyFormat() ? "NumberOfChannels" : "ch"); }
        }
        public static string SampleRate
        {
            get { return (mProject.IsPrettyFormat() ? "SampleRate" : "rate"); }
        }
        public static string BitDepth
        {
            get { return (mProject.IsPrettyFormat() ? "BitDepth" : "depth"); }
        }

        public static string enforceSinglePCMFormat
        {
            get { return (mProject.IsPrettyFormat() ? "enforceSinglePCMFormat" : "frceSglePCM"); }
        }


        public static string PCMFormat
        {
            get { return (mProject.IsPrettyFormat() ? "PCMFormat" : "PCM"); }
        }

        #endregion


        public static string DataFileDirectoryPath
        {
            get { return (mProject.IsPrettyFormat() ? "DataFileDirectoryPath" : "dtDir"); }
        }

        public static string DataFileRelativePath
        {
            get { return (mProject.IsPrettyFormat() ? "DataFileRelativePath" : "dtPath"); }
        }


        #region channels

        public static string ChannelItem
        {
            get { return (mProject.IsPrettyFormat() ? "ChannelItem" : "chItm"); }
        }

        public static string Channels
        {
            get { return (mProject.IsPrettyFormat() ? "Channels" : "chs"); }
        }

        #endregion

        #region undo-redo

        public static string UndoStack
        {
            get { return (mProject.IsPrettyFormat() ? "UndoStack" : "udo"); }
        }

        public static string RedoStack
        {
            get { return (mProject.IsPrettyFormat() ? "RedoStack" : "rdo"); }
        }

        public static string ActiveTransactions
        {
            get { return (mProject.IsPrettyFormat() ? "ActiveTransactions" : "trns"); }
        }

        #endregion




        #region factories


        public static string TreeNodeFactory
        {
            get { return (mProject.IsPrettyFormat() ? "TreeNodeFactory" : "nodFct"); }
        }

        public static string PropertyFactory
        {
            get { return (mProject.IsPrettyFormat() ? "PropertyFactory" : "prpFct"); }
        }

        public static string ChannelFactory
        {
            get { return (mProject.IsPrettyFormat() ? "ChannelFactory" : "chFct"); }
        }

        public static string MediaFactory
        {
            get { return (mProject.IsPrettyFormat() ? "MediaFactory" : "medFct"); }
        }

        public static string MediaDataFactory
        {
            get { return (mProject.IsPrettyFormat() ? "MediaDataFactory" : "medDtFct"); }
        }

        public static string DataProviderFactory
        {
            get { return (mProject.IsPrettyFormat() ? "DataProviderFactory" : "dtPrvFct"); }
        }
        public static string CommandFactory
        {
            get { return (mProject.IsPrettyFormat() ? "CommandFactory" : "cmdFct"); }
        }

        public static string MetadataFactory
        {
            get { return (mProject.IsPrettyFormat() ? "MetadataFactory" : "metadtFct"); }
        }

        public static string PresentationFactory
        {
            get { return (mProject.IsPrettyFormat() ? "PresentationFactory" : "presFct"); }
        }

        

        #endregion


        #region managers


        public static string ChannelsManager
        {
            get { return (mProject.IsPrettyFormat() ? "ChannelsManager" : "chsMan"); }
        }


        public static string MediaDataManager
        {
            get { return (mProject.IsPrettyFormat() ? "MediaDataManager" : "medDtMan"); }
        }


        public static string DataProviderManager
        {
            get { return (mProject.IsPrettyFormat() ? "DataProviderManager" : "dtProvMan"); }
        }


        public static string UndoRedoManager
        {
            get { return (mProject.IsPrettyFormat() ? "UndoRedoManager" : "udoRdoMan"); }
        }



        #endregion

#endregion

#region medium occurence

        public static string Sequence
        {
            get { return (mProject.IsPrettyFormat() ? "Sequence" : "sq"); }
        }

        public static string AllowMultipleMediaTypes
        {
            get { return (mProject.IsPrettyFormat() ? "AllowMultipleMediaTypes" : "alwMulTypes"); }
        }


        public static string Metadata
        {
            get { return (mProject.IsPrettyFormat() ? "Metadata" : "metadt"); }
        }


        #region undo-redo

        public static string ShortDescription
        {
            get { return (mProject.IsPrettyFormat() ? "shortDescription" : "shrtDsc"); }
        }

        public static string LongDescription
        {
            get { return (mProject.IsPrettyFormat() ? "longDescription" : "lngDsc"); }
        }

        public static string Commands
        {
            get { return (mProject.IsPrettyFormat() ? "Commands" : "cmds"); }
        }

        #endregion


        #region factories

        public static string RegisteredTypes
        {
            get { return (mProject.IsPrettyFormat() ? "RegisteredTypes" : "types"); }
        }

        public static string Type
        {
            get { return (mProject.IsPrettyFormat() ? "Type" : "type"); }
        }

        public static string XukLocalName
        {
            get { return (mProject.IsPrettyFormat() ? "XukLocalName" : "name"); }
        }

        public static string BaseXukLocalName
        {
            get { return (mProject.IsPrettyFormat() ? "BaseXukLocalName" : "baseName"); }
        }

        public static string XukNamespaceUri
        {
            get { return (mProject.IsPrettyFormat() ? "XukNamespaceUri" : "ns"); }
        }

        public static string BaseXukNamespaceUri
        {
            get { return (mProject.IsPrettyFormat() ? "BaseXukNamespaceUri" : "baseNs"); }
        }

        public static string AssemblyName
        {
            get { return (mProject.IsPrettyFormat() ? "AssemblyName" : "assbly"); }
        }

        public static string AssemblyVersion
        {
            get { return (mProject.IsPrettyFormat() ? "AssemblyVersion" : "assblyVer"); }
        }

        public static string FullName
        {
            get { return (mProject.IsPrettyFormat() ? "FullName" : "fName"); }
        }
        #endregion


        public static string Name
        {
            get { return (mProject.IsPrettyFormat() ? "Name" : "n"); }
        }

        public static string Value
        {
            get { return (mProject.IsPrettyFormat() ? "Value" : "v"); }
        }

        public static string LocalName
        {
            get { return (mProject.IsPrettyFormat() ? "LocalName" : "n"); }
        }

        public static string NamespaceUri
        {
            get { return (mProject.IsPrettyFormat() ? "NamespaceUri" : "ns"); }
        }


#endregion

#region high occurence


        public static string Language
        {
            get { return (mProject.IsPrettyFormat() ? "Language" : "lang"); }
        }

        public static string Uid
        {
            get { return (mProject.IsPrettyFormat() ? "Uid" : "uid"); }
        }

        public static string Src
        {
            get { return (mProject.IsPrettyFormat() ? "Src" : "s"); }
        }



        #region tree

        public static string Children
        {
            get { return (mProject.IsPrettyFormat() ? "Children" : "childs"); }
        }

        public static string Properties
        {
            get { return (mProject.IsPrettyFormat() ? "Properties" : "ps"); }
        }

        #endregion

        #region xml

        public static string XmlAttributes
        {
            get { return (mProject.IsPrettyFormat() ? "XmlAttributes" : "xmlAtts"); }
        }

        public static string XmlAttribute
        {
            get { return (mProject.IsPrettyFormat() ? "XmlAttribute" : "xAt"); }
        }
        #endregion



        #region media

        public static string AudioMediaDataUid
        {
            get { return (mProject.IsPrettyFormat() ? "AudioMediaDataUid" : "medDtUid"); }
        }
        public static string DataProviderItem
        {
            get { return (mProject.IsPrettyFormat() ? "DataProviderItem" : "dtPrvItm"); }
        }

        public static string DataLength
        {
            get { return (mProject.IsPrettyFormat() ? "DataLength" : "dtLen"); }
        }

        public static string MimeType
        {
            get { return (mProject.IsPrettyFormat() ? "MimeType" : "mime"); }
        }

        public static string DataProvider
        {
            get { return (mProject.IsPrettyFormat() ? "DataProvider" : "dtPrv"); }
        }

        public static string WavClips
        {
            get { return (mProject.IsPrettyFormat() ? "WavClips" : "wvCls"); }
        }

        public static string WavClip
        {
            get { return (mProject.IsPrettyFormat() ? "WavClip" : "wvCl"); }
        }

        public static string ClipBegin
        {
            get { return (mProject.IsPrettyFormat() ? "ClipBegin" : "cB"); }
        }
        public static string ClipEnd
        {
            get { return (mProject.IsPrettyFormat() ? "ClipEnd" : "cE"); }
        }

        public static string Text
        {
            get { return (mProject.IsPrettyFormat() ? "Text" : "txt"); }
        }

        public static string MediaData
        {
            get { return (mProject.IsPrettyFormat() ? "MediaData" : "medDt"); }
        }

        public static string MediaDataItem
        {
            get { return (mProject.IsPrettyFormat() ? "MediaDataItem" : "medDtItm"); }
        }

        #endregion


        #region channels

        public static string Channel
        {
            get { return (mProject.IsPrettyFormat() ? "Channel" : "c"); }
        }

        public static string TextChannel
        {
            get { return (mProject.IsPrettyFormat() ? "TextChannel" : "txCh"); }
        }

        public static string AudioChannel
        {
            get { return (mProject.IsPrettyFormat() ? "AudioChannel" : "auCh"); }
        }

        public static string ChannelMappings
        {
            get { return (mProject.IsPrettyFormat() ? "ChannelMappings" : "cMps"); }
        }

        public static string ChannelMapping
        {
            get { return (mProject.IsPrettyFormat() ? "ChannelMapping" : "cM"); }
        }
        
        public static string TextMedia
        {
            get { return (mProject.IsPrettyFormat() ? "TextMedia" : "tx"); }
        }

        public static string ExternalImageMedia
        {
            get { return (mProject.IsPrettyFormat() ? "ExternalImageMedia" : "exImgMed"); }
        }

        public static string ExternalVideoMedia
        {
            get { return (mProject.IsPrettyFormat() ? "ExternalVideoMedia" : "exVidMed"); }
        }

        public static string ExternalAudioMedia
        {
            get { return (mProject.IsPrettyFormat() ? "ExternalAudioMedia" : "exAuMed"); }
        }
        public static string ExternalTextMedia
        {
            get { return (mProject.IsPrettyFormat() ? "ExternalTextMedia" : "exTxtMed"); }
        }

        public static string Property
        {
            get { return (mProject.IsPrettyFormat() ? "Property" : "prp"); }
        }
        public static string XmlProperty
        {
            get { return (mProject.IsPrettyFormat() ? "XmlProperty" : "xP"); }
        }
        public static string ChannelsProperty
        {
            get { return (mProject.IsPrettyFormat() ? "ChannelsProperty" : "cP"); }
        }
        public static string ManagedAudioMedia
        {
            get { return (mProject.IsPrettyFormat() ? "ManagedAudioMedia" : "mAu"); }
        }
        public static string TreeNode
        {
            get { return (mProject.IsPrettyFormat() ? "TreeNode" : "n"); }
        }

        public static string FileDataProvider
        {
            get { return (mProject.IsPrettyFormat() ? "FileDataProvider" : "fdp"); }
        }
        public static string Presentation
        {
            get { return (mProject.IsPrettyFormat() ? "Presentation" : "prs"); }
        }
        public static string SequenceMedia
        {
            get { return (mProject.IsPrettyFormat() ? "SequenceMedia" : "sqMed"); }
        }
        public static string WavAudioMediaData
        {
            get { return (mProject.IsPrettyFormat() ? "WavAudioMediaData" : "wvAu"); }
        }
        public static string Project
        {
            get { return (mProject.IsPrettyFormat() ? "Project" : "proj"); }
        }
        public static string CompositeCommand
        {
            get { return (mProject.IsPrettyFormat() ? "CompositeCommand" : "cmpCmd"); }
        }

        public static string DefaultXmlNamespaceUri
        {
            get { return (mProject.IsPrettyFormat() ? "DefaultXmlNamespaceUri" : "xmlNS"); }
        }

        #endregion

#endregion

    }
}
