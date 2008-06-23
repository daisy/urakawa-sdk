package org.daisy.urakawa.event.command;

import org.daisy.urakawa.command.Command;

/**
 *
 *
 */
public class CommandExecutedEvent extends CommandEvent {
	/**
	 * @param source
	 */
	public CommandExecutedEvent(Command source) {
		super(source);
	}
}
