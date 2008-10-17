package org.daisy.urakawa;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Extension of the generic factory to handle one or more specific types derived
 * from the base specified class, in order to provide convenience create()
 * methods.
 * 
 * @depend - Create - org.daisy.urakawa.Presentation
 * @note Note that this factory does not know about its owner project
 */
public class PresentationFactory extends GenericXukAbleFactory<Presentation>
{
    /**
     * @return
     */
    public Presentation createPresentation()
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
