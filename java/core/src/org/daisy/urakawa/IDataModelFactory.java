package org.daisy.urakawa;

import org.daisy.urakawa.command.ICommandFactory;
import org.daisy.urakawa.core.ITreeNodeFactory;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.IMediaFactory;
import org.daisy.urakawa.media.data.IDataProviderFactory;
import org.daisy.urakawa.media.data.IDataProviderManager;
import org.daisy.urakawa.media.data.IMediaDataFactory;
import org.daisy.urakawa.media.data.IMediaDataManager;
import org.daisy.urakawa.metadata.IMetadataFactory;
import org.daisy.urakawa.property.IPropertyFactory;
import org.daisy.urakawa.property.channel.IChannelFactory;
import org.daisy.urakawa.property.channel.IChannelsManager;
import org.daisy.urakawa.undo.IUndoRedoManager;

/**
 * <p>
 * This is the factory for creating instances of the main components of the
 * Urakawa data model. This is a non-IXukAble factory that can only be extended
 * by sub-classing the class to override existing methods of by providing new
 * creation methods.
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Create - org.daisy.urakawa.core.ITreeNodeFactory
 * @depend - Create - org.daisy.urakawa.property.IPropertyFactory
 * @depend - Create - org.daisy.urakawa.property.channel.IChannelFactory
 * @depend - Create - org.daisy.urakawa.property.channel.IChannelsManager
 * @depend - Create - org.daisy.urakawa.undo.CommandFactory
 * @depend - Create - org.daisy.urakawa.undo.IUndoRedoManager
 * @depend - Create - org.daisy.urakawa.media.data.IDataProviderFactory
 * @depend - Create - org.daisy.urakawa.media.data.IDataProviderManager
 * @depend - Create - org.daisy.urakawa.media.data.IMediaDataFactory
 * @depend - Create - org.daisy.urakawa.media.data.IMediaDataManager
 * @depend - Create - org.daisy.urakawa.media.IMediaFactory
 * @depend - Create - org.daisy.urakawa.metadata.IMetadataFactory
 * @depend - Create - org.daisy.urakawa.IPresentation
 */
public interface IDataModelFactory {
	/**
	 * <p>
	 * Creates a default new object instance.
	 * </p>
	 * 
	 * @return always returns a non-null value.
	 */
	public IPresentation createPresentation();

	/**
	 * <p>
	 * Creates a new object instance.
	 * </p>
	 * 
	 * @param xukLocalName
	 *            cannot be null, cannot be empty string.
	 * @param xukNamespaceURI
	 *            cannot be null, but can be empty string.
	 * @return can return null (in case the QName specification does not match
	 *         any supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public IPresentation createPresentation(String xukLocalName,
			String xukNamespaceURI) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * <p>
	 * Creates a default new object instance.
	 * </p>
	 * 
	 * @return always returns a non-null value.
	 */
	public IMetadataFactory createMetadataFactory();

	/**
	 * <p>
	 * Creates a new object instance.
	 * </p>
	 * 
	 * @param xukLocalName
	 *            cannot be null, cannot be empty string.
	 * @param xukNamespaceURI
	 *            cannot be null, but can be empty string.
	 * @return can return null (in case the QName specification does not match
	 *         any supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public IMetadataFactory createMetadataFactory(String xukLocalName,
			String xukNamespaceURI) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * <p>
	 * Creates a default new object instance.
	 * </p>
	 * 
	 * @return always returns a non-null value.
	 */
	public ICommandFactory createCommandFactory();

	/**
	 * <p>
	 * Creates a new object instance.
	 * </p>
	 * 
	 * @param xukLocalName
	 *            cannot be null, cannot be empty string.
	 * @param xukNamespaceURI
	 *            cannot be null, but can be empty string.
	 * @return can return null (in case the QName specification does not match
	 *         any supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public ICommandFactory createCommandFactory(String xukLocalName,
			String xukNamespaceURI) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * <p>
	 * Creates a default new object instance.
	 * </p>
	 * 
	 * @return always returns a non-null value.
	 */
	public IUndoRedoManager createUndoRedoManager();

	/**
	 * <p>
	 * Creates a new object instance.
	 * </p>
	 * 
	 * @param xukLocalName
	 *            cannot be null, cannot be empty string.
	 * @param xukNamespaceURI
	 *            cannot be null, but can be empty string.
	 * @return can return null (in case the QName specification does not match
	 *         any supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public IUndoRedoManager createUndoRedoManager(String xukLocalName,
			String xukNamespaceURI) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * <p>
	 * Creates a default new object instance.
	 * </p>
	 * 
	 * @return always returns a non-null value.
	 */
	public ITreeNodeFactory createTreeNodeFactory();

