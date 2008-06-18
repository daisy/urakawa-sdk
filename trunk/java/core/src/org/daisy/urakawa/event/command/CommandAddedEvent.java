package org.daisy.urakawa.event.command;

import org.daisy.urakawa.command.Command;
import org.daisy.urakawa.command.CompositeCommand;

/**
 * 
 *
 */
public class CommandAddedEvent extends CommandEvent {
	/**
	 * @param source
	 * @param added
	 * @param index
	 */
	public CommandAddedEvent(CompositeCommand source, Command added, int index) {
		super(source);
		mSourceCompositeCommand = source;
		mAddedCommand = added;
		mIndex = index;
	}

	private CompositeCommand mSourceCompositeCommand;

	/**
	 * @return data
	 */
	public CompositeCommand getCompositeCommand() {
		return mSourceCompositeCommand;
	}

	private Command mAddedCommand;

	/**
	 * @return data
	 */
	public Command getAddedCommand() {
		return mAddedCommand;
	}

	private int mIndex;

	/**
	 * @return data
	 */
	public int getIndex() {
		return mIndex;
	}
}
