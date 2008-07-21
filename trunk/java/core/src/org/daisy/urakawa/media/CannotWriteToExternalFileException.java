package org.daisy.urakawa.media;

/**
 * <p>
 * This exception is thrown when an attempt to write data into an external file
 * fails (e.g. HTTP GET protocol-only, or FTP without write-access).
 * </p>
 */
public class CannotWriteToExternalFileException extends RuntimeException
{
    /**
	 * 
	 */
    private static final long serialVersionUID = 5159545051882308037L;
}
