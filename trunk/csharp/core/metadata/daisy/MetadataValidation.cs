using System;
using System.Collections.Generic;

namespace urakawa.metadata.daisy
{
    public class MetadataValidationError
    {
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
        //the criteria
        private MetadataDefinition m_Definition;
        public MetadataDefinition Definition
        {
            get
            {
                return m_Definition;
            }
            set
            {
                m_Definition = value;
            }
        }
        public MetadataValidationError(string description, MetadataDefinition definition)
        {
            m_Definition = definition;
            m_Description = description;
        }
    }
    public class MetadataValidationFormatError : MetadataValidationError
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

        public MetadataValidationFormatError(Metadata metadata, string description, MetadataDefinition definition) :
            base(description, definition)
        {
            m_Metadata = metadata;
        }
    }
    public class MetadataValidationMissingItemError : MetadataValidationError
    {
        public MetadataValidationMissingItemError(string description, MetadataDefinition definition) :
            base(description, definition)
        {
        }
    }
    public class MetadataValidationDuplicateItemError : MetadataValidationError
    {
        public MetadataValidationDuplicateItemError(string description, MetadataDefinition definition) :
            base(description, definition)
        {
        }
    }


    public class MetadataValidator
    {
        private List<MetadataDefinition> m_MetadataDefinitions;
        private MetadataDataTypeValidator m_DataTypeValidator;
        private MetadataOccurrenceValidator m_OccurrenceValidator;

        private List<MetadataValidationError> m_Errors;
        public List<MetadataValidationError> Errors
        {
            get
            {
                return m_Errors;
            }
        }

        public MetadataValidator(List<MetadataDefinition> metadataDefinitions)
        {
            m_MetadataDefinitions = metadataDefinitions;
            m_Errors = new List<MetadataValidationError>();
            m_DataTypeValidator = new MetadataDataTypeValidator(this);
            m_OccurrenceValidator = new MetadataOccurrenceValidator(this);
        }
        public MetadataDefinition FindDefinition(string name)
        {
            return m_MetadataDefinitions.Find(
                delegate(MetadataDefinition item)
                { return item.Name == name; });
        }
        //validate the entire set and generate a report
        public bool Validate(List<Metadata> metadatas)
        {
            m_Errors.Clear();
            bool isValid = true;

            //validate each item by itself
            foreach (Metadata metadata in metadatas)
            {
                if (!_validateItem(metadata))
                    isValid = false;
            }

            isValid = isValid & _validateAsSet(metadatas);

            return isValid;
        }

        //validate a single item (do not look at the entire set - do not look for repetitions)
        public bool ValidateItem(Metadata metadata)
        {
            m_Errors.Clear();
            return _validateItem(metadata);
        }
        internal void ReportError(MetadataValidationError item)
        {
            m_Errors.Add(item);
        }
        private bool _validateItem(Metadata metadata)
        {
            MetadataDefinition metadataDefinition = FindDefinition(metadata.Name);

            if (metadataDefinition == null)
            {
                metadataDefinition = SupportedMetadata_Z39862005.UnrecognizedMetadata;
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
                        ReportError(new MetadataValidationMissingItemError
                            (string.Format("Missing {0}", metadataDefinition.Name),
                            metadataDefinition));
                        isValid = false;
                    }
                }
            }

            //make sure repetitions are ok
            foreach (Metadata metadata in metadatas)
            {
                MetadataDefinition metadataDefinition = m_MetadataDefinitions.Find(
                    delegate(MetadataDefinition item)
                    { return item.Name == metadata.Name; });

                if (metadataDefinition != null && !metadataDefinition.IsRepeatable)
                {
                    List<Metadata> list = metadatas.FindAll(
                        delegate(Metadata item)
                        { return item.Name == metadata.Name; });

                    if (list.Count > 1)
                    {
                        ReportError(new MetadataValidationDuplicateItemError
                                        (string.Format("{0} must not appear more than once", metadata.Name),
                                         metadataDefinition));
                        isValid = false;
                    }
                }
            }

            return isValid;
        }
    }

    public class MetadataDataTypeValidator
    {
        private MetadataValidator m_ParentValidator;
        public MetadataDataTypeValidator(MetadataValidator parentValidator)
        {
            m_ParentValidator = parentValidator;
        }
        public bool Validate(Metadata metadata, MetadataDefinition definition)
        {
            if (definition.DataType == MetadataDataType.ClockValue)
            {
                return _validateClockValue(metadata, definition);
            }
            else if (definition.DataType == MetadataDataType.Date)
            {
                return _validateDate(metadata, definition);
            }
            else if (definition.DataType == MetadataDataType.FileUri)
            {
                return _validateFileUri(metadata, definition);
            }
            else if (definition.DataType == MetadataDataType.Integer)
            {
                return _validateInteger(metadata, definition);
            }
            else if (definition.DataType == MetadataDataType.Double)
            {
                return _validateDouble(metadata, definition);
            }
            else if (definition.DataType == MetadataDataType.Number)
            {
                return _validateNumber(metadata, definition);
            }
            else if (definition.DataType == MetadataDataType.LanguageCode)
            {
                return _validateLanguageCode(metadata, definition);
            }
            else if (definition.DataType == MetadataDataType.String)
            {
                return _validateString(metadata, definition);
            }
            return true;
        }
        private bool _validateClockValue(Metadata metadata, MetadataDefinition definition)
        {
            return true;
        }
        private bool _validateDate(Metadata metadata, MetadataDefinition definition)
        {
            string date = metadata.Content;

            //Require at least the year field
            if (date.Length < 4)
            {
                m_ParentValidator.ReportError(new MetadataValidationFormatError
                    (metadata,
                    "Minimum size is 4",
                    definition));
                return false;
            }

            //The longest it can be is 10
            if (date.Length > 10)
            {
                m_ParentValidator.ReportError(new MetadataValidationFormatError
                    (metadata,
                    "Maximum size is 10",
                    definition));
                return false;
            }


            string[] dateArray = date.Split('-');
            int year = 0;
            int month = 0;
            int day = 0;

            //the year has to be 4 digits
            if (dateArray[0].Length != 4)
            {
                m_ParentValidator.ReportError(new MetadataValidationFormatError
                    (metadata,
                    "Year must be 4 digits",
                    definition));
                return false;
            }


            //the year has to be digits
            try
            {
                year = Convert.ToInt32(dateArray[0]);
            }
            catch
            {
                m_ParentValidator.ReportError(new MetadataValidationFormatError
                    (metadata,
                    "Invalid year",
                    definition));
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
                    m_ParentValidator.ReportError(new MetadataValidationFormatError
                        (metadata,
                        "Invalid month",
                        definition));
                    return false;
                }
                //the month has to be in this range
                if (month < 1 || month > 12)
                {
                    m_ParentValidator.ReportError(new MetadataValidationFormatError
                        (metadata,
                        "Month out of range",
                        definition));
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
                    m_ParentValidator.ReportError(new MetadataValidationFormatError
                        (metadata,
                        "Invalid day",
                        definition));
                    return false;
                }
                //it has to be in this range
                if (day < 1 || day > 31)
                {
                    m_ParentValidator.ReportError(new MetadataValidationFormatError
                        (metadata,
                        "Day out of range",
                        definition));
                    return false;
                }
            }

            return true;
        }
        private bool _validateFileUri(Metadata metadata, MetadataDefinition definition)
        {
            return true;
        }
        private bool _validateInteger(Metadata metadata, MetadataDefinition definition)
        {
            try
            {
                int x = Convert.ToInt32(metadata.Content);
            }
            catch (Exception)
            {
                m_ParentValidator.ReportError(new MetadataValidationFormatError
                    (metadata,
                    "Invalid numeric value",
                    definition));
                return false;
            }
            return true;
        }
        private bool _validateDouble(Metadata metadata, MetadataDefinition definition)
        {
            try
            {
                double x = Convert.ToDouble(metadata.Content);
            }
            catch (Exception)
            {
                m_ParentValidator.ReportError(new MetadataValidationFormatError
                    (metadata,
                    "Invalid numeric value",
                    definition));
                return false;
            }
            return true;
        }
        //works for both double and int
        private bool _validateNumber(Metadata metadata, MetadataDefinition definition)
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
                m_ParentValidator.ReportError(new MetadataValidationFormatError
                    (metadata,
                    "Invalid numeric value",
                    definition));
                return false;
            }
            return true;
        }
        private bool _validateLanguageCode(Metadata metadata, MetadataDefinition definition)
        {
            return true;
        }
        private bool _validateString(Metadata metadata, MetadataDefinition definition)
        {
            return true;
        }
    }

    public class MetadataOccurrenceValidator
    {
        private MetadataValidator m_ParentValidator;
        public MetadataOccurrenceValidator(MetadataValidator parentValidator)
        {
            m_ParentValidator = parentValidator;
        }

        public bool Validate(Metadata metadata, MetadataDefinition definition)
        {
            //if it's a required field, it can't be empty
            if (definition.Occurrence == MetadataOccurrence.Required)
            {
                if (metadata.Content.Length > 0)
                {
                    return true;
                }
                else
                {
                    m_ParentValidator.ReportError(new MetadataValidationFormatError
                        (metadata,
                        "Content must not be empty",
                        definition));
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