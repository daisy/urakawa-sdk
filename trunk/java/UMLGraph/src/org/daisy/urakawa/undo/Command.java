package org.daisy.urakawa.undo;

import java.util.List;

import org.daisy.urakawa.media.data.MediaData;
import org.daisy.urakawa.xuk.XukAble;

/**
 * <p>
 * Classes realizing this interface must store the state of the object(s)
 * affected by the command execution (including undo/redo).
 * </p>
 */
public interface Command extends XukAble {
	/**
	 * Returns a list of MediaData objects that are in use by this command.
	 * 
	 * @return a non-null, possibly empty, list of Media objects
	 */
	public List<MediaData> getListOfUsedMediaData();

	/**
	 * <p>
	 * Return a human-readable name for the reverse Command
	 * </p>
	 * 
	 * @return cannot be null, or empty string.
	 */
	public String getShortDescription();

	/**
	 * <p>
	 * Return a human-readable name for the Command
	 * </p>
	 * 
	 * @return cannot be null, but can return an empty string.
	 */
	public String getLongDescription();

	/**
	 * <p>
	 * executes the Command
	 * </p>
	 */
	public void execute();

	/**
	 * <p>
	 * executes the reverse Command
	 * </p>
	 */
	public void unExecute();
}
