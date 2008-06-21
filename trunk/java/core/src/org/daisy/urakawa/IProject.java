package org.daisy.urakawa;

import java.net.URI;

import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.event.IEventHandler;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.IXukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * <p>
 * This is essentially a container for one or more
 * {@link org.daisy.urakawa.IPresentation}. It also provides methods for opening
 * and saving the XUK persistent XML format.
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Composition 1..n org.daisy.urakawa.IPresentation
 * @depend - Composition 1 org.daisy.urakawa.IDataModelFactory
 * @stereotype IXukAble
 */
public interface IProject extends IWithDataModelFactory, IWithPresentations,
		IXukAble, IValueEquatable<IProject>,
		IEventHandler<DataModelChangedEvent> {
	/**
	 * <p>
	 * Reads a XUK-formatted XML file, and generates the equivalent object data
	 * that makes the IProject.
	 * </p>
	 * 
	 * @param uri
	 *            cannot be null.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws XukDeserializationFailedException
	 *             if the operation fails
	 * @tagvalue Exceptions "MethodParameterIsNull-XukDeserializationFailed"
	 */
	public void openXUK(URI uri) throws MethodParameterIsNullException,
			XukDeserializationFailedException;

	/**
	 * <p>
	 * Writes the object data of the IProject into a XUK-formatted XML file.
	 * </p>
	 * 
	 * @param uri
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws XukSerializationFailedException
	 *             if the operation fails
	 * @tagvalue Exceptions "MethodParameterIsNull-XukSerializationFailed"
	 */
	public void saveXUK(URI uri) throws MethodParameterIsNullException,
			XukSerializationFailedException;

	/**
	 * This method calls {@link org.daisy.urakawa.IPresentation#cleanup()} for
	 * each owned IPresentation.
	 */
	public void cleanup();
}
