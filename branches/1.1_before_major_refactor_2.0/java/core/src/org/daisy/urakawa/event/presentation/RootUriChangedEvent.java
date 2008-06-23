package org.daisy.urakawa.event.presentation;

import java.net.URI;

import org.daisy.urakawa.Presentation;

/**
 * 
 *
 */
public class RootUriChangedEvent extends PresentationEvent {
	/**
	 * @param source
	 * @param newUriVal
	 * @param prevUriVal
	 */
	public RootUriChangedEvent(Presentation source, URI newUriVal,
			URI prevUriVal) {
		super(source);
		mNewUri = newUriVal;
		mPreviousUri = prevUriVal;
	}

	private URI mNewUri;
	private URI mPreviousUri;

	/**
	 * @return uri
	 */
	public URI getPreviousUri() {
		return mPreviousUri;
	}

	/**
	 * @return uri
	 */
	public URI getNewUri() {
		return mNewUri;
	}
}
