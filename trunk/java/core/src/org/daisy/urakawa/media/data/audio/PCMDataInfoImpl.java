package org.daisy.urakawa.media.data.audio;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.URI;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.data.InvalidDataFormatException;
import org.daisy.urakawa.media.timing.ITimeDelta;
import org.daisy.urakawa.nativeapi.IStream;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class PCMDataInfoImpl extends PCMFormatInfoImpl implements IPCMDataInfo {
	/**
	 * 
	 */
	public PCMDataInfoImpl() {
		setDataLength(0);
	}

	/**
	 * @param other
	 * @throws MethodParameterIsNullException
	 */
	public PCMDataInfoImpl(IPCMDataInfo other)
			throws MethodParameterIsNullException {
		super(other);
		setDataLength(other.getDataLength());
	}

	/**
	 * @param other
	 * @throws MethodParameterIsNullException
	 */
	public PCMDataInfoImpl(IPCMFormatInfo other)
			throws MethodParameterIsNullException {
		super(other);
		setDataLength(0);
	}

	private int mDataLength = 0;

	public int getDataLength() {
		return mDataLength;
	}

	public void setDataLength(int newValue) {
		mDataLength = newValue;
	}

	public ITimeDelta getDuration() {
		return getDuration(getDataLength());
	}

	public void writeRiffWaveHeader(IStream output) throws IOException,
			MethodParameterIsNullException {
		if (output == null) {
			throw new MethodParameterIsNullException();
		}
		OutputStream os = new OutputStream() {
			@Override
			public void write(@SuppressWarnings("unused") int b)
					throws IOException {
				// TODO replace with real IStream object (see method parameter)
				throw new IOException();
			}
		};
		BufferedOutputStream wr = new BufferedOutputStream(os);
		wr.write(getASCIIBytes("RIFF"));// Chunk UID
		int chunkSize = 4 + 8 + 16 + 8 + getDataLength();
		wr.write(chunkSize);// Chunk Size
		wr.write(getASCIIBytes("WAVE"));// Format field
		wr.write(getASCIIBytes("fmt "));// Format sub-chunk
		int formatChunkSize = 16;
		wr.write(formatChunkSize);
		short audioFormat = 1;// PCM format
		wr.write(audioFormat);
		wr.write(getNumberOfChannels());
		wr.write(getSampleRate());
		wr.write(getByteRate());
		wr.write(getBlockAlign());
		wr.write(getBitDepth());
		wr.write(getASCIIBytes("data"));
		wr.write(getDataLength());
	}

	private String getASCIIString(@SuppressWarnings("unused") byte[] b) {
		// TODO return real byte array for ASCII string
		return null;
	}

	private byte[] getASCIIBytes(@SuppressWarnings("unused") String string) {
		// TODO return real byte array for ASCII string
		return null;
	}

	public IPCMDataInfo parseRiffWaveHeader(IStream input)
			throws InvalidDataFormatException, MethodParameterIsNullException,
			IOException {
		if (input == null) {
			throw new MethodParameterIsNullException();
		}
		InputStream is = new InputStream() {
			@Override
			public int read() throws IOException {
				// TODO replace with real IStream object (see method parameter)
				throw new IOException();
			}
		};
		BufferedInputStream rd = new BufferedInputStream(is);
		if (input.getLength() - input.getPosition() < 12) {
			throw new InvalidDataFormatException();
		}
		byte[] b = new byte[4];
		@SuppressWarnings("unused")
		int read = rd.read(b);
		String chunkId = getASCIIString(b);
		if (chunkId != "RIFF") {
			throw new InvalidDataFormatException();
		}
		int chunkSize = readUInt32(rd);
		long chunkEndPos = input.getPosition() + chunkSize;
		if (chunkEndPos > input.getLength()) {
			throw new InvalidDataFormatException();
		}
		read = rd.read(b);
		String format = getASCIIString(b);
		if (format != "WAVE") {
			throw new InvalidDataFormatException();
		}
		boolean foundFormatSubChunk = false;
		IPCMDataInfo pcmInfo = new PCMDataInfoImpl();
		// Search for format subchunk
		while (input.getPosition() + 8 <= chunkEndPos) {
			read = rd.read(b);
			String formatSubChunkId = getASCIIString(b);
			int formatSubChunkSize = readUInt32(rd);
			if (input.getPosition() + formatSubChunkSize > chunkEndPos) {
				throw new InvalidDataFormatException();
			}
			if (formatSubChunkId == "fmt ") {
				foundFormatSubChunk = true;
				if (formatSubChunkSize < 2) {
					throw new InvalidDataFormatException();
				}
				short audioFormat = readUInt16(rd);
				if (audioFormat != 1) {
					throw new InvalidDataFormatException();
				}
				if (formatSubChunkSize != 16) {
					throw new InvalidDataFormatException();
				}
				short numChannels = readUInt16(rd);
				if (numChannels == 0) {
					throw new InvalidDataFormatException();
				}
				int sampleRate = readUInt32(rd);
				int byteRate = readUInt32(rd);
				short blockAlign = readUInt16(rd);
				short bitDepth = readUInt16(rd);
				if ((bitDepth % 8) != 0) {
					throw new InvalidDataFormatException();
				}
				if (blockAlign != (numChannels * bitDepth / 8)) {
					throw new InvalidDataFormatException();
				}
				if (byteRate != sampleRate * blockAlign) {
					throw new InvalidDataFormatException();
				}
				try {
					pcmInfo.setBitDepth(bitDepth);
					pcmInfo.setNumberOfChannels(numChannels);
					pcmInfo.setSampleRate(sampleRate);
				} catch (MethodParameterIsOutOfBoundsException e) {
					throw new InvalidDataFormatException();
				}
				break;
			} else {
				input.seek(formatSubChunkSize);
			}
		}
		if (!foundFormatSubChunk) {
			throw new InvalidDataFormatException();
		}
		boolean foundDataSubChunk = false;
		while (input.getPosition() + 8 <= chunkEndPos) {
			read = rd.read(b);
			String dataSubChunkId = getASCIIString(b);
			int dataSubChunkSize = readUInt32(rd);
			if (input.getPosition() + dataSubChunkSize > chunkEndPos) {
				throw new InvalidDataFormatException();
			}
			if (dataSubChunkId == "data") {
				foundDataSubChunk = true;
				pcmInfo.setDataLength(dataSubChunkSize);
				break;
			} else {
				input.seek(dataSubChunkSize);
			}
		}
		if (!foundDataSubChunk) {
			throw new InvalidDataFormatException();
		}
		return pcmInfo;
	}

	private short readUInt16(@SuppressWarnings("unused") BufferedInputStream rd) {
		// TODO implement !!
		return 0;
	}

	private int readUInt32(@SuppressWarnings("unused") BufferedInputStream rd) {
		// TODO TODO implement !!
		return 0;
	}

	public boolean compareStreamData(IStream s1, IStream s2, int length)
			throws IOException {
		byte[] d1 = new byte[length];
		byte[] d2 = new byte[length];
		if (s1.read(d1, 0, length) != length)
			return false;
		if (s2.read(d2, 0, length) != length)
			return false;
		for (int i = 0; i < length; i++) {
			if (d1[i] != d2[i])
				return false;
		}
		return true;
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
		super.xukInAttributes(source, ph);
		String attr = source.getAttribute("dataLength");
		if (attr == null) {
			throw new XukDeserializationFailedException();
		}
		int dl = 0;
		try {
			dl = Integer.parseInt(attr);
		} catch (NumberFormatException e) {
			throw new XukDeserializationFailedException();
		}
		setDataLength(dl);
	}

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
		destination.writeAttributeString("dataLength", Integer
				.toString(getDataLength()));
		super.xukOutAttributes(destination, baseUri, ph);
	}

	@Override
	public boolean ValueEquals(IPCMFormatInfo other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		if (!super.ValueEquals(other))
			return false;
		if (getDataLength() != ((IPCMDataInfo) other).getDataLength())
			return false;
		return true;
	}
}
