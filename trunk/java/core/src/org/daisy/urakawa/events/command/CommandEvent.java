package org.daisy.urakawa.events.command;

import org.daisy.urakawa.command.ICommand;
import org.daisy.urakawa.events.DataModelChangedEvent;

/**
 *
 *
 */
public class CommandEvent extends DataModelChangedEvent
{
    /**
     * @param source
     */
    public CommandEvent(ICommand source)
    {
        super(source);
        mSourceCommand = source;
    }

    private ICommand mSourceCommand;

    /**
     * @return data
     */
    public ICommand getSourceCommand()
    {
        return mSourceCommand;
    }
}
