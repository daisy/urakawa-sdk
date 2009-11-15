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
        private MetadataDataType m_DataType;
        public MetadataDataType DataType
        {
            get
            {
                return m_DataType;
            }
            set
            {
                m_DataType = value;
            }
        }

        private MetadataOccurrence m_Occurrence;
        public MetadataOccurrence Occurrence
        {
            get
            {
                return m_Occurrence;
            }
            set
            {
                m_Occurrence = value;
            }
        }

        public bool m_IsReadOnly;
        public bool IsReadOnly
        {
            get
            {
                return m_IsReadOnly;
            }
            set
            {
                m_IsReadOnly = value;
            }
        }

        private bool m_IsRepeatable;
        public bool IsRepeatable
        {
            get
            {
                return m_IsRepeatable;
            }
            set
            {
                m_IsRepeatable = value;
            }
        }

        public string m_Name;
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }

        public string m_Description;
        public string Description
        {
            get
            {
                return m_Description;
            }
            set
            {
                m_Description = value;
            }
        }

        private List<string> m_Synonyms;
        public List<string> Synonyms
        {
            get { return m_Synonyms; }
            set { m_Synonyms = value; }
        }

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