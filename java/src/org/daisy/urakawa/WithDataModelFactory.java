package org.daisy.urakawa;

import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * Getting and Setting the DataModelFactory of a Project.
 * </p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface WithDataModelFactory {
	/**
	 * Initializes the Project with the given DataModelFactory.
	 * 
	 * @param fact
	 *            the DataModelFactory, cannot be null
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws IsAlreadyInitializedException
	 *             when the Project already has a DataModelFactory
	 */
	public void setDataModelFactory(DataModelFactory fact)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException;

	/**
	 * Returns the DataModelFactory for this project, which is initialized
	 * lazily in the sense that if it is not already initialized with
	 * setDataModelFactory(), then a new default instance is created.
	 * 
	 * @return the DataModelFactory for this project. cannot be null
	 */
	public DataModelFactory getDataModelFactory();
}
