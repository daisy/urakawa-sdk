package org.daisy.urakawa;

import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.AbstractXukAble;

/**
 * Objects that are XUK-able in the data model always maintain a reference to
 * their "owner" (or "parent") Presentation. This concrete class is a
 * convenience implementation that prevents repetitive boiler-plate code.
 * 
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 */
public abstract class AbstractXukAbleWithPresentation extends AbstractXukAble
        implements IWithPresentation
{
    private Presentation mPresentation;

    /**
     * empty constructor
     */
    public AbstractXukAbleWithPresentation()
    {
        mPresentation = null;
    }

    public Presentation getPresentation() throws IsNotInitializedException
    {
        if (mPresentation == null)
        {
            throw new IsNotInitializedException();
        }
        return mPresentation;
    }

    public void setPresentation(Presentation iPresentation)
            throws MethodParameterIsNullException,
            IsAlreadyInitializedException
    {
        if (iPresentation == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (mPresentation != null)
        {
            throw new IsAlreadyInitializedException();
        }
        mPresentation = iPresentation;
    }
}
