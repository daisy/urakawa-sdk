package org.daisy.urakawa.property;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.core.WithTreeNodeOwner;
import org.daisy.urakawa.event.ChangeNotifier;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * <p>
 * This is the baseline for a Property object. It is recommended to extend this
 * basic type, in order to provide more specific behaviors.
 * </p>
 * <p>
 * The Urakawa data model provides 2 built-in concrete property types: see
 * {@link org.daisy.urakawa.property.xml.XmlProperty} and
 * {@link org.daisy.urakawa.property.channel.ChannelsProperty}.
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 1 org.daisy.urakawa.core.TreeNode
 * @depend - Clone - org.daisy.urakawa.core.property.Property
 * @stereotype XukAble
 */
public interface Property extends WithPresentation, WithTreeNodeOwner, XukAble,
		ValueEquatable<Property>, ChangeNotifier<DataModelChangedEvent> {
	/**
	 * Convenience method to get the PropertyFactory from the Presentation
	 * 
	 * @return the PropertyFactory
	 * @throws IsNotInitializedException
	 *             when the Presentation has not been initialized
	 */
	public PropertyFactory getPropertyFactory()
			throws IsNotInitializedException;

	/**
	 * Tests whether this Property can be added to the given TreeNode instance.
	 * 
	 * @param node
	 * @return true or false
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 */
	public boolean canBeAddedTo(TreeNode node)
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
	public Property copy() throws FactoryCannotCreateTypeException,
			IsNotInitializedException;

	/**
	 * Creates a new Property with identical content as this one, but compatible
	 * with the given Presentation (factories, managers, channels, etc.). The
	 * process consist in attempting to create copies with identical content (it
	 * may fail in which case the factory exception is raised). If this Property
	 * (or somewhere in its contents) is not compatible with the given
	 * destination Presentation (i.e. an attempt to create a copy using a
	 * factory with a given QName, fails), then the
	 * FactoryCannotCreateTypeException is raised.
	 * 
	 * @param destPres
	 *            the destination Presentation to which this property (and its
	 *            content) should be exported.
	 * @return a new property with identical content as this one, but compatible
	 *         with the given Presentation (factories, managers, channels,
	 *         etc.).
	 * @throws FactoryCannotCreateTypeException
	 *             if one of the factories in the given Presentation cannot
	 *             create a type based on a QName.
	 * @throws IsNotInitializedException
	 * @tagvalue Exceptions "FactoryCannotCreateType-MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public Property export(Presentation destPres)
			throws FactoryCannotCreateTypeException, IsNotInitializedException,
			MethodParameterIsNullException;
}
