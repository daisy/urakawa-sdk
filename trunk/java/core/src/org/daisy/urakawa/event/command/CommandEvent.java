package org.daisy.urakawa.event.command;

import org.daisy.urakawa.command.ICommand;
import org.daisy.urakawa.event.DataModelChangedEvent;

/**
 *
 *
 */
public class CommandEvent extends DataModelChangedEvent {
	/**
	 * @param source
	 */
	public CommandEvent(ICommand source) {
		super(source);
		mSourceCommand = source;
	}

	private ICommand mSourceCommand;

	/**
	 * @return data
	 */
	public ICommand getSourceCommand() {
		return mSourceCommand;
	}
}
