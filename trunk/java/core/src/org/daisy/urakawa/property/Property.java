package org.daisy.urakawa.property;

import java.net.URI;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.IPresentation;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.core.ITreeNode;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.event.Event;
import org.daisy.urakawa.event.IEventHandler;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.IEventListener;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.ObjectIsInDifferentPresentationException;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class Property extends WithPresentation implements IProperty {
	protected IEventHandler<Event> mDataModelEventNotifier = new EventHandler();
	public IEventListener<DataModelChangedEvent> mBubbleEventListener = new IEventListener<DataModelChangedEvent>() {
		public <K extends DataModelChangedEvent> void eventCallback(K event)
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
			IEventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		mDataModelEventNotifier.registerListener(listener, klass);
	}

	public <K extends DataModelChangedEvent> void unregisterListener(
			IEventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		mDataModelEventNotifier.unregisterListener(listener, klass);
	}

	protected Property() {
		;
	}

	private ITreeNode mOwner = null;

	public IPropertyFactory getPropertyFactory()
			throws IsNotInitializedException {
		return getPresentation().getPropertyFactory();
	}

	public boolean canBeAddedTo(ITreeNode node)
			throws MethodParameterIsNullException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		return true;
	}

	public IProperty copy() throws FactoryCannotCreateTypeException,
			IsNotInitializedException {
		return copyProtected();
	}

	protected IProperty copyProtected()
			throws FactoryCannotCreateTypeException, IsNotInitializedException {
		IProperty theCopy;
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

	public IProperty export(IPresentation destPres)
			throws FactoryCannotCreateTypeException, IsNotInitializedException,
			MethodParameterIsNullException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		return exportProtected(destPres);
	}

	protected IProperty exportProtected(IPresentation destPres)
			throws FactoryCannotCreateTypeException, IsNotInitializedException,
			MethodParameterIsNullException {
		IProperty exportedProp = null;
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

	public ITreeNode getTreeNodeOwner() throws IsNotInitializedException {
		if (mOwner == null) {
			throw new IsNotInitializedException();
		}
		return mOwner;
	}

	public void setTreeNodeOwner(ITreeNode newOwner)
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

	public boolean ValueEquals(IProperty other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		if (getClass() != other.getClass())
			return false;
		return true;
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukInAttributes(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
	}

	@Override
	protected void clear() {
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukOutAttributes(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukOutChildren(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
	}
}
