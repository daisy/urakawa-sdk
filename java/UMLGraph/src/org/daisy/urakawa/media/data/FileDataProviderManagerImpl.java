package org.daisy.urakawa.media.data;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.net.URI;
import java.net.URISyntaxException;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.daisy.urakawa.WithPresentationImpl;
import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsAlreadyManagerOfException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.IsNotManagerOfException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.data.utilities.Stream;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;
import org.daisy.urakawa.xuk.XukAbleImpl;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class FileDataProviderManagerImpl extends WithPresentationImpl implements
		FileDataProviderManager {
	private Map<String, DataProvider> mDataProvidersDictionary = new HashMap<String, DataProvider>();
	private Map<DataProvider, String> mReverseLookupDataProvidersDictionary = new HashMap<DataProvider, String>();
	private List<String> mXukedInFilDataProviderPaths = new LinkedList<String>();
	private String mDataFileDirectory;

	/**
	 * 
	 */
	public FileDataProviderManagerImpl() {
		mDataFileDirectory = null;
	}

	public void appendDataToProvider(Stream data, int count,
			DataProvider provider) throws MethodParameterIsNullException,
			OutputStreamIsOpenException, InputStreamIsOpenException,
			DataIsMissingException, IOException, InputStreamIsTooShortException {
		if (data == null || provider == null) {
			throw new MethodParameterIsNullException();
		}
		Stream provOutputStream = provider.getOutputStream();
		try {
			provOutputStream.seek(data.getLength());
			int bytesAppended = 0;
			byte[] buf = new byte[1024];
			while (bytesAppended < count) {
				if (bytesAppended + buf.length >= count) {
					buf = new byte[count - bytesAppended];
				}
				if (data.read(buf, 0, buf.length) != buf.length) {
					throw new InputStreamIsTooShortException();
				}
				provOutputStream.write(buf, 0, buf.length);
				bytesAppended += buf.length;
			}
		} finally {
			provOutputStream.close();
		}
	}

	public boolean compareDataProviderContent(DataProvider dp1, DataProvider dp2)
			throws MethodParameterIsNullException, DataIsMissingException,
			OutputStreamIsOpenException, IOException {
		if (dp1 == null || dp2 == null) {
			throw new MethodParameterIsNullException();
		}
		Stream s1 = null;
		Stream s2 = null;
		boolean allEq = true;
		try {
			s1 = dp1.getInputStream();
			s2 = dp2.getInputStream();
			allEq = ((s1.getLength() - s1.getPosition()) == (s2.getLength() - s2
					.getPosition()));
			while (allEq && (s1.getPosition() < s1.getLength())) {
				if (s1.readByte() != s2.readByte()) {
					allEq = false;
					break;
				}
			}
		} finally {
			if (s1 != null)
				s1.close();
			if (s2 != null)
				s2.close();
		}
		return allEq;
	}

	public String getDataFileDirectory() {
		if (mDataFileDirectory == null)
			mDataFileDirectory = "Data";
		return mDataFileDirectory;
	}

	public void setDataFileDirectory(String dataDir)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException,
			IsAlreadyInitializedException, URISyntaxException {
		if (dataDir == null) {
			throw new MethodParameterIsNullException();
		}
		if (dataDir == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		if (mDataFileDirectory != null) {
			throw new IsAlreadyInitializedException();
		}
		@SuppressWarnings("unused")
		URI tmp = new URI(mDataFileDirectory);
		mDataFileDirectory = dataDir;
	}

	public void moveDataFiles(String newDataFileDir, boolean deleteSource,
			boolean overwriteDestDir) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException,
			MethodParameterIsOutOfBoundsException, IOException,
			DataIsMissingException {
		if (newDataFileDir == null) {
			throw new MethodParameterIsNullException();
		}
		if (newDataFileDir == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		File file = new File(newDataFileDir);
		if (file.isAbsolute()) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		String oldPath = null;
		String newPath = null;
		try {
			oldPath = getDataFileDirectoryFullPath();
			newPath = getDataFileDirectoryFullPath();
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (URISyntaxException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		mDataFileDirectory = newDataFileDir;
		File fileNew = new File(newPath);
		if (fileNew.exists()) {
			if (overwriteDestDir)
				fileNew.delete();
		}
		copyDataFiles(oldPath, newPath);
		File fileOld = new File(oldPath);
		if (deleteSource && fileOld.exists()) {
			fileOld.delete();
		}
	}

	public FileDataProviderFactory getDataProviderFactory()
			throws IsNotInitializedException {
		FileDataProviderFactory fact = (FileDataProviderFactory) getPresentation()
				.getDataProviderFactory();
		return fact;
	}

	private void createDirectory(String path) throws IOException,
			MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (path == null) {
			throw new MethodParameterIsNullException();
		}
		if (path == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		File file = new File(path);
		if (!file.exists()) {
			File parent = file.getParentFile();
			if (!parent.exists())
				createDirectory(parent.getAbsolutePath());
			file.createNewFile();
		}
	}

	private void copyDataFiles(String source, String dest)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException, IOException,
			DataIsMissingException {
		if (source == null || dest == null) {
			throw new MethodParameterIsNullException();
		}
		if (source == "" || dest == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		createDirectory(dest);
		for (FileDataProvider fdp : getListOfManagedFileDataProviders()) {
			File file = new File(source, fdp.getDataFileRelativePath());
			if (!file.exists()) {
				throw new DataIsMissingException();
			}
			File file2 = new File(dest, fdp.getDataFileRelativePath());
			file2.createNewFile();
			FileInputStream fis = new FileInputStream(file);
			FileOutputStream fos = new FileOutputStream(file2);
			byte[] b = new byte[4096];
			@SuppressWarnings("unused")
			int read = 0;
			while ((read = fis.read(b)) > 0) {
				fos.write(b);
			}
			fis.close();
			fos.close();
		}
	}

	private String getDataFileDirectoryFullPath(URI baseUri)
			throws URISyntaxException {
		if (baseUri.getScheme() != "file") {
			throw new URISyntaxException(
					baseUri.toString(),
					"The base Uri of the presentation to which the FileDataProviderManager belongs must be a file Uri");
		}
		URI dataFileDirUri = new URI(getDataFileDirectory());
		dataFileDirUri.relativize(baseUri);
		return dataFileDirUri.getPath();
	}

	public String getDataFileDirectoryFullPath()
			throws IsNotInitializedException, URISyntaxException {
		return getDataFileDirectoryFullPath(getPresentation().getRootURI());
	}

	public void setDataFileDirectoryPath(String path)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException,
			IsAlreadyInitializedException, IOException {
		if (path == null) {
			throw new MethodParameterIsNullException();
		}
		if (path == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		if (mDataFileDirectory != null) {
			throw new IsAlreadyInitializedException();
		}
		File file = new File(path);
		if (!file.exists()) {
			file.createNewFile();
		}
		mDataFileDirectory = path;
	}

	public String getNewDataFileRelPath(String extension)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (extension == null) {
			throw new MethodParameterIsNullException();
		}
		if (extension == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		String res;
		while (true) {
			res = generateRandomFileName(extension);
			for (FileDataProvider prov : getListOfManagedFileDataProviders()) {
				if (res.toLowerCase() == prov.getDataFileRelativePath()
						.toLowerCase()) {
					continue;
				}
			}
			break;
		}
		return res;
	}

	private String generateRandomFileName(String extension) {
		// TODO: generate random string
		return "test." + extension;
	}

	public List<FileDataProvider> getListOfManagedFileDataProviders() {
		List<FileDataProvider> res = new LinkedList<FileDataProvider>();
		for (DataProvider prov : getListOfDataProviders()) {
			if (prov instanceof FileDataProvider) {
				res.add((FileDataProvider) prov);
			}
		}
		return res;
	}

	public void removeDataProvider(DataProvider provider, boolean delete)
			throws MethodParameterIsNullException, IsNotManagerOfException {
		if (provider == null) {
			throw new MethodParameterIsNullException();
		}
		if (delete) {
			try {
				provider.delete();
			} catch (OutputStreamIsOpenException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (InputStreamIsOpenException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		} else {
			String uid = getUidOfDataProvider(provider);
			try {
				removeDataProvider(uid, provider);
			} catch (MethodParameterIsEmptyStringException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
	}

	public void removeDataProvider(String uid, boolean delete)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException, IsNotManagerOfException {
		if (uid == null) {
			throw new MethodParameterIsNullException();
		}
		if (uid == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		DataProvider provider = getDataProvider(uid);
		if (delete) {
			try {
				provider.delete();
			} catch (OutputStreamIsOpenException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (InputStreamIsOpenException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		} else {
			removeDataProvider(uid, provider);
		}
	}

	private void removeDataProvider(String uid, DataProvider provider)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (uid == null || provider == null) {
			throw new MethodParameterIsNullException();
		}
		if (uid == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		mDataProvidersDictionary.remove(uid);
		mReverseLookupDataProvidersDictionary.remove(provider);
	}

	public String getUidOfDataProvider(DataProvider provider)
			throws MethodParameterIsNullException, IsNotManagerOfException {
		if (provider == null) {
			throw new MethodParameterIsNullException();
		}
		if (!mReverseLookupDataProvidersDictionary.containsKey(provider)) {
			throw new IsNotManagerOfException();
		}
		return mReverseLookupDataProvidersDictionary.get(provider);
	}

	public DataProvider getDataProvider(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException, IsNotManagerOfException {
		if (uid == null) {
			throw new MethodParameterIsNullException();
		}
		if (uid == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		if (!mDataProvidersDictionary.containsKey(uid)) {
			throw new IsNotManagerOfException();
		}
		return mDataProvidersDictionary.get(uid);
	}

	protected void addDataProvider(DataProvider provider, String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException, IsNotManagerOfException,
			IsAlreadyManagerOfException {
		if (provider == null || uid == null) {
			throw new MethodParameterIsNullException();
		}
		if (uid == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		if (mReverseLookupDataProvidersDictionary.containsKey(provider)) {
			throw new IsAlreadyManagerOfException();
		}
		if (mDataProvidersDictionary.containsKey(uid)) {
			throw new IsAlreadyManagerOfException();
		}
		try {
			if (provider.getDataProviderManager() != this) {
				throw new IsNotManagerOfException();
			}
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		mDataProvidersDictionary.put(uid, provider);
		mReverseLookupDataProvidersDictionary.put(provider, uid);
	}

	public void addDataProvider(DataProvider provider)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException, IsNotManagerOfException,
			IsAlreadyManagerOfException {
		if (provider == null) {
			throw new MethodParameterIsNullException();
		}
		addDataProvider(provider, getNextUid());
	}

	private String getNextUid() {
		long i = 0;
		while (i < Integer.MAX_VALUE) {
			String newId = String.format("DPID{0:0000}", i);
			if (!mDataProvidersDictionary.containsKey(newId))
				return newId;
			i++;
		}
		// Should never happen
		throw new RuntimeException("WTF ??!");
	}

	public boolean isManagerOf(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (uid == null) {
			throw new MethodParameterIsNullException();
		}
		if (uid == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		return mDataProvidersDictionary.containsKey(uid);
	}

	public void setDataProviderUid(DataProvider provider, String uid)
			throws MethodParameterIsNullException, IsNotManagerOfException,
			MethodParameterIsEmptyStringException, IsAlreadyManagerOfException {
		if (provider == null || uid == null) {
			throw new MethodParameterIsNullException();
		}
		if (uid == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		removeDataProvider(provider, false);
		addDataProvider(provider, uid);
	}

	public List<DataProvider> getListOfDataProviders() {
		return new LinkedList<DataProvider>(mDataProvidersDictionary.values());
	}

	public void removeUnusedDataProviders(boolean delete) {
		List<DataProvider> usedDataProviders = new LinkedList<DataProvider>();
		try {
			for (MediaData md : getPresentation().getMediaDataManager()
					.getListOfMediaData()) {
				for (DataProvider prov : md.getListOfUsedDataProviders()) {
					if (!usedDataProviders.contains(prov))
						usedDataProviders.add(prov);
				}
			}
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		for (DataProvider prov : getListOfDataProviders()) {
			if (!usedDataProviders.contains(prov)) {
				try {
					removeDataProvider(prov, delete);
				} catch (MethodParameterIsNullException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				} catch (IsNotManagerOfException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				}
			}
		}
	}

	@Override
	protected void clear() {
		mDataProvidersDictionary.clear();
		mDataFileDirectory = null;
		mReverseLookupDataProvidersDictionary.clear();
		mXukedInFilDataProviderPaths.clear();
		super.clear();
	}

	@Override
	protected void xukInAttributes(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		String dataFileDirectoryPath = source
				.getAttribute("dataFileDirectoryPath");
		if (dataFileDirectoryPath == null || dataFileDirectoryPath == "") {
			throw new XukDeserializationFailedException();
		}
		try {
			setDataFileDirectoryPath(dataFileDirectoryPath);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsAlreadyInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IOException e) {
			throw new XukDeserializationFailedException();
		}
	}

	@Override
	protected void xukInChild(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		boolean readItem = false;
		if (source.getNamespaceURI() == XukAbleImpl.XUK_NS) {
			readItem = true;
			if (source.getLocalName() == "mDataProviders") {
				xukInDataProviders(source);
			} else {
				readItem = false;
			}
		}
		if (!(readItem || source.isEmptyElement())) {
			source.readSubtree().close();// Read past invalid MediaDataItem
			// element
		}
	}

	private void xukInDataProviders(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == XmlDataReader.ELEMENT) {
					if (source.getLocalName() == "mDataProviderItem"
							&& source.getNamespaceURI() == XukAbleImpl.XUK_NS) {
						xukInDataProviderItem(source);
					} else if (!source.isEmptyElement()) {
						source.readSubtree().close();
					}
				} else if (source.getNodeType() == XmlDataReader.END_ELEMENT) {
					break;
				}
				if (source.isEOF())
					throw new XukDeserializationFailedException();
			}
		}
	}

	private void xukInDataProviderItem(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		String uid = source.getAttribute("uid");
		if (!source.isEmptyElement()) {
			boolean addedProvider = false;
			while (source.read()) {
				if (source.getNodeType() == XmlDataReader.ELEMENT) {
					DataProvider prov;
					try {
						prov = getDataProviderFactory()
								.createDataProvider("", source.getLocalName(),
										source.getNamespaceURI());
					} catch (MethodParameterIsEmptyStringException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					} catch (IsNotInitializedException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					}
					if (prov != null) {
						if (addedProvider) {
							throw new XukDeserializationFailedException();
						}
						prov.xukIn(source);
						if (prov instanceof FileDataProvider) {
							FileDataProvider fdProv = (FileDataProvider) prov;
							if (mXukedInFilDataProviderPaths.contains(fdProv
									.getDataFileRelativePath().toLowerCase())) {
								throw new XukDeserializationFailedException();
							}
							mXukedInFilDataProviderPaths.add(fdProv
									.getDataFileRelativePath().toLowerCase());
						}
						if (uid == null || uid == "") {
							throw new XukDeserializationFailedException();
						}
						try {
							if (isManagerOf(uid)) {
								if (getDataProvider(uid) != prov) {
									throw new XukDeserializationFailedException();
								}
							} else {
								setDataProviderUid(prov, uid);
							}
						} catch (MethodParameterIsEmptyStringException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsNotManagerOfException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsAlreadyManagerOfException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
						addedProvider = true;
					} else if (!source.isEmptyElement()) {
						source.readSubtree().close();
					}
				} else if (source.getNodeType() == XmlDataReader.END_ELEMENT) {
					break;
				}
				if (source.isEOF())
					throw new XukDeserializationFailedException();
			}
		}
	}

	@Override
	protected void xukOutAttributes(XmlDataWriter destination, URI baseUri)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
		URI presBaseUri;
		try {
			presBaseUri = getPresentation().getRootURI();
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		URI dfdUri;
		try {
			dfdUri = new URI(getDataFileDirectory());
		} catch (URISyntaxException e) {
			throw new XukSerializationFailedException();
		}
		dfdUri = dfdUri.relativize(presBaseUri);
		destination.writeAttributeString("dataFileDirectoryPath", presBaseUri
				.relativize(dfdUri).toString());
		super.xukOutAttributes(destination, baseUri);
	}

	@Override
	protected void xukOutChildren(XmlDataWriter destination, URI baseUri)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
		destination.writeStartElement("mDataProviders", XukAbleImpl.XUK_NS);
		for (DataProvider prov : getListOfDataProviders()) {
			destination.writeStartElement("mDataProviderItem",
					XukAbleImpl.XUK_NS);
			destination.writeAttributeString("uid", prov.getUid());
			prov.xukOut(destination, baseUri);
			destination.writeEndElement();
		}
		destination.writeEndElement();
		super.xukOutChildren(destination, baseUri);
	}

	public boolean ValueEquals(DataProviderManager other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		if (other instanceof FileDataProviderManager) {
			FileDataProviderManager o = (FileDataProviderManager) other;
			if (o.getDataFileDirectory() != getDataFileDirectory())
				return false;
			List<DataProvider> oDP = getListOfDataProviders();
			if (o.getListOfDataProviders().size() != oDP.size())
				return false;
			for (DataProvider dp : oDP) {
				String uid = dp.getUid();
				try {
					if (!o.isManagerOf(uid))
						return false;
				} catch (MethodParameterIsEmptyStringException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				}
				try {
					if (!o.getDataProvider(uid).ValueEquals(dp))
						return false;
				} catch (MethodParameterIsEmptyStringException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				} catch (IsNotManagerOfException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				}
			}
		}
		return true;
	}
}
