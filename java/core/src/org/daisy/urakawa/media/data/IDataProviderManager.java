package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.IValueEquatable;
import org.daisy.urakawa.IWithPresentation;
import org.daisy.urakawa.exception.IsAlreadyManagerOfException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.IsNotManagerOfException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * @depend - Aggregation 1 org.daisy.urakawa.IPresentation
 * @depend - Composition 0..n org.daisy.urakawa.media.data.IDataProvider
 * @stereotype IXukAble
 */
public interface IDataProviderManager extends IWithPresentation, IXukAble,
		IValueEquatable<IDataProviderManager> {
	/**
	 * @return the IDataProviderFactory for this IDataProviderManager
	 * @throws IsNotInitializedException 
	 */
	public IDataProviderFactory getDataProviderFactory() throws IsNotInitializedException;

	/**
	 * Gets the UID of a given IDataProvider
	 * 
	 * @param provider
	 * @return the UID
	 * @throws MethodParameterIsNullException
	 * @throws IsNotManagerOfException
	 */
	public String getUidOfDataProvider(IDataProvider provider)
			throws MethodParameterIsNullException, IsNotManagerOfException;

	/**
	 * Gets the IDataProvider with a given UID
	 * 
	 * @param uid
	 * @return the provider
	 * @throws IsNotManagerOfException
	 * @throws MethodParameterIsNullException
	 * @throws MethodParameterIsEmptyStringException
	 */
	public IDataProvider getDataProvider(String uid)
			throws IsNotManagerOfException, MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * Determines if the manager manages a IDataProvider with a given uid
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
	 * Removes one of the IDataProvider managed by the manager
	 * 
	 * @param provider
	 * @param delete
	 * @throws MethodParameterIsNullException
	 * @throws IsNotManagerOfException
	 */
	public void removeDataProvider(IDataProvider provider, boolean delete)
			throws MethodParameterIsNullException, IsNotManagerOfException;

	/**
	 * Removes the IDataProvider with a given UID from the // manager
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
	 * Adds a IDataProvider to the IDataProviderManager
	 * 
	 * @param provider
	 * @throws MethodParameterIsNullException
	 * @throws IsAlreadyManagerOfException
	 * @throws IsNotManagerOfException 
	 * @throws MethodParameterIsEmptyStringException 
	 */
	public void addDataProvider(IDataProvider provider)
			throws MethodParameterIsNullException, IsAlreadyManagerOfException, MethodParameterIsEmptyStringException, IsNotManagerOfException;

	/**
	 * Sets the uid of a given managed IDataProvider to a given value
	 * 
	 * @param provider
	 * @param uid
	 * @throws MethodParameterIsNullException
	 * @throws IsAlreadyManagerOfException
	 * @throws IsNotManagerOfException
	 * @throws MethodParameterIsEmptyStringException
	 */
	public void setDataProviderUid(IDataProvider provider, String uid)
			throws MethodParameterIsNullException, IsAlreadyManagerOfException,
			IsNotManagerOfException, MethodParameterIsEmptyStringException;

	/**
	 * Gets a list of the DataProviders that are managed by the
	 * IDataProviderManager
	 * 
	 * @return a non-null but potentially empty list
	 */
	public List<IDataProvider> getListOfDataProviders();

	/**
	 * Removes any DataProviders "not used", that is all IDataProvider that are
	 * not used by a IMediaData of the IPresentation
	 * 
	 * @param delete
	 */
	public void removeUnusedDataProviders(boolean delete);
}
