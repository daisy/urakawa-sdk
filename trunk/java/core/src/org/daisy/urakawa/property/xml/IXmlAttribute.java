package org.daisy.urakawa.property.xml;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.IPresentation;
import org.daisy.urakawa.IWithPresentation;
import org.daisy.urakawa.event.IEventHandler;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.ObjectIsInDifferentPresentationException;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * <p>
 * This is an XML attribute owned by an IXmlProperty.
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Clone - org.daisy.urakawa.property.xml.IXmlAttribute
 * @depend - Aggregation 1 org.daisy.urakawa.property.xml.IXmlProperty
 * 
 */
public interface IXmlAttribute extends IWithXmlProperty, IWithQualifiedName,
		IWithValue, IWithPresentation, IXukAble, IEventHandler<DataModelChangedEvent> {
	/**
	 * <p>
	 * Cloning method
	 * </p>
	 * 
	 * @param newParent
	 * @return a copy.
	 * @throws MethodParameterIsNullException
	 * @throws ObjectIsInDifferentPresentationException
	 * @throws FactoryCannotCreateTypeException
	 */
	public IXmlAttribute copy(IXmlProperty newParent)
			throws MethodParameterIsNullException,
			ObjectIsInDifferentPresentationException,
			FactoryCannotCreateTypeException;

	/**
	 * @param destPres
	 * @param parent
	 * @return attr
	 * @throws MethodParameterIsNullException
	 * @throws ObjectIsInDifferentPresentationException
	 * @throws FactoryCannotCreateTypeException
	 */
	public IXmlAttribute export(IPresentation destPres, IXmlProperty parent)
			throws MethodParameterIsNullException,
			ObjectIsInDifferentPresentationException,
			FactoryCannotCreateTypeException;
}
