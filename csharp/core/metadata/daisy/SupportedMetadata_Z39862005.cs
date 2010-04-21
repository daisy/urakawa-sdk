using System.Collections.Generic;

namespace urakawa.metadata.daisy
{
    //wraps the generic MetadataDefinitionSet
    public static class SupportedMetadata_Z39862005
    {
        public static readonly MetadataDefinitionSet DefinitionSet;

        private static readonly MetadataDefinition m_UnrecognizedItem;
        private static readonly List<string> m_IdentifierSynonyms;
        private static readonly List<string> m_TitleSynonyms;
        private static readonly List<MetadataDefinition> m_MetadataDefinitions;
        public static readonly string MagicStringEmpty = "[EMPTY]";
        
        static SupportedMetadata_Z39862005()
        {
            m_IdentifierSynonyms = new List<string>();
            m_IdentifierSynonyms.Add("dtb:uid");
            m_TitleSynonyms = new List<string>();
            m_TitleSynonyms.Add("dtb:title");

            m_UnrecognizedItem = new MetadataDefinition(
                        "",
                        MetadataDataType.String,
                        MetadataOccurrence.Optional,
                        false,
                        true,
                        UrakawaSDK_core_Lang.Metadata_desc_unrecognized,
                        null);
            
            m_MetadataDefinitions = new List<MetadataDefinition>();
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dc:Date",
                           MetadataDataType.Date,
                           MetadataOccurrence.Required,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcDate,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                            "dtb:sourceDate",
                            MetadataDataType.Date,
                            MetadataOccurrence.Recommended,
                            false,
                            false,
                            UrakawaSDK_core_Lang.Metadata_desc_dtbSourceDate,
                            null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                            "dtb:producedDate",
                            MetadataDataType.Date,
                            MetadataOccurrence.Optional,
                            false,
                            false,
                            UrakawaSDK_core_Lang.Metadata_desc_dtbProducedDate,
                            null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dtb:revisionDate",
                           MetadataDataType.Date,
                           MetadataOccurrence.Optional,
                           false,
                           false,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbRevisionDate,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dc:Title",
                           MetadataDataType.String,
                           MetadataOccurrence.Required,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcTitle,
                           new List<string>(m_TitleSynonyms)));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dc:Publisher",
                           MetadataDataType.String,
                           MetadataOccurrence.Required,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcPublisher,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dc:Language",
                           MetadataDataType.LanguageCode,
                           MetadataOccurrence.Required,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcLanguage,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dc:Identifier",
                           MetadataDataType.String,
                           MetadataOccurrence.Required,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcIdentifier,
                           new List<string>(m_IdentifierSynonyms)));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dc:Creator",
                           MetadataDataType.String,
                           MetadataOccurrence.Recommended,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcCreator,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dc:Subject",
                           MetadataDataType.String,
                           MetadataOccurrence.Recommended,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcSubject,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dc:Description",
                           MetadataDataType.String,
                           MetadataOccurrence.Optional,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcDescription,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dc:Contributor",
                           MetadataDataType.String,
                           MetadataOccurrence.Optional,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcContributor,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dc:Source",
                           MetadataDataType.String,
                           MetadataOccurrence.Recommended,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcSource,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dc:Relation",
                           MetadataDataType.String,
                           MetadataOccurrence.Optional,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcRelation,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dc:Coverage",
                           MetadataDataType.String,
                           MetadataOccurrence.Optional,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcCoverage,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dc:Rights",
                           MetadataDataType.String,
                           MetadataOccurrence.Optional,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcRights,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dtb:sourceEdition",
                           MetadataDataType.String,
                           MetadataOccurrence.Recommended,
                           false,
                           false,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbSourceEdition,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dtb:sourcePublisher",
                           MetadataDataType.String,
                           MetadataOccurrence.Recommended,
                           false,
                           false,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbSourcePublisher,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dtb:sourceRights",
                           MetadataDataType.String,
                           MetadataOccurrence.Recommended,
                           false,
                           false,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbSourceRights,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dtb:sourceTitle",
                           MetadataDataType.String,
                           MetadataOccurrence.Optional,
                           false,
                           false,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbSourceTitle,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dtb:narrator",
                           MetadataDataType.String,
                           MetadataOccurrence.Recommended,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbNarrator,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dtb:producer",
                           MetadataDataType.String,
                           MetadataOccurrence.Optional,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbProducer,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dtb:revisionDescription",
                           MetadataDataType.String,
                           MetadataOccurrence.Optional,
                           false,
                           false,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbRevisionDescription,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dtb:revision",
                           MetadataDataType.Number,
                           MetadataOccurrence.Optional,
                           false,
                           false,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbRevision,
                           null));
             //from mathML
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "z39-86-extension-version",
                           MetadataDataType.Number,
                           MetadataOccurrence.Optional,
                           false,
                           false,
                           UrakawaSDK_core_Lang.Metadata_desc_z3986ExtensionVersion,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "DTBook-XSLTFallback",
                           MetadataDataType.FileUri,
                           MetadataOccurrence.Optional,
                           false,
                           false,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbookXsltFallback,
                           null));
            
            //read-only: Tobi should fill them in for the user
            //things such as audio format might not be known until export
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dc:Format",
                           MetadataDataType.String,
                           MetadataOccurrence.Required,
                           true,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcFormat,
                           null));

            //audioOnly, audioNCX, audioPartText, audioFullText, textPartAudio, textNCX
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dtb:multimediaType",
                           MetadataDataType.String,
                           MetadataOccurrence.Required,
                           true,
                           false,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbMultimediaType,
                           null));
             //audio, text, and image
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dtb:multimediaContent",
                           MetadataDataType.String,
                           MetadataOccurrence.Required,
                           true,
                           false,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbMultimediaContent,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dtb:totalTime",
                           MetadataDataType.ClockValue,
                           MetadataOccurrence.Required,
                           true,
                           false,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbTotalTime,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           "dc:Type",
                           MetadataDataType.String,
                           MetadataOccurrence.Optional,
                           true,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcType,
                           null));
            //MP4-AAC, MP3, WAV
            m_MetadataDefinitions.Add(new MetadataDefinition(
                            "dtb:audioFormat",
                            MetadataDataType.String,
                            MetadataOccurrence.Recommended,
                            true,
                            true,
                            UrakawaSDK_core_Lang.Metadata_desc_dtbAudioFormat,
                            null));

            DefinitionSet = new MetadataDefinitionSet();
            DefinitionSet.UnrecognizedItemFallbackDefinition = m_UnrecognizedItem;
            DefinitionSet.Definitions = m_MetadataDefinitions;
        }      
    }
}