package org.daisy.urakawa.media;

/**
 * The type of a Media object. This is returned by the
 * {@link Media#getMediaType()} method.
 * 
 * @see Media#getMediaType()
 */
public enum MediaType {
	/**
	 * Audio media (e.g. WAV, MP3, OGG-VORBIS, etc.)
	 */
	AUDIO,
	/**
	 * Audio media (e.g. MPEG, OGG-THEORA, etc.)
	 */
	VIDEO,
	/**
	 * Text media (e.g. plain-text, HTML, etc.)
	 */
	TEXT,
	/**
	 * Image media (e.g. JPEG, PNG, GIF, etc.)
	 */
	IMAGE,
	/**
	 * Special media type to handle a sequence that is empty (it does not have a
	 * proper type yet)
	 */
	EMPTY_SEQUENCE;
}
