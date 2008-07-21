package org.daisy.urakawa.events.command;

import org.daisy.urakawa.command.ICommand;

/**
 *
 *
 */
public class CommandExecutedEvent extends CommandEvent
{
    /**
     * @param source
     */
    public CommandExecutedEvent(ICommand source)
    {
        super(source);
    }
}
