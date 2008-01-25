package org.daisy.urakawa.property;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.WithPresentationImpl;
import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.event.ChangeListener;
import org.daisy.urakawa.event.ChangeNotifier;
import org.daisy.urakawa.event.ChangeNotifierImpl;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.ObjectIsInDifferentPresentationException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class PropertyImpl extends WithPresentationImpl implements Property {
	protected ChangeNotifier<DataModelChangedEvent> mDataModelEventNotifier = new ChangeNotifierImpl();
	protected ChangeListener<DataModelChangedEvent> mBubbleEventListener = new ChangeListener<DataModelChangedEvent>() {
		
		public <K extends DataModelChangedEvent> void changeHappened(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			notifyListeners(event);
		}
	};

	public <K extends DataModelChangedEvent> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		if (event == null) {
			throw new MethodParameterIsNullException();
		}
		mDataModelEventNotifier.notifyListeners(event);
	}

	public <K extends DataModelChangedEvent> void registerListener(
			ChangeListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		mDataModelEventNotifier.registerListener(listener, klass);
	}

	public <K extends DataModelChangedEvent> void unregisterListener(
			ChangeListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		mDataModelEventNotifier.unregisterListener(listener, klass);
	}

	protected PropertyImpl() {
		;
	}

	private TreeNode mOwner = null;

	public PropertyFactory getPropertyFactory()
			throws IsNotInitializedException {
		return getPresentation().getPropertyFactory();
	}

	public boolean canBeAddedTo(TreeNode node)
			throws MethodParameterIsNullException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		return true;
	}

	public Property copy() throws FactoryCannotCreateTypeException,
			IsNotInitializedException {
		return copyProtected();
	}

	protected Property copyProtected() throws FactoryCannotCreateTypeException,
			IsNotInitializedException {
		Property theCopy;
		try {
			theCopy = getTreeNodeOwner().getPresentation().getPropertyFactory()
					.createProperty(getXukLocalName(), getXukNamespaceURI());
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (theCopy == null) {
			throw new FactoryCannotCreateTypeException();
		}
		return theCopy;
	}

	public Property export(Presentation destPres)
			throws FactoryCannotCreateTypeException, IsNotInitializedException,
			MethodParameterIsNullException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		return exportProtected(destPres);
	}

	protected Property exportProtected(Presentation destPres)
			throws FactoryCannotCreateTypeException, IsNotInitializedException,
			MethodParameterIsNullException {
		Property exportedProp = null;
		try {
			exportedProp = destPres.getPropertyFactory().createProperty(
					getXukLocalName(), getXukNamespaceURI());
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (exportedProp == null) {
			throw new FactoryCannotCreateTypeException();
		}
		return exportedProp;
	}

	public TreeNode getTreeNodeOwner() throws IsNotInitializedException {
		if (mOwner == null) {
			throw new IsNotInitializedException();
		}
		return mOwner;
	}

	public void setTreeNodeOwner(TreeNode newOwner)
			throws PropertyAlreadyHasOwnerException,
			ObjectIsInDifferentPresentationException {
		if (mOwner != null && newOwner != mOwner) {
			throw new PropertyAlreadyHasOwnerException();
		}
		try {
			if (newOwner.getPresentation() != getPresentation()) {
				throw new ObjectIsInDifferentPresentationException();
			}
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		mOwner = newOwner;
	}

	public boolean ValueEquals(Property other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		if (getClass() != other.getClass())
			return false;
		return true;
	}
}
