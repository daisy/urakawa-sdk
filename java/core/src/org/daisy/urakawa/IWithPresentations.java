package org.daisy.urakawa;

import java.util.List;

import org.daisy.urakawa.exception.IsAlreadyManagerOfException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;

/**
 * <p>
 * Getting and Setting the Presentations of a Project. This represents a UML
 * composition relationship, as the Project actually owns the Presentation and
 * is in control of destroying individual instances.
 * </p>
 */
public interface IWithPresentations
{
    /**
     * Creates a new Presentation and adds it to the Project. Also returns the
     * new instance.
     * 
     * @return the newly created Presentation
     */
    public Presentation addNewPresentation();

    /**
     * Adds an existing Presentation to the Project (append to the existing
     * list).
     * 
     * @param iPresentation
     *        cannot be null, must not already be registered by this Project
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws IsAlreadyManagerOfException
     *         when the given Presentation is already managed by this Project
     */
    public void addPresentation(Presentation iPresentation)
            throws MethodParameterIsNullException, IsAlreadyManagerOfException;

    /**
     * @return a list of the existing Presentations in this Project. Cannot
     *         return null, but can return an empty list.
     */
    public List<Presentation> getListOfPresentations();

    /**
     * @return the number of Presentations in this Project, >= 0
     */
    public int getNumberOfPresentations();

    /**
     * @param index
     *        a number in [0, getNumberOfPresentations()[
     * @return the Presentation at the given index
     * @throws MethodParameterIsOutOfBoundsException
     *         when the given index is not in [0, getNumberOfPresentations()[
     */
    public Presentation getPresentation(int index)
            throws MethodParameterIsOutOfBoundsException;

    /**
     * Destroys all the existing Presentations from this Project.
     */
    public void removeAllPresentations();

    /**
     * Removes the Presentation at the given index from the Project, but does
     * not destroy it.
     * 
     * @param index
     *        a number in [0, getNumberOfPresentations()[
     * @return the removed Presentation
     * @throws MethodParameterIsOutOfBoundsException
     *         when the given index is not in [0, getNumberOfPresentations()[
     * @tagvalue Events "PresentationRemoved"
     */
    public Presentation removePresentation(int index)
            throws MethodParameterIsOutOfBoundsException;

    /**
     * Replaces a Presentation at the given index, or appends to the existing
     * list if the given index is >= getNumberOfPresentations()
     * 
     * @param iPresentation
     *        the Presentation to set
     * @param index
     *        a number in [0, getNumberOfPresentations()]
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws MethodParameterIsOutOfBoundsException
     *         when the given index is not in [0, getNumberOfPresentations()]
     * @throws IsAlreadyManagerOfException
     *         when the given Presentation is already registered in this Project
     *         at a different index (nothing happens when trying to set a
     *         Presentation where it already is).
     * @tagvalue Events "PresentationAdded"
     */
    public void setPresentation(Presentation iPresentation, int index)
            throws MethodParameterIsNullException,
            MethodParameterIsOutOfBoundsException, IsAlreadyManagerOfException;
}
