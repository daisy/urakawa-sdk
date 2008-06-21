package org.daisy.urakawa.media.data.audio;

import java.net.URI;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.timing.TimeDelta;
import org.daisy.urakawa.media.timing.TimeDeltaImpl;
import org.daisy.urakawa.nativeapi.XmlDataReader;
import org.daisy.urakawa.nativeapi.XmlDataWriter;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.ProgressHandler;
import org.daisy.urakawa.xuk.XukAbleAbstractImpl;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class PCMFormatInfoImpl extends XukAbleAbstractImpl implements
		PCMFormatInfo {
	/**
	 * 
	 */
	public PCMFormatInfoImpl() {
		try {
			init((short) 1, 44100, (short) 16);
		} catch (MethodParameterIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	/**
	 * @param other
	 * @throws MethodParameterIsNullException
	 */
	public PCMFormatInfoImpl(PCMFormatInfo other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		try {
			init(other.getNumberOfChannels(), other.getSampleRate(), other
					.getBitDepth());
		} catch (MethodParameterIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	/**
	 * @param noc
	 * @param sr
	 * @param bd
	 * @throws MethodParameterIsOutOfBoundsException
	 */
	public PCMFormatInfoImpl(short noc, int sr, short bd)
			throws MethodParameterIsOutOfBoundsException {
		init(noc, sr, bd);
	}

	private void init(short noc, int sr, short bd)
			throws MethodParameterIsOutOfBoundsException {
		setNumberOfChannels(noc);
		setSampleRate(sr);
		setBitDepth(bd);
	}

	public PCMFormatInfo copy() {
		try {
			return new PCMFormatInfoImpl(this);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	private short mNumberOfChannels = 1;

	public short getNumberOfChannels() {
		return mNumberOfChannels;
	}

	public void setNumberOfChannels(short newValue)
			throws MethodParameterIsOutOfBoundsException {
		if (newValue < 1) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		mNumberOfChannels = newValue;
	}

	private int mSampleRate = 44100;

	public int getSampleRate() {
		return mSampleRate;
	}

	public void setSampleRate(int newValue)
			throws MethodParameterIsOutOfBoundsException {
		if (mSampleRate < 1) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		mSampleRate = newValue;
	}

	private short mBitDepth = 16;

	public short getBitDepth() {
		return mBitDepth;
	}

	public void setBitDepth(short newValue)
			throws MethodParameterIsOutOfBoundsException {
		if ((newValue % 8) != 0) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		if (newValue < 8) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		mBitDepth = newValue;
	}

	public int getByteRate() {
		return getNumberOfChannels() * getSampleRate() * getBitDepth() / 8;
	}

	public short getBlockAlign() {
		return (short) (getNumberOfChannels() * (getBitDepth() / 8));
	}

	public boolean isCompatibleWith(PCMFormatInfo pcmInfo)
			throws MethodParameterIsNullException {
		if (pcmInfo == null) {
			throw new MethodParameterIsNullException();
		}
		if (getNumberOfChannels() != pcmInfo.getNumberOfChannels())
			return false;
		if (getSampleRate() != pcmInfo.getSampleRate())
			return false;
		if (getBitDepth() != pcmInfo.getBitDepth())
			return false;
		return true;
	}

	public TimeDelta getDuration(int dataLen) {
		if (getByteRate() == 0) {
			// Should never happen
			throw new RuntimeException("WTF ??!");
		}
		double blockCount = dataLen / getBlockAlign();
		return new TimeDeltaImpl((long) (Math.round(getMillisecondsPerBlock()
				* blockCount)));
	}

	private double getMillisecondsPerBlock() {
		return 1000 / getSampleRate();
	}

	public int getDataLength(TimeDelta duration)
			throws MethodParameterIsNullException {
		if (duration == null) {
			throw new MethodParameterIsNullException();
		}
		int blockCount = (int) Math.round(((double) duration
				.getTimeDeltaAsMilliseconds())
				/ getMillisecondsPerBlock());
		int res = blockCount * getBlockAlign();
		return res;
	}

	@Override
	protected void xukInAttributes(XmlDataReader source, ProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}

		// To avoid event notification overhead, we bypass this:
		if (false && ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		String attr = source.getAttribute("numberOfChannels");
		if (attr == null) {
			throw new XukDeserializationFailedException();
		}
		short noc = 0;
		try {
			noc = (short) Integer.parseInt(attr);
		} catch (NumberFormatException e) {
			throw new XukDeserializationFailedException();
		}
		try {
			setNumberOfChannels(noc);
		} catch (MethodParameterIsOutOfBoundsException e1) {
			throw new XukDeserializationFailedException();
		}
		attr = source.getAttribute("sampleRate");
		if (attr == null) {
			throw new XukDeserializationFailedException();
		}
		int sr = 0;
		try {
			sr = Integer.parseInt(attr);
		} catch (NumberFormatException e) {
			throw new XukDeserializationFailedException();
		}
		try {
			setSampleRate(sr);
		} catch (MethodParameterIsOutOfBoundsException e1) {
			throw new XukDeserializationFailedException();
		}
		attr = source.getAttribute("bitDepth");
		if (attr == null) {
			throw new XukDeserializationFailedException();
		}
		short bd = 0;
		try {
			bd = (short) Integer.parseInt(attr);
		} catch (NumberFormatException e) {
			throw new XukDeserializationFailedException();
		}
		try {
			setBitDepth(bd);
		} catch (MethodParameterIsOutOfBoundsException e) {
			throw new XukDeserializationFailedException();
		}
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukOutAttributes(XmlDataWriter destination, URI baseUri,
			ProgressHandler ph) throws MethodParameterIsNullException,
			XukSerializationFailedException, ProgressCancelledException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		destination.writeAttributeString("numberOfChannels", Integer
				.toString(getNumberOfChannels()));
		destination.writeAttributeString("sampleRate", Integer
				.toString(getSampleRate()));
		destination.writeAttributeString("bitDepth", Integer
				.toString(getBitDepth()));
	}

	public boolean ValueEquals(PCMFormatInfo other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		if (other == null)
			return false;
		if (other.getClass() != getClass())
			return false;
		if (!isCompatibleWith(other))
			return false;
		return true;
	}

	@Override
	protected void clear() {
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukInChild(XmlDataReader source, ProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {

		// To avoid event notification overhead, we bypass this:
		if (false && ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		boolean readItem = false;
		// Read known children, when read set readItem to true
		if (!(readItem || source.isEmptyElement())) {
			source.readSubtree().close();// Read past unknown child
		}
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukOutChildren(XmlDataWriter destination, URI baseUri,
			ProgressHandler ph) throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
	}
}
