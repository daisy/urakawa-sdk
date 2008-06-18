package org.daisy.urakawa.event.undo;

import org.daisy.urakawa.command.Command;
import org.daisy.urakawa.undo.UndoRedoManager;

/**
 * 
 *
 */
public class CommandReDoneEvent extends UndoRedoManagerEvent {
	/**
	 * @param source
	 * @param reDone
	 */
	public CommandReDoneEvent(UndoRedoManager source, Command reDone) {
		super(source);
		mReDoneCommand = reDone;
	}

	private Command mReDoneCommand;

	/**
	 * @return data
	 */
	public Command getReDoneCommand() {
		return mReDoneCommand;
	}
}
