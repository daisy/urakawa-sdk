package org.daisy.urakawa.event.undo;

import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.undo.Command;

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
