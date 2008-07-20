package org.daisy.urakawa.property;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.IPresentation;
import org.daisy.urakawa.IValueEquatable;
import org.daisy.urakawa.IWithPresentation;
import org.daisy.urakawa.core.ITreeNode;
import org.daisy.urakawa.core.IWithTreeNodeOwner;
import org.daisy.urakawa.events.DataModelChangedEvent;
import org.daisy.urakawa.events.IEventHandler;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * <p>
 * This is the baseline for a IProperty object. It is recommended to extend this
 * basic type, in order to provide more specific behaviors.
 * </p>
 * <p>
 * The Urakawa data model provides 2 built-in concrete property types: see
 * {@link org.daisy.urakawa.property.xml.IXmlProperty} and
 * {@link org.daisy.urakawa.property.channel.IChannelsProperty}.
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 1 org.daisy.urakawa.core.ITreeNode
 * @depend - Clone - org.daisy.urakawa.property.IProperty
 * 
 */
public interface IProperty extends IWithPresentation, IWithTreeNodeOwner, IXukAble,
		IValueEquatable<IProperty>, IEventHandler<DataModelChangedEvent> {

	/**
	 * Tests whether this IProperty can be added to the given ITreeNode instance.
	 * 
	 * @param node
	 * @return true or false
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 */
	public boolean canBeAddedTo(ITreeNode node)
			throws MethodParameterIsNullException;

	/**
	 * <p>
	 * Clone method.
	 * </p>
	 * 
	 * @return cannot be null.
	 * @throws FactoryCannotCreateTypeException
	 * @throws IsNotInitializedException
	 */
	public IProperty copy() throws FactoryCannotCreateTypeException,
			IsNotInitializedException;

	/**
	 * Creates a new IProperty with identical content as this one, but compatible
	 * with the given IPresentation (factories, managers, channels, etc.). The
	 * process consist in attempting to create copies with identical content (it
	 * may fail in which case the factory exception is raised). If this IProperty
	 * (or somewhere in its contents) is not compatible with the given
	 * destination IPresentation (i.e. an attempt to create a copy using a
	 * factory with a given QName, fails), then the
	 * FactoryCannotCreateTypeException is raised.
	 * 
	 * @param destPres
	 *            the destination IPresentation to which this property (and its
	 *            content) should be exported.
	 * @return a new property with identical content as this one, but compatible
	 *         with the given IPresentation (factories, managers, channels,
	 *         etc.).
	 * @throws FactoryCannotCreateTypeException
	 *             if one of the factories in the given IPresentation cannot
	 *             create a type based on a QName.
	 * @throws IsNotInitializedException
	 * @tagvalue Exceptions "FactoryCannotCreateType-MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public IProperty export(IPresentation destPres)
			throws FactoryCannotCreateTypeException, IsNotInitializedException,
			MethodParameterIsNullException;
}
