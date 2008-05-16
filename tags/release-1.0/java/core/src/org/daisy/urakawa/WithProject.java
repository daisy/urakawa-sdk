package org.daisy.urakawa;

import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * Getting and Setting the project to which a Presentation belongs to. The
 * Project reference accessed by this getter/setter corresponds to a UML
 * aggregation relationship. This is merely a "track-back" feature to allow the
 * Presentation objects to be aware of the Project instance they belong to.
 * </p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface WithProject {
	/**
	 * @return the project. Cannot be null.
	 * @throws IsNotInitializedException
	 *             if the project reference is not initialized yet.
	 */
	public Project getProject() throws IsNotInitializedException;

	/**
	 * @param project
	 *            cannot be null.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws IsAlreadyInitializedException
	 *             when the project reference is already initialized
	 * @tagvalue Exceptions "MethodParameterIsNull-IsAlreadyInitialized"
	 * @stereotype Initialize
	 */
	public void setProject(Project project)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException;
}
