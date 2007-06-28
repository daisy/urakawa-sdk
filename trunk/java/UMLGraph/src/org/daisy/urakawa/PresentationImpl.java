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
import org.daisy.urakawa.metadata.MetadataFactory;
import org.daisy.urakawa.property.GenericPropertyFactory;
import org.daisy.urakawa.property.channel.ChannelFactory;
import org.daisy.urakawa.property.channel.ChannelsManager;
import org.daisy.urakawa.property.channel.ChannelsPropertyFactory;
import org.daisy.urakawa.property.xml.XmlPropertyFactory;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class PresentationImpl implements Presentation {
	public PropertyFactory getPropertyFactory() {
		return null;
	}

	public void setPropertyFactory(PropertyFactory factory)
			throws MethodParameterIsNullException {
	}

	public Project getProject() {
		return null;
	}

	public void setProject(Project project)
			throws MethodParameterIsNullException {
	}

	public URI getBaseUri() {
		return null;
	}

	public List<Media> getListOfUsedMedia() {
		return null;
	}

	public void setBaseUri(URI newBase) throws MethodParameterIsNullException {
	}

	public MediaFactory getMediaFactory() {
		return null;
	}

	public void setMediaFactory(MediaFactory factory)
			throws MethodParameterIsNullException {
	}

	public void notifyTreeNodeChangedListeners(TreeNodeChangedEvent changeEvent)
			throws MethodParameterIsNullException {
	}

	public void registerTreeNodeChangedListener(TreeNodeChangedListener listener)
			throws MethodParameterIsNullException {
	}

	public void unregisterTreeNodeChangedListener(
			TreeNodeChangedListener listener)
			throws MethodParameterIsNullException {
	}

	public void notifyTreeNodeAddedListeners(TreeNodeAddedEvent changeEvent)
			throws MethodParameterIsNullException {
	}

	public void notifyTreeNodeRemovedListeners(TreeNodeRemovedEvent changeEvent)
			throws MethodParameterIsNullException {
	}

	public void registerTreeNodeAddedRemovedListener(
			TreeNodeAddedRemovedListener listener)
			throws MethodParameterIsNullException {
	}

	public void unregisterTreeNodeAddedRemovedListener(
			TreeNodeAddedRemovedListener listener)
			throws MethodParameterIsNullException {
	}

	public TreeNode getTreeNode() {
		return null;
	}

	public void setTreeNode(TreeNode node)
			throws MethodParameterIsNullException {
	}

	public TreeNodeFactory getTreeNodeFactory() {
		return null;
	}

	public void setTreeNodeFactory(TreeNodeFactory factory)
			throws MethodParameterIsNullException {
	}

	public GenericPropertyFactory getCorePropertyFactory() {
		return null;
	}

	public void setCorePropertyFactory(GenericPropertyFactory factory)
			throws MethodParameterIsNullException {
	}

	public ChannelFactory getChannelFactory() {
		return null;
	}

	public void setChannelFactory(ChannelFactory factory)
			throws MethodParameterIsNullException {
	}

	public ChannelsPropertyFactory getChannelsPropertyFactory() {
		return null;
	}

	public void setChannelsPropertyFactory(ChannelsPropertyFactory factory)
			throws MethodParameterIsNullException {
	}

	public ChannelsManager getChannelsManager() {
		return null;
	}

	public void setChannelsManager(ChannelsManager manager)
			throws MethodParameterIsNullException {
	}

	public XmlPropertyFactory getXmlPropertyFactory() {
		return null;
	}

	public void setXmlPropertyFactory(XmlPropertyFactory factory)
			throws MethodParameterIsNullException {
	}

	public List<MediaData> getListOfUsedMediaData() {
		return null;
	}

	public MediaDataFactory getMediaDataFactory() {
		return null;
	}

	public void setMediaDataFactory(MediaDataFactory factory)
			throws MethodParameterIsNullException {
	}

	public MediaDataManager getMediaDataManager() {
		return null;
	}

	public void setMediaDataManager(MediaDataManager manager)
			throws MethodParameterIsNullException {
	}

	public DataProviderManager getDataProviderManager() {
		return null;
	}

	public void setDataProviderManager(DataProviderManager manager)
			throws MethodParameterIsNullException {
	}

	public DataProviderFactory getDataProviderFactory() {
		return null;
	}

	public void setDataProviderFactory(DataProviderFactory factory)
			throws MethodParameterIsNullException {
	}

	public boolean ValueEquals(Presentation other)
			throws MethodParameterIsNullException {
		return false;
	}

	public String getXukLocalName() {
		return null;
	}

	public String getXukNamespaceURI() {
		return null;
	}

	public void XukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}

	public MetadataFactory getMetadataFactory() {
		return null;
	}

	public void setMetadataFactory(MetadataFactory factory)
			throws MethodParameterIsNullException {
	}

	public void appendMetadata(Metadata metadata)
			throws MethodParameterIsNullException {
	}

	public void deleteMetadata(String name)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	public void deleteMetadata(Metadata metadata)
			throws MethodParameterIsNullException {
	}

	public List<Metadata> getListOfMetadata(String name)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	public List<Metadata> getListOfMetadata() {
		return null;
	}

	public String getLanguage() {
		return null;
	}

	public void setLanguage(String name)
			throws MethodParameterIsEmptyStringException {
	}

	public void cleanup() {
	}
}
