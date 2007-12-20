package org.daisy.urakawa.media.data.utilities;

import java.io.IOException;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * 
 *
 */
public class CloseNotifyingStream implements Stream {
	Stream mBaseStream;

	/**
	 * @param baseStm
	 * @throws MethodParameterIsNullException 
	 */
	public CloseNotifyingStream(Stream baseStm)
			throws MethodParameterIsNullException {
		if (baseStm == null) {
			throw new MethodParameterIsNullException();
		}
		mBaseStream = baseStm;
	}

	public void close() throws IOException {
		mBaseStream.close();
		// TODO: notify close event
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
}
