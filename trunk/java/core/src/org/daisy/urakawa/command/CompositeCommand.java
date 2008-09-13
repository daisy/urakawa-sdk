package org.daisy.urakawa.command;

import java.net.URI;
import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.events.command.CommandAddedEvent;
import org.daisy.urakawa.events.command.CommandExecutedEvent;
import org.daisy.urakawa.events.command.CommandUnExecutedEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.data.IMediaData;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.xuk.IXukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation
 */
public class CompositeCommand extends AbstractCommand implements
        ICompositeCommand
{
    private List<ICommand> mCommands;

    /**
     * Default constructor
     */
    public CompositeCommand()
    {
        mCommands = new LinkedList<ICommand>();
    }

    public void append(ICommand iCommand) throws MethodParameterIsNullException
    {
        try
        {
            insert(iCommand, getCount());
        }
        catch (MethodParameterIsOutOfBoundsException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    public int getCount()
    {
        return mCommands.size();
    }

    public List<ICommand> getListOfCommands()
    {
        return new LinkedList<ICommand>(mCommands);
    }

    public void insert(ICommand iCommand, int index)
            throws MethodParameterIsNullException,
            MethodParameterIsOutOfBoundsException
    {
        if (iCommand == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (index < 0 || index > mCommands.size())
        {
            throw new MethodParameterIsOutOfBoundsException();
        }
        mCommands.add(index, iCommand);
        notifyListeners(new CommandAddedEvent(this, iCommand, index));
    }

    @Override
    public void clear()
    {
        super.clear();
        mCommands.clear();
    }

    public boolean canExecute()
    {
        if (mCommands.size() == 0)
            return false;
        for (ICommand iCommand : mCommands)
        {
            if (!iCommand.canExecute())
            {
                return false;
            }
        }
        return true;
    }

    public boolean canUnExecute()
    {
        if (mCommands.size() == 0)
            return false;
        for (ICommand iCommand : mCommands)
        {
            if (!iCommand.canUnExecute())
            {
                return false;
            }
        }
        return true;
    }

    public void execute() throws CommandCannotExecuteException
    {
        if (mCommands.size() == 0)
            throw new CommandCannotExecuteException();
        for (ICommand iCommand : mCommands)
            iCommand.execute();
        try
        {
            notifyListeners(new CommandExecutedEvent(this));
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    public List<IMediaData> getListOfUsedMediaData()
    {
        List<IMediaData> res = new LinkedList<IMediaData>();
        for (ICommand cmd : mCommands)
        {
            res.addAll(cmd.getListOfUsedMediaData());
        }
        return res;
    }

    public void unExecute() throws CommandCannotUnExecuteException
    {
        if (mCommands.size() == 0)
            throw new CommandCannotUnExecuteException();
        for (int i = mCommands.size() - 1; i >= 0; --i)
            mCommands.get(i).unExecute();
        try
        {
            notifyListeners(new CommandUnExecutedEvent(this));
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    @Override
    public String getLongDescription()
    {
        if (mLongDescription != null && mLongDescription != "")
            return mLongDescription;
        String cmds = "-";
        if (mCommands.size() > 0)
        {
            cmds = mCommands.get(0).getLongDescription();
            for (int i = 1; i < mCommands.size(); i++)
            {
                cmds += "\n" + getLongDescription();
            }
        }
        return cmds;
    }

    @Override
    public String getShortDescription()
    {
        if (mShortDescription != null && mShortDescription != "")
            return mShortDescription;
        String cmds = "-";
        if (mCommands.size() > 0)
        {
            cmds = mCommands.get(0).getShortDescription();
            if (mCommands.size() > 1)
            {
                cmds += "..." + mCommands.get(mCommands.size() - 1);
            }
        }
        return cmds;
    }

    @SuppressWarnings("unused")
    @Override
    public void xukInAttributes(IXmlDataReader source, IProgressHandler ph)
            throws XukDeserializationFailedException,
            ProgressCancelledException
    {
        // To avoid event notification overhead, we bypass this:
        if (false && ph != null && ph.notifyProgress())
        {
            throw new ProgressCancelledException();
        }
        mShortDescription = source.getAttribute("shortDescription");
        mLongDescription = source.getAttribute("longDescription");
        // super.xukInAttributes(source);
    }

    @Override
    public void xukInChild(IXmlDataReader source, IProgressHandler ph)
            throws XukDeserializationFailedException,
            ProgressCancelledException
    {
        // To avoid event notification overhead, we bypass this:
        if (false && ph != null && ph.notifyProgress())
        {
            throw new ProgressCancelledException();
        }
        boolean readItem = false;
        if (source.getNamespaceURI() == IXukAble.XUK_NS)
        {
            if (source.getLocalName() == "mCommands")
            {
                xukInCommands(source, ph);
                readItem = true;
            }
        }
        if (!readItem)
        {
            try
            {
                super.xukInChild(source, ph);
            }
            catch (MethodParameterIsNullException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
    }

    private void xukInCommands(IXmlDataReader source, IProgressHandler ph)
            throws XukDeserializationFailedException,
            ProgressCancelledException
    {
        if (ph != null && ph.notifyProgress())
        {
            throw new ProgressCancelledException();
        }
        if (!source.isEmptyElement())
        {
            while (source.read())
            {
                if (source.getNodeType() == IXmlDataReader.ELEMENT)
                {
                    ICommand cmd;
                    try
                    {
                        cmd = getPresentation().getCommandFactory()
                                .create(source.getLocalName(),
                                        source.getNamespaceURI());
                    }
                    catch (MethodParameterIsNullException e1)
                    {
                        // Should never happen
                        throw new RuntimeException("WTF ??!", e1);
                    }
                    catch (MethodParameterIsEmptyStringException e1)
                    {
                        // Should never happen
                        throw new RuntimeException("WTF ??!", e1);
                    }
                    catch (IsNotInitializedException e1)
                    {
                        // Should never happen
                        throw new RuntimeException("WTF ??!", e1);
                    }
                    if (cmd == null)
                    {
                        throw new XukDeserializationFailedException();
                    }
                    try
                    {
                        append(cmd);
                        cmd.xukIn(source, ph);
                    }
                    catch (MethodParameterIsNullException e)
                    {
                        // Should never happen
                        throw new RuntimeException("WTF ??!", e);
                    }
                }
                else
                    if (source.getNodeType() == IXmlDataReader.END_ELEMENT)
                    {
                        break;
                    }
                if (source.isEOF())
                    throw new XukDeserializationFailedException();
            }
        }
    }

    @SuppressWarnings("unused")
    @Override
    public void xukOutAttributes(IXmlDataWriter destination, URI baseUri,
            IProgressHandler ph) throws XukSerializationFailedException,
            ProgressCancelledException
    {
        if (ph != null && ph.notifyProgress())
        {
            throw new ProgressCancelledException();
        }
        if (mShortDescription != null)
        {
            destination.writeAttributeString("shortDescription",
                    mShortDescription);
        }
        if (mLongDescription != null)
        {
            destination.writeAttributeString("longDescription",
                    mLongDescription);
        }
        // super.xukOutAttributes(destination, baseUri);
    }

    @Override
    public void xukOutChildren(IXmlDataWriter destination, URI baseUri,
            IProgressHandler ph) throws XukSerializationFailedException,
            ProgressCancelledException
    {
        if (ph != null && ph.notifyProgress())
        {
            throw new ProgressCancelledException();
        }
        destination.writeStartElement("mCommands", IXukAble.XUK_NS);
        for (ICommand cmd : getListOfCommands())
        {
            try
            {
                cmd.xukOut(destination, baseUri, ph);
            }
            catch (MethodParameterIsNullException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
        destination.writeEndElement();
        // super.xukOutChildren(destination, baseUri);
    }
}
