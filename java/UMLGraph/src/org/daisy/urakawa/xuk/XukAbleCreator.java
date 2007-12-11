package org.daisy.urakawa.xuk;

/**
 *
 */
public interface XukAbleCreator {
	/**
	 * @param localName
	 * @param namespace
	 * @return
	 */
	public XukAble createXukAble(String localName, String namespace);
}
