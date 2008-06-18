package org.daisy.urakawa.event.undo;

import org.daisy.urakawa.command.Command;

/**
 * 
 *
 */
public class CommandUnExecutedEvent extends CommandEvent {
	/**
	 * @param source
	 */
	public CommandUnExecutedEvent(Command source) {
		super(source);
	}
}
