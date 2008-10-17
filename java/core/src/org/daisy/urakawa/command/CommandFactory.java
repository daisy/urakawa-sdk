package org.daisy.urakawa.command;

import org.daisy.urakawa.GenericWithPresentationFactory;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Extension of the generic factory to handle one or more specific types derived
 * from the base specified class, in order to provide convenience create()
 * methods.
 * 
 * @xhas - - 1 org.daisy.urakawa.Presentation
 * @depend - Create - org.daisy.urakawa.command.CompositeCommand
 */
public final class CommandFactory extends GenericWithPresentationFactory<AbstractCommand>
{
    /**
     * @param pres
     * @throws MethodParameterIsNullException
     */
    public CommandFactory(Presentation pres)
            throws MethodParameterIsNullException
    {
        super(pres);
    }

    /**
     * @return
     */
    public CompositeCommand createCompositeCommand()
    {
        try
        {
            return create(CompositeCommand.class);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }
}
