package org.daisy.urakawa.media.data;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.utilities.Stream;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @depend - Clone - org.daisy.urakawa.media.data.DataProvider
 * @depend - Aggregation 1 org.daisy.urakawa.media.data.DataProviderManager
 * @stereotype XukAble
 */
public interface DataProvider extends WithDataProviderManager,
		WithPresentation, XukAble, ValueEquatable<DataProvider> {
	/**
	 * Convenience method to obtain the UID of the data provider via its
	 * manager.
	 * 
	 * @return the UID non-null, non empty
	 */
	public String getUid();

	/**
	 * Gets a Stream providing read access to the data. Make sure to close any
	 * Stream returned by this method when it is no longer needed. If there are
	 * any open input Streams, subsequent calls to methods getOutputStream() and
	 * delete() will cause to be thrown.
	 * 
	 * @return the stream
	 * @throws DataIsMissingException
	 * @throws OutputStreamIsOpenException
	 */
	public Stream getInputStream() throws DataIsMissingException,
			OutputStreamIsOpenException;

	/**
	 * Gets a Stream providing write access to the data Make sure to close any
	 * Stream returned by this method when it is no longer needed. If there are
	 * any open input Streams, subsequent calls to methods getOutputStream() and
	 * getInputStream() and delete() will cause OutputStreamOpenException to be
	 * thrown.
	 * 
	 * @return the stream
	 * @throws OutputStreamIsOpenException
	 */
	public Stream getOutputStream() throws OutputStreamIsOpenException;

	/**
	 * Deletes any resources associated with this permanently. Additionally
	 * removes the DataProvider from it's DataProviderManager
	 * 
	 * @throws OutputStreamIsOpenException
	 * @throws InputStreamIsOpenException
	 */
	public void delete() throws OutputStreamIsOpenException,
			InputStreamIsOpenException;

	/**
	 * @return a copy of this
	 */
	public DataProvider copy();

	/**
	 * @param destPres
	 * @return the exported provider
	 * @throws MethodParameterIsNullException
	 * @throws FactoryCannotCreateTypeException
	 */
	public DataProvider export(Presentation destPres)
			throws MethodParameterIsNullException,
			FactoryCannotCreateTypeException;

	/**
	 * @return the MIME type of the media stored in the data provider
	 */
	public String getMimeType();
}
