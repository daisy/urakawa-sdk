package org.daisy.urakawa.event;

/**
 * 
 *
 */
public class NameChangedEvent extends DataModelChangedEvent {
	/**
	 * @param source
	 * @param newNameValue
	 * @param prevNameValue
	 */
	public NameChangedEvent(Object source, String newNameValue,
			String prevNameValue) {
		super(source);
		mNewName = newNameValue;
		mPreviousName = prevNameValue;
	}

	private String mNewName;
	private String mPreviousName;

	/**
	 * @return str
	 */
	public String getPreviousName() {
		return mPreviousName;
	}

	/**
	 * @return str
	 */
	public String getNewName() {
		return mNewName;
	}
}