	/**
	 * <p>
	 * Creates a new object instance.
	 * </p>
	 * 
	 * @param xukLocalName
	 *            cannot be null, cannot be empty string.
	 * @param xukNamespaceURI
	 *            cannot be null, but can be empty string.
	 * @return can return null (in case the QName specification does not match
	 *         any supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public ITreeNodeFactory createTreeNodeFactory(String xukLocalName,
			String xukNamespaceURI) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * <p>
	 * Creates a default new object instance.
	 * </p>
	 * 
	 * @return always returns a non-null value.
	 */
	public IPropertyFactory createPropertyFactory();

	/**
	 * <p>
	 * Creates a new object instance.
	 * </p>
	 * 
	 * @param xukLocalName
	 *            cannot be null, cannot be empty string.
	 * @param xukNamespaceURI
	 *            cannot be null, but can be empty string.
	 * @return can return null (in case the QName specification does not match
	 *         any supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public IPropertyFactory createPropertyFactory(String xukLocalName,
			String xukNamespaceURI) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * <p>
	 * Creates a default new object instance.
	 * </p>
	 * 
	 * @return always returns a non-null value.
	 */
	public IChannelFactory createChannelFactory();

	/**
	 * <p>
	 * Creates a new object instance.
	 * </p>
	 * 
	 * @param xukLocalName
	 *            cannot be null, cannot be empty string.
	 * @param xukNamespaceURI
	 *            cannot be null, but can be empty string.
	 * @return can return null (in case the QName specification does not match
	 *         any supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public IChannelFactory createChannelFactory(String xukLocalName,
			String xukNamespaceURI) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * <p>
	 * Creates a default new object instance.
	 * </p>
	 * 
	 * @return always returns a non-null value.
	 */
	public IChannelsManager createChannelsManager();

	/**
	 * <p>
	 * Creates a new object instance.
	 * </p>
	 * 
	 * @param xukLocalName
	 *            cannot be null, cannot be empty string.
	 * @param xukNamespaceURI
	 *            cannot be null, but can be empty string.
	 * @return can return null (in case the QName specification does not match
	 *         any supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public IChannelsManager createChannelsManager(String xukLocalName,
			String xukNamespaceURI) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * <p>
	 * Creates a default new object instance.
	 * </p>
	 * 
	 * @return always returns a non-null value.
	 */
	public IMediaFactory createMediaFactory();

	/**
	 * <p>
	 * Creates a new object instance.
	 * </p>
	 * 
	 * @param xukLocalName
	 *            cannot be null, cannot be empty string.
	 * @param xukNamespaceURI
	 *            cannot be null, but can be empty string.
	 * @return can return null (in case the QName specification does not match
	 *         any supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public IMediaFactory createMediaFactory(String xukLocalName,
			String xukNamespaceURI) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * <p>
	 * Creates a default new object instance.
	 * </p>
	 * 
	 * @return always returns a non-null value.
	 */
	public IMediaDataFactory createMediaDataFactory();

	/**
	 * <p>
	 * Creates a new object instance.
	 * </p>
	 * 
	 * @param xukLocalName
	 *            cannot be null, cannot be empty string.
	 * @param xukNamespaceURI
	 *            cannot be null, but can be empty string.
	 * @return can return null (in case the QName specification does not match
	 *         any supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public IMediaDataFactory createMediaDataFactory(String xukLocalName,
			String xukNamespaceURI) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * <p>
	 * Creates a default new object instance.
	 * </p>
	 * 
	 * @return always returns a non-null value.
	 */
	public IMediaDataManager createMediaDataManager();

	/**
	 * <p>
	 * Creates a new object instance.
	 * </p>
	 * 
	 * @param xukLocalName
	 *            cannot be null, cannot be empty string.
	 * @param xukNamespaceURI
	 *            cannot be null, but can be empty string.
	 * @return can return null (in case the QName specification does not match
	 *         any supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public IMediaDataManager createMediaDataManager(String xukLocalName,
			String xukNamespaceURI) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * <p>
	 * Creates a default new object instance.
	 * </p>
	 * 
	 * @return always returns a non-null value.
	 */
	public IDataProviderFactory createDataProviderFactory();

	/**
	 * <p>
	 * Creates a new object instance.
	 * </p>
	 * 
	 * @param xukLocalName
	 *            cannot be null, cannot be empty string.
	 * @param xukNamespaceURI
	 *            cannot be null, but can be empty string.
	 * @return can return null (in case the QName specification does not match
	 *         any supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public IDataProviderFactory createDataProviderFactory(String xukLocalName,
			String xukNamespaceURI) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * <p>
	 * Creates a default new object instance.
	 * </p>
	 * 
	 * @return always returns a non-null value.
	 */
	public IDataProviderManager createDataProviderManager();

	/**
	 * <p>
	 * Creates a new object instance.
	 * </p>
	 * 
	 * @param xukLocalName
	 *            cannot be null, cannot be empty string.
	 * @param xukNamespaceURI
	 *            cannot be null, but can be empty string.
	 * @return can return null (in case the QName specification does not match
	 *         any supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public IDataProviderManager createDataProviderManager(String xukLocalName,
			String xukNamespaceURI) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;
}
