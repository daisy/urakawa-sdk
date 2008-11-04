package org.daisy.urakawa;

import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @param <T>
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 */
public abstract class GenericWithPresentationFactory<T extends AbstractXukAbleWithPresentation>
        extends GenericXukAbleFactory<T>
{
    private Presentation mPresentation;

    /**
     * @return the Presentation owner
     */
    public Presentation getPresentation()
    {
        return mPresentation;
    }

    /**
     * @param pres
     * @throws MethodParameterIsNullException
     */
    public GenericWithPresentationFactory(Presentation pres)
            throws MethodParameterIsNullException
    {
        if (pres == null)
        {
            throw new MethodParameterIsNullException();
        }
    }

    /**
     * Initializes a created instance by assigning it an Presentation owner. In
     * derived classes of this factory, this method can be overridden in order
     * to perform additional initialization, in which case
     * super.InitializeInstance(instance) must be called.
     */
    @Override
    protected void initializeInstance(T instance)
    {
        super.initializeInstance(instance);
        try
        {
            instance.setPresentation(getPresentation());
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (IsAlreadyInitializedException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }
}
