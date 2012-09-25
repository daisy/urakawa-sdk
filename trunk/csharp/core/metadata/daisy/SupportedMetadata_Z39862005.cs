using System.Collections.Generic;
using urakawa.data;

namespace urakawa.metadata.daisy
{
    //wraps the generic MetadataDefinitionSet
    public static class SupportedMetadata_Z39862005
    {
        public static readonly string MATHML_XSLT_METADATA = "DTBook-XSLTFallback";
        public static readonly string _z39_86_extension_version = "z39-86-extension-version";
        public static readonly string _builtInMathMLXSLT = "mathml-fallback-transform.xslt";

        
        public const string NS_PREFIX_DUBLIN_CORE = "dc";
        public const string NS_URL_DUBLIN_CORE = "http://purl/dc";

        public const string DC_AccessRights = "dc:accessRights";

        public const string DC_Type = "dc:Type";
        public const string DC_Format = "dc:Format";
        public const string DC_Rights = "dc:Rights";
        public const string DC_Coverage = "dc:Coverage";
        public const string DC_Relation = "dc:Relation";
        public const string D_Source = "dc:Source";
        public const string DC_Contributor = "dc:Contributor";
        public const string DC_Description = "dc:Description";
        public const string DC_Subject = "dc:Subject";
        public const string DC_Creator = "dc:Creator";
        public const string DC_Identifier = "dc:Identifier";
        public const string DC_Language = "dc:Language";
        public const string DC_Publisher = "dc:Publisher";
        public const string DC_Title = "dc:Title";
        public const string DC_Date = "dc:Date";


        public const string NS_PREFIX_DTB = "dtb";

        public const string DTB_GENERATOR = "dtb:generator";

        public const string DTB_UID = "dtb:uid";
        public const string DTB_TITLE = "dtb:title";
        public const string DTB_SOURCE_DATE = "dtb:sourceDate";
        public const string DTB_PRODUCED_DATE = "dtb:producedDate";
        public const string DTB_REVISION_DATE = "dtb:revisionDate";
        public const string DTB_SOURCE_EDITION = "dtb:sourceEdition";
        public const string DTB_SOURCE_PUBLISHER = "dtb:sourcePublisher";
        public const string DTB_SOURCE_RIGHTS = "dtb:sourceRights";
        public const string DTB_SOURCE_TITLE = "dtb:sourceTitle";
        public const string DTB_NARRATOR = "dtb:narrator";
        public const string DTB_PRODUCER = "dtb:producer";
        public const string DTB_REVISION_DESCRIPTION = "dtb:revisionDescription";
        public const string DTB_REVISION = "dtb:revision";
        public const string DTB_MULTIMEDIA_TYPE = "dtb:multimediaType";
        public const string DTB_MULTIMEDIA_CONTENT = "dtb:multimediaContent";
        public const string DTB_TOTAL_TIME = "dtb:totalTime";
        public const string DTB_AUDIO_FORMAT = "dtb:audioFormat";


        public const string DTB_DEPTH = "dtb:depth";
        public const string DTB_TOTAL_PAGE_COUNT = "dtb:totalPageCount";
        public const string DTB_MAX_PAGE_NUMBER = "dtb:maxPageNumber";
        public const string DTB_TOTAL_ELAPSED_TIME = "dtb:totalElapsedTime";
        

        public static readonly MetadataDefinitionSet DefinitionSet;

        private static readonly MetadataDefinition m_UnrecognizedItem;
        private static readonly List<string> m_IdentifierSynonyms;
        private static readonly List<string> m_TitleSynonyms;
        private static readonly List<MetadataDefinition> m_MetadataDefinitions;
        public static readonly string MagicStringEmpty = "[EMPTY]";
        
