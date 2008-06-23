package org.daisy.urakawa.event.command;

import org.daisy.urakawa.command.Command;
import org.daisy.urakawa.event.DataModelChangedEvent;

/**
 *
 *
 */
public class CommandEvent extends DataModelChangedEvent {
	/**
	 * @param source
	 */
	public CommandEvent(Command source) {
		super(source);
		mSourceCommand = source;
	}

	private Command mSourceCommand;

	/**
	 * @return data
	 */
	public Command getSourceCommand() {
		return mSourceCommand;
	}
}
