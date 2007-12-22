package org.daisy.urakawa.media.data;

import java.net.URI;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.daisy.urakawa.WithPresentationImpl;
import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.exception.IsAlreadyManagerOfException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.IsNotManagerOfException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.data.audio.AudioMediaData;
import org.daisy.urakawa.media.data.audio.PCMFormatInfo;
import org.daisy.urakawa.media.data.audio.PCMFormatInfoImpl;
import org.daisy.urakawa.media.data.utilities.CollectManagedMediaTreeNodeVisitor;
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
public class MediaDataManagerImpl extends WithPresentationImpl implements
		MediaDataManager {
	private static final String DEFAULT_UID_PREFIX = "UID";
	private Map<String, MediaData> mMediaDataDictionary = new HashMap<String, MediaData>();
	private Map<MediaData, String> mReverseLookupMediaDataDictionary = new HashMap<MediaData, String>();
	private long mUidNo = 0;
	private String mUidPrefix = DEFAULT_UID_PREFIX;
	private PCMFormatInfo mDefaultPCMFormat;
	private boolean mEnforceSinglePCMFormat;

	/**
	 * 
	 */
	public MediaDataManagerImpl() {
		mDefaultPCMFormat = new PCMFormatInfoImpl();
		mEnforceSinglePCMFormat = false;
	}

	@SuppressWarnings("unchecked")
	private boolean isNewDefaultPCMFormatOk(PCMFormatInfo newDefault)
			throws MethodParameterIsNullException {
		if (newDefault == null) {
			throw new MethodParameterIsNullException();
		}
		for (MediaData md : (List<MediaData>) getListOfMediaData()) {
			if (md instanceof AudioMediaData) {
				AudioMediaData amd = (AudioMediaData) md;
				try {
					if (!amd.getPCMFormat().ValueEquals(newDefault))
						return false;
				} catch (MethodParameterIsNullException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				}
			}
		}
		return true;
	}

	public MediaDataFactory getMediaDataFactory() {
		try {
			return getPresentation().getMediaDataFactory();
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public DataProviderFactory getDataProviderFactory() {
		try {
			return getPresentation().getDataProviderManager()
					.getDataProviderFactory();
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public PCMFormatInfo getDefaultPCMFormat() {
		return mDefaultPCMFormat.copy();
	}

	public void setDefaultPCMFormat(PCMFormatInfo newDefault)
			throws MethodParameterIsNullException, InvalidDataFormatException {
		if (newDefault == null) {
			throw new MethodParameterIsNullException();
		}
		if (!newDefault.ValueEquals(mDefaultPCMFormat)) {
			if (getEnforceSinglePCMFormat()) {
				if (!isNewDefaultPCMFormatOk(newDefault)) {
					throw new InvalidDataFormatException();
				}
			}
			mDefaultPCMFormat = newDefault.copy();
		}
	}

	public void setDefaultNumberOfChannels(short numberOfChannels) throws MethodParameterIsOutOfBoundsException {
		PCMFormatInfo newFormat = getDefaultPCMFormat();
		newFormat.setNumberOfChannels(numberOfChannels);
		try {
			setDefaultPCMFormat(newFormat);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (InvalidDataFormatException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public void setDefaultSampleRate(int sampleRate) throws MethodParameterIsOutOfBoundsException {
		PCMFormatInfo newFormat = getDefaultPCMFormat();
		newFormat.setSampleRate(sampleRate);
		try {
			setDefaultPCMFormat(newFormat);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (InvalidDataFormatException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public void setDefaultBitDepth(short bitDepth) throws MethodParameterIsOutOfBoundsException {
		PCMFormatInfo newFormat = getDefaultPCMFormat();
		newFormat.setBitDepth(bitDepth);
		try {
			setDefaultPCMFormat(newFormat);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (InvalidDataFormatException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public void setDefaultPCMFormat(short numberOfChannels, int sampleRate,
			short bitDepth) throws MethodParameterIsOutOfBoundsException {
		PCMFormatInfo newDefault = new PCMFormatInfoImpl();
		newDefault.setNumberOfChannels(numberOfChannels);
		newDefault.setSampleRate(sampleRate);
		newDefault.setBitDepth(bitDepth);
		try {
			setDefaultPCMFormat(newDefault);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (InvalidDataFormatException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public boolean getEnforceSinglePCMFormat() {
		return mEnforceSinglePCMFormat;
	}

	public void setEnforceSinglePCMFormat(boolean newValue)
			throws InvalidDataFormatException {
		if (newValue) {
			try {
				if (!isNewDefaultPCMFormatOk(getDefaultPCMFormat())) {
					throw new InvalidDataFormatException();
				}
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		mEnforceSinglePCMFormat = newValue;
	}

	public MediaData getMediaData(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (uid == null) {
			throw new MethodParameterIsNullException();
		}
		if (uid == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		if (mMediaDataDictionary.containsKey(uid)) {
			return mMediaDataDictionary.get(uid);
		} else {
			return null;
		}
	}

	public String getUidOfMediaData(MediaData data)
			throws MethodParameterIsNullException, IsNotManagerOfException {
		if (data == null) {
			throw new MethodParameterIsNullException();
		}
		if (!mReverseLookupMediaDataDictionary.containsKey(data)) {
			throw new IsNotManagerOfException();
		}
		return mReverseLookupMediaDataDictionary.get(data);
	}

	private String getNewUid() {
		while (true) {
			if (mUidNo < Integer.MAX_VALUE) {
				mUidNo++;
			} else {
				mUidPrefix += "X";
			}
			String key = String.format("{0}{1:00000000}", mUidPrefix, mUidNo);
			if (!mMediaDataDictionary.containsKey(key)) {
				return key;
			}
		}
	}

	public void addMediaData(MediaData data)
			throws MethodParameterIsNullException {
		if (data == null) {
			throw new MethodParameterIsNullException();
		}
		String uid = getNewUid();
		try {
			addMediaData(data, uid);
		} catch (IsAlreadyManagerOfException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (InvalidDataFormatException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	protected void addMediaData(MediaData data, String uid)
			throws IsAlreadyManagerOfException, InvalidDataFormatException,
			MethodParameterIsEmptyStringException,
			MethodParameterIsNullException {
		if (data == null || uid == null) {
			throw new MethodParameterIsNullException();
		}
		if (uid == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		if (mMediaDataDictionary.containsKey(uid)) {
			throw new IsAlreadyManagerOfException();
		}
		if (getEnforceSinglePCMFormat()) {
			if (data instanceof AudioMediaData) {
				AudioMediaData amdata = (AudioMediaData) data;
				try {
					if (!amdata.getPCMFormat().ValueEquals(
							getDefaultPCMFormat())) {
						throw new InvalidDataFormatException();
					}
				} catch (MethodParameterIsNullException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				}
			}
		}
		mMediaDataDictionary.put(uid, data);
		mReverseLookupMediaDataDictionary.put(data, uid);
	}

	public void setDataMediaDataUid(MediaData data, String uid)
			throws MethodParameterIsEmptyStringException,
			MethodParameterIsNullException {
		if (data == null || uid == null) {
			throw new MethodParameterIsNullException();
		}
		if (uid == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		removeMediaData(data);
		try {
			addMediaData(data, uid);
		} catch (IsAlreadyManagerOfException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (InvalidDataFormatException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
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
		return mMediaDataDictionary.containsKey(uid);
	}

	public void removeMediaData(MediaData data)
			throws MethodParameterIsNullException {
		if (data == null) {
			throw new MethodParameterIsNullException();
		}
		try {
			removeMediaData(getUidOfMediaData(data));
		} catch (IsNotManagerOfException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public void removeMediaData(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (uid == null) {
			throw new MethodParameterIsNullException();
		}
		if (uid == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		MediaData data = getMediaData(uid);
		mMediaDataDictionary.remove(uid);
		mReverseLookupMediaDataDictionary.remove(data);
	}

	public MediaData copyMediaData(MediaData data)
			throws MethodParameterIsNullException, IsNotManagerOfException {
		if (data == null) {
			throw new MethodParameterIsNullException();
		}
		try {
			if (data.getMediaDataManager() != this) {
				throw new IsNotManagerOfException();
			}
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return data.copy();
	}

	public MediaData copyMediaData(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException, IsNotManagerOfException {
		if (uid == null) {
			throw new MethodParameterIsNullException();
		}
		if (uid == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		MediaData data = getMediaData(uid);
		if (data == null) {
			throw new IsNotManagerOfException();
		}
		return copyMediaData(data);
	}

	public List<MediaData> getListOfMediaData() {
		return new LinkedList<MediaData>(mMediaDataDictionary.values());
	}

	public List<String> getListOfUids() {
		return new LinkedList<String>(mMediaDataDictionary.keySet());
	}

	public void deleteUnusedMediaData() {
		CollectManagedMediaTreeNodeVisitor visitor = new CollectManagedMediaTreeNodeVisitor();
		TreeNode root;
		try {
			root = getPresentation().getRootNode();
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (root != null) {
			try {
				root.acceptDepthFirst(visitor);
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		List<MediaData> usedMediaData = new LinkedList<MediaData>();
		for (MediaData mm : (List<MediaData>) getListOfMediaData()) {
			if (!usedMediaData.contains(mm))
				usedMediaData.add(mm);
		}
		for (MediaData md : getListOfMediaData()) {
			if (!usedMediaData.contains(md))
				md.delete();
		}
	}

	@Override
	protected void clear() {
		mMediaDataDictionary.clear();
		mReverseLookupMediaDataDictionary.clear();
		super.clear();
	}

	@Override
	protected void xukInAttributes(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		String attr = source.getAttribute("enforceSinglePCMFormat");
		if (attr == "true" || attr == "1") {
			try {
				setEnforceSinglePCMFormat(true);
			} catch (InvalidDataFormatException e) {
				throw new XukDeserializationFailedException();
			}
		} else {
			try {
				setEnforceSinglePCMFormat(false);
			} catch (InvalidDataFormatException e) {
				throw new XukDeserializationFailedException();
			}
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
			String str = source.getLocalName();
			if (str == "mDefaultPCMFormat") {
				xukInDefaultPCMFormat(source);
			} else if (str == "mMediaData") {
				xukInMediaData(source);
			} else {
				readItem = false;
			}
		}
		if (!(readItem || source.isEmptyElement())) {
			source.readSubtree().close();
		}
	}

	private void xukInDefaultPCMFormat(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == XmlDataReader.ELEMENT) {
					if (source.getLocalName() == "PCMFormatInfo"
							&& source.getNamespaceURI() == XukAbleImpl.XUK_NS) {
						PCMFormatInfo newInfo = new PCMFormatInfoImpl();
						newInfo.xukIn(source);
						boolean enf = getEnforceSinglePCMFormat();
						if (enf)
							try {
								setEnforceSinglePCMFormat(false);
							} catch (InvalidDataFormatException e) {
								// Should never happen
								throw new RuntimeException("WTF ??!", e);
							}
						try {
							setDefaultPCMFormat(newInfo);
						} catch (InvalidDataFormatException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
						if (enf)
							try {
								setEnforceSinglePCMFormat(true);
							} catch (InvalidDataFormatException e) {
								// Should never happen
								throw new RuntimeException("WTF ??!", e);
							}
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

	private void xukInMediaData(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == XmlDataReader.ELEMENT) {
					if (source.getLocalName() == "mMediaDataItem"
							&& source.getNamespaceURI() == XukAbleImpl.XUK_NS) {
						xukInMediaDataItem(source);
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

	private void xukInMediaDataItem(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		String uid = source.getAttribute("uid");
		MediaData data = null;
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == XmlDataReader.ELEMENT) {
					try {
						data = getMediaDataFactory()
								.createMediaData(source.getLocalName(),
										source.getNamespaceURI());
					} catch (MethodParameterIsEmptyStringException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					}
					if (data != null) {
						data.xukIn(source);
					}
				} else if (source.getNodeType() == XmlDataReader.END_ELEMENT) {
					break;
				}
				if (source.isEOF())
					throw new XukDeserializationFailedException();
			}
		}
		if (data != null) {
			if (uid == null && uid == "") {
				throw new XukDeserializationFailedException();
			}
			try {
				setDataMediaDataUid(data, uid);
			} catch (MethodParameterIsEmptyStringException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
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
		destination.writeAttributeString("enforceSinglePCMFormat",
				getEnforceSinglePCMFormat() ? "true" : "false");
		super.xukOutAttributes(destination, baseUri);
	}

	@Override
	protected void xukOutChildren(XmlDataWriter destination, URI baseUri)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
		destination.writeStartElement("mDefaultPCMFormat", XukAbleImpl.XUK_NS);
		try {
			getDefaultPCMFormat().xukOut(destination, baseUri);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (XukSerializationFailedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		destination.writeEndElement();
		destination.writeStartElement("mMediaData", XukAbleImpl.XUK_NS);
		for (String uid : mMediaDataDictionary.keySet()) {
			destination.writeStartElement("mMediaDataItem", XukAbleImpl.XUK_NS);
			destination.writeAttributeString("uid", uid);
			mMediaDataDictionary.get(uid).xukOut(destination, baseUri);
			destination.writeEndElement();
		}
		destination.writeEndElement();
		super.xukOutChildren(destination, baseUri);
	}

	@SuppressWarnings("unchecked")
	public boolean ValueEquals(MediaDataManager other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		List<MediaData> otherMediaData = (List<MediaData>) other
				.getListOfMediaData();
		if (mMediaDataDictionary.size() != otherMediaData.size())
			return false;
		for (MediaData oMD : otherMediaData) {
			try {
				if (!oMD
						.ValueEquals(getMediaData(other.getUidOfMediaData(oMD))))
					return false;
			} catch (MethodParameterIsEmptyStringException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (IsNotManagerOfException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		return true;
	}
}
