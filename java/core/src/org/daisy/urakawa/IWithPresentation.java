package org.daisy.urakawa;

import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * <p>
 * Getting and Setting the presentation. Object classes that realize this
 * interface implement an object type that is "owned" by a IPresentation
 * instance. In other words, the IPresentation reference accessed by this
 * getter/setter corresponds to a UML aggregation relationship. This is merely a
 * "track-back" feature to allow sub-level objects to be aware of the
 * IPresentation instance they belong to.
 * </p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface IWithPresentation extends IXukAble {
	/**
	 * @return the presentation object. Cannot be null.
	 * @throws IsNotInitializedException
	 *             when {@link IWithPresentation#setPresentation(IPresentation)}
	 *             has not been used yet to initialize the data.
	 * @tagvalue Exceptions "IsNotInitialized"
	 */
	public IPresentation getPresentation() throws IsNotInitializedException;

	/**
	 * @param iPresentation
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws IsAlreadyInitializedException
	 *             when the data has already been initialized using this method.
	 * @tagvalue Exceptions "MethodParameterIsNull-IsAlreadyInitialized"
	 * @stereotype Initialize
	 */
	public void setPresentation(IPresentation iPresentation)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException;
}
