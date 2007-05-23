package org.daisy.urakawa.core;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.core.event.CoreNodeAddedEvent;
import org.daisy.urakawa.core.event.CoreNodeAddedRemovedListener;
import org.daisy.urakawa.core.event.CoreNodeChangedEvent;
import org.daisy.urakawa.core.event.CoreNodeChangedListener;
import org.daisy.urakawa.core.event.CoreNodeRemovedEvent;
import org.daisy.urakawa.core.property.CorePropertyFactory;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * @depend - - - CoreNode
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
