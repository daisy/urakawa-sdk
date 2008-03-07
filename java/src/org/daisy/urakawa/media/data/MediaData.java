package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.event.ChangeNotifier;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * A MediaData defines the actual resource where the data for the media object
 * is stored. One or more DataProviders can be associated to this single
 * MediaData resource. For example, it can be a single file (FileDataProvider),
 * but it could also be a sequence of various sources (e.g. MemoryDataProvider,
 * MySQLDataProvider, HTTPDataProvider, etc.).
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 0..n org.daisy.urakawa.media.data.DataProvider
 * @depend - Clone - org.daisy.urakawa.media.data.MediaData
 * @depend - Aggregation 1 org.daisy.urakawa.media.data.MediaDataManager
 * @stereotype XukAble
 */
public interface MediaData extends WithName, WithPresentation, XukAble,
		ValueEquatable<MediaData>, ChangeNotifier<DataModelChangedEvent> {
	/**
	 * Convenience method that obtains the MediaDataManager via the
	 * Presentation.
	 * 
	 * @return the manager
	 * @throws IsNotInitializedException
	 */
	public MediaDataManager getMediaDataManager()
			throws IsNotInitializedException;

	/**
	 * Convenience method to get the UID of this mediadata via
	 * getMediaDataManager().getUidOfMediaData()
	 * 
	 * @return the UID
	 */
	public String getUID();

	/**
	 * Deletes the MediaData detaching it from it's manager and releasing any
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
	public MediaData copy();

	/**
	 * Returns the DataProviders used by this mediadata
	 * 
	 * @return a non-null list of DataProvider, potentially empty
	 */
	public List<DataProvider> getListOfUsedDataProviders();

	/**
	 * @param destPres
	 * @return can return null in case of failure.
	 * @throws FactoryCannotCreateTypeException
	 * @tagvalue Exceptions "FactoryCannotCreateType-MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public MediaData export(Presentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException;
}
