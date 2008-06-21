package org.daisy.urakawa;

import org.daisy.urakawa.command.ICommandFactory;
import org.daisy.urakawa.command.CommandFactoryImpl;
import org.daisy.urakawa.core.ITreeNodeFactory;
import org.daisy.urakawa.core.TreeNodeFactoryImpl;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.IMediaFactory;
import org.daisy.urakawa.media.MediaFactoryImpl;
import org.daisy.urakawa.media.data.IDataProviderFactory;
import org.daisy.urakawa.media.data.IDataProviderManager;
import org.daisy.urakawa.media.data.FileDataProviderFactoryImpl;
import org.daisy.urakawa.media.data.FileDataProviderManagerImpl;
import org.daisy.urakawa.media.data.IMediaDataFactory;
import org.daisy.urakawa.media.data.MediaDataFactoryImpl;
import org.daisy.urakawa.media.data.IMediaDataManager;
import org.daisy.urakawa.media.data.MediaDataManagerImpl;
import org.daisy.urakawa.metadata.IMetadataFactory;
import org.daisy.urakawa.metadata.MetadataFactoryImpl;
import org.daisy.urakawa.property.IPropertyFactory;
import org.daisy.urakawa.property.PropertyFactoryImpl;
import org.daisy.urakawa.property.channel.IChannelFactory;
import org.daisy.urakawa.property.channel.ChannelFactoryImpl;
import org.daisy.urakawa.property.channel.IChannelsManager;
import org.daisy.urakawa.property.channel.ChannelsManagerImpl;
import org.daisy.urakawa.undo.IUndoRedoManager;
import org.daisy.urakawa.undo.UndoRedoManagerImpl;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class DataModelFactoryImpl implements IDataModelFactory {
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
		// TODO: is there a less hacky way to handle the "Impl" naming
		// convention for concrete implementations of abstract interfaces ??
		if (str.endsWith("Impl")) {
			str = str.substring(0, str.length() - 4);
		}
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
		return new ChannelFactoryImpl();
	}

	public IChannelFactory createChannelFactory(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(ChannelFactoryImpl.class, xukLocalName, xukNamespaceUri);
	}

	public IChannelsManager createChannelsManager() {
		return new ChannelsManagerImpl();
	}

	public IChannelsManager createChannelsManager(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(ChannelsManagerImpl.class, xukLocalName, xukNamespaceUri);
	}

	public ICommandFactory createCommandFactory() {
		return new CommandFactoryImpl();
	}

	public ICommandFactory createCommandFactory(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(CommandFactoryImpl.class, xukLocalName, xukNamespaceUri);
	}

	public IDataProviderFactory createDataProviderFactory() {
		return new FileDataProviderFactoryImpl();
	}

	public IDataProviderFactory createDataProviderFactory(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(FileDataProviderFactoryImpl.class, xukLocalName,
				xukNamespaceUri);
	}

	public IDataProviderManager createDataProviderManager() {
		return new FileDataProviderManagerImpl();
	}

	public IDataProviderManager createDataProviderManager(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(FileDataProviderManagerImpl.class, xukLocalName,
				xukNamespaceUri);
	}

	public IMediaDataFactory createMediaDataFactory() {
		return new MediaDataFactoryImpl();
	}

	public IMediaDataFactory createMediaDataFactory(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(MediaDataFactoryImpl.class, xukLocalName, xukNamespaceUri);
	}

	public IMediaDataManager createMediaDataManager() {
		return new MediaDataManagerImpl();
	}

	public IMediaDataManager createMediaDataManager(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(MediaDataManagerImpl.class, xukLocalName, xukNamespaceUri);
	}

	public IMediaFactory createMediaFactory() {
		return new MediaFactoryImpl();
	}

	public IMediaFactory createMediaFactory(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(MediaFactoryImpl.class, xukLocalName, xukNamespaceUri);
	}

	public IMetadataFactory createMetadataFactory() {
		return new MetadataFactoryImpl();
	}

	public IMetadataFactory createMetadataFactory(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(MetadataFactoryImpl.class, xukLocalName, xukNamespaceUri);
	}

	public IPresentation createPresentation() {
		return new PresentationImpl();
	}

	public IPresentation createPresentation(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(PresentationImpl.class, xukLocalName, xukNamespaceUri);
	}

	public IPropertyFactory createPropertyFactory() {
		return new PropertyFactoryImpl();
	}

	public IPropertyFactory createPropertyFactory(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(PropertyFactoryImpl.class, xukLocalName, xukNamespaceUri);
	}

	public ITreeNodeFactory createTreeNodeFactory() {
		return new TreeNodeFactoryImpl();
	}

	public ITreeNodeFactory createTreeNodeFactory(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(TreeNodeFactoryImpl.class, xukLocalName, xukNamespaceUri);
	}

	public IUndoRedoManager createUndoRedoManager() {
		return new UndoRedoManagerImpl();
	}

	public IUndoRedoManager createUndoRedoManager(String xukLocalName,
			String xukNamespaceUri) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return create(UndoRedoManagerImpl.class, xukLocalName, xukNamespaceUri);
	}
}
