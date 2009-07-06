using System;
using System.Collections.Generic;

namespace urakawa.metadata.daisy
{
    public static class SupportedMetadata_Z39862005
    {
        public static readonly MetadataDefinition AlienMetadata = new MetadataDefinition(
            "",
            MetadataDataType.String,
            MetadataOccurrence.Optional,
            false,
            true,
            "An unrecognized metadata item",
            null);

        public static readonly List<MetadataDefinition> MetadataList =
            new List<MetadataDefinition>
                    {
                        new MetadataDefinition(
                            "dc:Date",
                            MetadataDataType.Date,
                            MetadataOccurrence.Required,
                            false,
                            true,
                            "Date of publication of the DTB. ",
                            null),
                        new MetadataDefinition(
                            "dtb:sourceDate",
                            MetadataDataType.Date,
                            MetadataOccurrence.Recommended,
                            false,
                            false,
                            "Date of publication of the resource (e.g., a print original, ebook, etc.) from which the DTB is derived.",
                            null),
                        new MetadataDefinition(
                            "dtb:producedDate",
                            MetadataDataType.Date,
                            MetadataOccurrence.Optional,
                            false,
                            false,
                            "Date of first generation of the complete DTB, i.e. Production completion date.",
                            null),
                        new MetadataDefinition(
                            "dtb:revisionDate",
                            MetadataDataType.Date,
                            MetadataOccurrence.Optional,
                            false,
                            false,
                            "Date associated with the specific dtb:revision.",
                            null),
                        new MetadataDefinition(
                            "dc:Title",
                            MetadataDataType.String,
                            MetadataOccurrence.Required,
                            false,
                            true,
                            "The title of the DTB, including any subtitles.",
                            null),
                        new MetadataDefinition(
                            "dc:Publisher",
                            MetadataDataType.String,
                            MetadataOccurrence.Required,
                            false,
                            true,
                            "The agency responsible for making the DTB available.",
                            null),
                        new MetadataDefinition(
                            "dc:Language",
                            MetadataDataType.LanguageCode,
                            MetadataOccurrence.Required,
                            false,
                            true,
                            "Language of the content of the publication.",
                            null),
                        new MetadataDefinition(
                            "dc:Identifier",
                            MetadataDataType.String,
                            MetadataOccurrence.Required,
                            false,
                            true,
                            "A string or number identifying the DTB.",
                            new List<string>{"dtb:uid"}),
                        new MetadataDefinition(
                            "dc:Creator",
                            MetadataDataType.String,
                            MetadataOccurrence.Recommended,
                            false,
                            true,
                            "Names of primary author or creator of the intellectual content of the publication.",
                            null),
                        new MetadataDefinition(
                            "dc:Subject",
                            MetadataDataType.String,
                            MetadataOccurrence.Recommended,
                            false,
                            true,
                            "The topic of the content of the publication.",
                            null),
                        new MetadataDefinition(
                            "dc:Description",
                            MetadataDataType.String,
                            MetadataOccurrence.Optional,
                            false,
                            true,
                            "Plain text describing the publication's content.",
                            null),
                        new MetadataDefinition(
                            "dc:Contributor",
                            MetadataDataType.String,
                            MetadataOccurrence.Optional,
                            false,
                            true,
                            "A party whose contribution to the publication is secondary to those named in dc:Creator.",
                            null),
                        new MetadataDefinition(
                            "dc:Source",
                            MetadataDataType.String,
                            MetadataOccurrence.Recommended,
                            false,
                            true,
                            "A reference to a resource (e.g., a print original, ebook, etc.) from which the DTB is derived. Best practice is to use the ISBN when available.",
                            null),
                        new MetadataDefinition(
                            "dc:Relation",
                            MetadataDataType.String,
                            MetadataOccurrence.Optional,
                            false,
                            true,
                            "A reference to a related resource.",
                            null),
                        new MetadataDefinition(
                            "dc:Coverage",
                            MetadataDataType.String,
                            MetadataOccurrence.Optional,
                            false,
                            true,
                            "The extent or scope of the content of the resource.",
                            null),
                        new MetadataDefinition(
                            "dc:Rights",
                            MetadataDataType.String,
                            MetadataOccurrence.Optional,
                            false,
                            true,
                            "Information about rights held in and over the DTB.",
                            null),
                        new MetadataDefinition(
                            "dtb:sourceEdition",
                            MetadataDataType.String,
                            MetadataOccurrence.Recommended,
                            false,
                            false,
                            "A string describing the edition of the resource (e.g., a print original, ebook, etc.) from which the DTB is derived.",
                            null),
                        new MetadataDefinition(
                            "dtb:sourcePublisher",
                            MetadataDataType.String,
                            MetadataOccurrence.Recommended,
                            false,
                            false,
                            "The agency responsible for making available the resource (e.g., a print original, ebook, etc.) from which the DTB is derived.",
                            null),
                        new MetadataDefinition(
                            "dtb:sourceRights",
                            MetadataDataType.String,
                            MetadataOccurrence.Recommended,
                            false,
                            false,
                            "Information about rights held in and over the resource (e.g., a print original, ebook, etc.) from which the DTB is derived.",
                            null),
                        new MetadataDefinition(
                            "dtb:sourceTitle",
                            MetadataDataType.String,
                            MetadataOccurrence.Optional,
                            false,
                            false,
                            "The title of the resource (e.g., a print original, ebook, etc.) from which the DTB is derived. To be used only if different from dc:Title.",
                            null),
                        new MetadataDefinition(
                            "dtb:narrator",
                            MetadataDataType.String,
                            MetadataOccurrence.Recommended,
                            false,
                            true,
                            "Name of the person whose recorded voice is embodied in the DTB.",
                            null),
                        new MetadataDefinition(
                            "dtb:producer",
                            MetadataDataType.String,
                            MetadataOccurrence.Optional,
                            false,
                            true,
                            "Name of the organization/production unit that created the DTB.",
                            null),
                        new MetadataDefinition(
                            "dtb:revisionDescription",
                            MetadataDataType.String,
                            MetadataOccurrence.Optional,
                            false,
                            false,
                            "The changes introduced in a specific dtb:revision",
                            null),
                        new MetadataDefinition(
                            "dtb:revision",
                            MetadataDataType.Number,
                            MetadataOccurrence.Optional,
                            false,
                            false,
                            "Non-negative integer value of the specific version of the DTB. Incremented each time the DTB is revised.",
                            null),
                        //from mathML
                        new MetadataDefinition(
                            "z39-86-extension-version",
                            MetadataDataType.Number,
                            MetadataOccurrence.Optional,
                            false,
                            false,
                            "The version of the extension to the core Z39.86 specification.",
                            null),
                        new MetadataDefinition(
                            "DTBook-XSLTFallback",
                            MetadataDataType.FileUri,
                            MetadataOccurrence.Optional,
                            false,
                            false,
                            "The fallback XSLT file",
                            null),

                        //read-only: Tobi should fill them in for the user
                        //things such as audio format might not be known until export
                        new MetadataDefinition(
                            "dc:Format",
                            MetadataDataType.String,
                            MetadataOccurrence.Required,
                            true,
                            true,
                            "The standard or specification to which the DTB was produced.",
                            null),
                        //audioOnly, audioNCX, audioPartText, audioFullText, textPartAudio, textNCX
                        new MetadataDefinition(
                            "dtb:multimediaType",
                            MetadataDataType.String,
                            MetadataOccurrence.Required,
                            true,
                            false,
                            "One of the six types of DTB defined in the Structure Guidelines.",
                            null),
                        //audio, text, and image
                        new MetadataDefinition(
                            "dtb:multimediaContent",
                            MetadataDataType.String,
                            MetadataOccurrence.Required,
                            true,
                            false,
                            "Summary of the general types of media used in the content of this DTB.",
                            null),
                        new MetadataDefinition(
                            "dtb:totalTime",
                            MetadataDataType.ClockValue,
                            MetadataOccurrence.Required,
                            true,
                            false,
                            "Total playing time of all SMIL files comprising the content of the DTB.",
                            null),
                        new MetadataDefinition(
                            "dc:Type",
                            MetadataDataType.String,
                            MetadataOccurrence.Optional,
                            true,
                            true,
                            "The nature of the content of the DTB (recommended are Dublin Core keywords \"audio\", \"text\", and \"image\").",
                            null),
                        //MP4-AAC, MP3, WAV
                        new MetadataDefinition(
                            "dtb:audioFormat",
                            MetadataDataType.String,
                            MetadataOccurrence.Recommended,
                            true,
                            true,
                            "The format in which the audio files in the DTB file set are written.",
                            null)

                            };
    }
    
}