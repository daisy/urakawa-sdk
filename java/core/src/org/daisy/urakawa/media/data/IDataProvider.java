package org.daisy.urakawa.media.data;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.IPresentation;
import org.daisy.urakawa.IValueEquatable;
import org.daisy.urakawa.IWithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.IStream;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * @depend - Clone - org.daisy.urakawa.media.data.IDataProvider
 * @depend - Aggregation 1 org.daisy.urakawa.media.data.IDataProviderManager
 * @stereotype IXukAble
 */
public interface IDataProvider extends IWithDataProviderManager,
		IWithPresentation, IXukAble, IValueEquatable<IDataProvider> {
	/**
	 * Convenience method to obtain the UID of the data provider via its
	 * manager.
	 * 
	 * @return the UID non-null, non empty
	 */
	public String getUid();

	/**
	 * Gets a IStream providing read access to the data. Make sure to close any
	 * IStream returned by this method when it is no longer needed. If there are
	 * any open input Streams, subsequent calls to methods getOutputStream() and
	 * delete() will cause to be thrown.
	 * 
	 * @return the stream
	 * @throws DataIsMissingException
	 * @throws OutputStreamIsOpenException
	 */
	public IStream getInputStream() throws DataIsMissingException,
			OutputStreamIsOpenException;

	/**
	 * Gets a IStream providing write access to the data Make sure to close any
	 * IStream returned by this method when it is no longer needed. If there are
	 * any open input Streams, subsequent calls to methods getOutputStream() and
	 * getInputStream() and delete() will cause OutputStreamOpenException to be
	 * thrown.
	 * 
	 * @return the stream
	 * @throws OutputStreamIsOpenException
	 * @throws InputStreamIsOpenException
	 * @throws DataIsMissingException 
	 */
	public IStream getOutputStream() throws OutputStreamIsOpenException,
			InputStreamIsOpenException, DataIsMissingException;

	/**
	 * Deletes any resources associated with this permanently. Additionally
	 * removes the IDataProvider from it's IDataProviderManager
	 * 
	 * @throws OutputStreamIsOpenException
	 * @throws InputStreamIsOpenException
	 */
	public void delete() throws OutputStreamIsOpenException,
			InputStreamIsOpenException;

	/**
	 * @return a copy of this
	 */
	public IDataProvider copy();

	/**
	 * @param destPres
	 * @return the exported provider
	 * @throws MethodParameterIsNullException
	 * @throws FactoryCannotCreateTypeException
	 */
	public IDataProvider export(IPresentation destPres)
			throws MethodParameterIsNullException,
			FactoryCannotCreateTypeException;

	/**
	 * @return the MIME type of the media stored in the data provider
	 */
	public String getMimeType();
}
