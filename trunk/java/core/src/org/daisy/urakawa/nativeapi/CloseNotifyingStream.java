package org.daisy.urakawa.nativeapi;

import java.io.IOException;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * This is a wrapper for a IStream that notifies when it's closed. This class
 * should be replaced by an equivalent IStream API in the implementing language.
 * The methods exposed here mimic the System.IO.Stream C# API.
 * 
 * @stereotype Language-Dependent
 */
public class CloseNotifyingStream implements IStream {
	IStream mBaseStream;

	/**
	 * @param baseStm
	 * @throws MethodParameterIsNullException
	 */
	public CloseNotifyingStream(IStream baseStm)
			throws MethodParameterIsNullException {
		if (baseStm == null) {
			throw new MethodParameterIsNullException();
		}
		mBaseStream = baseStm;
	}

	public void close() throws IOException {
		mBaseStream.close(); // Here there should be notification
	}

	public int getLength() {
		return mBaseStream.getLength();
	}

	public int getPosition() {
		return mBaseStream.getPosition();
	}

	public int read(byte[] buffer, int offset, int count) throws IOException {
		return mBaseStream.read(buffer, offset, count);
	}

	public void setPosition(int pos) {
		mBaseStream.setPosition(pos);
	}

	public void seek(int n) {
		mBaseStream.seek(n);
	}

	@SuppressWarnings("unused")
	public void write(byte[] buffer, int offset, int count) throws IOException {
	}

	public byte readByte() {
		return 0;
	}

	public byte[] readBytes(@SuppressWarnings("unused")
	int length) {
		return null;
	}
}
