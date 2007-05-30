package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.exceptions.CheckedException;

/**
 * This exception should be thrown when trying to add a Channel
 * which name is already used in the list of current channel.
 */
public class ChannelAlreadyExistsException extends CheckedException {

	/**
	 * 
	 */
	private static final long serialVersionUID = 3085958815065326578L;
}
