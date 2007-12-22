package org.daisy.urakawa.media.data.utilities;

import java.io.IOException;

/**
 * 
 *
 */
public interface Stream {
	/**
	 * @return int
	 */
	public int getLength();

	/**
	 * @return int
	 */
	public int getPosition();

	/**
	 * @param pos
	 */
	public void setPosition(int pos);

	/**
	 * @param buffer
	 * @param offset
	 * @param count
	 * @return read
	 * @throws IOException
	 */
	public int read(byte[] buffer, int offset, int count) throws IOException;

	/**
	 * @param buffer
	 * @param offset
	 * @param count
	 * @throws IOException
	 */
	public void write(byte[] buffer, int offset, int count) throws IOException;

	/**
	 * @throws IOException
	 */
	public void close() throws IOException;

	/**
	 * seek from CURRENT
	 * 
	 * @param n
	 */
	public void seek(int n);

	/**
	 * @return byte
	 */
	public byte readByte();

	/**
	 * @param length
	 * @return byte array
	 */
	public byte[] readBytes(int length);
}
