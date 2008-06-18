package org.daisy.urakawa.xuk;

import org.daisy.urakawa.command.CommandCannotExecuteException;
import org.daisy.urakawa.event.EventListener;
import org.daisy.urakawa.event.progress.ProgressEvent;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.progress.ProgressAction;

/**
 *
 */
public class OpenXukAction implements ProgressAction {

	public boolean canExecute() {
		return true;
	}

	public void execute() throws CommandCannotExecuteException {
		// TODO Auto-generated method stub		
	}

	public String getLongDescription() {
		return "Parse an XML file in the XUK format and builds the data model";
	}

	public String getShortDescription() {
		return "Open a XUK file";
	}

	public void setLongDescription(String str)
			throws MethodParameterIsNullException {
		
	}

	public void setShortDescription(String str)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	public void notifyCancelled() {
		// TODO Auto-generated method stub
		
	}

	public void notifyFinished() {
		// TODO Auto-generated method stub
		
	}

	public void notifyProgress() {
		// TODO Auto-generated method stub
		
	}

	public <K extends ProgressEvent> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		// TODO Auto-generated method stub
		
	}

	public <K extends ProgressEvent> void registerListener(
			EventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		// TODO Auto-generated method stub
		
	}

	public <K extends ProgressEvent> void unregisterListener(
			EventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		// TODO Auto-generated method stub
		
	}
}
