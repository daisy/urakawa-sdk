package org.daisy.urakawa;

import org.daisy.urakawa.command.ICommandFactory;
import org.daisy.urakawa.command.CommandFactory;
import org.daisy.urakawa.core.ITreeNodeFactory;
import org.daisy.urakawa.core.TreeNodeFactory;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.IMediaFactory;
import org.daisy.urakawa.media.MediaFactory;
import org.daisy.urakawa.media.data.IDataProviderFactory;
import org.daisy.urakawa.media.data.IDataProviderManager;
import org.daisy.urakawa.media.data.DataProviderFactory;
import org.daisy.urakawa.media.data.DataProviderManager;
import org.daisy.urakawa.media.data.IMediaDataFactory;
import org.daisy.urakawa.media.data.MediaDataFactory;
import org.daisy.urakawa.media.data.IMediaDataManager;
import org.daisy.urakawa.media.data.MediaDataManager;
import org.daisy.urakawa.metadata.IMetadataFactory;
import org.daisy.urakawa.metadata.MetadataFactory;
import org.daisy.urakawa.property.IPropertyFactory;
import org.daisy.urakawa.property.PropertyFactory;
import org.daisy.urakawa.property.channel.IChannelFactory;
import org.daisy.urakawa.property.channel.ChannelFactory;
import org.daisy.urakawa.property.channel.IChannelsManager;
import org.daisy.urakawa.property.channel.ChannelsManager;
import org.daisy.urakawa.undo.IUndoRedoManager;
import org.daisy.urakawa.undo.UndoRedoManager;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public final class DataModelFactory implements IDataModelFactory {
	/**
	 * TODO: Check that this instantiation mechanism actually works in Java 1.5
	 * 
	 * @param <T>
	 * @param klass
	 * @param xukLocalName
	 * @param xukNamespaceUri
	 * @return the created instance
	 * @throws MethodParameterIsNullException
	 * @throws MethodParameterIsEmptyStringException
	 */
	private <T> T create(Class<T> klass, String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (xukLocalName == null || xukNamespaceUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (xukLocalName.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		String str = klass.getSimpleName();
		
		if (str != xukLocalName || xukNamespaceUri != IXukAble.XUK_NS) {
			return null;
		}
		try {
			return klass.newInstance();
		} catch (InstantiationException e) {
			e.printStackTrace();
		} catch (IllegalAccessException e) {
			e.printStackTrace();
		}
		return null;
	}

	public IChannelFactory createChannelFactory() {
		return new ChannelFactory();
	}

	public IChannelFactory createChannelFactory(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(ChannelFactory.class, xukLocalName, xukNamespaceUri);
	}

	public IChannelsManager createChannelsManager() {
		return new ChannelsManager();
	}

	public IChannelsManager createChannelsManager(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(ChannelsManager.class, xukLocalName, xukNamespaceUri);
	}

	public ICommandFactory createCommandFactory() {
		return new CommandFactory();
	}

	public ICommandFactory createCommandFactory(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(CommandFactory.class, xukLocalName, xukNamespaceUri);
	}

	public IDataProviderFactory createDataProviderFactory() {
		return new DataProviderFactory();
	}

	public IDataProviderFactory createDataProviderFactory(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(DataProviderFactory.class, xukLocalName,
				xukNamespaceUri);
	}

	public IDataProviderManager createDataProviderManager() {
		return new DataProviderManager();
	}

	public IDataProviderManager createDataProviderManager(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(DataProviderManager.class, xukLocalName,
				xukNamespaceUri);
	}

	public IMediaDataFactory createMediaDataFactory() {
		return new MediaDataFactory();
	}

	public IMediaDataFactory createMediaDataFactory(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(MediaDataFactory.class, xukLocalName, xukNamespaceUri);
	}

	public IMediaDataManager createMediaDataManager() {
		return new MediaDataManager();
	}

	public IMediaDataManager createMediaDataManager(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(MediaDataManager.class, xukLocalName, xukNamespaceUri);
	}

	public IMediaFactory createMediaFactory() {
		return new MediaFactory();
	}

	public IMediaFactory createMediaFactory(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(MediaFactory.class, xukLocalName, xukNamespaceUri);
	}

	public IMetadataFactory createMetadataFactory() {
		return new MetadataFactory();
	}

	public IMetadataFactory createMetadataFactory(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(MetadataFactory.class, xukLocalName, xukNamespaceUri);
	}

	public IPresentation createPresentation() {
		return new Presentation();
	}

	public IPresentation createPresentation(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(Presentation.class, xukLocalName, xukNamespaceUri);
	}

	public IPropertyFactory createPropertyFactory() {
		return new PropertyFactory();
	}

	public IPropertyFactory createPropertyFactory(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(PropertyFactory.class, xukLocalName, xukNamespaceUri);
	}

	public ITreeNodeFactory createTreeNodeFactory() {
		return new TreeNodeFactory();
	}

	public ITreeNodeFactory createTreeNodeFactory(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(TreeNodeFactory.class, xukLocalName, xukNamespaceUri);
	}

	public IUndoRedoManager createUndoRedoManager() {
		return new UndoRedoManager();
	}

	public IUndoRedoManager createUndoRedoManager(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(UndoRedoManager.class, xukLocalName, xukNamespaceUri);
	}
}
