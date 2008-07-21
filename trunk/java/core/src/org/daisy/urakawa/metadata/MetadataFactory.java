package org.daisy.urakawa.metadata;

import org.daisy.urakawa.GenericFactory;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Extension of the generic factory to handle one or more specific types derived
 * from the base specified class, in order to provide convenience create()
 * methods.
 * 
 * @depend - Create - org.daisy.urakawa.metadata.Metadata
 */
public final class MetadataFactory extends GenericFactory<Metadata>
{
    /**
     * @return
     */
    public Metadata create()
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
