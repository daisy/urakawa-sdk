package org.daisy.urakawa.event;

/**
 * 
 *
 */
public class LanguageChangedEvent extends DataModelChangedEvent {
	/**
	 * @param src
	 * @param newLang
	 * @param prevLanguage
	 */
	public LanguageChangedEvent(Object src, String newLang, String prevLanguage) {
		super(src);
		mNewlanguage = newLang;
		mPreviousLanguage = prevLanguage;
	}

	private String mNewlanguage;
	private String mPreviousLanguage;

	/**
	 * @return str
	 */
	public String getPreviousLanguage() {
		return mPreviousLanguage;
	}

	/**
	 * @return str
	 */
	public String getNewlanguage() {
		return mNewlanguage;
	}
}
