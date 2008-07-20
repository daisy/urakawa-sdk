package org.daisy.urakawa.property.xml;

import org.daisy.urakawa.IWithPresentation;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.event.IEventHandler;
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
		IWithValue, IWithPresentation, IXukAble,
		IEventHandler<DataModelChangedEvent> {
	/**
	 * <p>
	 * Cloning method
	 * </p>
	 * 
	 * @return a copy.
	 */
	public IXmlAttribute copy();
}
