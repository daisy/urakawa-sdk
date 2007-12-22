package org.daisy.urakawa.media.data;

import java.io.File;
import java.io.IOException;
import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.IsNotManagerOfException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.utilities.CloseNotifyingStream;
import org.daisy.urakawa.media.data.utilities.FileStream;
import org.daisy.urakawa.media.data.utilities.Stream;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class FileDataProviderImpl implements FileDataProvider {
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
		setDataProviderManager(mngr);
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
		return new File(mManager.getDataFileDirectoryFullPath(),
				mDataFileRelativePath).getAbsolutePath();
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
		} catch (IsNotInitializedException e) {
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
}
