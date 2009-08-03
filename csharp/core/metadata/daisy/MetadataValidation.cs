using System;
using System.Collections.Generic;

namespace urakawa.metadata.daisy
{
    public class MetadataValidationReportItem
    {
        //what it is
        private Metadata m_Metadata;
        public Metadata Metadata
        {
            get
            {
                return m_Metadata;
            }
            set
            {
                m_Metadata = value;
            }
        }
        //what went wrong
        private string m_Description;
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

        public MetadataValidationReportItem(Metadata metadata, string description)
        {
            Metadata = metadata;
            Description = description;
        }
    }

    public class MetadataValidation
    {
        public delegate void ValidationError(MetadataValidationReportItem error);
        public event ValidationError ValidationErrorEvent;
        private void NotifyValidationError(MetadataValidationReportItem error)
        {
            if (ValidationErrorEvent != null)
                ValidationErrorEvent(error);
        }

        private List<MetadataDefinition> m_MetadataDefinitions;
        private MetadataDataTypeValidator m_DataTypeValidator;
        private MetadataOccurrenceValidator m_OccurrenceValidator;

        private List<MetadataValidationReportItem> m_Report;
        public List<MetadataValidationReportItem> Report
        {
            get
            {
                return m_Report;
            }
        }

        public MetadataValidation(List<MetadataDefinition> metadataDefinitions)
        {
            m_MetadataDefinitions = metadataDefinitions;
            m_Report = new List<MetadataValidationReportItem>();
            m_DataTypeValidator = new MetadataDataTypeValidator(this);
            m_OccurrenceValidator = new MetadataOccurrenceValidator(this);
        }

        //validate the entire set and generate a report
        public bool Validate(List<Metadata> metadatas)
        {
            m_Report.Clear();
            bool isValid = true;

            //validate each item by itself
            foreach (Metadata metadata in metadatas)
            {
                if (! _validateItem(metadata))
                    isValid = false;
            }

            isValid = _validateAsSet(metadatas);

            return isValid;
        }

        //validate a single item (do not look at the entire set - do not look for repetitions)
        public bool ValidateItem(Metadata metadata)
        {
            m_Report.Clear();
            return _validateItem(metadata);
        }
        internal void ReportError(MetadataValidationReportItem item)
        {
            m_Report.Add(item);
            NotifyValidationError(item);
        }
        private bool _validateItem(Metadata metadata)
        {
            MetadataDefinition metadataDefinition = m_MetadataDefinitions.Find(
                delegate(MetadataDefinition item) 
                    { return item.Name == metadata.Name; });
                
            if (metadataDefinition == null)
            {
                metadataDefinition = SupportedMetadata_Z39862005.AlienMetadata;
            }
            //check the occurrence requirement
            bool meetsOccurrenceRequirement = m_OccurrenceValidator.Validate(metadata, metadataDefinition);
            //check the data type
            bool meetsDataTypeRequirement = m_DataTypeValidator.Validate(metadata, metadataDefinition);

            if (!(meetsOccurrenceRequirement & meetsDataTypeRequirement))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool _validateAsSet(List<Metadata> metadatas)
        {
            bool isValid = true;
            //make sure all the required items are there
            foreach (MetadataDefinition metadataDefinition in m_MetadataDefinitions)
            {
                if (metadataDefinition.Occurrence == MetadataOccurrence.Required)
                {
                    Metadata metadata = metadatas.Find(
                        delegate(Metadata item) 
                            { return item.Name == metadataDefinition.Name; });

                    if (metadata == null)
                    {   
                        ReportError(new MetadataValidationReportItem(null,
                            string.Format("Missing {0}", metadataDefinition.Name)));
                        isValid = false;
                    }
                }
            }

            //make sure repetitions are ok
            foreach (Metadata metadata in metadatas)
            {
                List<Metadata> list = metadatas.FindAll(
                    delegate(Metadata item) 
                        { return item.Name == metadata.Name; });

                MetadataDefinition metadataDefinition = m_MetadataDefinitions.Find(
                    delegate(MetadataDefinition item)
                        { return item.Name == metadata.Name; });
                
                if (list.Count > 1 && metadataDefinition.IsRepeatable == false)
                {
                    ReportError(new MetadataValidationReportItem(metadata,
                        string.Format("{0} must not appear more than once", metadata.Name)));
                    isValid = false;
                }
            }

            return isValid;
        }
    }

