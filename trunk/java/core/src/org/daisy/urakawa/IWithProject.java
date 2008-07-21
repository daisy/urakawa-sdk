package org.daisy.urakawa;

import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * Getting and Setting the project to which a IPresentation belongs to. The
 * IProject reference accessed by this getter/setter corresponds to a UML
 * aggregation relationship. This is merely a "track-back" feature to allow the
 * IPresentation objects to be aware of the IProject instance they belong to.
 * </p>
 * 
 */
public interface IWithProject
{
    /**
     * @return the project. Cannot be null.
     * @throws IsNotInitializedException if the project reference is not
     *         initialized yet.
     */
    public IProject getProject() throws IsNotInitializedException;

    /**
     * @param iProject cannot be null.
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * @throws IsAlreadyInitializedException when the project reference is
     *         already initialized
     * 
     * @stereotype Initialize
     */
    public void setProject(IProject iProject)
            throws MethodParameterIsNullException,
            IsAlreadyInitializedException;
}
