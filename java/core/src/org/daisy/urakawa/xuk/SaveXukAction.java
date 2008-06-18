package org.daisy.urakawa.xuk;

import java.net.URI;

import org.daisy.urakawa.Project;
import org.daisy.urakawa.command.CommandCannotExecuteException;
import org.daisy.urakawa.event.CancellableEvent;
import org.daisy.urakawa.event.Event;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.EventHandlerImpl;
import org.daisy.urakawa.event.EventListener;
import org.daisy.urakawa.event.progress.ProgressEvent;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.FileStream;
import org.daisy.urakawa.nativeapi.XmlDataWriter;
import org.daisy.urakawa.nativeapi.XmlDataWriterImpl;
import org.daisy.urakawa.progress.ProgressAction;
import org.daisy.urakawa.progress.ProgressCancelledException;

/**
 *
 */
public class SaveXukAction extends ProgressAction implements
		EventListener<CancellableEvent> {
	protected EventHandler<Event> mEventNotifier = new EventHandlerImpl();
	private URI mUri;
	private Project mProject;
	private FileStream mFileStream;

	/**
	 * @param proj
	 * @param uri
	 * @throws MethodParameterIsNullException
	 */
	public SaveXukAction(Project proj, URI uri)
			throws MethodParameterIsNullException {
		mUri = uri;
		mProject = proj;
		//
		if (mUri == null || mProject == null) {
			throw new MethodParameterIsNullException();
		}
		mFileStream = null;
	}

	public boolean notifyProgress() {
		if (cancelHasBeenRequested()) {
			return true;
		}
		if (mFileStream == null) {
			return false;
		}
		ProgressEvent event = new ProgressEvent(mFileStream.getPosition(),
				mFileStream.getLength());
		try {
			notifyListeners(event);
		} catch (MethodParameterIsNullException e) {
			System.out.println("WTF ?! This should never happen !");
			e.printStackTrace();
		}
		if (event.isCancelled()) {
			return true;
		}
		return false;
	}

	public boolean canExecute() {
		return true;
	}

	@SuppressWarnings("unused")
	public void execute() throws CommandCannotExecuteException {
		mCancelHasBeenRequested = false;
		mFileStream = new FileStream(mUri.getPath());
		XmlDataWriter mWriter = new XmlDataWriterImpl(mFileStream);
		mWriter.writeStartDocument();
		mWriter.writeStartElement("Xuk", XukAble.XUK_NS);
		// TODO: add schema declaration in XML header
		try {
			registerListener(this, CancellableEvent.class);
		} catch (MethodParameterIsNullException e1) {
			System.out.println("WTF ?! This should never happen !");
			e1.printStackTrace();
		}
		try {
			mProject.xukOut(mWriter, mUri, this);
			notifyFinished();
		} catch (MethodParameterIsNullException e) {
			System.out.println("WTF ?! This should never happen !");
			e.printStackTrace();
		} catch (XukSerializationFailedException e) {
			mWriter.close();
			mFileStream = null;
			throw new RuntimeException(e);
		} catch (ProgressCancelledException e) {
			notifyCancelled();
		} finally {
			try {
				unregisterListener(this, CancellableEvent.class);
			} catch (MethodParameterIsNullException e) {
				System.out.println("WTF ?! This should never happen !");
				e.printStackTrace();
			}
		}
		mWriter.writeEndElement();
		mWriter.writeEndDocument();
		mWriter.close();
		mFileStream = null;
	}

	public String getLongDescription() {
		return null;
	}

	public String getShortDescription() {
		return null;
	}

	@SuppressWarnings("unused")
	public void setLongDescription(String str)
			throws MethodParameterIsNullException {
	}

	@SuppressWarnings("unused")
	public void setShortDescription(String str)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	public <K extends Event> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		if (event == null) {
			throw new MethodParameterIsNullException();
		}
		mEventNotifier.notifyListeners(event);
	}

	public <K extends Event> void registerListener(EventListener<K> listener,
			Class<K> klass) throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		mEventNotifier.registerListener(listener, klass);
	}

	public <K extends Event> void unregisterListener(EventListener<K> listener,
			Class<K> klass) throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		mEventNotifier.unregisterListener(listener, klass);
	}

	public <K extends CancellableEvent> void eventCallback(K event)
			throws MethodParameterIsNullException {
		if (event == null) {
			throw new MethodParameterIsNullException();
		}
		if (event.isCancelled()) {
			requestCancel();
		}
	}
}
