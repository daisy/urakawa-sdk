package org.daisy.urakawa.property;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class GenericPropertyFactory extends WithPresentation implements
		IGenericPropertyFactory {
	protected GenericPropertyFactory() {
	}

	public IProperty createProperty(String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (xukLocalName == null || xukNamespaceURI == null) {
			throw new MethodParameterIsNullException();
		}
		if (xukLocalName == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		if (xukNamespaceURI == IXukAble.XUK_NS) {
			if (xukLocalName == "IProperty") {
				IProperty newProp = new Property();
				try {
					newProp.setPresentation(getPresentation());
				} catch (IsAlreadyInitializedException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				} catch (IsNotInitializedException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				}
				return newProp;
			}
		}
		return null;
	}
}
