package org.daisy.urakawa;

import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.property.IPropertyFactory;

/**
 * <p>
 * Getting and Setting a factory for the IPresentation. This corresponds to a UML
 * composition relationship.
 * </p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface IWithPropertyFactory {
	/**
	 * @return the factory object. Cannot be null, because an instance is
	 *         created lazily via the IDataModelFactory when setPropertyFactory()
	 *         has not been explicitly called to initialize in the first place.
	 * @throws IsNotInitializedException
	 *             when the IProject is not initialized for the IPresentation.
	 */
	public IPropertyFactory getPropertyFactory()
			throws IsNotInitializedException;

	/**
	 * @param factory
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws IsAlreadyInitializedException
	 *             when the data was already initialized
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype Initialize
	 */
	public void setPropertyFactory(IPropertyFactory factory)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException;
}
