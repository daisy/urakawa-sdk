package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.IPresentation;
import org.daisy.urakawa.IValueEquatable;
import org.daisy.urakawa.IWithPresentation;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.event.IEventHandler;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * A IMediaData defines the actual resource where the data for the media object
 * is stored. One or more DataProviders can be associated to this single
 * IMediaData resource. For example, it can be a single file (IFileDataProvider),
 * but it could also be a sequence of various sources (e.g. MemoryDataProvider,
 * MySQLDataProvider, HTTPDataProvider, etc.).
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 0..n org.daisy.urakawa.media.data.IDataProvider
 * @depend - Clone - org.daisy.urakawa.media.data.IMediaData
 * @depend - Aggregation 1 org.daisy.urakawa.media.data.IMediaDataManager
 * 
 */
public interface IMediaData extends IWithName, IWithPresentation, IXukAble,
		IValueEquatable<IMediaData>, IEventHandler<DataModelChangedEvent> {

	/**
	 * Convenience method to get the UID of this mediadata via
	 * getMediaDataManager().getUidOfMediaData()
	 * 
	 * @return the UID
	 */
	public String getUID();

	/**
	 * Deletes the IMediaData detaching it from it's manager and releasing any
	 * DataProviders used
	 */
	public void delete();

	/**
	 * <p>
	 * Cloning method
	 * </p>
	 * 
	 * @return a copy.
	 */
	public IMediaData copy();

	/**
	 * Returns the DataProviders used by this mediadata
	 * 
	 * @return a non-null list of IDataProvider, potentially empty
	 */
	public List<IDataProvider> getListOfUsedDataProviders();

	/**
	 * @param destPres
	 * @return can return null in case of failure.
	 * @throws FactoryCannotCreateTypeException
	 * @tagvalue Exceptions "FactoryCannotCreateType-MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public IMediaData export(IPresentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException;
}
