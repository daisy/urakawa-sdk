package org.daisy.urakawa.core;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.core.events.CoreNodeAddedEvent;
import org.daisy.urakawa.core.events.CoreNodeAddedRemovedListener;
import org.daisy.urakawa.core.events.CoreNodeChangedEvent;
import org.daisy.urakawa.core.events.CoreNodeChangedListener;
import org.daisy.urakawa.core.events.CoreNodeRemovedEvent;
import org.daisy.urakawa.core.property.CorePropertyFactory;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 */
public class CorePresentationImpl implements CorePresentation {
	/**
	 * @hidden
	 */
	public CoreNode getRootNode() {
		return null;
	}

	/**
	 * @hidden
	 */
	public CoreNodeFactory getCoreNodeFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public CorePropertyFactory getPropertyFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setRootNode(CoreNode node) {
	}

	/**
	 * @hidden
	 */
	public void setCoreNodeFactory(CoreNodeFactory fact)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public void setPropertyFactory(CorePropertyFactory fact)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public boolean XukIn(XmlDataReader source) {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean XukOut(XmlDataWriter destination) {
		return false;
	}

	/**
	 * @hidden
	 */
	public String getXukLocalName() {
		return null;
	}

	/**
	 * @hidden
	 */
	public String getXukNamespaceURI() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void notifyCoreNodeChangedListeners(CoreNodeChangedEvent changeEvent)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public void registerCoreNodeChangedListener(CoreNodeChangedListener listener)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public void unregisterCoreNodeChangedListener(
			CoreNodeChangedListener listener)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public void notifyCoreNodeAddedListeners(CoreNodeAddedEvent changeEvent)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public void notifyCoreNodeRemovedListeners(CoreNodeRemovedEvent changeEvent)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public void registerCoreNodeAddedRemovedListener(
			CoreNodeAddedRemovedListener listener)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public void unregisterCoreNodeAddedRemovedListener(
			CoreNodeAddedRemovedListener listener)
			throws MethodParameterIsNullException {
	}
}
