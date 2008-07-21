package org.daisy.urakawa.command;

import java.util.List;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;

/**
 * <p>
 * A special command made of a sequence of "smaller" commands. It is recommended
 * to enforce a description (see {@link IWithShortLongDescription}), otherwise
 * the default behavior is to concatenate each description in the list of
 * commands (which results in a long string and defies the point of a
 * "human-understandable" description).
 * </p>
 * 
 * @depend - "Composition\n(ordered list)" 1..n org.daisy.urakawa.undo.Command
 * @depend - Event - org.daisy.urakawa.event.undo.CommandAddedEvent
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface ICompositeCommand extends ICommand
{
    /**
     * <p>
     * Inserts the given ICommand as a child of this node, at the given index.
     * Does NOT replace the existing child at the given index, but increments
     * (+1) the indexes of all children which index is >= insertIndex. If
     * insertIndex == children.size (no following siblings), then the given node
     * is appended at the end of the existing list.
     * </p>
     * 
     * @param iCommand cannot be null.
     * @param index must be in bounds [0..children.size].
     * @tagvalue Events "CommandAdded"
     * 
     * @throws MethodParameterIsOutOfBoundsException if the given index is not
     *         in bounds [0..children.size].
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     */
    public void insert(ICommand iCommand, int index)
            throws MethodParameterIsNullException,
            MethodParameterIsOutOfBoundsException;

    /**
     * <p>
     * Inserts the given ICommand at the end of the existing list.
     * </p>
     * 
     * @param iCommand cannot be null.
     * @tagvalue Events "CommandAdded"
     * 
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     */
    public void append(ICommand iCommand) throws MethodParameterIsNullException;

    /**
     * @return a non-null, potentially empty list of existing commands in the
     *         list.
     */
    public List<ICommand> getListOfCommands();

    /**
     * @return the number of existing commands in the list.
     */
    public int getCount();
}
