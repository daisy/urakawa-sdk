package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.IsAlreadyManagerOfException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.IsNotManagerOfException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 * @depend - Composition 0..n org.daisy.urakawa.media.data.DataProvider
 * @stereotype XukAble
 */
public interface DataProviderManager extends WithPresentation, XukAble,
		ValueEquatable<DataProviderManager> {
	/**
	 * @return the DataProviderFactory for this DataProviderManager
	 * @throws IsNotInitializedException 
	 */
	public DataProviderFactory getDataProviderFactory() throws IsNotInitializedException;

	/**
	 * Gets the UID of a given DataProvider
	 * 
	 * @param provider
	 * @return the UID
	 * @throws MethodParameterIsNullException
	 * @throws IsNotManagerOfException
	 */
	public String getUidOfDataProvider(DataProvider provider)
			throws MethodParameterIsNullException, IsNotManagerOfException;

	/**
	 * Gets the DataProvider with a given UID
	 * 
	 * @param uid
	 * @return the provider
	 * @throws IsNotManagerOfException
	 * @throws MethodParameterIsNullException
	 * @throws MethodParameterIsEmptyStringException
	 */
	public DataProvider getDataProvider(String uid)
			throws IsNotManagerOfException, MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * Determines if the manager manages a DataProvider with a given uid
	 * 
	 * @param uid
	 * @return true or false
	 * @throws MethodParameterIsNullException
	 * @throws MethodParameterIsEmptyStringException
	 */
	public boolean isManagerOf(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * Removes one of the DataProvider managed by the manager
	 * 
	 * @param provider
	 * @param delete
	 * @throws MethodParameterIsNullException
	 * @throws IsNotManagerOfException
	 */
	public void removeDataProvider(DataProvider provider, boolean delete)
			throws MethodParameterIsNullException, IsNotManagerOfException;

	/**
	 * Removes the DataProvider with a given UID from the // manager
	 * 
	 * @param uid
	 * @param delete
	 * @throws MethodParameterIsNullException
	 * @throws IsNotManagerOfException
	 * @throws MethodParameterIsEmptyStringException
	 */
	public void removeDataProvider(String uid, boolean delete)
			throws MethodParameterIsNullException, IsNotManagerOfException, MethodParameterIsEmptyStringException;

	/**
	 * Adds a DataProvider to the DataProviderManager
	 * 
	 * @param provider
	 * @throws MethodParameterIsNullException
	 * @throws IsAlreadyManagerOfException
	 * @throws IsNotManagerOfException 
	 * @throws MethodParameterIsEmptyStringException 
	 */
	public void addDataProvider(DataProvider provider)
			throws MethodParameterIsNullException, IsAlreadyManagerOfException, MethodParameterIsEmptyStringException, IsNotManagerOfException;

	/**
	 * Sets the uid of a given managed DataProvider to a given value
	 * 
	 * @param provider
	 * @param uid
	 * @throws MethodParameterIsNullException
	 * @throws IsAlreadyManagerOfException
	 * @throws IsNotManagerOfException
	 * @throws MethodParameterIsEmptyStringException
	 */
	public void setDataProviderUid(DataProvider provider, String uid)
			throws MethodParameterIsNullException, IsAlreadyManagerOfException,
			IsNotManagerOfException, MethodParameterIsEmptyStringException;

	/**
	 * Gets a list of the DataProviders that are managed by the
	 * DataProviderManager
	 * 
	 * @return a non-null but potentially empty list
	 */
	public List<DataProvider> getListOfDataProviders();

	/**
	 * Removes any DataProviders "not used", that is all DataProvider that are
	 * not used by a MediaData of the Presentation
	 * 
	 * @param delete
	 */
	public void removeUnusedDataProviders(boolean delete);
}
