package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.exceptions.CheckedException;

/**
 * This exception should be thrown when trying to remove a Channel
 * which name does not exist in the list of current channel.
 */
public class ChannelDoesNotExistException extends CheckedException {

	/**
	 * 
	 */
	private static final long serialVersionUID = 5910996831315559009L;
}