package org.daisy.urakawa.media;

/**
 * XML fragment identifier support
 * 
 * @zdepend - Composition - String
 */
public interface XmlFragmented {
	/**
	 * Gets the fragment identifier
	 * 
	 * @return can be a null value
	 */
	public String getFragmentIdentifier();

	/**
	 * Sets the fragment identifier
	 * 
	 * @param id
	 *            can be null
	 */
	public void setFragmentIdentifier(String id);
}
