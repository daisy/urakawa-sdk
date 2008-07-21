package org.daisy.urakawa.command;

import java.util.List;

import org.daisy.urakawa.IWithPresentation;
import org.daisy.urakawa.events.IEventHandler;
import org.daisy.urakawa.events.command.CommandEvent;
import org.daisy.urakawa.media.data.IMediaData;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * <p>
 * Classes realizing this interface must store the state of the object(s)
 * affected by the command execution.
 * </p>
 * 
 * @depend - Aggregation 1 org.daisy.urakawa.IPresentation
 * @depend - Event - org.daisy.urakawa.event.undo.CommandExecutedEvent
 * @depend - Event - org.daisy.urakawa.event.undo.CommandUnExecutedEvent
 */
public interface ICommand extends IAction, IXukAble, IWithPresentation,
        IEventHandler<CommandEvent>
{
    /**
     * <p>
     * Returns a list of IMediaData objects that are in use by this command.
     * </p>
     * 
     * @return a non-null, possibly empty, list of IMedia objects
     */
    public List<IMediaData> getListOfUsedMediaData();

    /**
     * <p>
     * executes the reverse ICommand
     * </p>
     * 
     * @tagvalue Events "CommandUnExecuted"
     * 
     * @throws CommandCannotUnExecuteException when the ICommand cannot be
     *         un-executed
     */
    public void unExecute() throws CommandCannotUnExecuteException;

    /**
     * <p>
     * Tests whether this command can be un-executed.
     * </p>
     * 
     * @return true if this command can be un-executed.
     */
    public boolean canUnExecute();
}
