package org.daisy.urakawa.mediaObject;

/**
 * The pre-defined types of MediaOject.
 * Each correspond to (Audio/Video/Text/Image) Media classes.
 * This is a convenience enumeration in order to not rely on "instanceof" operators when dealing with class and interface types.
 */
public enum MediaType {
        AUDIO, VIDEO, TEXT, IMAGE;
}
