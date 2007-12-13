package org.daisy.urakawa.property;

import org.daisy.urakawa.WithPresentationImpl;
import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAbleImpl;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class GenericPropertyFactoryImpl extends WithPresentationImpl implements
		GenericPropertyFactory {
	protected GenericPropertyFactoryImpl() {
	}

	public Property createProperty(String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (xukLocalName == null || xukNamespaceURI == null) {
			throw new MethodParameterIsNullException();
		}
		if (xukLocalName == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		if (xukNamespaceURI == XukAbleImpl.XUK_NS) {
			if (xukLocalName == "Property") {
				Property newProp = new PropertyImpl();
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
