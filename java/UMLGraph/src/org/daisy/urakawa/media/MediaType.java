package org.daisy.urakawa.media;

/**
 * This uniquely identify the type of a media object (e.g. video, audio, image,
 * or text). This is returned by the {@link Media#getMediaType()} method, so
 * that a media object that composes ("extends"/"implements") the urakawa.media
 * interface hierarchy can have a more discrete, well-defined type. This is
 * essentially an interface "lollypop" which should be implemented as a concrete
 * type depending on the programming language and supported media types of the
 * SDK implementation. This could therefore be an enumeration (e.g.{VIDEO,
 * AUDIO, IMAGE, TEXT}), but it could also be a concrete object type. For
 * example, Java has "java.lang.Class" to identify object types, C# has
 * "System.Type".
 * 
 * @see Media#getMediaType()
 * @stereotype language-dependent
 * 
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public class MediaType {
}
