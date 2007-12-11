package org.daisy.urakawa;

import org.daisy.urakawa.core.TreeNodeFactory;
import org.daisy.urakawa.core.TreeNodeFactoryImpl;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.MediaFactory;
import org.daisy.urakawa.media.MediaFactoryImpl;
import org.daisy.urakawa.media.data.DataProviderFactory;
import org.daisy.urakawa.media.data.DataProviderFactoryImpl;
import org.daisy.urakawa.media.data.DataProviderManager;
import org.daisy.urakawa.media.data.DataProviderManagerImpl;
import org.daisy.urakawa.media.data.MediaDataFactory;
import org.daisy.urakawa.media.data.MediaDataFactoryImpl;
import org.daisy.urakawa.media.data.MediaDataManager;
import org.daisy.urakawa.media.data.MediaDataManagerImpl;
import org.daisy.urakawa.metadata.MetadataFactory;
import org.daisy.urakawa.metadata.MetadataFactoryImpl;
import org.daisy.urakawa.property.channel.ChannelFactory;
import org.daisy.urakawa.property.channel.ChannelFactoryImpl;
import org.daisy.urakawa.property.channel.ChannelsManager;
import org.daisy.urakawa.property.channel.ChannelsManagerImpl;
import org.daisy.urakawa.undo.CommandFactory;
import org.daisy.urakawa.undo.CommandFactoryImpl;
import org.daisy.urakawa.undo.UndoRedoManager;
import org.daisy.urakawa.undo.UndoRedoManagerImpl;
import org.daisy.urakawa.xuk.XukAbleImpl;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class DataModelFactoryImpl implements DataModelFactory {
	/**
	 * TODO: Check that this instantiation mechanism actually works in Java 1.5
	 * @param <T>
	 * @param klass
	 * @param xukLocalName
	 * @param xukNamespaceUri
	 * @return
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
		if (klass.getSimpleName() != xukLocalName || xukNamespaceUri != XukAbleImpl.XUK_NS) {
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

	public ChannelFactory createChannelFactory() {
		return new ChannelFactoryImpl();
	}

	public ChannelFactory createChannelFactory(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(ChannelFactoryImpl.class, xukLocalName, xukNamespaceUri);
	}

	public ChannelsManager createChannelsManager() {
		return new ChannelsManagerImpl();
	}

	public ChannelsManager createChannelsManager(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(ChannelsManagerImpl.class, xukLocalName, xukNamespaceUri);
	}

	public CommandFactory createCommandFactory() {
		return new CommandFactoryImpl();
	}

	public CommandFactory createCommandFactory(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(CommandFactoryImpl.class, xukLocalName, xukNamespaceUri);
	}

	public DataProviderFactory createDataProviderFactory() {
		return new DataProviderFactoryImpl();
	}

	public DataProviderFactory createDataProviderFactory(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(DataProviderFactoryImpl.class, xukLocalName,
				xukNamespaceUri);
	}

	public DataProviderManager createDataProviderManager() {
		return new DataProviderManagerImpl();
	}

	public DataProviderManager createDataProviderManager(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(DataProviderManagerImpl.class, xukLocalName,
				xukNamespaceUri);
	}

	public MediaDataFactory createMediaDataFactory() {
		return new MediaDataFactoryImpl();
	}

	public MediaDataFactory createMediaDataFactory(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(MediaDataFactoryImpl.class, xukLocalName, xukNamespaceUri);
	}

	public MediaDataManager createMediaDataManager() {
		return new MediaDataManagerImpl();
	}

	public MediaDataManager createMediaDataManager(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(MediaDataManagerImpl.class, xukLocalName, xukNamespaceUri);
	}

	public MediaFactory createMediaFactory() {
		return new MediaFactoryImpl();
	}

	public MediaFactory createMediaFactory(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(MediaFactoryImpl.class, xukLocalName, xukNamespaceUri);
	}

	public MetadataFactory createMetadataFactory() {
		return new MetadataFactoryImpl();
	}

	public MetadataFactory createMetadataFactory(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(MetadataFactoryImpl.class, xukLocalName, xukNamespaceUri);
	}

	public Presentation createPresentation() {
		return new PresentationImpl();
	}

	public Presentation createPresentation(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(PresentationImpl.class, xukLocalName, xukNamespaceUri);
	}

	public PropertyFactory createPropertyFactory() {
		return new PropertyFactoryImpl();
	}

	public PropertyFactory createPropertyFactory(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(PropertyFactoryImpl.class, xukLocalName, xukNamespaceUri);
	}

	public TreeNodeFactory createTreeNodeFactory() {
		return new TreeNodeFactoryImpl();
	}

	public TreeNodeFactory createTreeNodeFactory(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(TreeNodeFactoryImpl.class, xukLocalName, xukNamespaceUri);
	}

	public UndoRedoManager createUndoRedoManager() {
		return new UndoRedoManagerImpl();
	}

	public UndoRedoManager createUndoRedoManager(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(UndoRedoManagerImpl.class, xukLocalName, xukNamespaceUri);
	}
}
