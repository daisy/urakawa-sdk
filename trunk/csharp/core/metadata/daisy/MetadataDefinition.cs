using System;
using System.Collections.Generic;

namespace urakawa.metadata.daisy
{   
    public enum MetadataDataType
    {
        String,
        Integer,
        Double,
        Number,
        ClockValue,
        LanguageCode,
        Date,
        FileUri
    } 

    public enum MetadataOccurrence
    {
        Required,
        Recommended,
        Optional
    } 

    /// <summary>
    /// Defines a type of metadata item (e.g. string, date, required, optional, etc)
    /// </summary>
    public class MetadataDefinition
    {
        public MetadataDataType DataType { get; set; }
        public MetadataOccurrence Occurrence { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsRepeatable { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Synonyms { get; set; }

        public MetadataDefinition(string name, MetadataDataType dataType,
            MetadataOccurrence occurrence, bool isReadOnly, bool isRepeatable, string description, 
            List<string> synonyms)
        {
            Name = name;
            DataType = dataType;
            Occurrence = occurrence;
            IsReadOnly = isReadOnly;
            IsRepeatable = isRepeatable;
            Description = description;
            Synonyms = synonyms;
        }
    }
}