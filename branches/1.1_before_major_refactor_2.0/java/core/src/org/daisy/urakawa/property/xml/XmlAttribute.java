package org.daisy.urakawa.property.xml;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.ObjectIsInDifferentPresentationException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * <p>
 * This is an XML attribute owned by an XmlProperty.
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Clone - org.daisy.urakawa.property.xml.XmlAttribute
 * @depend - Aggregation 1 org.daisy.urakawa.property.xml.XmlProperty
 * @stereotype XukAble
 */
public interface XmlAttribute extends WithXmlProperty, WithQualifiedName,
		WithValue, WithPresentation, XukAble, EventHandler<DataModelChangedEvent> {
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
	public XmlAttribute copy(XmlProperty newParent)
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
	public XmlAttribute export(Presentation destPres, XmlProperty parent)
			throws MethodParameterIsNullException,
			ObjectIsInDifferentPresentationException,
			FactoryCannotCreateTypeException;
}
