package org.daisy.urakawa.media.data;

import java.io.File;
import java.io.IOException;
import java.net.URI;
import java.net.URISyntaxException;
import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.WithPresentationImpl;
import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsAlreadyManagerOfException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.IsNotManagerOfException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.CloseNotifyingStream;
import org.daisy.urakawa.nativeapi.FileStream;
import org.daisy.urakawa.nativeapi.Stream;
import org.daisy.urakawa.nativeapi.XmlDataReader;
import org.daisy.urakawa.nativeapi.XmlDataWriter;
import org.daisy.urakawa.progress.ProgressHandler;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class FileDataProviderImpl extends WithPresentationImpl implements
		FileDataProvider {
	/**
	 * Constructs a new file data provider with a given manager and relative
	 * path
	 * 
	 * @param mngr
	 *            The manager of the constructed instance
	 * @param relPath
	 *            The relative path of the data file of the constructed instance
	 * @param mimeType
	 *            The MIME type of the data to store in the constructed instance
	 */
	public FileDataProviderImpl(FileDataProviderManager mngr, String relPath,
			String mimeType) {
		try {
			setDataProviderManager(mngr);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsAlreadyInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		mDataFileRelativePath = relPath;
		mMimeType = mimeType;
	}

	private FileDataProviderManager mManager;
	private String mDataFileRelativePath;
	List<CloseNotifyingStream> mOpenInputStreams = new LinkedList<CloseNotifyingStream>();
	CloseNotifyingStream mOpenOutputStream = null;

	public String getDataFileRelativePath() {
		return mDataFileRelativePath;
	}

	public String getDataFileFullPath() {
		try {
			return new File(mManager.getDataFileDirectoryFullPath(),
					mDataFileRelativePath).getAbsolutePath();
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (URISyntaxException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	private boolean hasBeenInitialized = false;

	public String getUid() {
		try {
			return getDataProviderManager().getUidOfDataProvider(this);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotManagerOfException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	private void checkDataFile() throws DataIsMissingException {
		File file = new File(getDataFileFullPath());
		File dir = file.getParentFile();
		if (!dir.exists())
			try {
				dir.createNewFile();
			} catch (IOException e) {
				throw new DataIsMissingException();
			}
		if (file.exists()) {
			if (!hasBeenInitialized) {
				file.delete();
			} else {
				return;
			}
		}
		if (hasBeenInitialized) {
			throw new DataIsMissingException();
		}
		try {
			file.createNewFile();
		} catch (IOException e) {
			throw new DataIsMissingException();
		}
		hasBeenInitialized = true;
	}

	public Stream getInputStream() throws OutputStreamIsOpenException,
			DataIsMissingException {
		if (mOpenOutputStream != null) {
			throw new OutputStreamIsOpenException();
		}
		FileStream inputFS;
		String fp = getDataFileFullPath();
		checkDataFile();
		inputFS = new FileStream(fp);
		CloseNotifyingStream res;
		try {
			res = new CloseNotifyingStream(inputFS);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		mOpenInputStreams.add(res);
		return res;
	}

	public Stream getOutputStream() throws OutputStreamIsOpenException,
			InputStreamIsOpenException, DataIsMissingException {
		FileStream outputFS;
		if (mOpenOutputStream != null) {
			throw new OutputStreamIsOpenException();
		}
		if (mOpenInputStreams.size() > 0) {
			throw new InputStreamIsOpenException();
		}
		checkDataFile();
		String fp = getDataFileFullPath();
		outputFS = new FileStream(fp);
		try {
			mOpenOutputStream = new CloseNotifyingStream(outputFS);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return mOpenOutputStream;
	}

	public void delete() throws OutputStreamIsOpenException,
			InputStreamIsOpenException {
		if (mOpenOutputStream != null) {
			throw new OutputStreamIsOpenException();
		}
		if (mOpenInputStreams.size() > 0) {
			throw new InputStreamIsOpenException();
		}
		File file = new File(getDataFileFullPath());
		if (file.exists()) {
			file.delete();
		}
		try {
			getDataProviderManager().removeDataProvider(this, false);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotManagerOfException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public FileDataProvider copy() {
		FileDataProvider c;
		try {
			c = (FileDataProvider) getFileDataProviderManager()
					.getDataProviderFactory().createDataProvider(getMimeType(),
							getXukLocalName(), getXukNamespaceURI());
		} catch (MethodParameterIsNullException e2) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e2);
		} catch (MethodParameterIsEmptyStringException e2) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e2);
		} catch (IsNotInitializedException e2) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e2);
		}
		Stream thisData;
		try {
			thisData = getInputStream();
		} catch (OutputStreamIsOpenException e1) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e1);
		} catch (DataIsMissingException e1) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e1);
		}
		try {
			new FileDataProviderManagerImpl().appendDataToProvider(thisData,
					(int) (thisData.getLength() - thisData.getPosition()), c);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (OutputStreamIsOpenException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (InputStreamIsOpenException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (DataIsMissingException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IOException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (InputStreamIsTooShortException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} finally {
			try {
				thisData.close();
			} catch (IOException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		return c;
	}

	public DataProviderManager getDataProviderManager() {
		return mManager;
	}

	public FileDataProviderManager getFileDataProviderManager() {
		return mManager;
	}

	private String mMimeType;

	public String getMimeType() {
		return mMimeType;
	}

	public void setDataProviderManager(DataProviderManager mngr)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException {
		if (mngr == null) {
			throw new MethodParameterIsNullException();
		}
		FileDataProviderManager fMngr = (FileDataProviderManager) mngr;
		setFileDataProviderManager(fMngr);
	}

	public void setFileDataProviderManager(FileDataProviderManager mngr)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException {
		if (mngr == null) {
			throw new MethodParameterIsNullException();
		}
		if (mManager != null) {
			throw new IsAlreadyInitializedException();
		}
		mManager = mngr;
		try {
			mManager.addDataProvider(this);
		} catch (IsAlreadyManagerOfException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotManagerOfException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	@Override
	protected void xukInAttributes(XmlDataReader source,
			@SuppressWarnings("unused") ProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		mDataFileRelativePath = source.getAttribute("dataFileRelativePath");
		if (mDataFileRelativePath == null || mDataFileRelativePath == "") {
			throw new XukDeserializationFailedException();
		}
		hasBeenInitialized = true;// Assume that the data file exists
		mMimeType = source.getAttribute("mimeType");
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukInChild(XmlDataReader source, ProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		boolean readItem = false;
		if (!(readItem || source.isEmptyElement())) {
			source.readSubtree().close();
		}
	}

	@Override
	protected void xukOutAttributes(XmlDataWriter destination, URI baseUri,
			@SuppressWarnings("unused") ProgressHandler ph)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
		try {
			checkDataFile();
		} catch (DataIsMissingException e) {
			throw new XukSerializationFailedException();
		}// Ensure that data file exist even if no data has yet been written
		// to it.
		destination.writeAttributeString("dataFileRelativePath",
				getDataFileRelativePath());
		destination.writeAttributeString("mimeType", getMimeType());
	}

	@Override
	@SuppressWarnings("unused")
	protected void xukOutChildren(XmlDataWriter destination, URI baseUri,
			ProgressHandler ph) {
	}

	public boolean ValueEquals(DataProvider other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		if (getClass() != other.getClass())
			return false;
		FileDataProvider o = (FileDataProvider) other;
		if (o.getMimeType() != getMimeType())
			return false;
		try {
			if (!new FileDataProviderManagerImpl().compareDataProviderContent(
					this, o))
				return false;
		} catch (DataIsMissingException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (OutputStreamIsOpenException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IOException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return true;
	}

	public FileDataProvider export(Presentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		FileDataProvider expFDP;
		try {
			expFDP = (FileDataProvider) destPres.getDataProviderFactory()
					.createDataProvider(getMimeType(), getXukLocalName(),
							getXukNamespaceURI());
		} catch (MethodParameterIsEmptyStringException e1) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e1);
		} catch (IsNotInitializedException e1) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e1);
		}
		if (expFDP == null) {
			throw new FactoryCannotCreateTypeException();
		}
		Stream thisStm;
		try {
			thisStm = getInputStream();
		} catch (OutputStreamIsOpenException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (DataIsMissingException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		try {
			try {
				new FileDataProviderManagerImpl().appendDataToProvider(thisStm,
						(int) thisStm.getLength(), expFDP);
			} catch (OutputStreamIsOpenException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (InputStreamIsOpenException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (DataIsMissingException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (IOException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (InputStreamIsTooShortException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		} finally {
			try {
				thisStm.close();
			} catch (IOException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		return expFDP;
	}
}
