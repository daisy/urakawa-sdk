package org.daisy.urakawa.metadata;

public interface MetadataFactory {
    public Metadata createMetadata();

    public Metadata createMetadata(String xukLocalName, String xukNamespaceUri);
}
