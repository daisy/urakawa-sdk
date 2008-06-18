package org.daisy.urakawa.event.undo;

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
