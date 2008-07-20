package org.daisy.urakawa.events.command;

import org.daisy.urakawa.command.ICommand;
import org.daisy.urakawa.command.ICompositeCommand;

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
	public CommandAddedEvent(ICompositeCommand source, ICommand added, int index) {
		super(source);
		mSourceCompositeCommand = source;
		mAddedCommand = added;
		mIndex = index;
	}

	private ICompositeCommand mSourceCompositeCommand;

	/**
	 * @return data
	 */
	public ICompositeCommand getCompositeCommand() {
		return mSourceCompositeCommand;
	}

	private ICommand mAddedCommand;

	/**
	 * @return data
	 */
	public ICommand getAddedCommand() {
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
