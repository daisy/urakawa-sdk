using urakawa.metadata;

namespace urakawa.events.metadata
{
    public class ValueChangedEventArgs : DataModelChangedEventArgs
    {
        public ValueChangedEventArgs(MetadataAttribute src, string newVal, string prevVal)
            : base(src)
        {
            SourceMetadataAttribute = src;
            NewValue = newVal;
            PreviousValue = prevVal;
        }

        public readonly MetadataAttribute SourceMetadataAttribute;
        public readonly string NewValue;
        public readonly string PreviousValue;
    }

    public class NameChangedEventArgs : DataModelChangedEventArgs
    {
        public NameChangedEventArgs(MetadataAttribute src, string newVal, string prevVal)
            : base(src)
        {
            SourceMetadataAttribute = src;
            NewName = newVal;
            PreviousName = prevVal;
        }

        public readonly MetadataAttribute SourceMetadataAttribute;
        public readonly string NewName;
        public readonly string PreviousName;
    }

    public class NamespaceChangedEventArgs : DataModelChangedEventArgs
    {
        public NamespaceChangedEventArgs(MetadataAttribute src, string newVal, string prevVal)
            : base(src)
        {
            SourceMetadataAttribute = src;
            NewNamespace = newVal;
            PreviousNamespace = prevVal;
        }

        public readonly MetadataAttribute SourceMetadataAttribute;
        public readonly string NewNamespace;
        public readonly string PreviousNamespace;
    }
}
