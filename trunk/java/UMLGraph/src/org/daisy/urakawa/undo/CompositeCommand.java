package org.daisy.urakawa.undo;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;

/**
 * A "mega-command" made of a series of "smaller" commands.
 * Usefull for merging small commands into one such as: user typing text letter by letter
 * (the undo/redo would work on full word or sentence, not for each character).
 *
 * @depend - Composition 1..n Command
 */
public interface CompositeCommand extends Command {
    /**
     * Inserts the given Command as a child of this node, at the given index.
     * Does NOT replace the existing child,
     * but increments (+1) the indexes of all children which index is >= insertIndex.
     * If insertIndex == children.size (no following siblings),
     * then the given node is appended at the end of the existing children list.
     *
     * @param command cannot be null.
     * @param index   must be in bounds [0..children.size].
     * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsOutOfBounds"
     * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
     */
    public void insert(Command command, int index) throws MethodParameterIsNullException, MethodParameterIsOutOfBoundsException;
}
