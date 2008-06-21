package org.daisy.urakawa;

import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * Getting and Setting the IDataModelFactory of a IProject.
 * </p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface IWithDataModelFactory {
	/**
	 * Initializes the IProject with the given IDataModelFactory.
	 * 
	 * @param fact
	 *            the IDataModelFactory, cannot be null
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws IsAlreadyInitializedException
	 *             when the IProject already has a IDataModelFactory
	 */
	public void setDataModelFactory(IDataModelFactory fact)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException;

	/**
	 * Returns the IDataModelFactory for this project, which is initialized
	 * lazily in the sense that if it is not already initialized with
	 * setDataModelFactory(), then a new default instance is created.
	 * 
	 * @return the IDataModelFactory for this project. cannot be null
	 */
	public IDataModelFactory getDataModelFactory();
}
