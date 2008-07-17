package org.daisy.urakawa.media.data;

import java.net.URI;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.IsAlreadyManagerOfException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.IsNotManagerOfException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.data.audio.IAudioMediaData;
import org.daisy.urakawa.media.data.audio.IPCMFormatInfo;
import org.daisy.urakawa.media.data.audio.PCMFormatInfo;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.xuk.IXukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public final class MediaDataManager extends WithPresentation implements
		IMediaDataManager {
	private static final String DEFAULT_UID_PREFIX = "UID";
	private Map<String, IMediaData> mMediaDataDictionary = new HashMap<String, IMediaData>();
	private Map<IMediaData, String> mReverseLookupMediaDataDictionary = new HashMap<IMediaData, String>();
	private long mUidNo = 0;
	private String mUidPrefix = DEFAULT_UID_PREFIX;
	private IPCMFormatInfo mDefaultPCMFormat;
	private boolean mEnforceSinglePCMFormat;

	/**
	 * 
	 */
	public MediaDataManager() {
		mDefaultPCMFormat = new PCMFormatInfo();
		mEnforceSinglePCMFormat = false;
	}

	private boolean isNewDefaultPCMFormatOk(IPCMFormatInfo newDefault)
			throws MethodParameterIsNullException {
		if (newDefault == null) {
			throw new MethodParameterIsNullException();
		}
		for (IMediaData md : getListOfMediaData()) {
			if (md instanceof IAudioMediaData) {
				IAudioMediaData amd = (IAudioMediaData) md;
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

	public IMediaDataFactory getMediaDataFactory() {
		try {
			return getPresentation().getMediaDataFactory();
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public IDataProviderFactory getDataProviderFactory() {
		try {
			return getPresentation().getDataProviderManager()
					.getDataProviderFactory();
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public IPCMFormatInfo getDefaultPCMFormat() {
		return mDefaultPCMFormat.copy();
	}

	public void setDefaultPCMFormat(IPCMFormatInfo newDefault)
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

	public void setDefaultNumberOfChannels(short numberOfChannels)
			throws MethodParameterIsOutOfBoundsException {
		IPCMFormatInfo newFormat = getDefaultPCMFormat();
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

	public void setDefaultSampleRate(int sampleRate)
			throws MethodParameterIsOutOfBoundsException {
		IPCMFormatInfo newFormat = getDefaultPCMFormat();
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

	public void setDefaultBitDepth(short bitDepth)
			throws MethodParameterIsOutOfBoundsException {
		IPCMFormatInfo newFormat = getDefaultPCMFormat();
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
		IPCMFormatInfo newDefault = new PCMFormatInfo();
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

	public IMediaData getMediaData(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (uid == null) {
			throw new MethodParameterIsNullException();
		}
		if (uid.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		if (mMediaDataDictionary.containsKey(uid)) {
			return mMediaDataDictionary.get(uid);
		}
		return null;

	}

	public String getUidOfMediaData(IMediaData data)
			throws MethodParameterIsNullException, IsNotManagerOfException {
		if (data == null) {
			throw new MethodParameterIsNullException();
		}
		if (!mReverseLookupMediaDataDictionary.containsKey(data)) {
			throw new IsNotManagerOfException();
		}
		return mReverseLookupMediaDataDictionary.get(data);
	}

	@SuppressWarnings("boxing")
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

	public void addMediaData(IMediaData data)
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

	protected void addMediaData(IMediaData data, String uid)
			throws IsAlreadyManagerOfException, InvalidDataFormatException,
			MethodParameterIsEmptyStringException,
			MethodParameterIsNullException {
		if (data == null || uid == null) {
			throw new MethodParameterIsNullException();
		}
		if (uid.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		if (mMediaDataDictionary.containsKey(uid)) {
			throw new IsAlreadyManagerOfException();
		}
		if (getEnforceSinglePCMFormat()) {
			if (data instanceof IAudioMediaData) {
				IAudioMediaData amdata = (IAudioMediaData) data;
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

	public void setDataMediaDataUid(IMediaData data, String uid)
			throws MethodParameterIsEmptyStringException,
			MethodParameterIsNullException {
		if (data == null || uid == null) {
			throw new MethodParameterIsNullException();
		}
		if (uid.length() == 0) {
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
		if (uid.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		return mMediaDataDictionary.containsKey(uid);
	}

	public void removeMediaData(IMediaData data)
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
		if (uid.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		IMediaData data = getMediaData(uid);
		mMediaDataDictionary.remove(uid);
		mReverseLookupMediaDataDictionary.remove(data);
	}

	public IMediaData copyMediaData(IMediaData data)
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

	public IMediaData copyMediaData(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException, IsNotManagerOfException {
		if (uid == null) {
			throw new MethodParameterIsNullException();
		}
		if (uid.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		IMediaData data = getMediaData(uid);
		if (data == null) {
			throw new IsNotManagerOfException();
		}
		return copyMediaData(data);
	}

	public List<IMediaData> getListOfMediaData() {
		return new LinkedList<IMediaData>(mMediaDataDictionary.values());
	}

	public List<String> getListOfUids() {
		return new LinkedList<String>(mMediaDataDictionary.keySet());
	}

	@Override
	protected void clear() {
		mMediaDataDictionary.clear();
		mReverseLookupMediaDataDictionary.clear();
		// super.clear();
	}

	@Override
	protected void xukInAttributes(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}

		// To avoid event notification overhead, we bypass this:
		if (false && ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
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
	protected void xukInChild(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}

		// To avoid event notification overhead, we bypass this:
		if (false && ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		boolean readItem = false;
		if (source.getNamespaceURI() == IXukAble.XUK_NS) {
			readItem = true;
			String str = source.getLocalName();
			if (str == "mDefaultPCMFormat") {
				xukInDefaultPCMFormat(source, ph);
			} else if (str == "mMediaData") {
				xukInMediaData(source, ph);
			} else {
				readItem = false;
			}
		}
		if (!readItem) {
			super.xukInChild(source, ph);
		}
	}

	private void xukInDefaultPCMFormat(IXmlDataReader source,
			IProgressHandler ph) throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == IXmlDataReader.ELEMENT) {
					if (source.getLocalName() == "IPCMFormatInfo"
							&& source.getNamespaceURI() == IXukAble.XUK_NS) {
						IPCMFormatInfo newInfo = new PCMFormatInfo();
						newInfo.xukIn(source, ph);
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
					} else {
						super.xukInChild(source, ph);
					}
				} else if (source.getNodeType() == IXmlDataReader.END_ELEMENT) {
					break;
				}
				if (source.isEOF())
					throw new XukDeserializationFailedException();
			}
		}
	}

	private void xukInMediaData(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == IXmlDataReader.ELEMENT) {
					if (source.getLocalName() == "mMediaDataItem"
							&& source.getNamespaceURI() == IXukAble.XUK_NS) {
						xukInMediaDataItem(source, ph);
					} else {
						super.xukInChild(source, ph);
					}
				} else if (source.getNodeType() == IXmlDataReader.END_ELEMENT) {
					break;
				}
				if (source.isEOF())
					throw new XukDeserializationFailedException();
			}
		}
	}

	private void xukInMediaDataItem(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		String uid = source.getAttribute("uid");
		IMediaData data = null;
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == IXmlDataReader.ELEMENT) {
					try {
						data = getMediaDataFactory()
								.createMediaData(source.getLocalName(),
										source.getNamespaceURI());
					} catch (MethodParameterIsEmptyStringException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					}
					if (data != null) {
						data.xukIn(source, ph);
					}
				} else if (source.getNodeType() == IXmlDataReader.END_ELEMENT) {
					break;
				}
				if (source.isEOF())
					throw new XukDeserializationFailedException();
			}
		}
		if (data != null) {
			if (uid == null || uid.length() == 0) {
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

	@SuppressWarnings("unused")
	@Override
	protected void xukOutAttributes(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws MethodParameterIsNullException,
			XukSerializationFailedException, ProgressCancelledException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		destination.writeAttributeString("enforceSinglePCMFormat",
				getEnforceSinglePCMFormat() ? "true" : "false");
		// super.xukOutAttributes(destination, baseUri, ph);
	}

	@Override
	protected void xukOutChildren(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws MethodParameterIsNullException,
			XukSerializationFailedException, ProgressCancelledException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		destination.writeStartElement("mDefaultPCMFormat", IXukAble.XUK_NS);
		try {
			getDefaultPCMFormat().xukOut(destination, baseUri, ph);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (XukSerializationFailedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		destination.writeEndElement();
		destination.writeStartElement("mMediaData", IXukAble.XUK_NS);
		for (String uid : mMediaDataDictionary.keySet()) {
			destination.writeStartElement("mMediaDataItem", IXukAble.XUK_NS);
			destination.writeAttributeString("uid", uid);
			mMediaDataDictionary.get(uid).xukOut(destination, baseUri, ph);
			destination.writeEndElement();
		}
		destination.writeEndElement();
		// super.xukOutChildren(destination, baseUri, ph);
	}

	public boolean ValueEquals(IMediaDataManager other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		List<IMediaData> otherMediaData = other.getListOfMediaData();
		if (mMediaDataDictionary.size() != otherMediaData.size())
			return false;
		for (IMediaData oMD : otherMediaData) {
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