    public class MetadataDataTypeValidator
    {
        private MetadataValidation m_ParentValidator;
        public MetadataDataTypeValidator(MetadataValidation parentValidator)
        {
            m_ParentValidator = parentValidator;
        }
        public bool Validate(Metadata metadata, MetadataDefinition metadataDefinition)
        {
            if (metadataDefinition.DataType == MetadataDataType.ClockValue)
            {
                return _validateClockValue(metadata);
            }
            else if (metadataDefinition.DataType == MetadataDataType.Date)
            {
                return _validateDate(metadata);
            }
            else if (metadataDefinition.DataType == MetadataDataType.FileUri)
            {
                return _validateFileUri(metadata);
            }
            else if (metadataDefinition.DataType == MetadataDataType.Integer)
            {
                return _validateInteger(metadata);
            }
            else if (metadataDefinition.DataType == MetadataDataType.Double)
            {
                return _validateDouble(metadata);
            }
            else if (metadataDefinition.DataType == MetadataDataType.Number)
            {
                return _validateNumber(metadata);
            }
            else if (metadataDefinition.DataType == MetadataDataType.LanguageCode)
            {
                return _validateLanguageCode(metadata);
            }
            else if (metadataDefinition.DataType == MetadataDataType.String)
            {
                return _validateString(metadata);
            }
            return true;
        }
        private bool _validateClockValue(Metadata metadata)
        {
            return true;
        }
        private bool _validateDate(Metadata metadata)
        {
            string date = metadata.Content;
            //Require at least the year field
            if (date.Length < 4)
            {
                m_ParentValidator.ReportError(new MetadataValidationReportItem(metadata, "Minimum size is 4"));
                return false;
            }

            //The longest it can be is 10
             if (date.Length > 10)
             {
                 m_ParentValidator.ReportError(new MetadataValidationReportItem(metadata, "Maximum size is 10"));
                 return false;
             }
                

            string[] dateArray = date.Split('-');
            int year = 0;
            int month = 0;
            int day = 0;

            //the year has to be 4 digits
            if (dateArray[0].Length != 4)
            {
                m_ParentValidator.ReportError(new MetadataValidationReportItem(metadata, "Year must be 4 digits"));
                return false;                
            }
                

            //the year has to be digits
            try
            {
                year = Convert.ToInt32(dateArray[0]);
            }
            catch
            {
                m_ParentValidator.ReportError(new MetadataValidationReportItem(metadata, "Invalid year"));
                return false;
            }

            //check for a month value (it's optional)
            if (dateArray.Length >= 2)
            {
                //the month has to be numeric
                try
                {
                    month = Convert.ToInt32(dateArray[1]);
                }
                catch
                {
                    m_ParentValidator.ReportError(new MetadataValidationReportItem(metadata, "Invalid month"));
                    return false;
                }
                //the month has to be in this range
                if (month < 1 || month > 12)
                {
                    m_ParentValidator.ReportError(new MetadataValidationReportItem(metadata, "Month out of range"));
                    return false;
                }
            }
            //check for a day value (it's optional but only if a month is specified)
            if (dateArray.Length == 3)
            {
                //the day has to be a number
                try
                {
                    day = Convert.ToInt32(dateArray[2]);
                }
                catch
                {
                    m_ParentValidator.ReportError(new MetadataValidationReportItem(metadata, "Invalid day"));
                    return false;
                }
                //it has to be in this range
                if (day < 1 || day > 31)
                {
                    m_ParentValidator.ReportError(new MetadataValidationReportItem(metadata, "Day out of range"));
                    return false;
                }
            }

            return true;
        }
        private bool _validateFileUri(Metadata metadata)
        {
            return true;
        }
        private bool _validateInteger(Metadata metadata)
        {
            try
            {
                int x = Convert.ToInt32(metadata.Content);
            }
            catch (Exception)
            {
                m_ParentValidator.ReportError(new MetadataValidationReportItem(metadata, "Invalid numeric value"));
                return false;
            }
            return true;
        }
        private bool _validateDouble(Metadata metadata)
        {
            try
            {
                double x = Convert.ToDouble(metadata.Content);
            }
            catch (Exception)
            {
                m_ParentValidator.ReportError(new MetadataValidationReportItem(metadata, "Invalid numeric value"));
                return false;
            }
            return true;
        }
        //works for both double and int
        private bool _validateNumber(Metadata metadata)
        {
            try
            {
                int x = Convert.ToInt32(metadata.Content);
            }
            catch (Exception)
            {
                double x = Convert.ToDouble(metadata.Content);
            }
            catch
            {
                m_ParentValidator.ReportError(new MetadataValidationReportItem(metadata, "Invalid numeric value"));
                return false;
            }
            return true;
        }
        private bool _validateLanguageCode(Metadata metadata)
        {
            return true;
        }
        private bool _validateString(Metadata metadata)
        {
            return true;
        }
    }

    public class MetadataOccurrenceValidator
    {
        private MetadataValidation m_ParentValidator;
        public MetadataOccurrenceValidator(MetadataValidation parentValidator)
        {
            m_ParentValidator = parentValidator;
        }
        
        public bool Validate(Metadata metadata, MetadataDefinition metadataDefinition)
        {
            //if it's a required field, it can't be empty
            if (metadataDefinition.Occurrence == MetadataOccurrence.Required)
            {
                if (metadata.Content.Length > 0)
                {
                    return true;
                }
                else
                {
                    m_ParentValidator.ReportError(new MetadataValidationReportItem(metadata, "Must not be empty"));
                    return false;
                }
            }
            else
            {
                return true;
            }

        }
    }

    public class MetadataAvailability
    {
        public static List<MetadataDefinition> GetAvailableMetadata(List<Metadata> alreadyUsedMetadata, List<MetadataDefinition> definitions)
        {
            List<MetadataDefinition> availableMetadata = new List<MetadataDefinition>();
            foreach (MetadataDefinition definition in definitions)
            {
                Metadata exists = alreadyUsedMetadata.Find(
                    delegate(Metadata item) 
                        { return item.Name == definition.Name; });
                
                if (exists == null)
                {
                    availableMetadata.Add(definition);
                }
                else
                {
                    if (definition.IsRepeatable == true)
                    {
                        availableMetadata.Add(definition);
                    }
                }
                
            }
            return availableMetadata;
        }
    }

}