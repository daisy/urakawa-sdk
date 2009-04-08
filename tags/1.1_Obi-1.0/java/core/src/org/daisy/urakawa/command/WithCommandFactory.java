package org.daisy.urakawa.command;

import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * Getting and Setting a factory for the Presentation. This corresponds to a UML
 * composition relationship.
 * </p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface WithCommandFactory {
	/**
	 * @return the factory object. Cannot be null, because an instance is
	 *         created lazily via the DataModelFactory when setCommandFactory()
	 *         has not been explicitly called to initialize in the first place.
	 * @throws IsNotInitializedException
	 *             when the Project is not initialized for the Presentation.
	 */
	public CommandFactory getCommandFactory() throws IsNotInitializedException;

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
	public void setCommandFactory(CommandFactory factory)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException;
}
