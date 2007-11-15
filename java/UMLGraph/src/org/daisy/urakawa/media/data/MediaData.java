package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
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
public interface MediaData extends WithMediaDataManager, WithPresentation, XukAble,
		ValueEquatable<MediaData> {
	public String getUid();

	public String getName();

	/**
	 * @param newName
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 */
	public void setName(String newName) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	public void delete();

	/**
	 * <p>
	 * Cloning method
	 * </p>
	 * 
	 * @return a copy.
	 */
	public MediaData copy();

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
