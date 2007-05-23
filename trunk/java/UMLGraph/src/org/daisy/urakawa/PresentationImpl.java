package org.daisy.urakawa;

import java.net.URI;

import org.daisy.urakawa.core.CoreNode;
import org.daisy.urakawa.core.CoreNodeFactory;
import org.daisy.urakawa.core.event.CoreNodeChangeEvent;
import org.daisy.urakawa.core.event.CoreNodeChangeListener;
import org.daisy.urakawa.core.property.CorePropertyFactory;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.media.MediaFactory;
import org.daisy.urakawa.media.data.DataProviderManager;
import org.daisy.urakawa.media.data.MediaDataManager;
import org.daisy.urakawa.media.data.MediaDataFactory;
import org.daisy.urakawa.properties.channel.ChannelFactory;
import org.daisy.urakawa.properties.channel.ChannelsManager;
import org.daisy.urakawa.properties.channel.ChannelsPropertyFactory;
import org.daisy.urakawa.properties.xml.XmlPropertyFactory;

public class PresentationImpl implements Presentation {
	/**
	 * @hidden
	 */
	public CoreNode getRootNode() {
		return null;
	}

	/**
	 * @hidden
	 */
	public ChannelFactory getChannelFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public ChannelsPropertyFactory getChannelsPropertyFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public ChannelsManager getChannelsManager() {
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
	public MediaDataManager getMediaAssetManager() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setMediaAssetManager(MediaDataManager man) {
	}

	/**
	 * @hidden
	 */
	public MediaDataFactory getMediaAssetFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setMediaAssetFactory(MediaDataFactory man) {
	}

	/**
	 * @hidden
	 */
	public MediaFactory getMediaFactory() {
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
	public void setChannelsManager(ChannelsManager man)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public void setChannelFactory(ChannelFactory fact)
			throws MethodParameterIsNullException {
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
	public void setMediaFactory(MediaFactory fact)
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
	public XmlPropertyFactory getXmlPropertyFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public boolean ValueEquals(Presentation other)
			throws MethodParameterIsNullException {
		return false;
	}

	/**
	 * @hidden
	 */

	public Project getProject() {

		return null;
	}

	/**
	 * @hidden
	 */

	public void setProject(Project project)
			throws MethodParameterIsNullException {

	}

	/**
	 * @hidden
	 */
	public void notifyCoreNodeChangeListeners(CoreNodeChangeEvent changeEvent)
			throws MethodParameterIsNullException {

	}

	/**
	 * @hidden
	 */
	public void registerCoreNodeChangeListener(CoreNodeChangeListener listener)
			throws MethodParameterIsNullException {

	}

	/**
	 * @hidden
	 */
	public void unregisterCoreNodeChangeListener(CoreNodeChangeListener listener)
			throws MethodParameterIsNullException {

	}
	/**
	 * @hidden
	 */
	public DataProviderManager getDataProviderManager() {
		
		return null;
	}
	/**
	 * @hidden
	 */
	public MediaDataManager getMediaDataManager() {
		
		return null;
	}
	/**
	 * @hidden
	 */
	public void setDataProviderManager(DataProviderManager man) {
	
		
	}
	/**
	 * @hidden
	 */
	public void setMediaDataManager(MediaDataManager man) {
	
		
	}
	/**
	 * @hidden
	 */
	public URI getBaseUri() {
	
		return null;
	}
	/**
	 * @hidden
	 */
	public void setBaseUri(URI newBase) {
	
		
	}
}
