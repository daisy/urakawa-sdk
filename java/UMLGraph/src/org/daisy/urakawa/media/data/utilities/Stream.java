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
	 * @return int
	 * @throws IOException
	 */
	public int read(byte[] buffer, int offset, int count) throws IOException;

	/**
	 * @throws IOException
	 */
	public void close() throws IOException;
}
