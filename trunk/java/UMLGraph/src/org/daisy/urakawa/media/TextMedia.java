package org.daisy.urakawa.media;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * The simple text media type, which stores text data internally (no external
 * location)
 * 
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public interface TextMedia extends Media {
	/**
	 * @return the text. Cannot be NULL
	 */
	public String getText();

	/**
	 * @param text
	 *            Cannot be NULL
	 * @throws MethodParameterIsNullException
	 *             if text is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public void setText(String text) throws MethodParameterIsNullException;
}
