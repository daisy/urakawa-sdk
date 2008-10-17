package org.daisy.urakawa.metadata;

import org.daisy.urakawa.GenericWithPresentationFactory;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Extension of the generic factory to handle one or more specific types derived
 * from the base specified class, in order to provide convenience create()
 * methods.
 * 
 * @xhas - - 1 org.daisy.urakawa.Presentation
 * @depend - Create - org.daisy.urakawa.metadata.Metadata
 */
public final class MetadataFactory extends GenericWithPresentationFactory<Metadata>
{
    /**
     * @param pres
     * @throws MethodParameterIsNullException
     */
    public MetadataFactory(Presentation pres)
            throws MethodParameterIsNullException
    {
        super(pres);
    }
    /**
     * @return
     */
    public Metadata createMetadata()
    {
        try
        {
            return create(Metadata.class);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }
}
