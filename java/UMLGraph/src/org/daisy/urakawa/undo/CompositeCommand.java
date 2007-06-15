package org.daisy.urakawa.undo;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;

/**
 * <p>
 * A special command made of a series of "smaller" commands.
 * </p>
 * 
 * @depend - Composition 1..n org.daisy.urakawa.undo.Command
 */
public interface CompositeCommand extends Command {
	/**
	 * <p>
	 * Inserts the given Command as a child of this node, at the given index.
	 * Does NOT replace the existing child at the given index, but increments
	 * (+1) the indexes of all children which index is >= insertIndex. If
	 * insertIndex == children.size (no following siblings), then the given node
	 * is appended at the end of the existing children list.
	 * </p>
	 * 
	 * @param command
	 *            cannot be null.
	 * @param index
	 *            must be in bounds [0..children.size].
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsOutOfBounds"
	 * @throws MethodParameterIsOutOfBoundsException
	 *             if the given index is not in bounds [0..children.size].
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public void insert(Command command, int index)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException;
}
