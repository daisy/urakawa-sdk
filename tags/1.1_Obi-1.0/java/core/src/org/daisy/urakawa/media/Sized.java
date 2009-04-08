package org.daisy.urakawa.media;

import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;

/**
 * A media type implementing this interface has a 2D surface
 */
public interface Sized {
	/**
	 * Gets the width in pixels
	 * 
	 * @return a value in [0..n] (non-negative). Implementors may want to use
	 *         "uint"/"unsigned int" ;)
	 */
	public int getWidth();

	/**
	 * Gets the height in pixels
	 * 
	 * @return a value in [0..n] (non-negative). Implementors may want to use
	 *         "uint"/"unsigned int" ;)
	 */
	public int getHeight();

	/**
	 * Sets the width in pixels
	 * 
	 * @param w
	 *            a value in [0..n] (non-negative). Implementors may want to use
	 *            "uint"/"unsigned int" ;)
	 * @throws MethodParameterIsOutOfBoundsException
	 *             if w is not an authorized value
	 * @tagvalue Exceptions "MethodParameterIsOutOfBounds"
	 */
	public void setWidth(int w) throws MethodParameterIsOutOfBoundsException;

	/**
	 * Sets the height in pixels
	 * 
	 * @param h
	 *            a value in [0..n] (non-negative). Implementors may want to use
	 *            "uint"/"unsigned int" ;)
	 * @throws MethodParameterIsOutOfBoundsException
	 *             if h is not an authorized value
	 * @tagvalue Exceptions "MethodParameterIsOutOfBounds"
	 */
	public void setHeight(int h) throws MethodParameterIsOutOfBoundsException;

	/**
	 * Convenience method, see {@link #setWidth(int)} and
	 * {@link #setHeight(int)}
	 * 
	 * @param newHeight
	 * @param newWidth
	 * @throws MethodParameterIsOutOfBoundsException
	 * @tagvalue Events "SizeChanged"
	 */
	public void setSize(int newHeight, int newWidth)
			throws MethodParameterIsOutOfBoundsException;
}
