package org.daisy.urakawa.command;

import java.net.URI;
import java.util.List;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.event.IEventListener;
import org.daisy.urakawa.event.command.CommandEvent;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.IMediaData;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

public class Command extends WithPresentation implements ICommand {

	@Override
	protected void clear() {
		; // Does nothing
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukInAttributes(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		; // Does nothing

	}

	@SuppressWarnings("unused")
	@Override
	protected void xukOutAttributes(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
		; // Does nothing

	}

	@SuppressWarnings("unused")
	@Override
	protected void xukOutChildren(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
		; // Does nothing

	}

	public boolean canUnExecute() {
		; // Does nothing
		return false;
	}

	public List<IMediaData> getListOfUsedMediaData() {
		; // Does nothing
		return null;
	}

	@SuppressWarnings("unused")
	public void unExecute() throws CommandCannotUnExecuteException {
		; // Does nothing

	}

	public boolean canExecute() {
		; // Does nothing
		return false;
	}

	@SuppressWarnings("unused")
	public void execute() throws CommandCannotExecuteException {
		; // Does nothing

	}

	public String getLongDescription() {
		; // Does nothing
		return null;
	}

	public String getShortDescription() {
		; // Does nothing
		return null;
	}

	@SuppressWarnings("unused")
	public void setLongDescription(String str)
			throws MethodParameterIsNullException {
		; // Does nothing

	}

	@SuppressWarnings("unused")
	public void setShortDescription(String str)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		; // Does nothing

	}

	@SuppressWarnings("unused")
	public <K extends CommandEvent> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		; // Does nothing

	}

	@SuppressWarnings("unused")
	public <K extends CommandEvent> void registerListener(
			IEventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		; // Does nothing

	}

	@SuppressWarnings("unused")
	public <K extends CommandEvent> void unregisterListener(
			IEventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		; // Does nothing

	}

}
