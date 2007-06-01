package org.daisy.urakawa;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Getting and Setting the project. Please take notice of the aggregation
 * or composition relationship for the object attribute described here, and also
 * be aware that this relationship may be explicitly overridden where this
 * interface is use.
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 * @depend - Aggregation 1 Project
 */
public interface WithProject {
	/**
	 * @return the project. Cannot be null.
	 */
	public Project getProject();

	/**
	 * @param project
	 *            cannot be null.
	 * @throws MethodParameterIsNullException
	 *             if project is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype Initialize
	 */
	public void setProject(Project project)
			throws MethodParameterIsNullException;
}
