package org.daisy.urakawa.events.command;

import org.daisy.urakawa.command.ICommand;

/**
 * 
 *
 */
public class CommandUnExecutedEvent extends CommandEvent
{
    /**
     * @param source
     */
    public CommandUnExecutedEvent(ICommand source)
    {
        super(source);
    }
}
