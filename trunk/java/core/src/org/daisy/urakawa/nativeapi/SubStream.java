package org.daisy.urakawa.nativeapi;

import java.io.IOException;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;

/**
 * Represents parts of a IStream, from a start byte offset to an end one. This
 * class should be replaced by an equivalent IStream API in the implementing
 * language. The methods exposed here mimic the System.IO.Stream C# API.
 * 
 * @stereotype Language-Dependent
 */
public class SubStream implements IStream {
	IStream mSource;
	int mStartPosition;
	int mLength;

	/**
	 * @param source
	 * @param start
	 * @param len
	 * @throws MethodParameterIsNullException
	 * @throws MethodParameterIsOutOfBoundsException
	 * @throws MethodParameterIsEmptyStringException
	 */
	public SubStream(IStream source, int start, int len)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException,
			MethodParameterIsEmptyStringException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		if (start < 0) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		if (len < 0) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		if (start + len > source.getLength()) {
			throw new MethodParameterIsEmptyStringException();
		}
		mSource = source;
		mStartPosition = start;
		mLength = len;
		setPosition(0);
	}

	/**
	 * @return int
	 */
	public int getLength() {
		return mLength;
	}

	/**
	 * @return int
	 */
	public int getPosition() {
		return mSource.getPosition() - mStartPosition;
	}

	/**
	 * @param value
	 */
	public void setPosition(int value) {
		int newPos = value;
		if (newPos < 0)
			newPos = 0;
		if (newPos > getLength())
			newPos = getLength();
		mSource.setPosition(mStartPosition + newPos);
	}

	public int read(byte buffer[], int offset, int count) throws IOException {
		if (buffer == null) {
			throw new IOException("The read buffer is null");
		}
		if (offset < 0) {
			throw new IOException("The offset is negative");
		}
		if (count < 0) {
			throw new IOException("The count is negative");
		}
		if (offset + count > buffer.length) {
			throw new IOException("The buffer is too small");
		}
		if (count == 0) {
			return 0;
		} else if (count > getLength() - getPosition()) {
			int count_ = (int) (getLength() - getPosition());
			return mSource.read(buffer, offset, count_);
		}
		return mSource.read(buffer, offset, count);
	}

	public void close() throws IOException {
		mSource.close();
	}

	public void seek(int n) {
		mSource.seek(n);
	}

	public byte readByte() {
		return 0;
	}

	@SuppressWarnings("unused")
	public void write(byte[] buffer, int offset, int count) throws IOException {
	}

	public byte[] readBytes(@SuppressWarnings("unused")
	int length) {
		return null;
	}
}
