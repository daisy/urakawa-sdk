package org.daisy.urakawa.media.data.audio;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.timing.TimeDelta;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class PCMFormatInfoImpl implements PCMFormatInfo {
	public short getBitDepth() {
		return 0;
	}

	public short getBlockAlign() {
		return 0;
	}

	public int getByteRate() {
		return 0;
	}

	public int getDataLength(TimeDelta duration) {
		return 0;
	}

	public TimeDelta getDuration(int dataLen) {
		return null;
	}

	public short getNumberOfChannels() {
		return 0;
	}

	public int getSampleRate() {
		return 0;
	}

	public boolean isCompatibleWith(PCMFormatInfo pcmInfo) {
		return false;
	}

	public void setBitDepth(short newValue) {
	}

	public void setNumberOfChannels(short newValue) {
	}

	public void setSampleRate(int newValue) {
	}

	public void XukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}

	public String getXukLocalName() {
		return null;
	}

	public String getXukNamespaceURI() {
		return null;
	}

	public boolean ValueEquals(PCMFormatInfo other)
			throws MethodParameterIsNullException {
		return false;
	}
}
