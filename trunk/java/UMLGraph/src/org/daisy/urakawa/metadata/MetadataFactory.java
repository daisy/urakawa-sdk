package org.daisy.urakawa.metadata;

public interface MetadataFactory {
    public Metadata createMetadata();

    public Metadata createMetadata(String strLocalName, String strNamespaceUri);
}
