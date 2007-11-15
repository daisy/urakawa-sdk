package org.daisy.urakawa;

import java.util.List;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;

/**
 * <p>
 * Getting and Setting the presentations.
 * </p>
 * <p>
 * When using this interface (e.g. by using "extend" or "implement"), the host
 * object type should explicitly declare the UML aggregation or composition
 * relationship, in order to clearly state the rules for object instance
 * ownership.
 * <p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface WithPresentations {

	public Presentation addNewPresentation();

	public void addPresentation(Presentation newPres)
			throws MethodParameterIsNullException;

	public List<Presentation> getListOfPresentations();

	public int getNumberOfPresentations();

	public Presentation getPresentation(int index)
			throws MethodParameterIsOutOfBoundsException;

	public void removeAllPresentations();

	public Presentation removePresentation(int index)
			throws MethodParameterIsOutOfBoundsException;

	public void setPresentation(Presentation newPres, int index)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException;
}