        static SupportedMetadata_Z39862005()
        {
            MATHML_XSLT_METADATA = FileDataProvider.EliminateForbiddenFileNameCharacters(MATHML_XSLT_METADATA);

            m_IdentifierSynonyms = new List<string>();
            m_IdentifierSynonyms.Add(DTB_UID);
            m_TitleSynonyms = new List<string>();
            m_TitleSynonyms.Add(DTB_TITLE);

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
                           DC_Date,
                           MetadataDataType.Date,
                           MetadataOccurrence.Required,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcDate,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DC_Title,
                           MetadataDataType.String,
                           MetadataOccurrence.Required,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcTitle,
                           new List<string>(m_TitleSynonyms)));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DC_Publisher,
                           MetadataDataType.String,
                           MetadataOccurrence.Required,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcPublisher,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DC_Language,
                           MetadataDataType.LanguageCode,
                           MetadataOccurrence.Required,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcLanguage,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DC_Identifier,
                           MetadataDataType.String,
                           MetadataOccurrence.Required,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcIdentifier,
                           new List<string>(m_IdentifierSynonyms)));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DC_Creator,
                           MetadataDataType.String,
                           MetadataOccurrence.Recommended,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcCreator,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DC_Subject,
                           MetadataDataType.String,
                           MetadataOccurrence.Recommended,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcSubject,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DC_Description,
                           MetadataDataType.String,
                           MetadataOccurrence.Optional,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcDescription,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DC_Contributor,
                           MetadataDataType.String,
                           MetadataOccurrence.Optional,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcContributor,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           D_Source,
                           MetadataDataType.String,
                           MetadataOccurrence.Recommended,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcSource,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DC_Relation,
                           MetadataDataType.String,
                           MetadataOccurrence.Optional,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcRelation,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DC_Coverage,
                           MetadataDataType.String,
                           MetadataOccurrence.Optional,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcCoverage,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DC_Rights,
                           MetadataDataType.String,
                           MetadataOccurrence.Optional,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcRights,
                           null));

            //read-only: Tobi should fill them in for the user
            //things such as audio format might not be known until export
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DC_Format,
                           MetadataDataType.String,
                           MetadataOccurrence.Required,
                           true,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcFormat,
                           null));

            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DC_Type,
                           MetadataDataType.String,
                           MetadataOccurrence.Optional,
                           true,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dcType,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                            DTB_SOURCE_DATE,
                            MetadataDataType.Date,
                            MetadataOccurrence.Recommended,
                            false,
                            false,
                            UrakawaSDK_core_Lang.Metadata_desc_dtbSourceDate,
                            null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                            DTB_PRODUCED_DATE,
                            MetadataDataType.Date,
                            MetadataOccurrence.Optional,
                            false,
                            false,
                            UrakawaSDK_core_Lang.Metadata_desc_dtbProducedDate,
                            null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DTB_REVISION_DATE,
                           MetadataDataType.Date,
                           MetadataOccurrence.Optional,
                           false,
                           false,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbRevisionDate,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DTB_SOURCE_EDITION,
                           MetadataDataType.String,
                           MetadataOccurrence.Recommended,
                           false,
                           false,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbSourceEdition,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DTB_SOURCE_PUBLISHER,
                           MetadataDataType.String,
                           MetadataOccurrence.Recommended,
                           false,
                           false,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbSourcePublisher,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DTB_SOURCE_RIGHTS,
                           MetadataDataType.String,
                           MetadataOccurrence.Recommended,
                           false,
                           false,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbSourceRights,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DTB_SOURCE_TITLE,
                           MetadataDataType.String,
                           MetadataOccurrence.Optional,
                           false,
                           false,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbSourceTitle,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DTB_NARRATOR,
                           MetadataDataType.String,
                           MetadataOccurrence.Recommended,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbNarrator,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DTB_PRODUCER,
                           MetadataDataType.String,
                           MetadataOccurrence.Optional,
                           false,
                           true,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbProducer,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DTB_REVISION_DESCRIPTION,
                           MetadataDataType.String,
                           MetadataOccurrence.Optional,
                           false,
                           false,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbRevisionDescription,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DTB_REVISION,
                           MetadataDataType.Number,
                           MetadataOccurrence.Optional,
                           false,
                           false,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbRevision,
                           null));
             //from mathML
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           _z39_86_extension_version,
                           MetadataDataType.Number,
                           MetadataOccurrence.Optional,
                           false,
                           false,
                           UrakawaSDK_core_Lang.Metadata_desc_z3986ExtensionVersion,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           MATHML_XSLT_METADATA,
                           MetadataDataType.FileUri,
                           MetadataOccurrence.Optional,
                           false,
                           false,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbookXsltFallback,
                           null));
            
            //audioOnly, audioNCX, audioPartText, audioFullText, textPartAudio, textNCX
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DTB_MULTIMEDIA_TYPE,
                           MetadataDataType.String,
                           MetadataOccurrence.Required,
                           true,
                           false,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbMultimediaType,
                           null));
             //audio, text, and image
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DTB_MULTIMEDIA_CONTENT,
                           MetadataDataType.String,
                           MetadataOccurrence.Required,
                           true,
                           false,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbMultimediaContent,
                           null));
            m_MetadataDefinitions.Add(new MetadataDefinition(
                           DTB_TOTAL_TIME,
                           MetadataDataType.ClockValue,
                           MetadataOccurrence.Required,
                           true,
                           false,
                           UrakawaSDK_core_Lang.Metadata_desc_dtbTotalTime,
                           null));

            //MP4-AAC, MP3, WAV
            m_MetadataDefinitions.Add(new MetadataDefinition(
                            DTB_AUDIO_FORMAT,
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