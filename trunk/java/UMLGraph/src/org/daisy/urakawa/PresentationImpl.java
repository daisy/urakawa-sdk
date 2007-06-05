package org.daisy.urakawa;

import java.net.URI;
import java.util.List;

import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.core.TreeNodeFactory;
import org.daisy.urakawa.core.events.TreeNodeAddedEvent;
import org.daisy.urakawa.core.events.TreeNodeAddedRemovedListener;
import org.daisy.urakawa.core.events.TreeNodeChangedEvent;
import org.daisy.urakawa.core.events.TreeNodeChangedListener;
import org.daisy.urakawa.core.events.TreeNodeRemovedEvent;
import org.daisy.urakawa.core.property.GenericPropertyFactory;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.media.MediaFactory;
import org.daisy.urakawa.media.data.DataProviderFactory;
import org.daisy.urakawa.media.data.DataProviderManager;
import org.daisy.urakawa.media.data.MediaData;
import org.daisy.urakawa.media.data.MediaDataFactory;
import org.daisy.urakawa.media.data.MediaDataManager;
import org.daisy.urakawa.metadata.Metadata;
import org.daisy.urakawa.properties.channel.ChannelFactory;
import org.daisy.urakawa.properties.channel.ChannelsManager;
import org.daisy.urakawa.properties.channel.ChannelsPropertyFactory;
import org.daisy.urakawa.properties.xml.XmlPropertyFactory;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class PresentationImpl implements Presentation {
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
	public ChannelFactory getChannelFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setChannelFactory(ChannelFactory factory)
			throws MethodParameterIsNullException {
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
	public void setChannelsPropertyFactory(ChannelsPropertyFactory factory)
			throws MethodParameterIsNullException {
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
	public void setChannelsManager(ChannelsManager manager)
			throws MethodParameterIsNullException {
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
	public void setXmlPropertyFactory(XmlPropertyFactory factory)
			throws MethodParameterIsNullException {
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
	public List<Media> getListOfUsedMedia() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setBaseUri(URI newBase) {
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
	public void setMediaFactory(MediaFactory factory)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public MediaDataFactory getMediaDataFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setMediaDataFactory(MediaDataFactory factory)
			throws MethodParameterIsNullException {
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
	public void setMediaDataManager(MediaDataManager manager)
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
	public void setDataProviderManager(DataProviderManager manager)
			throws MethodParameterIsNullException {
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
	public TreeNode getTreeNode() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setTreeNode(TreeNode node) {
	}

	/**
	 * @hidden
	 */
	public void notifyTreeNodeChangedListeners(TreeNodeChangedEvent changeEvent)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public void registerTreeNodeChangedListener(TreeNodeChangedListener listener)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public void unregisterTreeNodeChangedListener(
			TreeNodeChangedListener listener)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public void notifyTreeNodeAddedListeners(TreeNodeAddedEvent changeEvent)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public void notifyTreeNodeRemovedListeners(TreeNodeRemovedEvent changeEvent)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public void registerTreeNodeAddedRemovedListener(
			TreeNodeAddedRemovedListener listener)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public void unregisterTreeNodeAddedRemovedListener(
			TreeNodeAddedRemovedListener listener)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public TreeNodeFactory getTreeNodeFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setTreeNodeFactory(TreeNodeFactory factory)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public GenericPropertyFactory getCorePropertyFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setCorePropertyFactory(GenericPropertyFactory factory)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public boolean XukIn(XmlDataReader source)
			throws MethodParameterIsNullException {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException {
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
	public void appendMetadata(Metadata metadata)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public void deleteMetadata(String name)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	/**
	 * @hidden
	 */
	public void deleteMetadata(Metadata metadata)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public List<Metadata> getListOfMetadata(String name)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	/**
	 * @hidden
	 */
	public List<Metadata> getListOfMetadata() {
		return null;
	}

	/**
	 * @hidden
	 */
	public List<MediaData> getListOfUsedMediaData() {
		return null;
	}

	/**
	 * @hidden
	 */
	public PropertyFactory getPropertyFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setPropertyFactory(PropertyFactory factory)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public DataProviderFactory getDataProviderFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setDataProviderFactory(DataProviderFactory factory)
			throws MethodParameterIsNullException {
	}
}
