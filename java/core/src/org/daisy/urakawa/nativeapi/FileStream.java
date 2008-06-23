package org.daisy.urakawa.nativeapi;

import java.io.IOException;

/**
 * This is a wrapper for a Stream based on an File. This class should be
 * replaced by an equivalent Stream API in the implementing language. The
 * methods exposed here mimic the System.IO.Stream C# API.
 * 
 * @stereotype Language-Dependent
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

	public void seek(@SuppressWarnings("unused")
	int n) {
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
