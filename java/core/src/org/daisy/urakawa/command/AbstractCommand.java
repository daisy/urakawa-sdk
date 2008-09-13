package org.daisy.urakawa.command;

import org.daisy.urakawa.AbstractXukAbleWithPresentation;
import org.daisy.urakawa.events.Event;
import org.daisy.urakawa.events.EventHandler;
import org.daisy.urakawa.events.IEventHandler;
import org.daisy.urakawa.events.IEventListener;
import org.daisy.urakawa.events.command.CommandAddedEvent;
import org.daisy.urakawa.events.command.CommandEvent;
import org.daisy.urakawa.events.command.CommandExecutedEvent;
import org.daisy.urakawa.events.command.CommandUnExecutedEvent;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * 
 */
public abstract class AbstractCommand extends AbstractXukAbleWithPresentation
        implements ICommand
{
    protected String mLongDescription = "";
    protected String mShortDescription = "";

    public void setLongDescription(String str)
            throws MethodParameterIsNullException
    {
        if (str == null)
        {
            throw new MethodParameterIsNullException();
        }
        mShortDescription = str;
    }

    public void setShortDescription(String str)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException
    {
        if (str == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (str.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        mLongDescription = str;
    }

    public String getLongDescription()
    {
        return mLongDescription;
    }

    public String getShortDescription()
    {
        return mShortDescription;
    }

    protected IEventHandler<Event> mCommandExecutedEventNotifier = new EventHandler();
    protected IEventHandler<Event> mCommandUnExecutedEventNotifier = new EventHandler();
    protected IEventHandler<Event> mCommandAddedEventNotifier = new EventHandler();

    public <K extends CommandEvent> void notifyListeners(K event)
            throws MethodParameterIsNullException
    {
        if (event == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (CommandExecutedEvent.class.isAssignableFrom(event.getClass()))
        {
            mCommandExecutedEventNotifier.notifyListeners(event);
        }
        else
            if (CommandUnExecutedEvent.class.isAssignableFrom(event.getClass()))
            {
                mCommandUnExecutedEventNotifier.notifyListeners(event);
            }
            else
                if (CommandAddedEvent.class.isAssignableFrom(event.getClass()))
                {
                    mCommandAddedEventNotifier.notifyListeners(event);
                }
        // ICommand does know about the Presentation to which it is
        // attached, however there is no forwarding of the event upwards in the
        // hierarchy (bubbling-up). The rationale is that there would be too
        // many unfiltered CommandEvents to capture (e.g. ICompositeCommand with
        // many sub-Commands)
        // mDataModelEventNotifier.notifyListeners(event);
    }

    public <K extends CommandEvent> void registerListener(
            IEventListener<K> listener, Class<K> klass)
            throws MethodParameterIsNullException
    {
        if (listener == null || klass == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (CommandExecutedEvent.class.isAssignableFrom(klass))
        {
            mCommandExecutedEventNotifier.registerListener(listener, klass);
        }
        else
            if (CommandUnExecutedEvent.class.isAssignableFrom(klass))
            {
                mCommandUnExecutedEventNotifier.registerListener(listener,
                        klass);
            }
            else
                if (CommandAddedEvent.class.isAssignableFrom(klass))
                {
                    mCommandAddedEventNotifier
                            .registerListener(listener, klass);
                }
                else
                {
                    // ICommand does know anything about the Presentation to
                    // which
                    // it is attached, however there is no possible registration
                    // of
                    // listeners
                    // onto the generic event bus (used for bubbling-up).
                    // mDataModelEventNotifier.registerListener(listener,
                    // klass);
                }
    }

    public <K extends CommandEvent> void unregisterListener(
            IEventListener<K> listener, Class<K> klass)
            throws MethodParameterIsNullException
    {
        if (listener == null || klass == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (CommandExecutedEvent.class.isAssignableFrom(klass))
        {
            mCommandExecutedEventNotifier.unregisterListener(listener, klass);
        }
        else
            if (CommandUnExecutedEvent.class.isAssignableFrom(klass))
            {
                mCommandUnExecutedEventNotifier.unregisterListener(listener,
                        klass);
            }
            else
                if (CommandAddedEvent.class.isAssignableFrom(klass))
                {
                    mCommandAddedEventNotifier.unregisterListener(listener,
                            klass);
                }
                else
                {
                    // ICommand does know anything about the Presentation to
                    // which
                    // it is attached, however there is no possible
                    // unregistration of
                    // listeners
                    // from the generic event bus (used for bubbling-up).
                    // mDataModelEventNotifier.unregisterListener(listener,
                    // klass);
                }
    }

    @Override
    public void clear()
    {
        mShortDescription = null;
        mLongDescription = null;
        // super.clear();
    }
}
