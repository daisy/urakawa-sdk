package org.daisy.urakawa.event;

/**
 * 
 *
 */
public class DataModelChangedEvent {
	/**
	 * @param src
	 */
	public DataModelChangedEvent(Object src) {
		mSourceObject = src;
	}

	/**
	 * 
	 */
	private Object mSourceObject;

	/**
	 * @return object
	 */
	public Object getSourceObject() {
		return mSourceObject;
	}
}
