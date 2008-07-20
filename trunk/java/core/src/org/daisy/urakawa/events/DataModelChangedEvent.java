package org.daisy.urakawa.events;

/**
 * 
 *
 */
public class DataModelChangedEvent extends Event {
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
