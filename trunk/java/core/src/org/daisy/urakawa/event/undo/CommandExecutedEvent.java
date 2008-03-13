package org.daisy.urakawa.event.undo;

import org.daisy.urakawa.undo.Command;

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
