package org.daisy.urakawa.media.data.utilities;

import java.io.IOException;

/**
 *
 */
public class FileStream implements Stream {
	/**
	 * @param path
	 */
	public FileStream(@SuppressWarnings("unused")
	String path) {
	}

	@SuppressWarnings("unused")
	public void close() throws IOException {
	}

	public int getLength() {
		return 0;
	}

	public int getPosition() {
		return 0;
	}

	@SuppressWarnings("unused")
	public int read(byte[] buffer, int offset, int count) throws IOException {
		return 0;
	}

	public void setPosition(@SuppressWarnings("unused")
	int pos) {
	}
}
