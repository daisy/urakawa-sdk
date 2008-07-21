package org.daisy.urakawa;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Extension of the generic factory to handle one or more specific types derived
 * from the base specified class, in order to provide convenience create()
 * methods.
 * 
 * @depend - Create - org.daisy.urakawa.Presentation
 */
public class PresentationFactory extends GenericFactory<Presentation>
{
    /**
     * @return
     */
    public Presentation create()
    {
        try
        {
            return create(Presentation.class);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }
}
