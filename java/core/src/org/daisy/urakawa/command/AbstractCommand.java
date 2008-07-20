package org.daisy.urakawa.command;

import java.net.URI;
import java.util.List;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.events.IEventListener;
import org.daisy.urakawa.events.command.CommandEvent;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.IMediaData;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * 
 */
public abstract class AbstractCommand extends WithPresentation implements
		ICommand {

	@Override
	protected abstract void clear();

	@Override
	protected abstract void xukInAttributes(IXmlDataReader source,
			IProgressHandler ph) throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException;

	@Override
	protected abstract void xukOutAttributes(IXmlDataWriter destination,
			URI baseUri, IProgressHandler ph)
			throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException;

	@Override
	protected abstract void xukOutChildren(IXmlDataWriter destination,
			URI baseUri, IProgressHandler ph)
			throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException;

	public abstract boolean canUnExecute();

	public abstract List<IMediaData> getListOfUsedMediaData();

	public abstract void unExecute() throws CommandCannotUnExecuteException;

	public abstract boolean canExecute();

	public abstract void execute() throws CommandCannotExecuteException;

	public abstract String getLongDescription();

	public abstract String getShortDescription();

	public abstract void setLongDescription(String str)
			throws MethodParameterIsNullException;

	public abstract void setShortDescription(String str)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	public abstract <K extends CommandEvent> void notifyListeners(K event)
			throws MethodParameterIsNullException;

	public abstract <K extends CommandEvent> void registerListener(
			IEventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException;

	public abstract <K extends CommandEvent> void unregisterListener(
			IEventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException;

}
