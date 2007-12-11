package org.daisy.urakawa.undo;

import java.net.URI;
import java.util.List;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.data.MediaData;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation
 *
 */
public class CompositeCommandImpl implements CompositeCommand {
	public void append(Command command) throws MethodParameterIsNullException {
		// TODO Auto-generated method stub
	}

	public int getCount() {
		// TODO Auto-generated method stub
		return 0;
	}

	public List<Command> getListOfCommands() {
		// TODO Auto-generated method stub
		return null;
	}

	public void insert(Command command, int index)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException {
		// TODO Auto-generated method stub
	}

	public boolean canExecute() {
		// TODO Auto-generated method stub
		return false;
	}

	public boolean canUnExecute() {
		// TODO Auto-generated method stub
		return false;
	}

	public void execute() throws CommandCannotExecuteException {
		// TODO Auto-generated method stub
	}

	public List<MediaData> getListOfUsedMediaData() {
		// TODO Auto-generated method stub
		return null;
	}

	public void unExecute() throws CommandCannotUnExecuteException {
		// TODO Auto-generated method stub
	}

	public String getXukLocalName() {
		// TODO Auto-generated method stub
		return null;
	}

	public String getXukNamespaceURI() {
		// TODO Auto-generated method stub
		return null;
	}

	public void xukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		// TODO Auto-generated method stub
	}

	public void xukOut(XmlDataWriter destination, URI baseURI)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
		// TODO Auto-generated method stub
	}

	public Presentation getPresentation() throws IsNotInitializedException {
		// TODO Auto-generated method stub
		return null;
	}

	public void setPresentation(Presentation presentation)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException {
		// TODO Auto-generated method stub
	}

	public String getLongDescription() {
		// TODO Auto-generated method stub
		return null;
	}

	public String getShortDescription() {
		// TODO Auto-generated method stub
		return null;
	}

	public void setLongDescription(String str)
			throws MethodParameterIsNullException {
		// TODO Auto-generated method stub
	}

	public void setShortDescription(String str)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		// TODO Auto-generated method stub
	}
}
