package org.daisy.urakawa;

import java.util.List;

import org.daisy.urakawa.exception.IsAlreadyManagerOfException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;

/**
 * <p>
 * Getting and Setting the Presentations of a IProject. This represents a UML
 * composition relationship, as the IProject actually owns the IPresentation and
 * is in control of destroying individual instances.
 * </p>
 * 
 */
public interface IWithPresentations
{
    /**
     * Creates a new IPresentation and adds it to the IProject. Also returns the
     * new instance.
     * 
     * @return the newly created IPresentation
     */
    public IPresentation addNewPresentation();

    /**
     * Adds an existing IPresentation to the IProject (append to the existing
     * list).
     * 
     * @param iPresentation cannot be null, must not already be registered by
     *        this IProject
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * @throws IsAlreadyManagerOfException when the given IPresentation is
     *         already managed by this IProject
     */
    public void addPresentation(IPresentation iPresentation)
            throws MethodParameterIsNullException, IsAlreadyManagerOfException;

    /**
     * @return a list of the existing Presentations in this IProject. Cannot
     *         return null, but can return an empty list.
     */
    public List<IPresentation> getListOfPresentations();

    /**
     * @return the number of Presentations in this IProject, >= 0
     */
    public int getNumberOfPresentations();

    /**
     * @param index a number in [0, getNumberOfPresentations()[
     * @return the IPresentation at the given index
     * @throws MethodParameterIsOutOfBoundsException when the given index is not
     *         in [0, getNumberOfPresentations()[
     */
    public IPresentation getPresentation(int index)
            throws MethodParameterIsOutOfBoundsException;

    /**
     * Destroys all the existing Presentations from this IProject.
     */
    public void removeAllPresentations();

    /**
     * Removes the IPresentation at the given index from the IProject, but does
     * not destroy it.
     * 
     * @param index a number in [0, getNumberOfPresentations()[
     * @return the removed IPresentation
     * @throws MethodParameterIsOutOfBoundsException when the given index is not
     *         in [0, getNumberOfPresentations()[
     * @tagvalue Events "PresentationRemoved"
     */
    public IPresentation removePresentation(int index)
            throws MethodParameterIsOutOfBoundsException;

    /**
     * Replaces a IPresentation at the given index, or appends to the existing
     * list if the given index is >= getNumberOfPresentations()
     * 
     * @param iPresentation the IPresentation to set
     * @param index a number in [0, getNumberOfPresentations()]
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * @throws MethodParameterIsOutOfBoundsException when the given index is not
     *         in [0, getNumberOfPresentations()]
     * @throws IsAlreadyManagerOfException when the given IPresentation is
     *         already registered in this IProject at a different index (nothing
     *         happens when trying to set a IPresentation where it already is).
     * @tagvalue Events "PresentationAdded"
     */
    public void setPresentation(IPresentation iPresentation, int index)
            throws MethodParameterIsNullException,
            MethodParameterIsOutOfBoundsException, IsAlreadyManagerOfException;
}
